using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class PoolManager : SerializedMonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    [OdinSerialize] public Dictionary<HitEffectType, HitPool> hitPools;
    [OdinSerialize] public Dictionary<BulletType, ProjectilePool> projectilePools;

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

public enum HitEffectType
{
    Blood,
    GreenBlood,
    Sand,
    PurpleBlood
}

public enum BulletType
{
    Bullet,
    Fire,
    Blaster,
    Arrow,
    Syringe,
    Potion,
    Card,
    Ice,
    Kunai
}