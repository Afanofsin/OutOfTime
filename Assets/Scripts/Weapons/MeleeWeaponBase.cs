using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public abstract class WeaponBase : SerializedMonoBehaviour, IWeapon, IEquipable
{
    public abstract IReadOnlyDictionary<DamageType, float> BaseDamage { get; }
    public abstract void PerformAttack(float angle, Dictionary<DamageType, float> damage, float durationMultiplier);
    public abstract StatModifier GetStatModifier();
    [SerializeField] public readonly int id;
}


public abstract class MeleeWeaponBase : WeaponBase, IPickable
{
    [SerializeField] private float speedValue;
    [SerializeField] private float attackSpeedValue;
    
    [OdinSerialize] private ISwing swing;
    [SerializeField] private Sprite weaponSprite;
    [OdinSerialize] private Dictionary<DamageType, float> _baseDamage = new();
    public override IReadOnlyDictionary<DamageType, float> BaseDamage => _baseDamage;
    
    public Sprite PickableSprite => weaponSprite;
    
    private StatModifier _speedMod;
    private StatModifier _attackSpeedModifier;
    private void Awake()
    {
        _speedMod = new StatModifierBase(StatType.Speed, x => x + speedValue);
        gameObject.GetComponentInChildren<SpriteRenderer>().sprite = PickableSprite;
    } 
    
    public override void PerformAttack(float angle, Dictionary<DamageType, float> damageType, float durationModifier)
    {
        var merged = DictionaryUtils.MergeIntersection(_baseDamage, damageType, (x, y) => x + x * y/100);
        var duration = attackSpeedValue * durationModifier / 100;
        swing.StartSwing(angle, merged, Mathf.Max(0.1f, duration));
    }
    
    public override StatModifier GetStatModifier() => _speedMod;

    public void PickUp(Inventory context)
    {
        if (context.TryAdd(this))
        {
            Destroy(gameObject);
        }
    }
}

