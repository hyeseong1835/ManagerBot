public class GroupData
{
    public string? GroupName { get; private init; }
    public ulong RoleId { get; private init; }
    public ulong ChannelId { get; private init; }
    public ulong OwnerId { get; private init; }

    public GroupData(string groupName, ulong roleId, ulong channelId, ulong ownerId)
    {
        GroupName = groupName;
        RoleId = roleId;
        ChannelId = channelId;
        OwnerId = ownerId;
    }
}