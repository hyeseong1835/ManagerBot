using Discord;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerBot.SlashCommandSystem.InfoCommandGroup;

public class Command : SubCommand
{
    public Command(string name, string description) : base(name, description) { }

    protected override async ValueTask OnSubCommandExecuted(SocketSlashCommand command, SocketSlashCommandDataOption data)
    {
        EmbedBuilder embedBuilder = new();
        {
            embedBuilder.WithColor(Color.Blue);
            embedBuilder.WithTitle("명령어");
            embedBuilder.WithDescription(
                string.Join(
                    "\n",
                    SlashCommand.commands.Values
                        .Select(c => $"`/{c.Name}`: {c.Description}")
                )
            );
        };
        await command.RespondAsync(embed: embedBuilder.Build(), ephemeral: true);
    }
}
