using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteAlways]
public class MeleeWeaponBase : MonoBehaviour, IWeapon
{
    [SerializeField] private float speedModifier;
    [SerializeField] private float attackSpeed;
    [SerializeField] private WeaponArcSwing swing;
    [SerializeField] private Sprite weaponSprite;
    [SerializeField] private DamageTypeEntry[] damageKeys;
    [SerializeField] private List<WeaponModifier> modifiers;
    
    private Dictionary<DamageType, float> _damageMap = new();

    public Dictionary<DamageType, float> Damage => _damageMap;
    
    public float SpeedModifier => speedModifier;
    
    private void Awake()
    {
        gameObject.GetOrAddComponent<SpriteRenderer>().sprite = weaponSprite;
    }

    private void Update()
    {
        foreach (var VARIABLE in _damageMap)
        {
            Debug.Log(VARIABLE);
        }
    }

    public void PerformAttack(float angle)
    {
        swing.StartSwing(angle, Damage);
    }
    
    public void AddDamageType(DamageTypeEntry damageEntry)
    {
        if (_damageMap.TryGetValue(damageEntry.key, out var value))
        {
            _damageMap[damageEntry.key] = value + damageEntry.value;
        }
        else
        {
            _damageMap.Add(damageEntry.key, damageEntry.value);
        }
    }

    public void AddDamageType(DamageTypeEntry[] damageEntries)
    {
        for (var i = 0; i < damageEntries.Length; i++)
        {
            if (_damageMap.TryGetValue(damageKeys[i].key, out var value))
            {
                _damageMap[damageKeys[i].key] = value + damageKeys[i].value;
            }
            else
            {
                _damageMap.Add(damageKeys[i].key, damageKeys[i].value);
            }
        }
    }

    public void ApplyWeaponModifiers()
    {
        foreach (var modifier in modifiers)
        {
            switch (modifier.modifiers)
            {
                case WeaponModifierType.Damage:
                {
                    if (_damageMap.TryGetValue(modifier.damageModifierType, out var dmgValue ))
                    {
                        _damageMap[modifier.damageModifierType] = dmgValue * modifier.modifierValue;
                    }
                    break;
                }
                case WeaponModifierType.AttackSpeed:
                {
                    attackSpeed *= modifier.modifierValue;
                    break;
                }
                case WeaponModifierType.WalkSpeed:
                {
                    speedModifier *= modifier.modifierValue;
                    break;
                }
            }
        }
    }
    
    public void OnValidate()
    {
        _damageMap.Clear();
        AddDamageType(damageKeys);
        ApplyWeaponModifiers();
    }
}
