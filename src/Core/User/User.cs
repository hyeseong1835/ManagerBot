using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

using Discord;

using HS.Common.IO.AsciiJson;

namespace ManagerBot.Core.User;

public class ServerUser
{
    #region Static

    readonly static string userJsonPath = $"{PathHelper.DataDirectoryPath}/users.json";
    static ServerUser[]? users;

    [OnInitializeMethod(0)]
    public static async ValueTask Initialize()
    {
        // 파일이 없으면 생성
        users = await LoadUsers();
        await SaveAsync();
    }

    [OnStopMethod(0)]
    public static async ValueTask SaveAsync()
    {
        if (users == null)
            throw new InvalidOperationException("유저 정보가 초기화되지 않았습니다.");

        using (FileStream stream = File.Open(userJsonPath, FileMode.Create, FileAccess.Write))
        {
            Utf8JsonWriter writer = new(stream, new JsonWriterOptions { Indented = true });
            writer.WriteStartArray();
            {
                for (int i = 0; i < users.Length; i++)
                {
                    writer.WriteStartObject();
                    {
                        writer.WriteNumber("roleId", users[i]._roleId);

                        writer.WritePropertyName("accountIds");
                        writer.WriteStartArray();
                        {
                            for (int j = 0; j < users[i]._accountIds.Length; j++)
                            {
                                writer.WriteNumberValue(users[i]._accountIds[j]);
                            }
                        }
                        writer.WriteEndArray();
                    }
                    writer.WriteEndObject();
                }
            }
            writer.WriteEndArray();

            await writer.FlushAsync();
        }
    }

    static async ValueTask<ServerUser[]> LoadUsers()
    {
        SortedList<ulong, ServerUser> userList = new();
        Utf8JsonReader userJsonReader;
        List<ulong> accountIds = new();

        if (File.Exists(userJsonPath))
        {
            using (FileStream stream = File.Open(userJsonPath, FileMode.OpenOrCreate, FileAccess.Read))
            {
                AsciiJsonArrayStreamAsyncReader reader = new(stream);
                AsciiJsonArrayStreamAsyncReader.ReadResult result;
                while (true)
                {
                    result = await reader.ReadAsync();

                    if (result.isEndRead)
                        break;

                    userJsonReader = new Utf8JsonReader(result.jsonElementBytes.Span, isFinalBlock: result.isEndRead, state: default);
                    ulong roleId = 0;
                    while (userJsonReader.Read())
                    {
                        if (userJsonReader.TokenType == JsonTokenType.PropertyName)
                        {
                            if (userJsonReader.ValueSpan.SequenceEqual("roleId"u8))
                            {
                                userJsonReader.Read();
                                roleId = userJsonReader.GetUInt64();
                            }
                            else if (userJsonReader.ValueSpan.SequenceEqual("accountIds"u8))
                            {
                                userJsonReader.Read();
                                accountIds.Clear();

                                while (userJsonReader.Read())
                                {
                                    if (userJsonReader.TokenType == JsonTokenType.EndArray)
                                        break;

                                    accountIds.Add(userJsonReader.GetUInt64());
                                }

                            }
                        }
                    }
                    userList.Add(accountIds[0], new ServerUser(roleId, accountIds.ToArray()));
                }
            }
        }

        await foreach (IReadOnlyCollection<IGuildUser>? users in ManagerBotCore.Guild.GetUsersAsync())
        {
            if (users == null)
                continue;

            foreach (IGuildUser user in users)
            {
                if (user.IsBot)
                    continue;

                userList.TryAdd(user.Id, new ServerUser(0, [user.Id]));
            }
        }

        return userList.Values.ToArray();
    }

    public static ServerUser GetUserByRoleId(ulong roleId)
    {
        if (roleId == 0)
            throw new ArgumentOutOfRangeException(nameof(roleId), "찾을 역할의 ID는 0이 될 수 없습니다.");

        if (users == null)
            throw new InvalidOperationException("유저 정보가 초기화되지 않았습니다.");

        foreach (ServerUser user in users)
        {
            if (user._roleId == roleId)
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
