using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class SerializableDictionary<TKey, TValue> 
    : ISerializationCallbackReceiver
{
    [SerializeField] private List<TKey> keys = new();
    [SerializeField] private List<TValue> values = new();

    protected Dictionary<TKey, TValue> dictionary = new();

    public IReadOnlyDictionary<TKey, TValue> Dictionary => dictionary;

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();

        foreach (var kvp in dictionary)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        dictionary.Clear();

        for (int i = 0; i < Mathf.Min(keys.Count, values.Count); i++)
        {
            dictionary[keys[i]] = values[i];
        }
    }
}

[System.Serializable]
public class DamageTypeFloatDictionary : SerializableDictionary<DamageType, float> {}