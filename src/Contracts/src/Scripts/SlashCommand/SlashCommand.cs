#pragma warning disable CS8600 // null 리터럴 또는 가능한 null 값을 null을 허용하지 않는 형식으로 변환하는 중입니다.
#pragma warning disable CS8602 // null 가능 참조에 대한 역참조입니다.
#pragma warning disable CS1998

using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Discord;
using Discord.WebSocket;

using HS.Common.Reflection;

using ManagerBot.Core;
using ManagerBot.Utils.Extension.Discord.WebSocket;

namespace ManagerBot.SlashCommandSystem;

public abstract class SlashCommand
{
    #region 정적

    public static Dictionary<string, SlashCommand> commands { get; private set; } = new Dictionary<string, SlashCommand>();
    public static Dictionary<Type, string> commandNames { get; private set; } = new Dictionary<Type, string>();

    [OnInitializeMethod(3)]
    public static async ValueTask Initialize()
    {
        IEnumerable<Type> types = TypeUtility.GetChildTypes<SlashCommand>();

        foreach (Type type in types)
        {
            SlashCommand? command = (SlashCommand?)Activator.CreateInstance(type);
            if (command == null)
            {
                Debug.LogError("SlashCommand", $"[ ERROR ] {type.Name}의 인스턴스를 만들 수 없습니다. ");
                continue;
            }
            commands.Add(command.Name, command);
            commandNames.Add(type, command.Name);
        }

        ManagerBotCore.client.SlashCommandExecuted += Client_SlashCommandExecuted;
    }

    static async Task Client_SlashCommandExecuted(SocketSlashCommand command)
    {
        if (commands.TryGetValue(command.CommandName, out SlashCommand? c))
        {
            try
            {
                await c.OnCommandExecuted(command);
            }
            catch(Exception e)
            {
                await command.ErrorRespond(e);
                Debug.LogError("SlashCommand", e);
            }
        }
    }

    public static Task<IReadOnlyCollection<SocketApplicationCommand>> ReloadCommands()
    {
        ApplicationCommandProperties[] properties = new ApplicationCommandProperties[commands.Count];
        int i = 0;
        foreach (SlashCommand command in commands.Values)
        {
            properties[i++] = command.CommandDefine();
        }

        return ManagerBotCore.client.BulkOverwriteGlobalApplicationCommandsAsync(properties);
    }

    #endregion

    public virtual string DebugName => Name;

    public abstract string Name { get; }
    public abstract string Description { get; }

    public virtual SlashCommandProperties CommandDefine()
    {
        SlashCommandBuilder builder = new SlashCommandBuilder();
        {
            builder.WithName(Name);
            builder.WithDescription(Description);
        }
        return builder.Build();
    }
    public virtual ValueTask OnCommandExecuted(SocketSlashCommand command)
    {
        return ValueTask.CompletedTask;
    }
}
