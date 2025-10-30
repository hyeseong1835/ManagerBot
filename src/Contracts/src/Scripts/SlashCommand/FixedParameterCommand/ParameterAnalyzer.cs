using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Discord;
using Discord.WebSocket;

using ManagerBot.SlashCommandSystem.FixedParameterCommand.ParameterAnalyzers;

namespace ManagerBot.SlashCommandSystem.FixedParameterCommand;

public abstract class ParameterAnalyzer
{
    static Dictionary<Type, ParameterAnalyzer> analyzers = new Dictionary<Type, ParameterAnalyzer>()
    {
        { typeof(Attachment),       new AttachmentAnalyzer() },
        { typeof(bool),             new BoolParameterAnalyzer() },
        { typeof(double),           new DoubleParameterAnalyzer() },
        { typeof(long),             new LongParameterAnalyzer() },
        { typeof(SocketChannel),    new SocketChannelParameterAnalyzer() },
        { typeof(SocketGuildUser),  new SocketGuildUserParameterAnalyzer() },
        { typeof(SocketRole),       new SocketRoleParameterAnalyzer() },
        { typeof(string),           new StringParameterAnalyzer() },
    };
    public static ParameterAnalyzer<T> GetAnalyzer<T>()
    {
        return (ParameterAnalyzer<T>)analyzers[typeof(T)];
    }


    public abstract ApplicationCommandOptionType OptionType { get; }
}
public abstract class ParameterAnalyzer<T> : ParameterAnalyzer
{
    public abstract bool TryAnalyze(SocketSlashCommandDataOption option, [NotNullWhen(true)] out T? value);
}
