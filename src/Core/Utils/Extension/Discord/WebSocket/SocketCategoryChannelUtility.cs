
namespace Discord.WebSocket;
public static class SocketCategoryChannelUtility
{
    public static string GetMention(this SocketCategoryChannel category)
    {
        return MentionUtility.MentionChannel(category.Id);
    }
    public static bool ContainsChannel(this SocketCategoryChannel category, ulong channelId)
    {
        foreach (SocketGuildChannel channel in category.Channels)
        {
            if (channel.Id == channelId) return true;
        }
        return false;
    }
    public static EmbedBuilder GetDebugEmbedBuilder(this SocketCategoryChannel category)
    {
        return new EmbedBuilder()
        {
            Title = MentionUtility.MentionChannel(category.Id),
            Color = Color.Blue,
            Fields = new List<EmbedFieldBuilder>
            {
                new EmbedFieldBuilder()
                {
                    Name = "ID",
                    Value = $"{category.Id}",
                    IsInline = true
                },
                new EmbedFieldBuilder()
                {
                    Name = "Position",
                    Value = category.Position,
                    IsInline = true
                },
                new EmbedFieldBuilder()
                {
                    Name = "Flags",
                    Value = category.Flags,
                    IsInline = true
                },
                new EmbedFieldBuilder()
                {
                    Name = "CreatedAt",
                    Value = category.CreatedAt,
                    IsInline = true
                },
                new EmbedFieldBuilder()
                {
                    Name = "Users",
                    Value = string.Join('\n', category.Users.Where(u => !u.IsBot).Select(u => u.Mention)),
                },
                new EmbedFieldBuilder()
                {
                    Name = "Channels",
                    Value = string.Join('\n', category.Channels.Select(c => $"<#{c.Id}> ({c.Flags})")),
                },
                new EmbedFieldBuilder()
                {
                    Name = "PermissionOverwrites",
                    Value = string.Join('\n', category.PermissionOverwrites.Select(o => $"{o.TargetMention()}: {o.Permissions}"))
                }
            }
        };
    }
    public static EmbedFieldBuilder GetDebugEmbedFieldBuilder(this SocketCategoryChannel category)
    {
        return new EmbedFieldBuilder()
        {
            Name = category.Name,
            Value = $"ID: {category.Id}\n" +
                    $"Position: {category.Position}\n" +
                    $"Flags: {category.Flags}\n" +
                    $"Users: {string.Join(',', category.Users.Select(u => u.Mention))}\n" +
                    $"Channels: {string.Join(',', category.Channels.Select(c => $"{c.Name} ({c.Flags})"))}\n" +
                    $"CreatedAt: {category.CreatedAt}\n" +
                    $"PermissionOverwrites: {string.Join(',', category.PermissionOverwrites.Select(o => $"{o.TargetMention()}: {o.Permissions}"))}"
        };
    }
    public static EmbedFieldBuilder GetEmbedFieldBuilder(this SocketCategoryChannel category)
    {
        return new EmbedFieldBuilder()
        {
            Name = $"<#{category.Id}>",
            Value = $"ID: {category.Id}\n" +
                    $"Users: {string.Join(',', category.Users.Select(u => u.Mention))}\n" +
                    $"CreatedAt: {category.CreatedAt}\n"
        };
    }
}