using System.Text;
using Discord.WebSocket;

namespace ManagerBot.Core.Features;

public class PrivateChannel : Feature
{
    public override string Name => "Private Channel";

    public override async ValueTask Load()
    {
        Program.client.UserJoined += OnUserJoined;
        await UpdatePrivateChannel();
    }
    public static async Task OnUserJoined(SocketGuildUser user)
    {
        if (user.IsBot) return;

        await UpdatePrivateChannel();
    }
    public static string GetPrivateChannelName(SocketGuildUser user)
    {
        StringBuilder nameBuilder = new();
        for (int i = 0; i < user.Username.Length; i++)
        {
            char c = user.Username[i];
            if (CharRange.EnglishLowerCaseRange.IsRange(c)) nameBuilder.Append(c);
            else if (CharRange.NumberRange.IsRange(c)) nameBuilder.Append(c);
            else if (c == '_') nameBuilder.Append(c);
            else if (c == privateChannelNameSpliter) break;
        }
        if (nameBuilder.Length == 0) nameBuilder.Append("null");
        return $"{nameBuilder}{privateChannelNameSpliter}{user.Id}";
    }

    public static async Task UpdatePrivateChannel()
    {
        SocketCategoryChannel privateChannelCategory = GuildData.guild.GetCategoryChannel(privateChannelCategoryId);

        IReadOnlyCollection<SocketGuildUser> users = GuildData.guild.Users;
        HashSet<UserFindData> userWrapperHash = new(users.Count);

        foreach (SocketGuildUser user in users)
        {
            if (user.IsBot)
                continue;

            userWrapperHash.Add(new UserFindData(user));
        }

        foreach (SocketGuildChannel channel in privateChannelCategory.Channels)
        {
            int splitIndex = channel.Name.IndexOf(privateChannelNameSpliter);
            string userIdString = splitIndex == -1 ? channel.Name : channel.Name.Substring(splitIndex + 1);

            if (!ulong.TryParse(userIdString, out ulong userId)) continue;

            SocketGuildUser? user = GuildData.guild.GetUser(userId);
            if (user == null)
            {
                await channel.MoveToTrashBin();
                userWrapperHash.RemoveWhere(x => x.user.Id == userId);
                continue;
            }

            UserFindData? findData = userWrapperHash.FirstOrDefault(x => x.user.Id == userId);
            if (findData == null)
            {
                await channel.MoveToTrashBin();
                continue;
            }

            switch (channel.ChannelType)
            {
                case ChannelType.Text:
                    {
                        findData.textChannel = channel;
                        break;
                    }
                case ChannelType.Voice:
                    {
                        findData.voiceChannel = channel;
                        break;
                    }
                default:
                    {
                        await GuildData.LogError($"개인 채널 형식이 잘못되었습니다. {channel.Name}");
                        await channel.MoveToTrashBin();
                        continue;
                    }
            }
        }
        foreach (UserFindData userFindData in userWrapperHash)
        {
            if (userFindData.textChannel == null)
            {
                await CreatePrivateTextChannel(userFindData.user);
            }
            if (userFindData.voiceChannel == null)
            {
                await CreatePrivateVoiceChannel(userFindData.user);
            }
        }
    }
    static async Task CreatePrivateTextChannel(SocketGuildUser user)
    {
        await GuildData.guild.CreateTextChannelAsync(
            GetPrivateChannelName(user),
            x =>
            {
                x.CategoryId = privateChannelCategoryId;
                x.PermissionOverwrites = new List<Overwrite>()
                {
                    new Overwrite(
                        GuildData.guild.EveryoneRole.Id,
                        PermissionTarget.Role,
                        new OverwritePermissions(
                            viewChannel: PermValue.Deny
                        )
                    ),
                    new Overwrite(
                        user.Id,
                        PermissionTarget.User,
                        new OverwritePermissions(
                            sendMessages: PermValue.Allow,
                            viewChannel: PermValue.Allow
                        )
                    )
                };
            }
        );
    }
    static async Task CreatePrivateVoiceChannel(SocketGuildUser user)
    {
        await GuildData.guild.CreateVoiceChannelAsync(
            GetPrivateChannelName(user),
            x =>
            {
                x.CategoryId = privateChannelCategoryId;
                x.PermissionOverwrites = new List<Overwrite>()
                {
                    new Overwrite(
                        GuildData.guild.EveryoneRole.Id,
                        PermissionTarget.Role,
                        new OverwritePermissions(
                            viewChannel: PermValue.Deny
                        )
                    ),
                    new Overwrite(
                        user.Id,
                        PermissionTarget.User,
                        new OverwritePermissions(
                            viewChannel: PermValue.Allow
                        )
                    )
                };
            }
        );
    }
}