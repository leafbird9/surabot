using System;
using System.Net;
using System.Net.Sockets;
using MySql.Data.MySqlClient;
using surabot.Common;
using surabot.Utils;

namespace surabot.Helpers
{
    public static class PortHelper
    {
        /// <summary>
        /// 현재 컨테이너가 사용 중인 포트를 조회
        /// </summary>
        public static int GetAssignedPort()
        {
            try
            {
                string hostName = Dns.GetHostName();
                var ipHost = Dns.GetHostEntry(hostName);
                var localIP = ipHost.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

                if (localIP != null)
                {
                    using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                    {
                        socket.Bind(new IPEndPoint(IPAddress.Any, 0));
                        return ((IPEndPoint)socket.LocalEndPoint).Port; // ✅ 사용 가능한 포트 조회
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(LogCategory.Error, $"❌ 포트 조회 중 오류 발생: {ex.Message}");
            }

            return -1;
        }

        /// <summary>
        /// 컨테이너가 실행될 때 본인의 포트를 DB에 업데이트
        /// </summary>
        public static void RegisterContainerPort(DbHelper dbHelper)
        {
            try
            {
                if (string.IsNullOrEmpty(CommonHelper.DiscordGuildId))
                {
                    LogHelper.WriteLog(LogCategory.Error, "❌ DiscordGuildId가 설정되지 않았습니다. 포트 등록을 중단합니다.");
                    return;
                }

                int assignedPort = GetAssignedPort();
                if (assignedPort > 0)
                {
                    string query = @"
                    INSERT INTO bot_instances (guild_id, api_port, last_updated)
                    VALUES (@GuildId, @Port, NOW())
                    ON DUPLICATE KEY UPDATE 
                    api_port = VALUES(api_port), 
                    last_updated = NOW();";

                    dbHelper.ExecuteNonQuery(query,
                        new MySqlParameter("@Port", assignedPort),
                        new MySqlParameter("@GuildId", CommonHelper.DiscordGuildId));

                    LogHelper.WriteLog(LogCategory.System, $"✅ 컨테이너 포트 {assignedPort}가 데이터베이스에 등록됨.");
                }
                else
                {
                    LogHelper.WriteLog(LogCategory.Error, "❌ 컨테이너 포트 조회 실패.");
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(LogCategory.Error, $"❌ 컨테이너 포트 등록 중 오류 발생: {ex.Message}");
            }
        }

        public static int GetRegisteredPort(DbHelper dbHelper)
        {
            try
            {
                string query = "SELECT api_port FROM bot_instances WHERE guild_id = @GuildId LIMIT 1";
                var dt = dbHelper.ExecuteQuery(query, new MySqlParameter("@GuildId", CommonHelper.DiscordGuildId));

                if (dt.Rows.Count > 0)
                {
                    return Convert.ToInt32(dt.Rows[0]["api_port"]);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(LogCategory.Error, $"❌ 등록된 포트 조회 중 오류 발생: {ex.Message}");
            }

            return -1; // 포트를 찾을 수 없는 경우
        }

    }
}
