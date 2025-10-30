using Discord.WebSocket;
using System.Threading.Tasks;

namespace ManagerBot.Features;

public class HostContext : IHostContext
{
    public DiscordSocketClient discordSocketClient;
    public DiscordSocketClient DiscordSocketClient => discordSocketClient;

    public HostContext(DiscordSocketClient discordSocketClient)
    {
        this.discordSocketClient = discordSocketClient;
    }

    public ValueTask<bool> UnloadFeature(IFeature feature)
    {
        return FeatureSystem.UnloadFeature(feature);
    }
}
