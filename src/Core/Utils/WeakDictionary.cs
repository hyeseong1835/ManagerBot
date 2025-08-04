using System.Diagnostics.CodeAnalysis;

namespace System.Collections.Generic;
public class WeakDictionary<TKey, TValue> : Dictionary<TKey, WeakReference<TValue>>, ICollection<KeyValuePair<TKey, TValue>>
    where TKey : notnull
    where TValue : class
{
    public bool IsReadOnly => throw new NotImplementedException();

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, new WeakReference<TValue>(item.Value));
    }
    public void Add(TKey key, TValue value)
    {
        Add(key, new WeakReference<TValue>(value));
    }

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        if (TryGetValue(key, out WeakReference<TValue>? weakValue))
        {
            return weakValue.TryGetTarget(out value);
        }
        else
        {
            value = default;
            return false;
        }
    }

    public void Collect()
    {
        Enumerator enumerator = GetEnumerator();

        do {
            KeyValuePair<TKey, WeakReference<TValue>> pair = enumerator.Current;
            if (!pair.Value.TryGetTarget(out TValue? target))
            {
                Remove(pair.Key);
            }
        }
        while (enumerator.MoveNext());
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
        => TryGetValue(item.Key, out WeakReference<TValue>? value) && value.TryGetTarget(out TValue? target) && target == item.Value;

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        => throw new NotImplementedException();

    public bool Remove(KeyValuePair<TKey, TValue> item)
        => TryGetValue(item.Key, out WeakReference<TValue>? value) && value.TryGetTarget(out TValue? target) && target == item.Value && Remove(item.Key);

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        => throw new NotImplementedException();
}
