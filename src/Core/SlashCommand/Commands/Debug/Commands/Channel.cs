using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

using ManagerBot.Utils.Extension.Discord;
using ManagerBot.Utils.Extension.Discord.WebSocket;
using ManagerBot.Utils.Extension.System;

namespace ManagerBot.SlashCommandSystem.DebugCommandGroup;
public class Channel : SubCommand
{
    public override List<SlashCommandOptionBuilder>? Options => new List<SlashCommandOptionBuilder>()
    {
        new SlashCommandOptionBuilder()
        {
            Type = ApplicationCommandOptionType.Channel,
            Name = "channel",
            Description = "대상 채널",
            IsRequired = true
        }
    };
    public Channel(string name, string description) : base(name, description) { }

    protected override ValueTask OnSubCommandExecuted(SocketSlashCommand command, SocketSlashCommandDataOption data)
    {
        SocketGuildChannel? channel = data.GetValue<SocketGuildChannel>("channel");
        if (channel == null) {
            _ = command.RespondAsync("채널이 아닙니다.", ephemeral: true);
            return ValueTask.CompletedTask;
        }

        _ = command.RespondAsync(
            embeds: new Embed[]
            {
                new EmbedBuilder()
                {
                    Title = $"{channel.Name}",
                    Color = Color.Blue,
                    Fields = new List<EmbedFieldBuilder>
                    {
                        new EmbedFieldBuilder()
                        {
                            Name = nameof(channel.Id),
                            Value = $"{MentionUtility.MentionChannel(channel.Id)} [{channel.Id}]"
                        },
                        new EmbedFieldBuilder()
                        {
                            Name = nameof(channel.Users),
                            Value = string.Join("\n", channel.Users.Select(u => $"{u.Mention} [{u.Id}]"))
                        },
                        new EmbedFieldBuilder()
                        {
                            Name = nameof(channel.Flags),
                            Value = $"{channel.Flags.GetFlagsString(ChannelFlags.Pinned, ChannelFlags.RequireTag, ChannelFlags.HideMediaDownloadOption)}"
                        },
                        new EmbedFieldBuilder()
                        {
                            Name = nameof(channel.Position),
                            Value = $"{channel.Position}"
                        },
                        new EmbedFieldBuilder()
                        {
                            Name = nameof(channel.CreatedAt),
                            Value = $"{channel.CreatedAt:yyyy-MM-dd HH:mm:ss}"
                        },
                        new EmbedFieldBuilder()
                        {
                            Name = nameof(channel.PermissionOverwrites),
                            Value = string.Join('\n', channel.PermissionOverwrites.Select(o => $"{o.TargetMention()}: ```{string.Concat(o.Permissions.ToAllowList().Select(c => $"{c}: Allow\n"))}{string.Join('\n', o.Permissions.ToDenyList().Select(c => $"{c}: Deny"))}```"))
                        }
                    }
                }.Build()
            },
            ephemeral: true
        );
        return ValueTask.CompletedTask;
    }
}
