using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

using Discord;
using Discord.WebSocket;

using ManagerBot.Utils.Extension.Discord.WebSocket;

namespace ManagerBot.SlashCommandSystem.FixedParameterCommand;

public readonly struct ParameterInfo<T>
{
    public readonly string name;
    public readonly string description;

    public readonly ParameterAnalyzer<T> analyzer;
    public readonly ApplicationCommandOptionType optionType;

    public ParameterInfo(string name, string description)
    {
        this.name = name;
        this.description = description;

        this.analyzer = ParameterAnalyzer.GetAnalyzer<T>();
        this.optionType = this.analyzer.OptionType;
    }
    public ParameterInfo(string name, string description, ParameterAnalyzer<T> analyzer)
    {
        this.name = name;
        this.description = description;

        this.analyzer = analyzer;
        this.optionType = analyzer.OptionType;
    }
    public void AddTo(SlashCommandBuilder builder)
    {
        builder.AddOption(new SlashCommandOptionBuilder()
        {
            Name = name,
            Description = description,
            Type = optionType,
            IsRequired = true
        });
    }
    public bool TryGetValue(SocketSlashCommand command, [NotNullWhen(true)] out T? value, [NotNullWhen(false)] out string? errorMessage)
    {
        foreach (SocketSlashCommandDataOption option in command.Data.Options)
        {
            if (option.Name == name)
            {
                if (analyzer.TryAnalyze(option, out value))
                {
                    errorMessage = null;
                    return true;
                }
                else
                {
                    errorMessage = $"옵션 {name}의 값을 분석할 수 없습니다.";
                    return false;
                }
            }
        }
        errorMessage = $"옵션 {name}을 찾을 수 없습니다.";
        value = default;
        return false;
    }
}
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
    public abstract ValueTask OnFixedParameterCommandExecuted(SocketSlashCommand command);
}
public abstract class FixedParameterCommand<TParam> : SlashCommand
{
    public override string DebugName => $"{Name} <{typeof(TParam).Name}>";
    protected abstract ParameterInfo<TParam> ParamInfo { get; set; }

    public override SlashCommandProperties CommandDefine()
    {
        SlashCommandBuilder builder = new SlashCommandBuilder();
        {
            builder.WithName(Name);
            builder.WithDescription(Description);

            ParamInfo.AddTo(builder);
        }
        return builder.Build();
    }
    sealed public override ValueTask OnCommandExecuted(SocketSlashCommand command)
    {
        string? errorMessage;
        if (ParamInfo.TryGetValue(command, out TParam? param, out errorMessage))
        {
            return OnFixedParameterCommandExecuted(command, param);
        }

        _ = command.ErrorRespond(errorMessage);
        return ValueTask.CompletedTask;
    }

    protected abstract ValueTask OnFixedParameterCommandExecuted(SocketSlashCommand command, TParam param);
}
public abstract class FixedParameterCommand<TParam1, TParam2> : SlashCommand
{
    public override string DebugName => $"{Name} <{typeof(TParam1).Name}, {typeof(TParam2).Name}>";

    protected abstract ParameterInfo<TParam1> Param1Info { get; set; }
    protected abstract ParameterInfo<TParam2> Param2Info { get; set; }

    public override SlashCommandProperties CommandDefine()
    {
        SlashCommandBuilder builder = new SlashCommandBuilder();
        {
            builder.WithName(Name);
            builder.WithDescription(Description);

            Param1Info.AddTo(builder);
            Param2Info.AddTo(builder);
        }
        return builder.Build();
    }
    sealed public override ValueTask OnCommandExecuted(SocketSlashCommand command)
    {
        string? errorMessage;
        if (Param1Info.TryGetValue(command, out TParam1? param1, out errorMessage)
            && Param2Info.TryGetValue(command, out TParam2? param2, out errorMessage))
        {
            return OnFixedParameterCommandExecuted(command, param1, param2);
        }

        _ = command.ErrorRespond(errorMessage);
        return ValueTask.CompletedTask;
    }
    protected abstract ValueTask OnFixedParameterCommandExecuted(SocketSlashCommand command, TParam1 param1, TParam2 param2);
}
public abstract class FixedParameterCommand<TParam1, TParam2, TParam3> : SlashCommand
{
    public override string DebugName => $"{Name} <{typeof(TParam1).Name}, {typeof(TParam2).Name}, {typeof(TParam3).Name}>";

    protected abstract ParameterInfo<TParam1> Param1Info { get; set; }
    protected abstract ParameterInfo<TParam2> Param2Info { get; set; }
    protected abstract ParameterInfo<TParam3> Param3Info { get; set; }

    public override SlashCommandProperties CommandDefine()
    {
        SlashCommandBuilder builder = new SlashCommandBuilder();
        {
            builder.WithName(Name);
            builder.WithDescription(Description);

            Param1Info.AddTo(builder);
            Param2Info.AddTo(builder);
            Param3Info.AddTo(builder);
        }
        return builder.Build();
    }
    sealed public override ValueTask OnCommandExecuted(SocketSlashCommand command)
    {
        string? errorMessage;
        if (Param1Info.TryGetValue(command, out TParam1? param1, out errorMessage)
            && Param2Info.TryGetValue(command, out TParam2? param2, out errorMessage)
            && Param3Info.TryGetValue(command, out TParam3? param3, out errorMessage))
        {
            return OnFixedParameterCommandExecuted(command, param1, param2, param3);
        }

        _ = command.ErrorRespond(errorMessage);
        return ValueTask.CompletedTask;
    }
    protected abstract ValueTask OnFixedParameterCommandExecuted(SocketSlashCommand command, TParam1 param1, TParam2 param2, TParam3 param3);
}
