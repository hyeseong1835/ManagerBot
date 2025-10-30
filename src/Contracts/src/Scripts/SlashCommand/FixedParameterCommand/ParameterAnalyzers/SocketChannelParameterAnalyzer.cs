using Discord;
using Discord.WebSocket;
using System.Diagnostics.CodeAnalysis;

namespace ManagerBot.SlashCommandSystem.FixedParameterCommand.ParameterAnalyzers;

public class SocketChannelParameterAnalyzer : ParameterAnalyzer<SocketChannel>
{
    public override ApplicationCommandOptionType OptionType => ApplicationCommandOptionType.Channel;
    public override bool TryAnalyze(SocketSlashCommandDataOption option, [NotNullWhen(true)] out SocketChannel? value)
    {
        value = option.Value as SocketChannel;

        return value != null;
    }
}
