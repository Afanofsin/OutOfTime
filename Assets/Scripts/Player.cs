using System;
using System.Collections;
using System.Collections.Generic;
using Interfaces;
using JetBrains.Annotations;
using PrimeTween;
using UnityEngine;

public class Player : EntityBase, IDamageable
{
    [SerializeField] private BaseStats baseStats;
    public WeaponBase HeldWeapon { get; private set; }
    public SkillBase HeldSkill { get; private set; }
    [SerializeField] private PlayerController controller;
    [SerializeField] private Inventory inventory;
    public PlayerStats PlayerStats { get; private set; }
    public ParticleSystem playerParticles;
    
    [SerializeField] private float invulnerabilityDuration;
    private float _invulnerableUntil;
    public bool CanTakeDamage => Time.time >= _invulnerableUntil;
    
    [Header("Bandage")]
    public bool IsBandaged { get; private set; }
    private const float BandageTime = 20f;
    private WaitForSeconds _bandageTime = new(BandageTime);
    private Coroutine _bandageRoutine;
    [Header("SkillCD")] 
    private WaitForSeconds _skillCooldown;
    private bool _isSkilled = true;
    
    public override void Awake()
    {
        base.Awake();
        PlayerStats = new PlayerStats(baseStats, new StatsMediator());
    }

    public void Start()
    {
        //Equip(inventory.GetSlotItem(0));
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
        
        Equip(inventory.CurrentItem);
    }

    public void Previous()
    {
        if (inventory.Previous() == null) return;
        
        Equip(inventory.CurrentItem);
    }

    public void Drop()
    {
        var droppedItem = inventory.TryDropCurrent();
        if (droppedItem == null)
            return;

        SpawnDropped(droppedItem);
        
        Equip(inventory.CurrentItem);
    }

    private IEnumerator SkillCooldown()
    {
        _isSkilled = false;
        yield return _skillCooldown;
        _isSkilled = true;
    }

    public void UseSkill()
    {
        if (HeldSkill != null && CheckCost(HeldSkill) && _isSkilled)
        {
            CurrentHealth -= HeldSkill.Cost;
            HeldSkill.Perform();
            _skillCooldown = new WaitForSeconds(HeldSkill.CoolDown);
            StartCoroutine(SkillCooldown());
        }
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

    private void Update()
    {
        
    }

    public void UseBandage()
    {
        if (!inventory.Bandages) return;

        if (_bandageRoutine != null)
        {
            StopCoroutine(_bandageRoutine);
        }

        _bandageRoutine = StartCoroutine(Bandaged());
    }

    private IEnumerator Bandaged()
    {
        inventory.SubtractBandage();
        IsBandaged = true;

        yield return _bandageTime;

        IsBandaged = false;
        _bandageRoutine = null;
    }

    public void PickUp(IPickable item)
    {
        if (item is IBoon boon)
        {
            foreach (var boonMod in boon.GetBoonMods())
                PlayerStats.Mediator.AddModifier(boonMod);

            PlayerStats.Mediator.Update();
        }

        if (item is SkillBase skill)
        {
            if (HeldSkill != null)
            {
                SpawnDropped(HeldSkill);
            }

            HeldSkill = SkillDatabase.Instance.GetSkillByID(skill.id);
        }

        item.PickUp(inventory);
    }
    
    private void SpawnDropped(IPickable item)
    {
        if (item is WeaponBase weapon)
        {
            var instance = Instantiate(weapon, transform.position, Quaternion.identity);

            var interactCollider = instance.interactCollider;
            interactCollider.enabled = false;

            PlayDropTween(instance.transform, () =>
            {
                interactCollider.enabled = true;
            });
        }

        if (item is SkillBase skill)
        {
            var instance = Instantiate(skill, transform.position, Quaternion.identity);
            
            PlayDropTween(instance.transform, () => { });
        }
    }
    private void PlayDropTween(Transform target, Action onComplete)
    {
        target.rotation = Quaternion.identity;

        var startPos = target.position;
        var forward = PlayerController.Instance.transform.right;

        var midPos = startPos + forward * 0.3f + Vector3.up * 0.4f;
        var endPos = startPos + forward * 0.6f;

        Tween.Position(target, midPos, 0.15f, Ease.OutQuad)
            .Chain(
                Tween.Position(target, endPos, 0.15f, Ease.InQuad)
            )
            .OnComplete(() =>
            {
                target.rotation = Quaternion.identity;
                onComplete?.Invoke();
            });
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

    private bool CheckCost(SkillBase skill) => CurrentHealth - skill.Cost > 0;
    

    public void TickSubtract(float tickAmount)
    {
        if (IsBandaged) return;
        CurrentHealth -= tickAmount;
        playerParticles.Play();
    }
    public void TickAdd(float tickAmount) => CurrentHealth += tickAmount;
}
