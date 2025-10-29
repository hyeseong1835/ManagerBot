
using System;
using System.Threading.Tasks;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Collections.Generic;

using HS.Common.Reflection;

namespace ManagerBot.Core;

public abstract class ConsoleCommand
{
    static RootCommand? _rootCommand;
    public static RootCommand? RootCommand => _rootCommand;

    static RootCommand CreateRootCommand()
    {
        RootCommand rootCommand = new RootCommand();
        Dictionary<Type, Command> commands = new();
        List<ConsoleCommand> orphanageCommands = new();

        foreach (Type type in TypeUtility.GetChildTypes<ConsoleCommand>())
        {
            ConsoleCommand? consoleCommand = (ConsoleCommand?)Activator.CreateInstance(type);
            if (consoleCommand == null)
            {
                Debug.LogError("ConsoleCommand", $"콘솔 커맨드 인스턴스를 생성할 수 없습니다. : {type.Name}");
                continue;
            }

            Command? parentCommand;
            if (consoleCommand.parentType == null)
            {
                parentCommand = rootCommand;
            }
            else
            {
                commands.TryGetValue(consoleCommand.parentType, out parentCommand);
            }

            if (parentCommand == null)
            {
                orphanageCommands.Add(consoleCommand);
                continue;
            }
            else
            {
                Command command = consoleCommand.Create();
                rootCommand.Add(command);
                commands.Add(type, command);

                for (int i = 0; i < orphanageCommands.Count; i++)
                {
                    ConsoleCommand orphanageCommand = orphanageCommands[i];
                    if (orphanageCommand.parentType == type)
                    {
                        command.Add(orphanageCommand.Create());
                    }
                }
                continue;
            }
        }

        return rootCommand;
    }
    public static async ValueTask StartRead()
    {
        _rootCommand = CreateRootCommand();

        while (true)
        {
            string? line = null;
            while (line == null)
            {
                line = await Console.In.ReadLineAsync();
            }

            ParseResult parseResult = _rootCommand.Parse(line);
            if (parseResult.Errors.Count >= 1)
            {
                foreach (ParseError parseError in parseResult.Errors)
                {
                    Console.Error.WriteLine(parseError.Message);
                }
                return;
            }

            await _rootCommand.Parse(line).InvokeAsync();
        }
    }


    public virtual Type? parentType => null;
    public abstract Command Create();
}
