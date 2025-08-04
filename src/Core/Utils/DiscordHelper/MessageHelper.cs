using Discord;
using Discord.WebSocket;

namespace ManagerBot.Core.Utils.DiscordHelper;

public class MessageHelper
{
    public static Task<IMessage?> GetMessageAsync(SocketTextChannel channel, ulong messageId)
    {
        if (channel == null)
            return Task.FromResult<IMessage?>(null);

        if (messageId == 0)
            return Task.FromResult<IMessage?>(null);

        return channel.GetMessageAsync(messageId);
    }
}