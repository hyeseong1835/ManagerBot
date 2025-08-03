using Discord.Rest;
using Discord.WebSocket;

namespace ManagerBot.Core.Utils.WaitingVoiceChannelUtils;

public struct PrivateWaitingVoiceChannel
{
    public readonly RestVoiceChannel voiceChannel;
    public readonly SocketGuildUser targetUser;

    public PrivateWaitingVoiceChannel(RestVoiceChannel voiceChannel, SocketGuildUser targetUser)
    {
        this.voiceChannel = voiceChannel;
        this.targetUser = targetUser;
    }
}