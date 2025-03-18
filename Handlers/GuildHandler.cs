using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using MySql.Data.MySqlClient;
using surabot.Utils;
using surabot.Common;
using surabot.Services;
using Discord;

namespace surabot.Handlers
{
    public class GuildHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly DbHelper _dbHelper;
        private readonly BotSettingsService _botSettingsService;
        private readonly SurabotService _botService;
        private bool _isInitialized = false;

        public GuildHandler(DiscordSocketClient client, DbHelper dbHelper, BotSettingsService botSettingsService, SurabotService botService)
        {
            _client = client;
            _dbHelper = dbHelper;
            _botSettingsService = botSettingsService;
            _botService = botService;

            _client.GuildAvailable += OnGuildAvailable;
            _client.UserJoined += OnUserJoined;
        }

        private async Task OnGuildAvailable(SocketGuild guild)
        {
            if (_isInitialized) return;
            _isInitialized = true;

            LogHelper.WriteLog(LogCategory.System, $"✅ 봇이 길드({guild.Id})에 입장함: {guild.Name}");

            try
            {
                // ✅ 길드 정보 업데이트 (`owner_id`, `name`, `icon`, `region`, `member_count` 포함)
                string updateGuildQuery = @"
            UPDATE dc_guilds 
            SET owner_id = @OwnerId, name = @Name, icon = @Icon, region = @Region, 
                member_count = @MemberCount, bot_joined = 1, joined_at = NOW()
            WHERE guild_id = @GuildId";

                _dbHelper.ExecuteNonQuery(updateGuildQuery,
                    new MySqlParameter("@GuildId", guild.Id),
                    new MySqlParameter("@OwnerId", guild.OwnerId),
                    new MySqlParameter("@Name", guild.Name),
                    new MySqlParameter("@Icon", guild.IconUrl ?? ""),
                    new MySqlParameter("@Region", guild.VoiceRegionId),
                    new MySqlParameter("@MemberCount", guild.MemberCount)
                );
                LogHelper.WriteLog(LogCategory.Database, $"📡 길드 정보 업데이트 완료: {guild.Name} (ID: {guild.Id})");

                // ✅ 입장 로그 기록
                string insertJoinLogQuery = "INSERT INTO bot_join_logs (guild_id, event_type, event_time, bot_status) VALUES (@GuildId, 'join', NOW(), 1)";
                _dbHelper.ExecuteNonQuery(insertJoinLogQuery, new MySqlParameter("@GuildId", guild.Id));
                LogHelper.WriteLog(LogCategory.Database, $"📡 봇 입장 로그 기록 완료: {guild.Name} (ID: {guild.Id})");

                // ✅ `bot_features`에서 기능 확인
                bool isChzzkEnabled = await _botSettingsService.IsFeatureEnabledAsync(guild.Id, "Chzzk"); // ✅ featureName 추가
                if (isChzzkEnabled)
                {
                    LogHelper.WriteLog(LogCategory.Chzzk, "🎥 Chzzk 기능 활성화됨");
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(LogCategory.Error, $"❌ 길드 입장 처리 중 오류 발생: {ex.Message}");
                LogHelper.WriteLog(LogCategory.Error, ex.StackTrace);
            }
        }

        private async Task OnUserJoined(SocketGuildUser user)
        {
            try
            {
                LogHelper.WriteLog(LogCategory.System, $"👤 신규 유저 입장: {user.Username} ({user.Id})");

                // ✅ 캐싱된 메시지 사용
                if (!string.IsNullOrEmpty(CommonEntities.WelcomeMessage.Message))
                {
                    await user.SendMessageAsync(CommonEntities.WelcomeMessage.Message);
                    LogHelper.WriteLog(LogCategory.Discord, $"📩 환영 메시지 전송 완료: {user.Username}");
                }
                else
                {
                    LogHelper.WriteLog(LogCategory.Warning, "⚠ 환영 메시지가 설정되지 않음.");
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(LogCategory.Error, $"❌ 환영 메시지 전송 중 오류 발생: {ex.Message}");
            }
        }
    }
}
