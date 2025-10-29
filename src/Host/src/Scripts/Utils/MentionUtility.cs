
using Discord;

public static class MentionUtility
{
    public static string MentionUser(ulong id)
    {
        return $"<@{id}>";
    }
    public static string MentionRole(ulong id)
    {
        return $"<@&{id}>";
    }
    public static string MentionChannel(ulong id)
    {
        return $"<#{id}>";
    }
    public static string GetChannelMentionAndName(IChannel channel)
    {
        return $"<#{channel.Id}> ({channel.Name})";
    }
}