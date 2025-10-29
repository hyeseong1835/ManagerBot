#pragma warning disable CS8600 // null 리터럴 또는 가능한 null 값을 null을 허용하지 않는 형식으로 변환하는 중입니다.
#pragma warning disable CS8602 // null 가능 참조에 대한 역참조입니다.
#pragma warning disable CS1998

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

using HS.Common.Reflection;
using ManagerBot.Core;
using ManagerBot.Utils.Extension.Discord.WebSocket;

namespace ManagerBot.SlashCommandSystem;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class CustomInitializeCommandAttribute : Attribute
{
    public string? initializeMethodName;

    public CustomInitializeCommandAttribute(string? initializeMethodName = null)
    {
        this.initializeMethodName = initializeMethodName;
    }
}

public abstract class SlashCommand
{
    #region 정적
    public static Dictionary<string, SlashCommand> globalCommands { get; private set; } = new Dictionary<string, SlashCommand>();
    public static Dictionary<Type, string> commandNames { get; private set; } = new Dictionary<Type, string>();

    [OnInitializeMethod(3)]
    public static async ValueTask Initialize()
    {
        List<ApplicationCommandProperties> commandList = new List<ApplicationCommandProperties>();
        IEnumerable<Type> types = TypeUtility.GetChildTypes<SlashCommand>();

        foreach (Type type in types)
        {
            CustomInitializeCommandAttribute? customCommandAttribute = type.GetCustomAttribute<CustomInitializeCommandAttribute>();
            if (customCommandAttribute != null)
            {
                if (customCommandAttribute.initializeMethodName != null)
                {
                    MethodInfo? methodInfo = type.GetMethod(customCommandAttribute.initializeMethodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    if (methodInfo == null)
                    {
                        Console.WriteLine($"[ ERROR ] 메서드를 찾을 수 없습니다. : {type.Name}/{customCommandAttribute.initializeMethodName}");
                        continue;
                    }

                    if (methodInfo.ReturnType == typeof(Task))
                    {
                        await (Task)methodInfo.Invoke(null, [commandList]);
                    }
                    else
                    {
                        methodInfo.Invoke(null, [commandList]);
                    }
                }
                continue;
            }

            SlashCommand? instance = (SlashCommand?)Activator.CreateInstance(type);
            if (instance == null)
            {
                Debug.LogError("SlashCommand", $"[ ERROR ] {type.Name}의 인스턴스를 만들 수 없습니다. ");
                continue;
            }
            AddCommand(instance, type);
            commandList.Add(instance.CommandDefine());
        }

        ManagerBotCore.client.SlashCommandExecuted += Client_SlashCommandExecuted;

        foreach (SlashCommand command in globalCommands.Values)
        {
            try
            {
                await ManagerBotCore.Guild!.CreateApplicationCommandAsync(command.CommandDefine());
            }
            catch (Exception e)
            {
                Debug.LogError("SlashCommand", e);
                continue;
            }
        }
    }

    static async Task Client_SlashCommandExecuted(SocketSlashCommand command)
    {
        if (globalCommands.TryGetValue(command.CommandName, out SlashCommand? c))
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
    public static void AddCommand(SlashCommand command, Type type)
    {
        globalCommands.Add(command.Name, command);
        commandNames.Add(type, command.Name);
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
