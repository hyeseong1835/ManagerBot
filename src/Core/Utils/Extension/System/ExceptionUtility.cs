using Discord;
namespace System;
public static class ExceptionUtility
{
    public static Embed GetEmbed(this Exception error)
    {
        return new EmbedBuilder()
        {
            Title = error.GetType().Name,
            Description = error.Message,
            Color = new Color(255, 0, 0)
        }.Build();
    }
    public static Embed GetEmbed(this Exception error, string? title = null, string? description = null)
    {
        return new EmbedBuilder()
        {
            Title = (title == null)? error.GetType().Name : $"{title} ({error.GetType().Name})",
            Description = (description == null)? error.Message : $"{description}\n{error.Message}",
            Color = new Color(255, 0, 0)
        }.Build();
    }
    public static Embed[] GetDebugEmbed(this Exception error, string title, Color color)
    {
        return new Embed[] {
            new EmbedBuilder()
            {
                Title = title,
                Description = error.Message,
                Color = color
            }.Build(),
            new EmbedBuilder()
            {
                Title = "Stack Trace",
                Description = error.StackTrace,
                Color = color
            }.Build()
        };
    }
    public static Embed[] GetDebugEmbed(this Exception error, Color embedColor, Color stackTraceColor)
    {
        return new Embed[] {
            new EmbedBuilder()
            {
                Title = error.GetType().Name,
                Description = error.Message,
                Color = embedColor
            }.Build(),
            new EmbedBuilder()
            {
                Title = "Stack Trace",
                Description = error.StackTrace,
                Color = stackTraceColor
            }.Build()
        };
    }
    public static Embed[] GetEmbed(this Exception error, Color color)
    {
        return GetDebugEmbed(error, error.GetType().Name, color);
    }
}