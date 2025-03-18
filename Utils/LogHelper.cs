using System;
using System.IO;
using surabot.Common;

namespace surabot.Utils
{
    public static class LogHelper
    {
        private static readonly string LogDirectory = "logs";
        private static readonly string LogFilePath = Path.Combine(LogDirectory, "bot_log.txt");

        static LogHelper()
        {
            if (!Directory.Exists(LogDirectory))
                Directory.CreateDirectory(LogDirectory);
        }

        public static void WriteLog(LogCategory category, string message)
        {
            string emoji = category.GetEmoji();
            string formattedCategory = category.GetFormattedName(); // 일정한 길이 유지
            string logMessage = $"{emoji} [{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}][{formattedCategory}] - {message}";

            Console.WriteLine(logMessage);
            File.AppendAllText(LogFilePath, logMessage + Environment.NewLine);
        }
    }
}
