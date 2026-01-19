using System;
using System.Collections.Generic;
using Interfaces;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Player : MonoBehaviour, IHealth, IAttackReactor, IDamageable
{
    [SerializeField] private BaseStats baseStats;
    [SerializeField] private MeleeWeaponBase heldWeapon;
    
    public float CurrentHealth { get; }
    public float MaxHealth { get; }
    
    public PlayerStats _playerStats { get; private set; }

    private void Awake()
    {
        _playerStats = new PlayerStats(baseStats, new StatsMediator());
        Equip(heldWeapon);
    }
    
    public void Attack(float angle)
    {
        heldWeapon.PerformAttack(angle, _playerStats.Attack, _playerStats.AttackSpeed);
    }

    private void Equip(IEquipable item)
    {
        Debug.Log(_playerStats.Speed);
        _playerStats.Mediator.AddModifier(item.GetStatModifier());
        Debug.Log(_playerStats.Speed);
    }

    private void Unequip(IEquipable item)
    {
        
    }
    
    public void Heal(float amount)
    {
        throw new NotImplementedException();
    }

    public void Die()
    {
        throw new NotImplementedException();
    }

    public void React()
    {
        throw new NotImplementedException();
    }

    public void TakeDamage(IReadOnlyDictionary<DamageType, float> amount)
    {
        throw new NotImplementedException();
    }
}
