using System.Collections.Generic;
using Interfaces;
using JetBrains.Annotations;
using UnityEngine;

public class Player : EntityBase, IDamageable
{
    [SerializeField] private BaseStats baseStats;
    public WeaponBase HeldWeapon { get; private set; }
    [SerializeField] private PlayerController controller;
    [SerializeField] private Inventory inventory;
    public PlayerStats PlayerStats { get; private set; }
    public ParticleSystem ps;
    
    [SerializeField] private float invulnerabilityDuration = 0.15f;
    private float _invulnerableUntil;
    public bool CanTakeDamage => Time.time >= _invulnerableUntil;
    
    public override void Awake()
    {
        base.Awake();
        PlayerStats = new PlayerStats(baseStats, new StatsMediator());
    }

    public void Start()
    {
        Equip(inventory.GetSlotItem(0));
    }
    
    public void Attack()
    {
        HeldWeapon.PerformAttack(PlayerStats.Attack, PlayerStats.AttackSpeed);
    }

    private void Equip(WeaponBase item)
    {
        if (HeldWeapon != null)
        {
            Unequip(HeldWeapon);
        }
        
        HeldWeapon = Instantiate(item, gameObject.transform);
        HeldWeapon.transform.localRotation = PlayerController.Instance.GetQuaternion();
        
        var mod = HeldWeapon.GetStatModifier();
        HeldWeapon.interactCollider.enabled = false;
        if (mod.MarkedForRemoval) return;
        
        mod.MarkedForRemoval = true;
        PlayerStats.Mediator.AddModifier(mod);
    }

    public void Next()
    {
        if (inventory.Next() == null) return;
        
        if (HeldWeapon is RangeWeaponBase)
        {
            ProjectilePool.Instance.Clear();
        }
        
        Equip(inventory.CurrentItem);
    }

    public void Previous()
    {
        if (inventory.Previous() == null) return;
        
        if (HeldWeapon is RangeWeaponBase)
        {
            ProjectilePool.Instance.Clear();
        }
        
        Equip(inventory.CurrentItem);
    }

    public void Drop()
    {
        var droppedItem = inventory.TryDropCurrent();
        if (droppedItem == null)
            return;

        SpawnDroppedWeapon(droppedItem);
        
        if (HeldWeapon is RangeWeaponBase)
        {
            ProjectilePool.Instance.Clear();
        }

        Equip(inventory.CurrentItem);
    }

    private void Unequip(WeaponBase item)
    {
        PlayerStats.Mediator.Update();
        item.GetStatModifier().MarkedForRemoval = false;
        Destroy(HeldWeapon.gameObject);
    }
    
    public override void React()
    {
        
    }

    public void PickUp(IPickable item)
    {
        item.PickUp(inventory);
    }
    
    private void SpawnDroppedWeapon(WeaponBase weapon)
    {
        var instance = Instantiate(weapon, transform.position, Quaternion.identity);
        instance.interactCollider.enabled = true;
        
    }

    public void TakeDamage(IReadOnlyDictionary<DamageType, float> damage)
    {
        if (!CanTakeDamage) return;
        
        _invulnerableUntil = Time.time + invulnerabilityDuration;

        foreach (var damageKvp in damage)
        {
            CurrentHealth -= Mathf.Max(0, damageKvp.Value - damageKvp.Value * (resists[damageKvp.Key] / 100));
        }
        React();
    }
}
