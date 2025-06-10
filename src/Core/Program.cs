using Discord;
using Discord.WebSocket;

namespace ManagerBot.Core;

public class Program
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

    public static void Main(string[] args)
    {

    }
}