using System.Text.Json.Serialization;

using Discord;
using Discord.WebSocket;

using ManagerBot.Core;
using ManagerBot.Utils.DiscordHelper;

namespace ManagerBot.Features.PrivateChannelFeature;

public class TemporaryChannelSetting
{
    [JsonPropertyName("categoryId")]
    public ulong CategoryId { get; set; } = 0;

    [JsonPropertyName("parentChannelId")]
    public ulong ParentChannelId { get; set; } = 0;

    [JsonPropertyName("waitingVoiceChannelId")]
    public ulong WaitingVoiceChannelId { get; set; } = 0;

    [JsonPropertyName("dashboardMessageId")]
    public ulong DashboardMessageId { get; set; } = 0;
}

[FeatureInfo("TemporaryChannel")]
public class Feature_TemporaryChannel : Feature
{
    // 커스텀 아이디
    public const string customId_Button_DashboardUpdate = "temporarychannel/dashboard-update";
    public const string customId_Button_CreateTemporaryChannel = "temporarychannel/create-temporary-channel";
    public const string customId_Modal_CreateTemporaryChannelModal = "temporarychannel/create-temporary-channel-modal";

    // 싱글턴 인스턴스
    static Feature_TemporaryChannel? instance;
    public static Feature_TemporaryChannel Instance => instance!;

    // 설정
    static TemporaryChannelSetting? setting;
    public static TemporaryChannelSetting Setting => setting!;

    // 스레드 부모 채널
    static SocketTextChannel? parentChannel;
    public static SocketTextChannel ParentChannel => parentChannel!;

    // 활성화된 임시 채널 목록
    internal static List<TemporaryChannel> activeTemporaryChannels = new();


    // 생성자
    public Feature_TemporaryChannel()
    {
        instance = this;
    }


    #region 이벤트

    public override async ValueTask Load()
    {
        // 설정 로드
        setting = await LoadSettingAsync<TemporaryChannelSetting>();

        // 스레드 부모 채널 로드
        parentChannel = ChannelHelper.GetTextChannel(Feature_TemporaryChannel.Setting.ParentChannelId);

        // 이벤트 구독
        ManagerBotCore.client.ButtonExecuted += OnButtonExecuted;
        ManagerBotCore.client.ModalSubmitted += OnModalSubmitted;

        ManagerBotCore.client.ThreadDeleted += OnThreadDeleted;

        ManagerBotCore.client.ThreadMemberJoined += OnThreadMemberJoined;
        ManagerBotCore.client.ThreadMemberLeft += OnThreadMemberLeft;
    }


    [OnBotInitializeMethod(Feature.featureInitializePriority + 1)]
    static void AfterFeatureInitialize()
    {
        if (instance == null || instance.IsLoadSuccss == false)
            return;

        ManagerBotCore.client.ButtonExecuted += async (SocketMessageComponent component) =>
        {
            if (component.Data.CustomId == Feature_TemporaryChannel.customId_Button_DashboardUpdate)
            {
                await UpdateDashboard();
                await component.AutoRemoveRespondAsync();
            }
        };

        UpdateDashboard();
    }


    static async Task OnButtonExecuted(SocketMessageComponent component)
    {
        switch (component.Data.CustomId)
        {
            case customId_Button_CreateTemporaryChannel:
            {
                await component.RespondWithModalAsync(
                    new ModalBuilder() {
                        Title = "임시 채널 생성",
                        CustomId = customId_Modal_CreateTemporaryChannelModal,
                        Components = new ModalComponentBuilder()
                            .WithTextInput(
                                "채널 이름",
                                "channel-name",
                                TextInputStyle.Short,
                                placeholder: "채널 이름을 입력하세요."
                            )
                    }.Build()
                );
                break;
            }
        }
    }
    static async Task OnModalSubmitted(SocketModal modal)
    {
        switch (modal.Data.CustomId)
        {
            case customId_Modal_CreateTemporaryChannelModal:
            {
                string channelName = modal.Data.Components.First().Value;
                if (string.IsNullOrWhiteSpace(channelName)) {
                    await modal.ErrorRespond("채널 이름이 잘못되었습니다.");
                    return;
                }
                try
                {
                    TemporaryChannel channel = await TemporaryChannel.Create(
                        channelName
                    );
                    await channel.thread.AddUserAsync((SocketGuildUser)modal.User);
                }
                catch (Exception ex)
                {
                    await modal.ErrorRespond($"채널 생성 중 오류가 발생했습니다: {ex.Message}");
                    return;
                }

                await modal.RespondAsync($"임시 채널 '{channelName}' 생성 완료!", ephemeral: true);
                break;
            }
        }
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


    #region 대시보드

    static Task UpdateDashboard()
    {
        if (parentChannel == null)
            return Task.CompletedTask;

        return parentChannel.ModifyMessageAsync(Setting.DashboardMessageId, msg =>
        {
            msg.Content = null;
            msg.Embed = new EmbedBuilder()
            {
                Title = "임시 채널 대시보드"
            }.Build();
            msg.Components = new ComponentBuilder()
            {
                ActionRows = [
                    new ActionRowBuilder()
                    {
                        Components = [
                            new ButtonBuilder()
                            {
                                CustomId = Feature_TemporaryChannel.customId_Button_CreateTemporaryChannel,
                                Label = "생성",
                                Style = ButtonStyle.Success
                            }.Build(),
                            new ButtonBuilder()
                            {
                                CustomId = Feature_TemporaryChannel.customId_Button_DashboardUpdate,
                                Label = "새로고침",
                                Style = ButtonStyle.Primary
                            }.Build()
                        ]
                    }
                ]
            }.Build();
        });
    }

    #endregion
}