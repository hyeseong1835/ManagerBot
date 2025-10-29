using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

using ManagerBot.Core;
using ManagerBot.Utils;
using ManagerBot.Utils.Extension.Discord.WebSocket;

namespace ManagerBot.SlashCommandSystem;

public abstract class SlashCommandGroup : SlashCommand
{
    public override string Description => $"{Name} 명령어 그룹입니다.";
    public override SlashCommandProperties CommandDefine()
    {
        SlashCommandBuilder builder = new SlashCommandBuilder();
        {
            builder.Name = Name;
            builder.Description = Description;
            subCommands = SlashCommandGroupDefine();

            List<SlashCommandOptionBuilder> options = new();
            foreach (SlashCommandGroupElement element in subCommands)
            {
                options.Add(element.Define());
            }
            builder.Options = options;
        }
        return builder.Build();
    }
    public abstract SlashCommandGroupElement[] SlashCommandGroupDefine();
    public SlashCommandGroupElement[]? subCommands;

    public override async ValueTask OnCommandExecuted(SocketSlashCommand command)
    {
        if (command.CommandName != Name) return;
        if (subCommands == null)
        {
            Debug.LogError("SlashCommandGroup", $"{Name}의 서브 커맨드가 정의되지 않았습니다.");
            return;
        }

        SocketSlashCommandDataOption data = command.Data.Options.First();

        foreach (SlashCommandGroupElement subCommand in subCommands)
        {
            if (subCommand.Name == data.Name)
            {
                await subCommand.OnTrigger(command, data);
                return;
            }
        }
        Debug.LogError("SlashCommandGroup", $"{Name}의 서브 커맨드를 찾을 수 없습니다.");
    }
}
public interface SlashCommandGroupElement
{
    public string Name { get; }
    public SlashCommandOptionBuilder Define();
    public virtual ValueTask OnTrigger(SocketSlashCommand command, SocketSlashCommandDataOption parentData)
    {
        return ValueTask.CompletedTask;
    }
}
public class SubCommandGroup : SlashCommandGroupElement
{
    public string Name => name;
    public string name;
    public string description;

    public SubCommand[] subCommands;

    public SubCommandGroup(string name, string description, params SubCommand[] subCommands)
    {
        this.name = name;
        this.description = description;
        this.subCommands = subCommands;
    }
    public SubCommandGroup(string name, params SubCommand[] subCommands)
    {
        this.name = name;
        this.description = $"{name} 명령어 그룹입니다.";
        this.subCommands = subCommands;
    }
    public SlashCommandOptionBuilder Define()
    {
        SlashCommandOptionBuilder builder = new();
        {
            builder.Name = name;
            builder.Description = description;
            builder.Type = ApplicationCommandOptionType.SubCommandGroup;

            List<SlashCommandOptionBuilder> options = new();
            foreach (SubCommand element in subCommands)
            {
                options.Add(element.Define());
            }
            builder.Options = options;
        }
        return builder;
    }
    public async ValueTask OnTrigger(SocketSlashCommand command, SocketSlashCommandDataOption parentData)
    {
        SocketSlashCommandDataOption data = parentData.Options.ElementAt(0);

        foreach (SubCommand element in subCommands)
        {
            if (data.Name == element.Name)
            {
                await element.OnTrigger(command, data);
                return;
            }
        }

        Console.WriteLine($"[ ERROR ] {name}의 서브 그룹을 찾을 수 없습니다.");
    }
}
public abstract class SubCommand : SlashCommandGroupElement
{
    public string Name => name;
    public string name;
    public string description;
    public virtual List<SlashCommandOptionBuilder>? Options => null;

    public SubCommand(string name, string description)
    {
        this.name = name;
        this.description = description;
    }

    public virtual SlashCommandOptionBuilder Define()
    {
        SlashCommandOptionBuilder builder = new SlashCommandOptionBuilder();
        {
            builder.Name = name;
            builder.Description = description;
            builder.Type = ApplicationCommandOptionType.SubCommand;

            List<SlashCommandOptionBuilder>? options = Options;
            if (options != null) builder.Options = options;
        }

        return builder;
    }

    public async ValueTask OnTrigger(SocketSlashCommand command, SocketSlashCommandDataOption option)
    {
        try
        {
            await OnSubCommandExecuted(command, option);
        }
        catch (Exception e)
        {
            await command.ErrorRespond(e.Message);
            Debug.LogError("SlashCommandGroup", e);
        }
    }
    protected virtual ValueTask OnSubCommandExecuted(SocketSlashCommand command, SocketSlashCommandDataOption option)
    {
        return ValueTask.CompletedTask;
    }
}
public abstract class FixedParameterSubCommand<TParam> : SubCommand
{
    public abstract string ParameterName { get; }
    public abstract string ParameterDescription { get; }
    public FixedParameterSubCommand(string name, string description) : base(name, description) { }

    public override SlashCommandOptionBuilder Define()
    {
        SlashCommandOptionBuilder builder = new SlashCommandOptionBuilder();
        {
            builder.Name = name;
            builder.Description = description;
            builder.Type = ApplicationCommandOptionType.SubCommand;

            builder.AddOption(new SlashCommandOptionBuilder()
            {
                Name = "parameter",
                Description = "파라미터",
                Type = ApplicationCommandOptionTypeUtility.typeDictionary[typeof(TParam)],
                IsRequired = true
            });
        }
        return builder;
    }
    protected sealed override ValueTask OnSubCommandExecuted(SocketSlashCommand command, SocketSlashCommandDataOption option)
    {
        return OnFixedParameterSubCommandExecuted(command, (TParam)option.Options.ElementAt(0).Value);
    }
    protected abstract ValueTask OnFixedParameterSubCommandExecuted(SocketSlashCommand command, TParam param);
}
public abstract class FixedParameterSubCommand<TParam1, TParam2> : SubCommand
{
    public abstract string Parameter1Name { get; }
    public abstract string Parameter1Description { get; }
    public abstract string Parameter2Name { get; }
    public abstract string Parameter2Description { get; }
    public FixedParameterSubCommand(string name, string description) : base(name, description) { }

    public override SlashCommandOptionBuilder Define()
    {
        SlashCommandOptionBuilder builder = new SlashCommandOptionBuilder();
        {
            builder.Name = name;
            builder.Description = description;
            builder.Type = ApplicationCommandOptionType.SubCommand;

            builder.AddOption(new SlashCommandOptionBuilder()
            {
                Name = Parameter1Name,
                Description = Parameter1Description,
                Type = ApplicationCommandOptionTypeUtility.typeDictionary[typeof(TParam1)],
                IsRequired = true
            });
            builder.AddOption(new SlashCommandOptionBuilder()
            {
                Name = Parameter2Name,
                Description = Parameter2Description,
                Type = ApplicationCommandOptionTypeUtility.typeDictionary[typeof(TParam2)],
                IsRequired = true
            });
        }
        return builder;
    }
    protected sealed override ValueTask OnSubCommandExecuted(SocketSlashCommand command, SocketSlashCommandDataOption option)
    {
        return OnFixedParameterSubCommandExecuted(command, (TParam1)option.Options.ElementAt(0).Value, (TParam2)option.Options.ElementAt(1).Value);
    }
    protected abstract ValueTask OnFixedParameterSubCommandExecuted(SocketSlashCommand command, TParam1 param1, TParam2 param2);
}
