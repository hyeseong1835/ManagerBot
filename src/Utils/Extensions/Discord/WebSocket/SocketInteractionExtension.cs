using System;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using ManagerBot.Utils.Extension.System;

namespace ManagerBot.Utils.Extension.Discord.WebSocket;
public static class SocketInteractionExtension
{
    #region ErrorRespond

    public static async ValueTask ErrorRespond(this SocketInteraction interaction, string title)
    {
        EmbedBuilder embedBuilder = new();
        {
            embedBuilder.WithColor(Color.Red);
            embedBuilder.WithTitle(title);
        }
        await interaction.RespondAsync(embed: embedBuilder.Build(), ephemeral: true);
    }
    public static async Task ErrorRespond(this SocketInteraction interaction, string title, string description)
    {
        EmbedBuilder embedBuilder = new();
        {
            embedBuilder.WithColor(Color.Red);
            embedBuilder.WithTitle(title);
            embedBuilder.WithDescription(description);
        }
        await interaction.RespondAsync(embed: embedBuilder.Build(), ephemeral: true);
    }
    public static async Task ErrorRespond(this SocketInteraction interaction,
        Exception exception, string? title = null, string? description = null)
    {
        await interaction.RespondAsync(embed: exception.GetEmbed(title, description), ephemeral: true);
    }

    #endregion

    #region ErrorFollowup

    public static async Task<RestFollowupMessage> ErrorFollowup(this SocketInteraction interaction, string title)
    {
        EmbedBuilder embedBuilder = new();
        {
            embedBuilder.WithColor(Color.Red);
            embedBuilder.WithTitle(title);
        }
        return await interaction.FollowupAsync(embed: embedBuilder.Build(), ephemeral: true);
    }
    public static async Task<RestFollowupMessage> ErrorFollowup(this SocketInteraction interaction, string title, string description)
    {
        EmbedBuilder embedBuilder = new();
        {
            embedBuilder.WithColor(Color.Red);
            embedBuilder.WithTitle(title);
            embedBuilder.WithDescription(description);
        }
        return await interaction.FollowupAsync(embed: embedBuilder.Build(), ephemeral: true);
    }
    public static async Task<RestFollowupMessage> ErrorFollowup(this SocketInteraction interaction,
        Exception exception, string? title = null, string? description = null)
    {
        return await interaction.FollowupAsync(embed: exception.GetEmbed(title, description), ephemeral: true);
    }

    #endregion

    public static async Task AutoRemoveRespond(this SocketInteraction interaction,
        string text = "...", Embed[]? embeds = null, bool isTTS = false,
        bool ephemeral = true, AllowedMentions? allowedMentions = null,
        MessageComponent? components = null, Embed? embed = null, RequestOptions? options = null,
        PollProperties? poll = null, int delay = 0)
    {
        await interaction.RespondAsync(
            text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options, poll
        );
        if(delay != 0) await Task.Delay(delay);
        await interaction.DeleteOriginalResponseAsync();
    }
    public static async Task AutoRemoveFollowup(this SocketInteraction interaction,
        string text = "...", Embed[]? embeds = null, bool isTTS = false,
        bool ephemeral = true, AllowedMentions? allowedMentions = null,
        MessageComponent? components = null, Embed? embed = null, RequestOptions? options = null,
        PollProperties? poll = null, int delay = 0)
    {
        await interaction.FollowupAsync(
            text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options, poll
        );
        if (delay != 0) await Task.Delay(delay);
        await interaction.DeleteOriginalResponseAsync();
    }
}
