using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

using ManagerBot.Utils.Encodings;
using ManagerBot.Utils.Extension.Discord.WebSocket;
using ManagerBot.Utils.Extension.System;

namespace ManagerBot.SlashCommandSystem.DebugCommandGroup;

public class Category : SubCommand
{
    public override List<SlashCommandOptionBuilder>? Options => new List<SlashCommandOptionBuilder>()
    {
        new SlashCommandOptionBuilder()
        {
            Type = ApplicationCommandOptionType.Channel,
            Name = "category",
            Description = "대상 채널",
            IsRequired = true
        }
    };
    public Category(string name, string description) : base(name, description) { }

    protected override async ValueTask OnSubCommandExecuted(SocketSlashCommand command, SocketSlashCommandDataOption data)
    {
        SocketCategoryChannel? category = data.GetValue<SocketCategoryChannel>("category");
        if (category == null) {
            await command.RespondAsync("카테고리가 아닙니다.", ephemeral: true);
            return;
        }

        await command.RespondAsync(
            embeds: new Embed[]
            {
                new EmbedBuilder()
                {
                    Title = $"{category.Name}",
                    Color = Color.Blue,
                    Fields = new List<EmbedFieldBuilder>
                    {
                        new EmbedFieldBuilder()
                        {
                            Name = nameof(category.Id),
                            Value = $"{MentionUtility.MentionChannel(category.Id)} [({category.Id}){KoreanString.UlongToKoreanString(category.Id)}]"
                        },
                        new EmbedFieldBuilder()
                        {
                            Name = nameof(category.Users),
                            Value = string.Join(
                                "\n",
                                category.Users.Select(u => $"{MentionUtility.MentionUser(u.Id)} [({u.Id}){KoreanString.UlongToKoreanString(u.Id)}]")
                            )
                        },
                        new EmbedFieldBuilder()
                        {
                            Name = nameof(category.Flags),
                            Value = $"{category.Flags.GetFlagsString(ChannelFlags.Pinned, ChannelFlags.RequireTag, ChannelFlags.HideMediaDownloadOption)}"
                        },
                        new EmbedFieldBuilder()
                        {
                            Name = nameof(category.Position),
                            Value = $"{category.Position}"
                        },
                        new EmbedFieldBuilder()
                        {
                            Name = nameof(category.Channels),
                            Value = string.Join(
                                "\n",
                                category.Channels.Select(c => $"{MentionUtility.MentionChannel(c.Id)} [({c.Id}){KoreanString.UlongToKoreanString(c.Id)}]")
                            )
                        },
                        new EmbedFieldBuilder()
                        {
                            Name = nameof(category.CreatedAt),
                            Value = $"{category.CreatedAt:yyyy-MM-dd HH:mm:ss}"
                        },
                    }
                }.Build()
            },
            ephemeral: true
        );
    }
}
