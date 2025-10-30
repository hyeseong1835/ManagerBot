using Discord;
using Discord.WebSocket;
using System.Diagnostics.CodeAnalysis;

namespace ManagerBot.SlashCommandSystem.FixedParameterCommand.ParameterAnalyzers;

public class LongParameterAnalyzer : ParameterAnalyzer<long>
{
    public override ApplicationCommandOptionType OptionType => ApplicationCommandOptionType.String;
    public override bool TryAnalyze(SocketSlashCommandDataOption option, [NotNullWhen(true)] out long value)
    {
        value = (long)option.Value;
        return true;
    }
}
