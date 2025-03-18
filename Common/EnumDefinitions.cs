namespace surabot.Common
{
    public enum LogCategory
    {
        System = 0,  // 🖥️ 시스템 관련 로그
        Database = 1,  // 🗄️ 데이터베이스 관련 로그
        Discord = 2,  // 🔷 디스코드 관련 로그
        Chzzk = 3,  // 🎥 치지직 관련 로그
        YouTube = 4,  // ▶️ 유튜브 관련 로그
        Warning = 5,  // ⚠️ 경고 로그
        Error = 6   // ❌ 에러 로그
    }

    public static class LogCategoryExtensions
    {
        public static string GetEmoji(this LogCategory category)
        {
            return category switch
            {
                LogCategory.System => "🖥️",
                LogCategory.Database => "🗄️",
                LogCategory.Discord => "🔷",
                LogCategory.Chzzk => "🎥",
                LogCategory.YouTube => "▶️",
                LogCategory.Warning => "⚠️",
                LogCategory.Error => "❌",
                _ => "ℹ️"
            };
        }

        public static string GetFormattedName(this LogCategory category)
        {
            return category switch
            {
                LogCategory.System => "System   ",
                LogCategory.Database => "Database ",
                LogCategory.Discord => "Discord  ",
                LogCategory.Chzzk => "Chzzk    ",
                LogCategory.YouTube => "YouTube  ",
                LogCategory.Warning => "Warning  ",
                LogCategory.Error => "Error    ",
                _ => "Unknown  "
            };
        }
    }

    public enum ErrorOccur
    {
        Form = -1,
        Entity = -2,
        Database = -3,
        API = -4,
        Unknown = -99
    }

    public enum ResultType
    {
        Success = 1,
        DataNotFound = 0,
        DataOverFlow = -1,
        DataUnderFlow = -2,
        DataNotEquals = -3,
        DBConnectError = -4,
        DBQueryError = -9,
        UnknownError = -99
    }
}
