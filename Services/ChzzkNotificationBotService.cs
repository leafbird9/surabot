using System;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using surabot.Common;
using surabot.Utils;
using surabot.Models; // ✅ Models 네임스페이스 추가

namespace surabot.Services
{
    public class ChzzkNotificationBotService
    {
        private readonly DiscordSocketClient _discordClient;
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly string _chzzkChannelId;
        private readonly ulong _discordChannelId;
        private bool _isLive = false;

        public ChzzkNotificationBotService(DiscordSocketClient discordClient, string chzzkChannelId, ulong discordChannelId)
        {
            _discordClient = discordClient;
            _chzzkChannelId = chzzkChannelId;
            _discordChannelId = discordChannelId;
        }

        public async Task StartAsync()
        {
            LogHelper.WriteLog(LogCategory.Chzzk, "✅ 치지직 라이브 모니터링 시작!");

            while (true)
            {
                await CheckLiveStatusAsync();
                await Task.Delay(60000); // 1분마다 확인
            }
        }

        private async Task CheckLiveStatusAsync()
        {
            try
            {
                LogHelper.WriteLog(LogCategory.Chzzk, "🔍 치지직 방송 상태 확인 중...");

                string apiUrl = $"https://api.chzzk.naver.com/service/v2/channels/{_chzzkChannelId}/live-detail";
                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                HttpResponseMessage response = await _httpClient.SendAsync(request);

#if DEBUG
                LogHelper.WriteLog(LogCategory.Chzzk, $"📢 API 응답 코드: {response.StatusCode}");
#endif

                if (!response.IsSuccessStatusCode)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    LogHelper.WriteLog(LogCategory.Chzzk, $"⚠ API 호출 실패: {response.StatusCode}");
                    LogHelper.WriteLog(LogCategory.Chzzk, $"⚠ 오류 내용: {errorContent}");
                    return;
                }

                string json = await response.Content.ReadAsStringAsync();
#if DEBUG
                LogHelper.WriteLog(LogCategory.Chzzk, $"📢 API 응답 내용: {json}");
#endif

                JObject root = JObject.Parse(json);
                int statusCode = root["code"]?.Value<int>() ?? 0;
                if (statusCode != 200)
                {
                    LogHelper.WriteLog(LogCategory.Chzzk, "⚠ API 응답에서 'code' 값이 200이 아님.");
                    return;
                }

                JObject content = root["content"] as JObject;
                if (content == null)
                {
                    LogHelper.WriteLog(LogCategory.Chzzk, "⚠ API 응답에서 'content' 키를 찾을 수 없음.");
                    return;
                }

                // ✅ DTO 생성 및 데이터 매핑
                var streamInfo = new LiveStreamInfo
                {
                    LiveId = content["liveId"]?.Value<int>() ?? 0,
                    Title = content["liveTitle"]?.Value<string>() ?? "제목 없음",
                    Status = content["status"]?.Value<string>() ?? "UNKNOWN",
                    ThumbnailUrl = (content["liveImageUrl"]?.Value<string>() ?? "").Replace("{type}", "480"),
                    ViewerCount = content["concurrentUserCount"]?.Value<int>() ?? 0,
                    AccumulateCount = content["accumulateCount"]?.Value<int>() ?? 0,
                    OpenDate = DateTime.Parse(content["openDate"]?.Value<string>() ?? DateTime.MinValue.ToString()),
                    CloseDate = content["closeDate"]?.Value<string>() != null ? DateTime.Parse(content["closeDate"].Value<string>()) : (DateTime?)null,
                    IsAdult = content["adult"]?.Value<bool>() ?? false,
                    IsChatActive = content["chatActive"]?.Value<bool>() ?? false,
                    ChatChannelId = content["chatChannelId"]?.Value<string>() ?? "",
                    CategoryType = content["categoryType"]?.Value<string>() ?? "UNKNOWN",
                    Category = content["liveCategoryValue"]?.Value<string>() ?? "카테고리 없음",
                    Tags = content["tags"] is JArray tagsArray ? string.Join(", ", tagsArray.Select(t => t.ToString())) : "태그 없음",
                    PaidPromotion = content["paidPromotion"]?.Value<bool>() ?? false,
                    StreamUrl = $"https://chzzk.naver.com/live/{_chzzkChannelId}", // ✅ 기본 URL 형식으로 설정
                    Channel = new LiveChannelInfo
                    {
                        ChannelId = content["channel"]?["channelId"]?.Value<string>() ?? "",
                        ChannelName = content["channel"]?["channelName"]?.Value<string>() ?? "알 수 없음",
                        ChannelImageUrl = content["channel"]?["channelImageUrl"]?.Value<string>() ?? "",
                        VerifiedMark = content["channel"]?["verifiedMark"]?.Value<bool>() ?? false
                    },
                    Media = new LiveMediaInfo
                    {
                        VideoId = content["livePlaybackJson"] != null
                            ? JObject.Parse(content["livePlaybackJson"].Value<string>())["meta"]?["videoId"]?.Value<string>() ?? ""
                            : "",
                        EncodingQuality = content["p2pQuality"] is JArray qualityArray
                            ? string.Join(", ", qualityArray.Select(q => q.ToString()))
                            : "알 수 없음"
                    }
                };

                if (streamInfo.Status == "OPEN" && !_isLive)
                {
                    await NotifyLiveStartAsync(streamInfo);
                    _isLive = true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(LogCategory.Chzzk, $"❌ 오류 발생: {ex.Message}");
            }
        }


        private async Task NotifyLiveStartAsync(LiveStreamInfo streamInfo)
        {
            try
            {
                var embed = new EmbedBuilder()
                    .WithColor(Color.Green)
                    .WithTitle(streamInfo.Title)
                    .WithUrl(streamInfo.StreamUrl)
                    .WithDescription($"**{streamInfo.Channel.ChannelName}** 님이 치지직에서 라이브를 시작했습니다! 🎬")
                    .WithImageUrl(streamInfo.ThumbnailUrl)
                    .WithAuthor(streamInfo.Channel.ChannelName, iconUrl: streamInfo.Channel.ChannelImageUrl, url: streamInfo.Media.StreamUrl)
                    .WithFooter($"{streamInfo.Channel.ChannelName} 치지직 방송 알림", streamInfo.Channel.ChannelImageUrl)
                    .WithCurrentTimestamp()
                    .AddField("시청자 수", streamInfo.ViewerCount.ToString(), true)
                    .AddField("카테고리", streamInfo.Category, true)
                    .AddField("태그", streamInfo.Tags, false)
                    .Build();

                var component = new ComponentBuilder()
                    .WithButton("바로가기", null, ButtonStyle.Link, null, streamInfo.StreamUrl) // ✅ CustomId 제거
                    .Build();

                IMessageChannel textChannel = _discordClient.GetChannel(_discordChannelId) as IMessageChannel;

                // ✅ REST API를 사용하여 채널 가져오기 (누락된 부분 추가)
                if (textChannel == null)
                {
                    try
                    {
                        var restChannel = await _discordClient.Rest.GetChannelAsync(_discordChannelId);
                        textChannel = restChannel as IMessageChannel;
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteLog(LogCategory.Chzzk, $"❌ REST API 호출 중 오류 발생: {ex.Message}");
                    }
                }

                // ✅ 메시지 전송
                if (textChannel != null)
                {
                    await textChannel.SendMessageAsync(embed: embed, components: component);
                    LogHelper.WriteLog(LogCategory.Chzzk, $"📢 방송 알림 전송 완료: {streamInfo.Title}");
                }
                else
                {
                    LogHelper.WriteLog(LogCategory.Chzzk, $"⚠ 채널을 IMessageChannel로 가져올 수 없음. (채널 ID: {_discordChannelId})");
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(LogCategory.Chzzk, $"❌ 오류 발생: {ex.Message}");
            }
        }

    }
}
