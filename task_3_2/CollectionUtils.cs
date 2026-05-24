using System;
using System.Collections.Generic;

public static class CollectionUtils
{
    public static List<T> Distinct<T>(List<T> source)
    {
        var result = new List<T>();
        var seen = new HashSet<T>();

        foreach (T item in source)
        {
            if (seen.Add(item))
            {
                result.Add(item);
            }
        }

        return result;
    }

    public static Dictionary<TKey, List<TValue>> GroupBy<TValue, TKey>(
        List<TValue> source,
        Func<TValue, TKey> keySelector) where TKey : notnull
    {
        var result = new Dictionary<TKey, List<TValue>>();

        foreach (TValue item in source)
        {
            TKey key = keySelector(item);

            if (!result.TryGetValue(key, out List<TValue>? group))
            {
                group = new List<TValue>();
                result[key] = group;
            }

            group.Add(item);
        }

        return result;
    }

    public static Dictionary<TKey, TValue> Merge<TKey, TValue>(
        Dictionary<TKey, TValue> first,
        Dictionary<TKey, TValue> second,
        Func<TValue, TValue, TValue> conflictResolver) where TKey : notnull
    {
        var result = new Dictionary<TKey, TValue>(first);

        foreach (KeyValuePair<TKey, TValue> pair in second)
        {
            if (result.TryGetValue(pair.Key, out TValue? existing))
            {
                result[pair.Key] = conflictResolver(existing, pair.Value);
            }
            else
            {
                result[pair.Key] = pair.Value;
            }
        }

        return result;
    }

    public static T MaxBy<T, TKey>(List<T> source, Func<T, TKey> selector)
        where TKey : IComparable<TKey>
    {
        if (source.Count == 0)
        {
            throw new InvalidOperationException("Sequence contains no elements.");
        }

        T maxItem = source[0];
        TKey maxKey = selector(maxItem);

        for (int i = 1; i < source.Count; i++)
        {
            T item = source[i];
            TKey key = selector(item);

            if (key.CompareTo(maxKey) > 0)
            {
                maxItem = item;
                maxKey = key;
            }
        }

        return maxItem;
    }
}
