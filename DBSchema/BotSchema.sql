CREATE TABLE bot_settings (
    id INT AUTO_INCREMENT PRIMARY KEY,
    discord_guild_id BIGINT NOT NULL,
    setting_key VARCHAR(255) NOT NULL,
    setting_value TEXT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UNIQUE (discord_guild_id, setting_key)
);

CREATE TABLE bot_logs (
    id INT AUTO_INCREMENT PRIMARY KEY,
    discord_guild_id BIGINT NOT NULL,
    log_type VARCHAR(50) NOT NULL,  -- 예: 'command', 'error', 'warning'
    message TEXT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE bot_welcome_messages (
    id INT AUTO_INCREMENT PRIMARY KEY,
    discord_guild_id BIGINT NOT NULL UNIQUE,
    title VARCHAR(255) NOT NULL,
    message TEXT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

CREATE TABLE bot_chzzk_alerts (
    id INT AUTO_INCREMENT PRIMARY KEY,
    discord_guild_id BIGINT NOT NULL,
    stream_channel_id VARCHAR(255) NOT NULL,
    stream_title VARCHAR(255) NOT NULL,
    stream_url VARCHAR(255) NOT NULL,
    is_enabled TINYINT(1) DEFAULT 1,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);
