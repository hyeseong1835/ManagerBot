using Discord;
using Discord.WebSocket;
using System.Diagnostics.CodeAnalysis;

namespace ManagerBot.SlashCommandSystem.FixedParameterCommand.ParameterAnalyzers;

public class DoubleParameterAnalyzer : ParameterAnalyzer<double>
{
    public override ApplicationCommandOptionType OptionType => ApplicationCommandOptionType.String;
    public override bool TryAnalyze(SocketSlashCommandDataOption option, [NotNullWhen(true)] out double value)
    {
        value = (double)option.Value;
        return true;
    }
}
