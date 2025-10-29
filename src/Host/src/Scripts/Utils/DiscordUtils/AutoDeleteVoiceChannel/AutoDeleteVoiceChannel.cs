using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Discord;
using Discord.WebSocket;

using ManagerBot.Core;

namespace ManagerBot.Utils.AutoDeleteVoiceChannelUtils;

public struct AutoDeleteVoiceChannel
{
    static List<AutoDeleteVoiceChannel> channels = new();

    #region 이벤트

    [OnInitializeMethod]
    static ValueTask Initialize()
    {
        ManagerBotCore.client.UserVoiceStateUpdated += UserVoiceStateUpdated;
        ManagerBotCore.client.ChannelDestroyed += ChannelDestroyed;

        return ValueTask.CompletedTask;
    }

    static Task UserVoiceStateUpdated(SocketUser user, SocketVoiceState before, SocketVoiceState after)
    {
        if (before.VoiceChannel == null)
            return Task.CompletedTask;

        return AutoDeleteChannels();
    }
    static async Task AutoDeleteChannels()
    {
        for (int i = channels.Count - 1; i >= 0; i--)
        {
            AutoDeleteVoiceChannel channel = channels[i];
            if (channel.voiceChannel.ConnectedUsers.Count <= channel.playerCount)
            {
                if (channel.onDeleted != null)
                {
                    await channel.onDeleted.Invoke();
                }
                await channel.voiceChannel.DeleteAsync();
                continue;
            }
        }
    }

    static Task ChannelDestroyed(SocketChannel channel)
    {
        if (channel.ChannelType != ChannelType.Voice)
            return Task.CompletedTask;

        for (int i = channels.Count - 1; i >= 0; i--)
        {
            if (channels[i].voiceChannel.Id == channel.Id)
            {
                channels.RemoveAt(i);
                return Task.CompletedTask;
            }
        }

        return Task.CompletedTask;
    }

    #endregion


    public static void Regist(SocketVoiceChannel voiceChannel, int playerCount, Func<ValueTask>? onDeleted = null)
    {
        for (int i = 0; i < channels.Count; i++)
        {
            if (channels[i].voiceChannel.Id == voiceChannel.Id)
            {
                return;
            }
        }

        AutoDeleteVoiceChannel autoDeleteChannel = new AutoDeleteVoiceChannel(voiceChannel, playerCount, onDeleted);
        channels.Add(autoDeleteChannel);
    }
    public static void Unregist(SocketVoiceChannel voiceChannel)
    {
        for (int i = 0; i < channels.Count; i++)
        {
            if (channels[i].voiceChannel.Id == voiceChannel.Id)
            {
                channels.RemoveAt(i);
                return;
            }
        }
    }

    SocketVoiceChannel voiceChannel;
    int playerCount;

    Func<ValueTask>? onDeleted;


    AutoDeleteVoiceChannel(SocketVoiceChannel voiceChannel, int playerCount, Func<ValueTask>? onDeleted)
    {
        this.voiceChannel = voiceChannel;
        this.playerCount = playerCount;

        this.onDeleted = onDeleted;
    }
}
