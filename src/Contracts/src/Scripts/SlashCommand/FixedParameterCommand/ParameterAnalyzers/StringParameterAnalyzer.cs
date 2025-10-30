using Discord;
using Discord.WebSocket;
using System.Diagnostics.CodeAnalysis;

namespace ManagerBot.SlashCommandSystem.FixedParameterCommand.ParameterAnalyzers;

public class StringParameterAnalyzer : ParameterAnalyzer<string>
{
    public override ApplicationCommandOptionType OptionType => ApplicationCommandOptionType.Integer;
    public override bool TryAnalyze(SocketSlashCommandDataOption option, [NotNullWhen(true)] out string? value)
    {
        value = (string)option.Value;
        return true;
    }
}
