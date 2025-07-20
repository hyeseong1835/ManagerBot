using HS.Discord.Core.Resources.SendJson;
using ManagerBot.Core;

public class Group
{
    public static async ValueTask<Group> Create(ReadOnlyMemory<byte> nameU8, ulong ownerId)
    {
        await ManagerBotCore.rawGuildClient.CreateGuildRole(
            new CreateGuildRoleJsonDocument(
                new(nameU8),
                new()
            )
        );
        GroupData data = new GroupData(nameU8, ownerId);
    }

    public GroupData groupData;


    public Group(GroupData data)
    {
        groupData = data;
    }
}