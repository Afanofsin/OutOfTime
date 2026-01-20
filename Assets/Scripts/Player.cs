using System;
using System.Collections.Generic;
using Interfaces;
using JetBrains.Annotations;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Player : EntityBase, IDamageable
{
    [SerializeField] private BaseStats baseStats;
    [SerializeField] [CanBeNull] private MeleeWeaponBase heldWeapon;
    [SerializeField] private PlayerController controller;
    public PlayerStats PlayerStats { get; private set; }

    public override void Awake()
    {
        base.Awake();
        PlayerStats = new PlayerStats(baseStats, new StatsMediator());
        Equip(heldWeapon);
    }

    public void Update()
    { 
        controller.moveSpeed = PlayerStats.Speed;
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Unequip(heldWeapon);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Equip(heldWeapon);
        }
    }

    public void Attack(float angle)
    {
        heldWeapon.PerformAttack(angle, PlayerStats.Attack, PlayerStats.AttackSpeed);
    }

    private void Equip(IEquipable item)
    {
        var mod = item.GetStatModifier();
        if (mod.MarkedForRemoval) return;
        
        mod.MarkedForRemoval = true;
        PlayerStats.Mediator.AddModifier(item.GetStatModifier());

    }

    private void Unequip(IEquipable item)
    {
        PlayerStats.Mediator.Update();
        item.GetStatModifier().MarkedForRemoval = false;
    }
    
    public override void Heal(float amount)
    {
        
    }
    
    public override void React()
    {
        
    }

    public void TakeDamage(IReadOnlyDictionary<DamageType, float> damage)
    {
        foreach (var damageKvp in damage)
        {
            CurrentHealth -= Mathf.Max(0, damageKvp.Value - damageKvp.Value * (resists[damageKvp.Key] / 100));
        }
        React();
    }
}
