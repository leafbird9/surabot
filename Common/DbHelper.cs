using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace surabot.Common
{
    public class DbHelper
    {
        private readonly string _connectionString;

        public DbHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// 데이터 조회를 수행하고 DataTable 반환
        /// </summary>
        public DataTable ExecuteQuery(string query, params MySqlParameter[] parameters)
        {
            DataTable dt = new DataTable();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            return dt;
        }

        /// <summary>
        /// INSERT, UPDATE, DELETE 수행
        /// </summary>
        public int ExecuteNonQuery(string query, params MySqlParameter[] parameters)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 특정 길드의 설정 데이터를 동적으로 가져오는 제너릭 메서드
        /// </summary>
        public T GetSettings<T>(string tableName, string guildId) where T : new()
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                var query = $"SELECT * FROM {tableName} WHERE DiscordGuildId = @GuildId LIMIT 1";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@GuildId", guildId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var item = new T();
                            var properties = typeof(T).GetProperties();

                            foreach (var prop in properties)
                            {
                                if (!reader.HasColumn(prop.Name) || reader[prop.Name] == DBNull.Value)
                                    continue;
                                prop.SetValue(item, Convert.ChangeType(reader[prop.Name], prop.PropertyType));
                            }
                            return item;
                        }
                    }
                }
            }
            return default; // 설정이 없을 경우 기본값 반환
        }
    }

    /// <summary>
    /// MySQL DataReader 확장 메서드 (컬럼 존재 여부 확인)
    /// </summary>
    public static class DataReaderExtensions
    {
        public static bool HasColumn(this IDataRecord reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}
