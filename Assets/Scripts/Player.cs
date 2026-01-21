using System.Collections.Generic;
using Interfaces;
using JetBrains.Annotations;
using UnityEngine;

public class Player : EntityBase, IDamageable
{
    [SerializeField] private BaseStats baseStats;
    [SerializeField] private WeaponBase heldWeapon;
    [SerializeField] private PlayerController controller;
    [SerializeField] private Inventory inventory;
    public PlayerStats PlayerStats { get; private set; }
    
    public override void Awake()
    {
        base.Awake();
        PlayerStats = new PlayerStats(baseStats, new StatsMediator());
    }

    public void Start()
    {
        Equip(inventory.GetSlotItem(0));
    }

    public void Update()
    { 
        controller.moveSpeed = PlayerStats.Speed;
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Equip(inventory.GetSlotItem(0));
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Equip(inventory.GetSlotItem(1));
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Equip(inventory.GetSlotItem(2));
        }
    }

    public void Attack(float angle)
    {
        heldWeapon.PerformAttack(angle, PlayerStats.Attack, PlayerStats.AttackSpeed);
    }

    private void Equip(WeaponBase item)
    {
        if (heldWeapon != null)
        {
            Unequip(heldWeapon);
        }
        
        heldWeapon = Instantiate(item, gameObject.transform);
        
        var mod = heldWeapon.GetStatModifier();
        if (mod.MarkedForRemoval) return;
        
        mod.MarkedForRemoval = true;
        PlayerStats.Mediator.AddModifier(mod);

    }

    private void Unequip(WeaponBase item)
    {
        PlayerStats.Mediator.Update();
        item.GetStatModifier().MarkedForRemoval = false;
        Destroy(heldWeapon.gameObject);
    }
    
    public override void Heal(float amount)
    {
        
    }
    
    public override void React()
    {
        
    }

    public void PickUp(IPickable item)
    {
        item.PickUp(inventory);
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
