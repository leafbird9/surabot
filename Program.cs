using System;
using System.Threading.Tasks;
using surabot.Common;
using surabot.Services;
using surabot.Utils;

class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            CommonHelper.InitializeEnvironment();
            LogHelper.WriteLog(LogCategory.System, "🚀 Surabot Starting...");

            var botService = new SurabotService();
            await botService.RunAsync();
        }
        catch (Exception ex)
        {
            LogHelper.WriteLog(LogCategory.Error, $"❌ 프로그램 실행 중 오류 발생: {ex.Message}");
        }
        finally
        {
            LogHelper.WriteLog(LogCategory.System, "🛑 Surabot Stopped.");
        }
    }
}
