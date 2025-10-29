using Discord;
using Discord.WebSocket;
using ManagerBot.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerBot.SlashCommandSystem.InfoCommandGroup;
public class User : SubCommand
{
    public User(string name, string description) : base(name, description) { }

    protected override async ValueTask OnSubCommandExecuted(SocketSlashCommand command, SocketSlashCommandDataOption data)
    {
        EmbedBuilder embedBuilder = new();
        {
            embedBuilder.WithColor(Color.Blue);
            embedBuilder.WithTitle("명령어");
        };

        foreach (SocketCategoryChannel category in ManagerBotCore.Guild!.CategoryChannels)
        {
            List<SocketVoiceChannel> voiceChannels = new();

            foreach (SocketGuildChannel channel in category.Channels)
            {
                if (channel is not SocketVoiceChannel voiceChannel) continue;

                if (voiceChannel.Users.Count == 0) continue;

                voiceChannels.Add(voiceChannel);
            }

            if (voiceChannels.Count == 0) continue;

            IEnumerable<SocketGuildUser> users = voiceChannels.SelectMany(x => x.ConnectedUsers);
            if (users.Count() == 0) continue;

            embedBuilder.AddField(new EmbedFieldBuilder()
            {
                Name = category.Id.ToString(),
                Value = string.Join(
                    "\n",
                    string.Join(
                        "\n",
                        users.Select(u => $"{u.DisplayName} ({u.GlobalName})")
                    )
                )
            });
        }

        await command.RespondAsync(embed: embedBuilder.Build(), ephemeral: true);
    }
}
