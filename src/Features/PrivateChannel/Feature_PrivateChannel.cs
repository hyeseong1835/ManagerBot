using System.Text.Json.Serialization;
using Discord.WebSocket;
using ManagerBot.Core.Utils.WaitingVoiceChannelUtils;

namespace ManagerBot.Core.Features.PrivateChannelFeature;

public class Feature_PrivateChannel : Feature
{
    public class FeatureSetting
    {
        [JsonPropertyName("categoryId")]
        public ulong CategoryId { get; set; } = 0;

        [JsonPropertyName("textChannelParentId")]
        public ulong TextChannelParentId { get; set; } = 0;

        [JsonPropertyName("waitingVoiceChannelId")]
        public ulong WaitingVoiceChannelId { get; set; } = 0;
    }


    // 싱글턴 인스턴스
    static Feature_PrivateChannel? instance;
    public static Feature_PrivateChannel Instance => instance!;


    public override string Name => "PrivateChannel";

    FeatureSetting? setting;
    public static FeatureSetting Setting => instance!.setting!;

    SocketGuildChannel? textChannelParent;
    public SocketGuildChannel TextChannelParent => textChannelParent!;

    public Feature_PrivateChannel()
    {
        instance = this;
    }

    public override async ValueTask Load()
    {
        // 설정 로드
        setting = await LoadSettingAsync<FeatureSetting>().ConfigureAwait(false)
            ?? throw LoadSettingFailException();

        // 텍스트 채널 부모 로드
        textChannelParent = ChannelHelper.GetTextChannel(setting.TextChannelParentId);

        // 대기실 음성 채널 로드
        WaitingVoiceChannel waitingVoiceChannel = WaitingVoiceChannel.Add(setting.WaitingVoiceChannelId);
        waitingVoiceChannel.onPrivateChannelCreated += async (PrivateWaitingVoiceChannel privateChannel) =>
        {
            await privateChannel.voiceChannel.SendMessageAsync(
                $"비공개 음성 채널이 생성되었습니다."
            );
        };

    }
    public override async ValueTask Unload()
    {
        await SaveSettingAsync(setting);
    }
}