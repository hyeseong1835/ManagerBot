using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace ManagerBot.Core.Utils.WaitingVoiceChannelUtils;

public class WaitingVoiceChannel
{
    public static List<WaitingVoiceChannel> waitingVoiceChannels = new(1);
    public static List<SocketVoiceChannel> privateWaitingVoiceChannels = new(1);


    [OnBotInitializeMethod]
    public static void Initialize()
    {
        ManagerBotCore.client.UserVoiceStateUpdated += UserVoiceStateUpdated;
    }
    static Task UserVoiceStateUpdated(SocketUser user, SocketVoiceState before, SocketVoiceState after)
    {
        ulong joinedChannelId = after.VoiceChannel.Id;
        WaitingVoiceChannel? waitingVoiceChannel = WaitingVoiceChannel.Find(joinedChannelId);
        if (waitingVoiceChannel == null)
            return Task.CompletedTask;

        waitingVoiceChannel.voiceChannel = after.VoiceChannel;
        return waitingVoiceChannel.MoveToNewPrivateChannel((SocketGuildUser)user).AsTask();
    }

    static WaitingVoiceChannel? Find(ulong voiceChannelId)
    {
        for (int i = 0; i < waitingVoiceChannels.Count; i++)
        {
            WaitingVoiceChannel waitingVoiceChannel = waitingVoiceChannels[i];
            if (waitingVoiceChannel.voiceChannel.Id == voiceChannelId)
                return waitingVoiceChannel;
        }
        return null;
    }
    public static WaitingVoiceChannel Add(SocketVoiceChannel voiceChannel)
    {
        WaitingVoiceChannel channel = new WaitingVoiceChannel(voiceChannel);
        waitingVoiceChannels.Add(channel);
        return channel;
    }
    public static WaitingVoiceChannel Add(ulong voiceChannelId)
        => Add(ChannelHelper.GetVoiceChannel(voiceChannelId));



    SocketVoiceChannel voiceChannel;
    List<PrivateWaitingVoiceChannel> privateChannels = new();

    public Func<PrivateWaitingVoiceChannel, ValueTask>? onPrivateChannelCreated;

    public WaitingVoiceChannel(SocketVoiceChannel voiceChannel)
    {
        this.voiceChannel = voiceChannel;
    }

    public async ValueTask MoveToNewPrivateChannel(SocketGuildUser targetUser)
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
            $"{voiceChannel.Name} ({targetUser.DisplayName})",
            properties =>
            {
                properties.CategoryId = voiceChannel.Category?.Id;
                properties.Position = voiceChannel.Position;
                properties.PermissionOverwrites = overwriteBuffer;
                PermissionHelper.ReturnSingleOverwriteBuffer(overwriteBuffer);
                properties.UserLimit = 1;
            }
        );
        await targetUser.ModifyAsync(
            properties =>
            {
                properties.Channel = voiceChannel;
            }
        );
        if (onPrivateChannelCreated != null)
        {
            await onPrivateChannelCreated.Invoke(
                new PrivateWaitingVoiceChannel(
                    privateVoiceChannel,
                    targetUser
                )
            );
        }
    }
}