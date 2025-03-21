using System;
using System.Data;
using MySql.Data.MySqlClient;
using surabot.Common;
using surabot.Utils;

namespace surabot.Services
{
    public class BotSettingsService
    {
        private readonly DbHelper _dbHelper;

        public BotSettingsService(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        /// <summary>
        /// 특정 기능이 활성화되어 있는지 확인
        /// </summary>
        public bool IsFeatureEnabled(ulong guildId, string featureKey)
        {
            try
            {
                string query = @"
                    SELECT IsEnabled 
                    FROM Botsetting 
                    WHERE GuildId = @GuildId AND FeatureKey = @FeatureKey";

                var parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@GuildId", guildId),
                    new MySqlParameter("@FeatureKey", featureKey)
                };

                DataTable result = _dbHelper.ExecuteQuery(query, parameters);
                if (result.Rows.Count > 0)
                {
                    return result.Rows[0]["IsEnabled"].ToString() == "1";
                }
                return false;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(LogCategory.System, $"❌ BotSettingsService::IsFeatureEnabled 예외 발생 - {ex.Message}");
                return false;
            }
        }
    }
}
