using System;
using System.Data;
using MySql.Data.MySqlClient;
using surabot.Common;
using surabot.Utils;

namespace surabot.Models
{
    public class ChzzkSetting
    {
        public int Id { get; private set; }
        public ulong DiscordGuildId { get; private set; }
        public ulong DiscordChannelId { get; private set; }
        public string ChzzkChannelId { get; private set; }
        public bool IsAlertEnabled { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public ChzzkSetting()
        {
            Clear();
        }

        public void Clear()
        {
            Id = 0;
            DiscordGuildId = 0;
            DiscordChannelId = 0;
            ChzzkChannelId = string.Empty;
            IsAlertEnabled = false;
            CreatedAt = DateTime.MinValue;
            UpdatedAt = DateTime.MinValue;
        }
    }
}
