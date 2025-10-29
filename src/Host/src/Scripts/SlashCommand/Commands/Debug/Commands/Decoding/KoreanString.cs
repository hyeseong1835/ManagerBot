using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerBot.SlashCommandSystem.DebugCommandGroup.Decoding;
public class KoreanString : SubCommand
{
    public override List<SlashCommandOptionBuilder>? Options => new List<SlashCommandOptionBuilder>
    {
        new SlashCommandOptionBuilder()
        {
            Name = "koreanstring",
            Description = "KoreanString 형식의 문자열",
            Type = ApplicationCommandOptionType.String,
        }
    };

    public KoreanString(string name, string description) : base(name, description) { }

    protected override async ValueTask OnSubCommandExecuted(SocketSlashCommand command, SocketSlashCommandDataOption parentData)
    {
        SocketSlashCommandDataOption option = parentData.Options.First();

        await command.RespondAsync(
            ManagerBot.Utils.Encodings.KoreanString.KoreanStringToUlongId((string)option.Value).ToString(),
            ephemeral: true
        );
    }
}
