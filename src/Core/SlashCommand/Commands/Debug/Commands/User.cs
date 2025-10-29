using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

using ManagerBot.Utils.Encodings;
using ManagerBot.Utils.Extension.Discord.WebSocket;
using ManagerBot.Utils.Extension.System;

namespace ManagerBot.SlashCommandSystem.DebugCommandGroup;
public class User : SubCommand
{
    public override List<SlashCommandOptionBuilder>? Options => new List<SlashCommandOptionBuilder>
    {
        new SlashCommandOptionBuilder()
        {
            Name = "user",
            Description = "유저",
            Type = ApplicationCommandOptionType.User,
            IsRequired = true
        }
    };
    public User(string name, string description) : base(name, description) { }

    protected override async ValueTask OnSubCommandExecuted(SocketSlashCommand command, SocketSlashCommandDataOption data)
    {
        await command.DeferAsync();

        SocketGuildUser? user = data.GetValue<SocketGuildUser>("user");
        if (user == null)
        {
            await command.ErrorFollowup("유저를 찾을 수 없습니다.");
            return;
        }
        if (user.IsBot)
        {
            await command.ErrorFollowup("봇의 정보는 볼 수 없습니다.");
            return;
        }
        if (user.IsWebhook)
        {
            await command.ErrorFollowup("웹훅의 정보는 볼 수 없습니다.");
            return;
        }
        await command.FollowupAsync(
            embed: GetUserEmbedBuilder(user).Build()
        );
    }
    static EmbedBuilder GetUserEmbedBuilder(SocketGuildUser user)
    {
        EmbedBuilder embedBuilder = new();
        {
            embedBuilder.Title = user.DisplayName;
            embedBuilder.Description = user.Mention;
            embedBuilder.Color = Color.Blue;
            embedBuilder.Fields = new List<EmbedFieldBuilder>()
            {
                new()
                {
                    Name = "Name",
                    Value = $"{nameof(user.DisplayName)}: {user.DisplayName}"
                        + $"\n{nameof(user.GlobalName)}: {user.GlobalName}"
                        + $"\n{nameof(user.Username)}: {user.Username}"
                        + $"\n{nameof(user.Nickname)}: {user.Nickname}"
                        + $"\n{nameof(user.Discriminator)}: {user.Discriminator} ({user.DiscriminatorValue})",
                    IsInline = true
                },
                new()
                {
                    Name = "ID",
                    Value = $"{nameof(user.Id)}: {user.Id}"
                        + $"\nBase64: {Base64Utility.UlongToBase64(user.Id)}",
                    IsInline = true
                },
                new()
                {
                    Name = "Group",
                    Value = "개발중",
                    IsInline = true
                    //string.Join('\n', Group.GetGroups(user, Group.GroupRoleRange).Select(x => $""))
                },
                new()
                {
                    Name = "Roles",
                    Value = (user.Roles.Count == 0)? "-" : string.Join(", ", user.Roles.Select(x => x.Mention)),
                    IsInline = true
                },
                new()
                {
                    Name = nameof(user.Hierarchy),
                    Value = user.Hierarchy,
                    IsInline = true
                },
                new()
                {
                    Name = "Info",
                    Value = $"{nameof(user.ActiveClients)}: {string.Join(", ", user.ActiveClients.Select(x => Enum.GetName(x)))}"
                        + $"\n{nameof(user.CreatedAt)}: {user.CreatedAt}"
                        + $"\n{nameof(user.JoinedAt)}: {user.JoinedAt}"
                        + $"\n{nameof(user.PremiumSince)}: {user.PremiumSince}"
                        + $"\n{nameof(user.RequestToSpeakTimestamp)}: {user.RequestToSpeakTimestamp}"
                        + $"\n{nameof(user.TimedOutUntil)}: {user.TimedOutUntil}",
                    IsInline = true
                },
                new()
                {
                    Name = "Avatar",
                    Value = $"{nameof(user.AvatarDecorationHash)}: {user.AvatarDecorationHash}"
                        + $"\n{nameof(user.AvatarDecorationSkuId)}: {user.AvatarDecorationSkuId}"
                        + $"\n{nameof(user.AvatarId)}: {user.AvatarId}"
                        + $"\n{nameof(user.DisplayAvatarId)}: {user.DisplayAvatarId}"
                        + $"\n{nameof(user.GuildAvatarId)}: {user.GuildAvatarId}"
                        + $"\n{nameof(user.GuildBannerHash)}: {user.GuildBannerHash}",
                    IsInline = true
                },
                new()
                {
                    Name = "State",
                    Value = $"{nameof(user.Status)}: {Enum.GetName(user.Status)}"
                        + $"\n{nameof(user.IsDeafened)}: {user.IsDeafened}"
                        + $"\n{nameof(user.IsMuted)}: {user.IsMuted}"
                        + $"\n{nameof(user.IsPending)}: {user.IsPending}"
                        + $"\n{nameof(user.IsSelfDeafened)}: {user.IsSelfDeafened}"
                        + $"\n{nameof(user.IsSelfMuted)}: {user.IsSelfMuted}"
                        + $"\n{nameof(user.IsStreaming)}: {user.IsStreaming}"
                        + $"\n{nameof(user.IsSuppressed)}: {user.IsSuppressed}"
                        + $"\n{nameof(user.IsVideoing)}: {user.IsVideoing}",
                    IsInline = true
                },
                new()
                {
                    Name = nameof(user.Activities),
                    Value = (user.Activities.Count == 0)? "-" : string.Join(
                        '\n',
                        user.Activities.Select(x => $"{x.Name} ({x.Type}): {x.Details}")
                    ),
                    IsInline = true
                },
                new()
                {
                    Name = "Voice",
                    Value = $"{nameof(user.VoiceChannel)}: {user.VoiceChannel}"
                        + $"\n{nameof(user.VoiceState)}: {user.VoiceState}"
                        + $"\n{nameof(user.VoiceSessionId)}: {user.VoiceSessionId}"
                        + $"\n{nameof(user.VoiceState)}: {user.VoiceState}"
                        + $"\n{nameof(user.AudioStream)}: {user.AudioStream}",
                    IsInline = true
                },
                new()
                {
                    Name = nameof(user.GuildPermissions),
                    Value = user.GuildPermissions,
                    IsInline = true
                },
                new()
                {
                    Name = nameof(user.MutualGuilds),
                    Value = (user.MutualGuilds.Count == 0)? "-" : string.Join('\n', user.MutualGuilds.Select(x => x.Name)),
                    IsInline = true
                },
                new()
                {
                    Name = nameof(user.Flags),
                    Value = user.Flags.GetFlagsString(
                        GuildUserFlags.DidRejoin,
                        GuildUserFlags.CompletedOnboarding,
                        GuildUserFlags.BypassesVerification,
                        GuildUserFlags.StartedOnboarding,
                        GuildUserFlags.IsGuest,
                        GuildUserFlags.StartedHomeActions,
                        GuildUserFlags.CompletedHomeActions,
                        GuildUserFlags.AutomodQuarantinedUsername,
                        GuildUserFlags.DmSettingUpsellAcknowledged
                    ),
                    IsInline = true
                },
                new()
                {
                    Name = nameof(user.PublicFlags),
                    Value = (user.PublicFlags == null)? "NULL" : user.PublicFlags.Value.GetFlagsString(
                        UserProperties.Staff,
                        UserProperties.Partner,
                        UserProperties.HypeSquadEvents,
                        UserProperties.BugHunterLevel1,
                        UserProperties.HypeSquadBravery,
                        UserProperties.HypeSquadBrilliance,
                        UserProperties.HypeSquadBalance,
                        UserProperties.EarlySupporter,
                        UserProperties.TeamUser,
                        UserProperties.System,
                        UserProperties.BugHunterLevel2,
                        UserProperties.VerifiedBot,
                        UserProperties.EarlyVerifiedBotDeveloper,
                        UserProperties.DiscordCertifiedModerator,
                        UserProperties.BotHTTPInteractions,
                        UserProperties.ActiveDeveloper
                    ),
                    IsInline = true
                },
            };
        };
        return embedBuilder;
    }
}
