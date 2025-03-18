using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using surabot.Common;
using surabot.Models;
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
        /// 특정 길드에서 특정 기능이 활성화되어 있는지 확인
        /// </summary>
        public async Task<bool> IsFeatureEnabledAsync(ulong guildId, string featureName)
        {
            string query = "SELECT is_enabled FROM bot_features WHERE guild_id = @GuildId AND feature_name = @FeatureName";
            DataTable dt = _dbHelper.ExecuteQuery(query,
                new MySqlParameter("@GuildId", guildId),
                new MySqlParameter("@FeatureName", featureName)
            );

            if (dt.Rows.Count > 0)
            {
                return Convert.ToBoolean(dt.Rows[0]["is_enabled"]);
            }

            return false;
        }
    }
}
