
using Discord;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManagerBot.Utils.Extension.Discord;
public static class EmbedBuilderExtension
{
    public const int MaxEmbedCount = 10;
    public static EmbedBuilder AddFields<TElement>(this EmbedBuilder embedBuilder, IEnumerable<TElement> collection, Func<int, TElement, string> title, Func<int, TElement, string> value)
    {
        int i = 0;
        foreach (TElement element in collection)
        {
            embedBuilder.AddField(
                title(i, element),
                value(i, element)
            );
            i++;
        }
        return embedBuilder;
    }
    public static async ValueTask<EmbedBuilder> AddFieldsAsync<TElement>(this EmbedBuilder embedBuilder, IAsyncEnumerable<TElement> collection, Func<int, TElement, string> title, Func<int, TElement, string> value)
    {
        int i = 0;
        await foreach (TElement element in collection)
        {
            embedBuilder.AddField(
                title(i, element),
                value(i, element)
            );
            i++;
        }
        return embedBuilder;
    }
}
