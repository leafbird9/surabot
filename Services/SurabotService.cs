using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using surabot.Common;
using surabot.Utils;
using surabot.Handlers;
using surabot.Services;
using surabot.Helpers;

namespace surabot.Services
{
    public class SurabotService
    {
        private readonly DiscordSocketClient _client;
        private readonly GuildHandler _guildHandler;
        private readonly DbHelper _dbHelper;
        private readonly BotSettingsService _botSettingsService;
        private readonly ChzzkNotificationBotService _chzzkNotificationBotService;
        private readonly ApiService _apiService;

        public SurabotService()
        {
            if (string.IsNullOrEmpty(CommonHelper.DiscordToken))
            {
                throw new Exception("❌ DISCORD_TOKEN이 설정되지 않았습니다!");
            }

            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.MessageContent
            });

            _dbHelper = new DbHelper(CommonHelper.DbConnectionString);
            _botSettingsService = new BotSettingsService(_dbHelper);

            _guildHandler = new GuildHandler(_client, _dbHelper, _botSettingsService, this);

            try
            {
                // ✅ 포트 등록 (DbHelper를 넘겨주어 중복 생성 방지)
                PortHelper.RegisterContainerPort(_dbHelper);
                // ✅ DB에서 등록된 포트를 가져옴
                int assignedPort = PortHelper.GetRegisteredPort(_dbHelper);

                if (assignedPort > 0)
                {
                    _apiService = new ApiService(assignedPort);
                    _ = _apiService.StartHttpServerAsync(); // ✅ API 서버 실행
                }
                else
                {
                    LogHelper.WriteLog(LogCategory.Error, "❌ API 서버를 시작할 수 없습니다. 포트 할당 실패.");
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(LogCategory.Error, $"❌ API 서버 초기화 중 오류 발생: {ex.Message}");
            }

            // ✅ 모든 설정 로드
            CommonEntities.LoadSettings();

            // ✅ 치지직 설정을 객체 기반으로 적용
            if (!string.IsNullOrEmpty(CommonEntities.ChzzkSetting.ChzzkChannelId) &&
                CommonEntities.ChzzkSetting.DiscordChannelId != 0)
            {
                _chzzkNotificationBotService = new ChzzkNotificationBotService(
                    _client,
                    CommonEntities.ChzzkSetting.ChzzkChannelId,
                    CommonEntities.ChzzkSetting.DiscordChannelId
                );
            }

            _client.Log += LogAsync;
            _client.Ready += ReadyAsync;
        }

        public async Task RunAsync()
        {
            if (_chzzkNotificationBotService != null)
            {
                _ = _chzzkNotificationBotService.StartAsync();
            }

            await _client.LoginAsync(TokenType.Bot, CommonHelper.DiscordToken);
            await _client.StartAsync();
            await Task.Delay(-1);
        }

        private Task LogAsync(LogMessage message)
        {
            LogHelper.WriteLog(LogCategory.Discord, message.ToString());
            return Task.CompletedTask;
        }

        private Task ReadyAsync()
        {
            LogHelper.WriteLog(LogCategory.System, "✅ Surabot이 정상적으로 실행되었습니다!");
            return Task.CompletedTask;
        }
    }
}
