using System;
using System.Collections.Generic;

public static class DictionaryUtils
{
    public static Dictionary<TKey, TValue> Merge<TKey, TValue>(
        Dictionary<TKey, TValue> a,
        Dictionary<TKey, TValue> b,
        Func<TValue, TValue, TValue> mergeFunc)
    {
        var result = new Dictionary<TKey, TValue>(a);

        foreach (var (key, value) in b)
        {
            result[key] = result.TryGetValue(key, out var existing) ? mergeFunc(existing, value) : value;
        }
        return result;
    }
    public static Dictionary<TKey, TValue> MergeIntersection<TKey, TValue>(
        Dictionary<TKey, TValue> a,
        Dictionary<TKey, TValue> b,
        Func<TValue, TValue, TValue> mergeFunc)
    {
        var result = new Dictionary<TKey, TValue>();

        foreach (var (key, valueA) in a)
        {
            if (b.TryGetValue(key, out var valueB))
            {
                result[key] = mergeFunc(valueA, valueB);
            }
        }

        return result;
    }
}