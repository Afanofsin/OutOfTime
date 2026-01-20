using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;


public abstract class MeleeWeaponBase : SerializedMonoBehaviour, IWeapon, IEquipable
{
    [SerializeField] private float speedValue;
    private StatModifier _speedModifier;
    [SerializeField] private float attackSpeedValue;
    private StatModifier _attackSpeedModifier;
    [OdinSerialize] private ISwing swing;
    [SerializeField] private Sprite weaponSprite;
    [OdinSerialize] private Dictionary<DamageType, float> _baseDamage = new();
    public IReadOnlyDictionary<DamageType, float> BaseDamage => _baseDamage;

    private StatModifier speedMod;

    private void Awake()
    {
        speedMod = new StatModifierBase(StatType.Speed, x => x + speedValue);
        gameObject.GetComponentInChildren<SpriteRenderer>().sprite = weaponSprite;
    } 
    
    public void PerformAttack(float angle, Dictionary<DamageType, float> damageType, float durationModifier)
    {
        var merged = DictionaryUtils.MergeIntersection(_baseDamage, damageType, (x, y) => x + x * y/100);
        var duration = attackSpeedValue * durationModifier / 100;
        swing.StartSwing(angle, merged, Mathf.Max(0.1f, duration));
    }
    public StatModifier GetStatModifier() => speedMod;
}

