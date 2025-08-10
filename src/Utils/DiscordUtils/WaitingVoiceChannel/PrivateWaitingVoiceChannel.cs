using Discord;
using Discord.Rest;
using Discord.WebSocket;
using ManagerBot.Core.Utils.DiscordHelper;

namespace ManagerBot.Core.Utils.WaitingVoiceChannelUtils;

public struct PrivateWaitingVoiceChannel
{
    public ref struct Handle
    {
        public readonly PrivateWaitingVoiceChannel channel;
        int index;

        public bool IsNull => (channel.voiceChannel == null);

        public Handle(PrivateWaitingVoiceChannel channel, int index)
        {
            this.channel = channel;
            this.index = index;
        }
        public int GetIndex()
        {
            if (IsNull)
                throw new InvalidOperationException("PrivateWaitingVoiceChannelHandle가 null입니다.");

            if (index < 0 || index >= PrivateWaitingVoiceChannel.InstanceCount)
            {
                return PrivateWaitingVoiceChannel.Find(channel.voiceChannel.Id).index;
            }
            else
            {
                PrivateWaitingVoiceChannel privateWaitingChannel = PrivateWaitingVoiceChannel.instances[index];
                if (privateWaitingChannel.voiceChannel.Id != channel.voiceChannel.Id)
                {
                    return PrivateWaitingVoiceChannel.Find(channel.voiceChannel.Id).index;
                }
                return index;
            }
        }
        public Task DeleteAsync()
        {
            if (IsNull)
                throw new InvalidOperationException("PrivateWaitingVoiceChannelHandle가 null입니다.");

            PrivateWaitingVoiceChannel.instances.RemoveAt(GetIndex());
            return channel.voiceChannel.DeleteAsync();
        }
    }

    static List<PrivateWaitingVoiceChannel> instances = new(1);
    public static int InstanceCount => instances.Count;

    [OnBotInitializeMethod]
    public static void Initialize()
    {
        ManagerBotCore.client.UserVoiceStateUpdated += UserVoiceStateUpdated;
    }
    static Task UserVoiceStateUpdated(SocketUser user, SocketVoiceState before, SocketVoiceState after)
    {
        if (before.VoiceChannel == null)
            return Task.CompletedTask;

        ulong originalChannelId = before.VoiceChannel.Id;
        Handle privateWaitingChannelHandle = PrivateWaitingVoiceChannel.Find(originalChannelId);
        if (privateWaitingChannelHandle.IsNull)
            return Task.CompletedTask;

        return privateWaitingChannelHandle.DeleteAsync();
    }

    [OnBotStopMethod]
    public static async ValueTask Stop()
    {
        for (int i = 0; i < instances.Count; i++)
        {
            PrivateWaitingVoiceChannel privateChannel = instances[i];
            await privateChannel.voiceChannel.DeleteAsync();
        }
        instances.Clear();
    }

    public static Handle Find(ulong voiceChannelId)
    {
        if (voiceChannelId == 0)
            return default;

        int min = 0;
        int max = instances.Count - 1;
        while (min <= max)
        {
            int mid = (min + max) >> 1;
            PrivateWaitingVoiceChannel privateWaitingChannel = instances[mid];
            if (privateWaitingChannel.voiceChannel.Id == voiceChannelId)
                return new Handle(privateWaitingChannel, mid);

            if (privateWaitingChannel.voiceChannel.Id < voiceChannelId)
            {
                min = mid + 1;
            }
            else
            {
                max = mid - 1;
            }
        }
        return default;
    }

    public readonly WaitingVoiceChannel origin;
    public readonly RestVoiceChannel voiceChannel;
    public readonly SocketGuildUser targetUser;

    PrivateWaitingVoiceChannel(WaitingVoiceChannel origin, RestVoiceChannel voiceChannel, SocketGuildUser targetUser)
    {
        this.origin = origin;
        this.voiceChannel = voiceChannel;
        this.targetUser = targetUser;
    }


    public static async ValueTask<PrivateWaitingVoiceChannel> MoveToNewPrivateChannel(WaitingVoiceChannel origin, SocketGuildUser targetUser)
    {
        Overwrite[] overwriteBuffer = PermissionHelper.RentSingleOverwriteBuffer();
        overwriteBuffer[0] = new Overwrite(
            targetUser.Id,
            PermissionTarget.User,
            PermissionHelper.denyAllPermissions.Modify(
                viewChannel: PermValue.Allow,
                connect: PermValue.Allow
            )
        );
        RestVoiceChannel privateVoiceChannel = await ManagerBotCore.Guild.CreateVoiceChannelAsync(
            $"{origin.voiceChannel.Name} ({targetUser.DisplayName})",
            properties =>
            {
                properties.CategoryId = origin.voiceChannel.Category?.Id;
                properties.Position = origin.voiceChannel.Position;
                properties.PermissionOverwrites = overwriteBuffer;
                PermissionHelper.ReturnSingleOverwriteBuffer(overwriteBuffer);
                properties.UserLimit = 1;
            }
        );
        PrivateWaitingVoiceChannel privateChannel = new(origin, privateVoiceChannel, targetUser);
        instances.Add(privateChannel);

        await targetUser.ModifyAsync(
            properties =>
            {
                properties.Channel = privateVoiceChannel;
            }
        );
        if (origin.onPrivateChannelCreated != null)
        {
            await origin.onPrivateChannelCreated.Invoke(privateChannel);
        }

        return privateChannel;
    }
}
