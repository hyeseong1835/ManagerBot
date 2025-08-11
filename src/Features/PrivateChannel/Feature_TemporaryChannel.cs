using System.Text.Json.Serialization;

using Discord;
using Discord.WebSocket;

using ManagerBot.Core;
using ManagerBot.Utils.DiscordHelper;
using ManagerBot.Utils.WaitingVoiceChannelUtils;

namespace ManagerBot.Features.PrivateChannelFeature;

public class PrivateChannelSetting
{
    [JsonPropertyName("categoryId")]
    public ulong CategoryId { get; set; } = 0;

    [JsonPropertyName("parentChannelId")]
    public ulong ParentChannelId { get; set; } = 0;

    [JsonPropertyName("waitingVoiceChannelId")]
    public ulong WaitingVoiceChannelId { get; set; } = 0;
}

[FeatureInfo("TemporaryChannel")]
public class Feature_TemporaryChannel : Feature
{
    // 싱글턴 인스턴스
    static Feature_TemporaryChannel? instance;
    public static Feature_TemporaryChannel Instance => instance!;

    // 설정
    static PrivateChannelSetting? setting;
    public static PrivateChannelSetting Setting => setting!;

    // 스레드 부모 채널
    static SocketTextChannel? parentChannel;
    public static SocketTextChannel ParentChannel => parentChannel!;

    // 활성화된 임시 채널 목록
    internal static List<TemporaryChannel> activeTemporaryChannels = new();

    // 권한
    public static OverwritePermissions threadVoiceChannelEveryonePermissions =
        new OverwritePermissions(
            viewChannel: PermValue.Deny
        );
    public static OverwritePermissions threadVoiceChannelUserPermissions =
        new OverwritePermissions(
            viewChannel: PermValue.Allow
        );
    public static readonly ThreadArchiveDuration DefaultThreadArchiveDuration = ThreadArchiveDuration.OneDay;


    // 생성자
    public Feature_TemporaryChannel()
    {
        instance = this;
    }


    #region 이벤트

    public override async ValueTask Load()
    {
        // 설정 로드
        setting = await LoadSettingAsync<PrivateChannelSetting>()
            .ConfigureAwait(false);

        // 스레드 부모 채널 로드
        parentChannel = ChannelHelper.GetTextChannel(Feature_TemporaryChannel.Setting.ParentChannelId);

        // 대기실 음성 채널 로드
        WaitingVoiceChannel waitingVoiceChannel = WaitingVoiceChannel.Add(setting.WaitingVoiceChannelId);
        waitingVoiceChannel.onPrivateChannelCreated += async (PrivateWaitingVoiceChannel privateChannel) =>
        {
            await privateChannel.voiceChannel.SendMessageAsync(
                $"비공개 음성 채널이 생성되었습니다."
            );
        };

        // 이벤트 구독
        ManagerBotCore.client.ThreadDeleted += OnThreadDeleted;

        ManagerBotCore.client.ThreadMemberJoined += OnThreadMemberJoined;
        ManagerBotCore.client.ThreadMemberLeft += OnThreadMemberLeft;
    }

    static Task OnThreadDeleted(Cacheable<SocketThreadChannel, ulong> thread)
    {
        if (thread.HasValue == false) return Task.CompletedTask;
        if (thread.Value.ParentChannel.Id != Feature_TemporaryChannel.Setting.ParentChannelId) return Task.CompletedTask;

        TemporaryChannel? tempChannel = FindWithThreadChannelId(thread.Id);
        if (tempChannel == null)
            return Task.CompletedTask;

        // 음성 채널이 있다면 삭제
        if (tempChannel.voiceChannel != null)
        {
            tempChannel.DeleteVoiceChannel();
        }

        // 활성화된 임시 채널 목록에서 제거
        activeTemporaryChannels.Remove(tempChannel);
        return Task.CompletedTask;
    }

    static Task OnThreadMemberJoined(SocketThreadUser user)
    {
        if (user.Thread.ParentChannel.Id != Feature_TemporaryChannel.Setting.ParentChannelId)
            return Task.CompletedTask;

        return SyncVoiceChannelUsersToThread(user.Thread.Id);
    }
    static Task OnThreadMemberLeft(SocketThreadUser user)
    {
        if (user.Thread.ParentChannel.Id != Feature_TemporaryChannel.Setting.ParentChannelId)
            return Task.CompletedTask;

        return SyncVoiceChannelUsersToThread(user.Thread.Id);
    }
    static Task SyncVoiceChannelUsersToThread(ulong threadId)
    {
        TemporaryChannel? channel = FindWithThreadChannelId(threadId);
        if (channel == null)
            return Task.CompletedTask;

        if (channel.voiceChannel == null)
            return Task.CompletedTask;

        return channel.SyncVoiceChannelUserToThread();
    }

    #endregion


    #region 찾기

    static TemporaryChannel? FindWithVoiceChannelId(ulong id)
    {
        for (int i = 0; i < activeTemporaryChannels.Count; i++)
        {
            TemporaryChannel channel = activeTemporaryChannels[i];
            if (channel.voiceChannel == null) continue;

            if (channel.voiceChannel.Id == id)
            {
                return channel;
            }
        }

        return null;
    }
    static TemporaryChannel? FindWithThreadChannelId(ulong id)
    {
        for (int i = 0; i < activeTemporaryChannels.Count; i++)
        {
            TemporaryChannel channel = activeTemporaryChannels[i];
            if (channel.thread.Id == id)
            {
                return channel;
            }
        }

        return null;
    }

    #endregion
}