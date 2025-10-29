
using Discord;

namespace ManagerBot.Utils.Extension.Discord.WebSocket;

public static class IChannelExtension
{
    public static string GetMention(this IChannel channel)
    {
        return MentionUtility.MentionChannel(channel.Id);
    }
}
