using Discord;
using Discord.WebSocket;
using HS.Discord.Core;

namespace ManagerBot.Core;

public static class ManagerBotCore
{
    public readonly static DiscordSocketClient client = new DiscordSocketClient(
        new DiscordSocketConfig
        {
            MessageCacheSize = 100,
            GatewayIntents = GatewayIntents.All,
            AuditLogCacheSize = 0,
            LogLevel = LogSeverity.Info
        }
    );
    public readonly static ManagerBotSetting setting = ManagerBotSetting.Load(PathHelper.settingFilePath);
    public readonly static RawDiscordHttpClient rawClient = new RawDiscordHttpClient(setting.BotToken, new byte[4096]);
    public readonly static RawDiscordGuildHttpClient rawGuildClient = rawClient.CreateGuildClient(setting.GuildId);

    public static ValueTask Initialize()
    {
        return ValueTask.CompletedTask;
    }
}