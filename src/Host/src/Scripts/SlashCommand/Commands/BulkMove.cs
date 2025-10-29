using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

using ManagerBot.Utils.Extension.Discord.WebSocket;

using ManagerBot.SlashCommandSystem;

public class BulkMove : FixedParameterCommand<IGuildChannel>
{
    public override string Name => "일괄이동";
    public override string Description => "현재 통화방의 모든 사용자를 다른 통화방으로 이동시킵니다.";
    protected override string ParameterName => "channel";
    protected override string ParameterDescription => "이동시킬 대상 채널을 선택합니다.";

    protected override async ValueTask OnFixedParameterCommandExecuted(SocketSlashCommand command, IGuildChannel targetChannel)
    {
        SocketGuildUser? user = command.User as SocketGuildUser;
        if (user == null) {
            await command.RespondAsync("서버에서만 사용할 수 있습니다.");
            return;
        }
        if (user.VoiceChannel == null) {
            await command.RespondAsync("음성 채널에 들어가 있지 않습니다.");
            return;
        }
        if (targetChannel == null) {
            await command.RespondAsync("대상 채널을 찾을 수 없습니다.");
            return;
        }
        if (targetChannel is not SocketVoiceChannel targetVoiceChannel) {
            await command.RespondAsync($"대상 채널은 음성 채널이여야 합니다. : {targetChannel.GetType().Name}");
            return;
        }
        if (user.VoiceChannel.Users.Contains(user)) {
            await command.RespondAsync("현재 음성 채널의 권한이 없습니다.");
            return;
        }
        if (targetVoiceChannel.Users.Contains(user)) {
            await command.RespondAsync("대상 음성 채널의 권한이 없습니다.");
            return;
        }
        foreach (SocketGuildUser targetUser in user.VoiceChannel.ConnectedUsers) {
            await targetUser.ModifyAsync(x => x.Channel = targetVoiceChannel);
        }
        await command.AutoRemoveRespond();
    }
}
