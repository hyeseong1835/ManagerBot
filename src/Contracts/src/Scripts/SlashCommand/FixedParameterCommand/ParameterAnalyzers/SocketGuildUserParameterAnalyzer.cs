using Discord;
using Discord.WebSocket;
using System.Diagnostics.CodeAnalysis;

namespace ManagerBot.SlashCommandSystem.FixedParameterCommand.ParameterAnalyzers;

public class SocketGuildUserParameterAnalyzer : ParameterAnalyzer<SocketGuildUser>
{
    public override ApplicationCommandOptionType OptionType => ApplicationCommandOptionType.User;
    public override bool TryAnalyze(SocketSlashCommandDataOption option, [NotNullWhen(true)] out SocketGuildUser? value)
    {
        value = option.Value as SocketGuildUser;

        return value != null;
    }
}
