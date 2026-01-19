using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[CreateAssetMenu(fileName = "BaseStats", menuName = "Stats/BaseStats")]
public class BaseStats : SerializedScriptableObject
{
    [OdinSerialize] public Dictionary<DamageType, float> attack;
    [OdinSerialize] public Dictionary<DamageType, float> resists;
    
    public float attackSpeed;
    public float health;
    public float speed;
    public float dashRange;
}

public enum StatType
{
    Attack,
    AttackSpeed,
    Speed,
    DashRange,
    Resist
}