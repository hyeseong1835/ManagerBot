
namespace Discord.WebSocket;
public static class SocketRoleUtility
{
    public static string ToString(this SocketRole role)
    {
        return $"{role.Name} [{role.GetMention()}]";
    }
    public static string GetMention(this SocketRole role)
    {
        return MentionUtility.MentionRole(role.Id);
    }
    public static EmbedBuilder GetDebugEmbedBuilder(this SocketRole role)
    {
        return new EmbedBuilder()
        {
            Title = role.GetMention(),
            Color = role.Color,
            Fields = new List<EmbedFieldBuilder>
            {
                new EmbedFieldBuilder()
                {
                    Name = "ID",
                    Value = $"{role.Id}",
                    IsInline = true
                },
                new EmbedFieldBuilder()
                {
                    Name = "Color",
                    Value = role.Color,
                    IsInline = true
                },
                new EmbedFieldBuilder()
                {
                    Name = "Position",
                    Value = role.Position,
                    IsInline = true
                },
                new EmbedFieldBuilder()
                {
                    Name = "Permissions",
                    Value = role.Permissions,
                    IsInline = true
                },
                new EmbedFieldBuilder()
                {
                    Name = "CreatedAt",
                    Value = role.CreatedAt,
                    IsInline = true
                },
                new EmbedFieldBuilder()
                {
                    Name = "IsHoisted",
                    Value = role.IsHoisted,
                    IsInline = true
                },
                new EmbedFieldBuilder()
                {
                    Name = "IsMentionable",
                    Value = role.IsMentionable,
                    IsInline = true
                },
                new EmbedFieldBuilder()
                {
                    Name = "IsManaged",
                    Value = role.IsManaged,
                    IsInline = true
                },
                new EmbedFieldBuilder()
                {
                    Name = "Members",
                    Value = string.Join('\n', role.Members.Where(u => !u.IsBot).Select(u => u.Mention)),
                },
            }
        };
    }
    public static EmbedFieldBuilder GetDebugEmbedFieldBuilder(this SocketRole role)
    {
        return new EmbedFieldBuilder()
        {
            Name = role.Name,
            Value = $"ID: {role.Id}\n" +
                    $"Color: {role.Color}\n" +
                    $"Position: {role.Position}\n" +
                    $"Members: {string.Join(',', role.Members.Select(u => u.Mention))}\n" +
                    $"Permissions: {role.Permissions}\n" +
                    $"CreatedAt: {role.CreatedAt}\n" +
                    $"IsHoisted: {role.IsHoisted}\n" +
                    $"IsMentionable: {role.IsMentionable}\n" +
                    $"IsManaged: {role.IsManaged}"
        };
    }
    public static EmbedFieldBuilder GetEmbedFieldBuilder(this SocketRole role)
    {
        return new EmbedFieldBuilder()
        {
            Name = role.Name,
            Value = $"ID: {role.Id}\n" +
                    $"Members: {string.Join(',', role.Members.Select(u => u.Mention))}\n" +
                    $"Permissions: {role.Permissions}\n" +
                    $"CreatedAt: {role.CreatedAt}\n"
        };
    }
}
