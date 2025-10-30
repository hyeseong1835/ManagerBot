using System.Threading.Tasks;

using Discord.WebSocket;

namespace ManagerBot.Features;

public interface IHostContext
{
    DiscordSocketClient DiscordSocketClient { get; }

    ValueTask<bool> UnloadFeature(IFeature feature);
}
