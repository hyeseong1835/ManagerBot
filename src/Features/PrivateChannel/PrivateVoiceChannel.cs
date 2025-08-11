using Discord;
using Discord.Rest;
using Discord.WebSocket;
using ManagerBot.Core;
using ManagerBot.Features.PrivateChannelFeature;

public class PrivateChannel
{
    public static async ValueTask<PrivateChannel> Create(SocketGuildUser owner, int whitelistCapacity = 1)
    {
        RestTextChannel textChannel = await ManagerBotCore.Guild.CreateTextChannelAsync(
            $"비공개 채팅 ({owner.DisplayName})",
            properties =>
            {
                properties.PermissionOverwrites = new Optional<IEnumerable<Overwrite>>(
                    new List<Overwrite>
                    {
                        new Overwrite(
                            owner.Id,
                            PermissionTarget.User,
                            new OverwritePermissions(viewChannel: PermValue.Allow)
                        )
                    }
                );
                properties.CategoryId = Feature_TemporaryChannel.Setting.CategoryId;
            }
        );

        RestVoiceChannel voicechannel = await owner.Guild.CreateVoiceChannelAsync(
            $"비공개 음성 채널 ({owner.DisplayName})",
            properties =>
            {
                properties.UserLimit = whitelistCapacity;
                properties.PermissionOverwrites = new Optional<IEnumerable<Overwrite>>(
                    new List<Overwrite>
                    {
                        new Overwrite(
                            owner.Id,
                            PermissionTarget.User,
                            new OverwritePermissions(viewChannel: PermValue.Allow)
                        )
                    }
                );
                properties.CategoryId = Feature_TemporaryChannel.Setting.CategoryId;
            }
        );

        List<ulong> whitelist = new(whitelistCapacity)
        {
            owner.Id
        };

        return new PrivateChannel(
            owner,
            ManagerBotCore.Guild.GetThreadChannel(textChannel.Id),
            ManagerBotCore.Guild.GetVoiceChannel(voicechannel.Id),
            whitelist
        );
    }

    public SocketGuildUser owner;
    public SocketThreadChannel textChannel;
    public SocketVoiceChannel voiceChannel;
    List<ulong> whitelist;

    PrivateChannel(SocketGuildUser owner, SocketThreadChannel textChannel, SocketVoiceChannel voiceChannel, List<ulong> whitelist)
    {
        this.owner = owner;
        this.textChannel = textChannel;
        this.voiceChannel = voiceChannel;
        this.whitelist = whitelist;
    }

    public void AddToWhitelist(ulong userId)
    {
        int min = 0;
        int max = whitelist.Count - 1;

        int mid;
        ulong midUserId;

        while (min <= max)
        {
            mid = (min + max) >> 1;
            midUserId = whitelist[mid];
            if (midUserId == userId)
            {
                return; // 이미 화이트리스트에 있음
            }
            else if (midUserId < userId)
            {
                min = mid + 1;
            }
            else
            {
                max = mid - 1;
            }
        }

        whitelist.Insert(min, userId);
    }
    public void RemoveFromWhitelist(ulong userId)
    {
        int min = 0;
        int max = whitelist.Count - 1;

        int mid;
        ulong midUserId;

        while (min <= max)
        {
            mid = (min + max) >> 1;
            midUserId = whitelist[mid];
            if (midUserId == userId)
            {
                whitelist.RemoveAt(mid);
                return; // 화이트리스트에서 제거됨
            }
            else if (midUserId < userId)
            {
                min = mid + 1;
            }
            else
            {
                max = mid - 1;
            }
        }
    }
    public void SetWhitelistCapacity(int capacity)
    {
        whitelist.Capacity = capacity;
    }

    public bool IsWhiteListed(ulong userId)
    {
        int min = 0;
        int max = whitelist.Count - 1;

        int mid;
        ulong midUserId;

        while (min <= max)
        {
            mid = (min + max) >> 1;
            midUserId = whitelist[mid];
            if (midUserId == userId)
            {
                return true;
            }
            else if (midUserId < userId)
            {
                min = mid + 1;
            }
            else
            {
                max = mid - 1;
            }
        }
        return false; // 화이트리스트에 없음
    }

    public async ValueTask UpdateAsync()
    {
        await voiceChannel.ModifyAsync(
            properties =>
            {
                properties.Name = $"비공개 음성 채널 ({owner.DisplayName})";
                properties.UserLimit = whitelist.Count;
                properties.PermissionOverwrites = new Optional<IEnumerable<Overwrite>>(
                    whitelist.Select(
                        userId => new Overwrite(
                            userId,
                            PermissionTarget.User,
                            new OverwritePermissions(viewChannel: PermValue.Allow)
                        )
                    )
                );
                properties.CategoryId = Feature_TemporaryChannel.Setting.CategoryId;
            }
        );
    }
}