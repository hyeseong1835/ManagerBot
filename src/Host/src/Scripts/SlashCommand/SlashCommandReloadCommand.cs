using Discord.WebSocket;
using ManagerBot.SlashCommandSystem;
using System.Threading.Tasks;

public class SlashCommandReloadCommand : SlashCommand
{
    public override string Name => "reload";
    public override string Description => "Reloads all slash commands.";

    public override async ValueTask OnCommandExecuted(SocketSlashCommand command)
    {
        await SlashCommand.ReloadCommands();

        _ = command.RespondAsync("슬래시 명령어를 다시 불러왔습니다.");
    }
}
