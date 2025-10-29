using System.Text.Encodings;
using System.Threading.Tasks;
using System.Collections.Generic;

using Discord;
using Discord.WebSocket;
using ManagerBot.Utils.Encodings;
using System.Linq;

namespace ManagerBot.SlashCommandSystem.DebugCommandGroup.Decoding;
public class Base64 : SubCommand
{
    public override List<SlashCommandOptionBuilder>? Options => new List<SlashCommandOptionBuilder>
    {
        new SlashCommandOptionBuilder()
        {
            Name = "idstring",
            Description = "Base64 형식의 문자열",
            Type = ApplicationCommandOptionType.String,
        }
    };

    public Base64(string name, string description) : base(name, description) { }

    protected override ValueTask OnSubCommandExecuted(SocketSlashCommand command, SocketSlashCommandDataOption parentData)
    {
        SocketSlashCommandDataOption option = parentData.Options.First();

        _ = command.RespondAsync(
            Base64Utility.Base64ToUlong((string)option.Value).ToString(),
            ephemeral: true
        );
        return ValueTask.CompletedTask;
    }
}
