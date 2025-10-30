using Discord;
using Discord.WebSocket;
using System.Diagnostics.CodeAnalysis;

namespace ManagerBot.SlashCommandSystem.FixedParameterCommand.ParameterAnalyzers;

public class AttachmentAnalyzer : ParameterAnalyzer<Attachment>
{
    public override ApplicationCommandOptionType OptionType => ApplicationCommandOptionType.Attachment;
    public override bool TryAnalyze(SocketSlashCommandDataOption option, [NotNullWhen(true)] out Attachment? value)
    {
        value = option.Value as Attachment;

        return value != null;
    }
}
