namespace System.Collections.Generic;

public static class DictionaryUtility
{
    public static TValue GetOrAddValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TValue> valueGetter)
        where TKey : notnull
    {
        if (dictionary.TryGetValue(key, out TValue? v) == false)
        {
            v = valueGetter();
        }
        return v;
    }
    public static TValue GetOrAddValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        where TKey : notnull
    {
        if (dictionary.TryGetValue(key, out TValue? v) == false)
        {
            v = value;
        }
        return v;
    }
}