using System;
using System.Data;
using MySql.Data.MySqlClient;
using surabot.Common;
using surabot.Utils;

namespace surabot.Models
{
    public class WelcomeMessageSetting
    {
        public int Id { get; private set; }
        public ulong DiscordGuildId { get; private set; }
        public string Title { get; private set; }
        public string Message { get; private set; }
        public bool IsEnabled { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public WelcomeMessageSetting()
        {
            Clear();
        }

        public void Clear()
        {
            Id = 0;
            DiscordGuildId = 0;
            Title = string.Empty;
            Message = string.Empty;
            IsEnabled = false;
            CreatedAt = DateTime.MinValue;
            UpdatedAt = DateTime.MinValue;
        }

    }
}
