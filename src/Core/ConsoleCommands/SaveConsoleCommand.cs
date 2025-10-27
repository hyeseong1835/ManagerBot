using System.CommandLine;

namespace ManagerBot.Core;

public class SaveConsoleCommand : ConsoleCommand
{
    public override Command Create()
    {
        Command command = new Command("save", "모든 데이터를 저장합니다.");

        command.SetAction(async _ => await ManagerBotCore.Save());

        return command;
    }
}
