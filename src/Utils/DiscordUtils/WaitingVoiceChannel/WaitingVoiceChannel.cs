using Discord.WebSocket;

using ManagerBot.Core.Utils.DiscordHelper;

namespace ManagerBot.Core.Utils.WaitingVoiceChannelUtils;

public class WaitingVoiceChannel
{
    public static List<WaitingVoiceChannel> instances = new(1);


    [OnBotInitializeMethod]
    public static void Initialize()
    {
        ManagerBotCore.client.UserVoiceStateUpdated += UserVoiceStateUpdated;
    }
    static Task UserVoiceStateUpdated(SocketUser user, SocketVoiceState before, SocketVoiceState after)
    {
        if (after.VoiceChannel == null)
            return Task.CompletedTask;

        ulong joinedChannelId = after.VoiceChannel.Id;
        WaitingVoiceChannel? waitingVoiceChannel = WaitingVoiceChannel.Find(joinedChannelId);
        if (waitingVoiceChannel == null)
            return Task.CompletedTask;

        return PrivateWaitingVoiceChannel.MoveToNewPrivateChannel(waitingVoiceChannel, (SocketGuildUser)user).AsTask();
    }

    static WaitingVoiceChannel? Find(ulong voiceChannelId)
    {
        if (voiceChannelId == 0)
            return null;

        int min = 0;
        int max = instances.Count - 1;

        while (min <= max)
        {
            int mid = (min + max) >> 1;
            WaitingVoiceChannel waitingVoiceChannel = instances[mid];
            if (waitingVoiceChannel.voiceChannel.Id == voiceChannelId)
                return waitingVoiceChannel;

            if (waitingVoiceChannel.voiceChannel.Id < voiceChannelId)
            {
                min = mid + 1;
            }
            else
            {
                max = mid - 1;
            }
        }

        return null;
    }
    public static WaitingVoiceChannel Add(SocketVoiceChannel voiceChannel)
    {
        WaitingVoiceChannel channel = new WaitingVoiceChannel(voiceChannel);
        instances.Add(channel);
        return channel;
    }
    public static WaitingVoiceChannel Add(ulong voiceChannelId)
        => Add(ChannelHelper.GetVoiceChannel(voiceChannelId));



    public readonly SocketVoiceChannel voiceChannel;

    public Func<PrivateWaitingVoiceChannel, ValueTask>? onPrivateChannelCreated;

    WaitingVoiceChannel(SocketVoiceChannel voiceChannel)
    {
        this.voiceChannel = voiceChannel;
    }
}