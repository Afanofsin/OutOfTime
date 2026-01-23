using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class PoolManager : SerializedMonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    [OdinSerialize] public Dictionary<HitEffectType, HitPool> pools;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}