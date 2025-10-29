using Discord.WebSocket;
using ManagerBot.Core;
using ManagerBot.SlashCommandSystem;
using System.Threading;
using System.Threading.Tasks;
public class ForceQuit : SlashCommand
{
    public override string Name => "강제종료";

    public override string Description => "프로그램을 강제로 종료합니다.";

    public override ValueTask OnCommandExecuted(SocketSlashCommand command)
    {
        command.RespondAsync("프로그램을 강제로 종료합니다.", ephemeral: true);

        Thread.Sleep(1000);

        _ = ManagerBotCore.Stop();

        return ValueTask.CompletedTask;
    }
}
