using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

using ManagerBot.Utils;

namespace ManagerBot.SlashCommandSystem;

public abstract class FixedParameterCommand : SlashCommand
{
    public override string DebugName => $"{Name} <>";
    protected abstract string ParameterName { get; }
    protected abstract string ParameterDescription { get; }

    public override SlashCommandProperties CommandDefine()
    {
        SlashCommandBuilder builder = new SlashCommandBuilder();
        {
            builder.WithName(Name);
            builder.WithDescription(Description);
        }
        return builder.Build();
    }
    public override ValueTask OnCommandExecuted(SocketSlashCommand command)
    {
        return OnFixedParameterCommandExecuted(command);
    }
    public virtual ValueTask OnFixedParameterCommandExecuted(SocketSlashCommand command)
    {
        return ValueTask.CompletedTask;
    }
}
public abstract class FixedParameterCommand<TParam> : SlashCommand
{
    public override string DebugName => $"{Name} <{typeof(TParam).Name}>";
    protected abstract string ParameterName { get; }
    protected abstract string ParameterDescription { get; }

    public override SlashCommandProperties CommandDefine()
    {
        SlashCommandBuilder builder = new SlashCommandBuilder();
        {
            builder.WithName(Name);
            builder.WithDescription(Description);
            builder.AddOption(new SlashCommandOptionBuilder()
            {
                Name = ParameterName,
                Description = ParameterDescription,
                Type = ApplicationCommandOptionTypeUtility.typeDictionary[typeof(TParam)],
                IsRequired = true
            });
        }
        return builder.Build();
    }
    sealed public override ValueTask OnCommandExecuted(SocketSlashCommand command)
    {
        return OnFixedParameterCommandExecuted(command, (TParam)command.Data.Options.ElementAt(0).Value);
    }
    protected virtual ValueTask OnFixedParameterCommandExecuted(SocketSlashCommand command, TParam param)
    {
        return ValueTask.CompletedTask;
    }
}
public abstract class FixedParameterCommand<TParam1, TParam2> : SlashCommand
{
    public override string DebugName => $"{Name} <{typeof(TParam1).Name}, {typeof(TParam2).Name}>";

    protected abstract string Parameter1Name { get; }
    protected abstract string Parameter1Description { get; }
    protected abstract string Parameter2Name { get; }
    protected abstract string Parameter2Description { get; }

    public override SlashCommandProperties CommandDefine()
    {
        SlashCommandBuilder builder = new SlashCommandBuilder();
        {
            builder.WithName(Name);
            builder.WithDescription(Description);
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
        return builder.Build();
    }
    sealed public override ValueTask OnCommandExecuted(SocketSlashCommand command)
    {
        return OnFixedParameterCommandExecuted(command, (TParam1)command.Data.Options.ElementAt(0).Value, (TParam2)command.Data.Options.ElementAt(1).Value);
    }
    protected virtual ValueTask OnFixedParameterCommandExecuted(SocketSlashCommand command, TParam1 param1, TParam2 param2)
    {
        return ValueTask.CompletedTask;
    }
}
public abstract class FixedParameterCommand<TParam1, TParam2, TParam3> : SlashCommand
{
    public override string DebugName => $"{Name} <{typeof(TParam1).Name}, {typeof(TParam2).Name}, {typeof(TParam3).Name}>";
    protected abstract string Parameter1Name { get; }
    protected abstract string Parameter1Description { get; }
    protected abstract string Parameter2Name { get; }
    protected abstract string Parameter2Description { get; }
    protected abstract string Parameter3Name { get; }
    protected abstract string Parameter3Description { get; }

    public override SlashCommandProperties CommandDefine()
    {
        SlashCommandBuilder builder = new SlashCommandBuilder();
        {
            builder.WithName(Name);
            builder.WithDescription(Description);
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
            builder.AddOption(new SlashCommandOptionBuilder()
            {
                Name = Parameter3Name,
                Description = Parameter3Description,
                Type = ApplicationCommandOptionTypeUtility.typeDictionary[typeof(TParam3)],
                IsRequired = true
            });
        }
        return builder.Build();
    }
    sealed public override ValueTask OnCommandExecuted(SocketSlashCommand command)
    {
        return OnFixedParameterCommandExecuted(command, (TParam1)command.Data.Options.ElementAt(0).Value, (TParam2)command.Data.Options.ElementAt(1).Value, (TParam3)command.Data.Options.ElementAt(2).Value);
    }
    protected virtual ValueTask OnFixedParameterCommandExecuted(SocketSlashCommand command, TParam1 param1, TParam2 param2, TParam3 param3)
    {
        return ValueTask.CompletedTask;
    }
}
