using Discord;
using Discord.WebSocket;
using System.Diagnostics.CodeAnalysis;

namespace ManagerBot.SlashCommandSystem.FixedParameterCommand.ParameterAnalyzers;

public class SocketRoleParameterAnalyzer : ParameterAnalyzer<SocketRole>
{
    public override ApplicationCommandOptionType OptionType => ApplicationCommandOptionType.Role;
    public override bool TryAnalyze(SocketSlashCommandDataOption option, [NotNullWhen(true)] out SocketRole? value)
    {
        value = option.Value as SocketRole;

        return value != null;
    }
}
