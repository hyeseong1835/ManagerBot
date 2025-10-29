using System.Threading.Tasks;
using System.Collections.Generic;

using Discord;
using Discord.WebSocket;
using ManagerBot.Utils.Encodings;
using ManagerBot.Utils.Extension.Discord.WebSocket;
using ManagerBot.Utils.Extension.System;

namespace ManagerBot.SlashCommandSystem.DebugCommandGroup;
public class Message : SubCommand
{
    public override List<SlashCommandOptionBuilder>? Options => new List<SlashCommandOptionBuilder>()
    {
        new SlashCommandOptionBuilder()
        {
            Type = ApplicationCommandOptionType.String,
            Name = "messageid",
            Description = "대상 메시지 ID",
            IsRequired = true
        }
    };
    public Message(string name, string description) : base(name, description) { }

    protected override async ValueTask OnSubCommandExecuted(SocketSlashCommand command, SocketSlashCommandDataOption data)
    {
        ulong messageId = ulong.Parse((string)data.GetValue("messageid")!);

        /*
        IThreadChannel Thread { get; }
        IReadOnlyCollection<IAttachment> Attachments { get; }
        IReadOnlyCollection<IEmbed> Embeds { get; }
        IReadOnlyCollection<ITag> Tags { get; }
        IReadOnlyCollection<ulong> MentionedChannelIds { get; }
        IReadOnlyCollection<ulong> MentionedRoleIds { get; }
        IReadOnlyCollection<ulong> MentionedUserIds { get; }
        MessageActivity Activity { get; }
        MessageApplication Application { get; }
        MessageReference Reference { get; }
        IReadOnlyDictionary<IEmote, ReactionMetadata> Reactions { get; }
        IReadOnlyCollection<IMessageComponent> Components { get; }
        IReadOnlyCollection<IStickerItem> Stickers { get; }

        // MessageFlags? Flags { get; }

        MessageRoleSubscriptionData RoleSubscriptionData { get; }
        PurchaseNotification PurchaseNotification { get; }
        MessageCallData? CallData { get; }

        IAsyncEnumerable<IReadOnlyCollection<IUser>> GetReactionUsersAsync(IEmote emoji, int limit, RequestOptions options = null,
            ReactionType type = ReactionType.Normal);
            */

        IMessage message = await command.Channel.GetMessageAsync(messageId);

        _ = command.RespondAsync(
            embeds: new Embed[]
            {
                new EmbedBuilder()
                {
                    Title = message.Author.Username,
                    ImageUrl = message.Author.AvatarId,
                    Color = Color.Blue,
                    Fields = new List<EmbedFieldBuilder>
                    {
                        new EmbedFieldBuilder()
                        {
                            Name = nameof(message.Type),
                            Value = message.Type,
                            IsInline = true
                        },
                        new EmbedFieldBuilder()
                        {
                            Name = nameof(message.Source),
                            Value = message.Source,
                            IsInline = true
                        },
                        new EmbedFieldBuilder()
                        {
                            Name = nameof(message.IsTTS),
                            Value = message.IsTTS.ToEmojiString(),
                            IsInline = true
                        },
                        new EmbedFieldBuilder()
                        {
                            Name = nameof(message.IsPinned),
                            Value = message.IsPinned.ToEmojiString(),
                            IsInline = true
                        },
                        new EmbedFieldBuilder()
                        {
                            Name = nameof(message.IsSuppressed),
                            Value = message.IsSuppressed.ToEmojiString(),
                            IsInline = true
                        },
                        new EmbedFieldBuilder()
                        {
                            Name = nameof(message.Content),
                            Value = message.Content,
                            IsInline = true
                        },
                        new EmbedFieldBuilder()
                        {
                            Name = nameof(message.CleanContent),
                            Value = message.CleanContent,
                            IsInline = true
                        },
                        new EmbedFieldBuilder()
                        {
                            Name = nameof(message.Timestamp),
                            Value = message.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                            IsInline = true
                        },
                        new EmbedFieldBuilder()
                        {
                            Name = nameof(message.EditedTimestamp),
                            Value = message.EditedTimestamp?.ToString("yyyy-MM-dd HH:mm:ss") ?? "-",
                            IsInline = true
                        },
                        new EmbedFieldBuilder()
                        {
                            Name = nameof(message.Channel),
                            Value = message.Channel.GetMention(),
                            IsInline = true
                        },
                        new EmbedFieldBuilder()
                        {
                            Name = nameof(message.Author),
                            Value = message.Author.Mention,
                            IsInline = true
                        },
                        new EmbedFieldBuilder()
                        {
                            Name = nameof(message.Id),
                            Value = $"{message.Id} (Base64: {Base64Utility.UlongToBase64(message.Id)})",
                            IsInline = true
                        },
                        new EmbedFieldBuilder()
                        {
                            Name = nameof(message.Flags),
                            Value = message.Flags?.GetFlagsString()?? "null",
                            IsInline = true
                        },
                    }
                }.Build()
            },
            ephemeral: true
        );
    }
}
