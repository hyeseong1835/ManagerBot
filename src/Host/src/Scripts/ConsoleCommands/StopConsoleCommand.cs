using System;
using System.CommandLine;

namespace ManagerBot.Core;

public class StopConsoleCommand : ConsoleCommand
{
    public override Command Create()
    {
        Command command = new Command("stop", "매니저봇을 종료합니다.");

        command.SetAction(async _ =>
        {
            await ManagerBotCore.Save();
            await ManagerBotCore.Stop();
        });

        return command;
    }
}
