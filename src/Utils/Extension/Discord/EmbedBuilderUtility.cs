
namespace Discord;
public static class EmbedBuilderUtility
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
    public static async Task<EmbedBuilder> AddFieldsAsync<TElement>(this EmbedBuilder embedBuilder, IAsyncEnumerable<TElement> collection, Func<int, TElement, string> title, Func<int, TElement, string> value)
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
