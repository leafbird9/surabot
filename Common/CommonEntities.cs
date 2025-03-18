using MySql.Data.MySqlClient;
using surabot.Models;
using surabot.Utils;

namespace surabot.Common
{
    public static class CommonEntities
    {
        public static ChzzkSetting ChzzkSetting { get; private set; }
        public static WelcomeMessageSetting WelcomeMessage { get; private set; }

        /// <summary>
        /// 모든 설정을 한 번에 로드하는 메서드
        /// </summary>
        public static void LoadSettings()
        {
            var dbHelper = new DbHelper(CommonHelper.DbConnectionString);

            ChzzkSetting = dbHelper.GetSettings<ChzzkSetting>("ChzzkSettings", CommonHelper.DiscordGuildId);
            WelcomeMessage = dbHelper.GetSettings<WelcomeMessageSetting>("WelcomeMessages", CommonHelper.DiscordGuildId);

            LogHelper.WriteLog(LogCategory.System, "🔄 모든 설정이 로드되었습니다.");
        }
    }
}
