using surabot.Utils;
using System;

namespace surabot.Common
{
    public static class CommonHelper
    {
        public static string DiscordToken { get; private set; }
        public static string DiscordGuildId { get; private set; }
        public static string DbConnectionString { get; private set; }
        public static bool DebugModeEnabled { get; private set; }

        public static void InitializeEnvironment()
        {
#if DEBUG
            DiscordToken = "MTM0ODc1MjE4NDI3OTU2NDMwOA.GGRgdH.9aiB6xzCq5c5JbDtIczey6zMjQm7POmOilbzG4";
            DiscordGuildId = "1153967439772659712";
            DbConnectionString = "Server=58.229.105.96;Database=sql_chzbot;User=sql_chzbot;Password=cb3df373bfba7";
            LogHelper.WriteLog(LogCategory.System, "🛠️ DEBUG 모드 활성화됨");
            DebugModeEnabled = true;
#else
            DiscordToken = Environment.GetEnvironmentVariable("DISCORD_TOKEN");
            DiscordGuildId = Environment.GetEnvironmentVariable("DISCORD_GUILD_ID");
            DbConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            DebugModeEnabled = false;
#endif

            if (string.IsNullOrEmpty(DiscordToken) || string.IsNullOrEmpty(DiscordGuildId) || string.IsNullOrEmpty(DbConnectionString))
            {
                LogHelper.WriteLog(LogCategory.Error, "❌ 필수 환경 변수가 누락되었습니다.");
                Environment.Exit(1);
            }
        }
    }
}