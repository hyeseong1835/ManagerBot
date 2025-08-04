using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace ManagerBot.Core.Utils.DiscordHelper;

public static class ChannelHelper
{
    public static SocketTextChannel GetTextChannel(ulong id)
    {
        if (id == 0)
            throw new ArgumentException("채널 ID는 0이 될 수 없습니다.", nameof(id));

        SocketTextChannel? channel = ManagerBotCore.Guild.GetTextChannel(id);
        if (channel == null)
            throw new InvalidOperationException($"ID '{id}'에 해당하는 텍스트 채널을 찾을 수 없습니다.");

        return channel;
    }
    public static SocketVoiceChannel GetVoiceChannel(ulong id)
    {
        if (id == 0)
            throw new ArgumentException("채널 ID는 0이 될 수 없습니다.", nameof(id));

        SocketVoiceChannel? channel = ManagerBotCore.Guild.GetVoiceChannel(id);
        if (channel == null)
            throw new InvalidOperationException($"ID '{id}'에 해당하는 음성 채널을 찾을 수 없습니다.");

        return channel;
    }

    public static Task<RestTextChannel> CreateTextChannel(string name,
        Action<TextChannelProperties>? func = null, RequestOptions? options = null)
    {
        return ManagerBotCore.Guild.CreateTextChannelAsync(name, func, options);
    }
}