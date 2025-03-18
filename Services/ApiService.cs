using System;
using System.Net;
using System.Threading.Tasks;
using surabot.Common;
using surabot.Utils;

namespace surabot.Services
{
    public class ApiService
    {
        private readonly HttpListener _listener;
        private readonly int _assignedPort;

        public ApiService(int assignedPort)
        {
            _assignedPort = assignedPort;
            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://localhost:{_assignedPort}/reload/");
        }

        public async Task StartHttpServerAsync()
        {
            try
            {
                _listener.Start();
                LogHelper.WriteLog(LogCategory.System, $"🌐 API 서버가 {_assignedPort} 포트에서 실행 중...");

                while (true)
                {
                    HttpListenerContext context = await _listener.GetContextAsync();
                    HttpListenerResponse response = context.Response;

                    try
                    {
                        LogHelper.WriteLog(LogCategory.System, "🔄 웹 요청을 통해 설정을 다시 로드합니다.");
                        CommonEntities.LoadSettings();

                        string responseString = "✅ 설정이 다시 로드되었습니다.";
                        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                        response.ContentLength64 = buffer.Length;
                        response.OutputStream.Write(buffer, 0, buffer.Length);
                        response.OutputStream.Close();
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteLog(LogCategory.Error, $"❌ 설정 리로드 중 오류 발생: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(LogCategory.Error, $"❌ API 서버 실행 중 오류 발생: {ex.Message}");
            }
        }
    }
}
