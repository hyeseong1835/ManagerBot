using Discord;
using Discord.WebSocket;
using System.Diagnostics.CodeAnalysis;

namespace ManagerBot.SlashCommandSystem.FixedParameterCommand.ParameterAnalyzers;

public class BoolParameterAnalyzer : ParameterAnalyzer<bool>
{
    public override ApplicationCommandOptionType OptionType => ApplicationCommandOptionType.String;
    public override bool TryAnalyze(SocketSlashCommandDataOption option, [NotNullWhen(true)] out bool value)
    {
        value = (bool)option.Value;
        return true;
    }
}
