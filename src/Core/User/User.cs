using Discord;
using System.Text.Json;

namespace ManagerBot.Core.User;

public class ServerUser
{
    #region Static

    readonly static string userJsonPath = $"{PathHelper.DataDirectoryPath}/users.json";
    static ServerUser[]? users;

    [OnBotInitializeMethod(0)]
    public static async ValueTask Initialize()
    {
        // 파일이 없으면 생성
        users = await LoadUsers();
    }
    public static async ValueTask SaveAsync()
    {
        if (users == null)
            throw new InvalidOperationException("유저 정보가 초기화되지 않았습니다.");

        using (FileStream stream = File.Open(userJsonPath, FileMode.Create, FileAccess.Write))
        {
            await JsonSerializer.SerializeAsync(stream, users, JsonHelper.readableJsonSerializationOptions);
        }
    }

    static async ValueTask<ServerUser[]> LoadUsers()
    {
        SortedList<ulong, ServerUser> userList = new();

        if (File.Exists(userJsonPath))
        {
            using (FileStream stream = File.Open(userJsonPath, FileMode.OpenOrCreate, FileAccess.Read))
            {
                Utf8JsonReader reader = new();
            }
        }
        else
        {
            ServerUser[] newUsers = new ServerUser[userList.Count];
            userList.Values.CopyTo(newUsers, 0);

            return newUsers;
        }

        await foreach (IReadOnlyCollection<IGuildUser>? users in ManagerBotCore.Guild.GetUsersAsync())
        {
            if (users == null)
                continue;

            foreach (IGuildUser user in users)
            {
                if (user.IsBot)
                    continue;

                userList.Add(user.Id, new ServerUser(0, [user.Id]));
            }
        }
    }

    public static ServerUser GetUserByRoleId(ulong roleId)
    {
        if (users == null)
            throw new InvalidOperationException("유저 정보가 초기화되지 않았습니다.");

        foreach (ServerUser user in users)
        {
            if (user._accountIds.Contains(accountId))
                return user;
        }

        throw new KeyNotFoundException("해당 계정 ID에 대한 유저를 찾을 수 없습니다.");
    }

    #endregion


    #region Instance

    readonly ulong _roleId;

    ulong[] _accountIds;

    public ServerUser(ulong roleId, ulong[] accountIds)
    {
        _roleId = roleId;
        _accountIds = accountIds;
    }

    #endregion
}
