using System.Text.Json.Serialization;

using Discord;
using Discord.Rest;
using Discord.WebSocket;
using ManagerBot.Core.Features.ManagerBotDebugFeature;
using ManagerBot.Core.Utils.DiscordHelper;

namespace ManagerBot.Core.Features.MinecraftServerFeature;

public class MinecraftServerSetting
{
    [JsonPropertyName("dashboardChannelId")]
    public ulong DashboardChannelId { get; set; } = 0;

    [JsonPropertyName("dashboardMessageId")]
    public ulong DashboardMessageId { get; set; } = 0;

    [JsonPropertyName("servers")]
    public string[] Servers { get; set; } = Array.Empty<string>();
}

[FeatureInfo("MinecraftServer")]
public class Feature_MinecraftServer : Feature
{
    // 싱글턴 인스턴스
    static Feature_MinecraftServer? instance;
    public static Feature_MinecraftServer Instance => instance!;

    // 커스텀 아이디
    public const string customId_Button_PannelUpdate = "managerbotdebug/pannel-update";

    // 설정
    MinecraftServerSetting? setting;

    // 대시보드 채널
    SocketTextChannel? dashboardChannel;

    // 생성자
    public Feature_MinecraftServer()
    {
        instance = this;
    }

    #region 이벤트

    public override async ValueTask Load()
    {
        setting = await LoadSettingAsync<MinecraftServerSetting>()
            .ConfigureAwait(false);
    }

    [OnBotInitializeMethod(Feature.featureInitializePriority + 1)]
    static async ValueTask AfterFeatureInitialize()
    {
        Feature_MinecraftServer instance = Feature_MinecraftServer.instance!;

        if (instance.IsLoadSuccss == false)
            return;

        MinecraftServerSetting setting = instance.setting!;
        SocketTextChannel dashboardChannel = instance.dashboardChannel!;

        bool isSettingDirty = false;

        if (dashboardChannel == null)
        {
            RestTextChannel restChannel = await ChannelHelper.CreateTextChannel("매니저봇");
            instance.dashboardChannel = ChannelHelper.GetTextChannel(restChannel.Id);
            dashboardChannel = instance.dashboardChannel;

            setting.DashboardChannelId = restChannel.Id;
            isSettingDirty = true;
        }

        IMessage? dashboardMessage = await MessageHelper.GetMessageAsync(dashboardChannel, setting.DashboardMessageId);

        if (dashboardMessage == null)
        {
            RestUserMessage restMessage = await dashboardChannel.SendMessageAsync("...");
            setting.DashboardMessageId = restMessage.Id;
            isSettingDirty = true;
        }

        if (isSettingDirty)
        {
            await instance.SaveSettingAsync(setting);
        }

        try
        {
            await instance.PannelUpdateAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"패널 업데이트 실패: {ex.Message}");
        }

        ManagerBotCore.client.ButtonExecuted += async (SocketMessageComponent component) =>
        {
            if (component.Data.CustomId == Feature_MinecraftServer.customId_Button_PannelUpdate)
            {
                await instance.PannelUpdateAsync();
                await component.AutoRemoveRespondAsync();
            }
        };
    }

    #endregion


    public Task<IUserMessage> PannelUpdateAsync()
    {
        // 설정은 여기서 반드시 Null이 아님.
        if (setting == null) throw new NullReferenceException();

        // 대시보드 채널은 여기서 반드시 Null이 아님.
        if (dashboardChannel == null) throw new NullReferenceException();

        // 메시지 수정
        return dashboardChannel.ModifyMessageAsync(
            setting.DashboardMessageId,
            (msg) =>
            {
                msg.Content = null;
                msg.Embed = new EmbedBuilder()
                {
                    Title = "매니저봇 디버그 패널",
                    Fields = [
                        new EmbedFieldBuilder()
                        {
                            Name = "기능",
                            Value = $"개수: {Feature.LoadedFeatureCount} / {Feature.FeatureCount}"
                                  + "\n목록"
                                  + "\n  " + string.Join("\n  ", Feature.LoadedFeatures.Select(static (feature) => feature.DescriptionInManagerBotPanel))
                        },
                        new EmbedFieldBuilder()
                        {
                            Name = "핑",
                            Value = $"{ManagerBotCore.client.Latency}ms"
                        },
                        Program.machineInfo.GetMachineInfoEmbedFieldBuilder()
                    ],
                    Color = Color.Blue,
                    Timestamp = DateTime.UtcNow.AddHours(9)
                }.Build();

                msg.Components = new ComponentBuilder()
                {
                    ActionRows = [
                        new ActionRowBuilder()
                        {
                            Components = [
                                new ButtonBuilder()
                                {
                                    CustomId = Feature_ManagerBotDebug.customId_Button_PannelUpdate,
                                    Label = "새로고침",
                                    Style = ButtonStyle.Primary,
                                    // Emote = new Emote(0x1F501, "repeat", false), // 🔁
                                    IsDisabled = false
                                }.Build()
                            ]
                        }
                    ]
                }.Build();
            }
        );
    }
}