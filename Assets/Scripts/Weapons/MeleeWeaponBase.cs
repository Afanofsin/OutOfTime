using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public abstract class WeaponBase : SerializedMonoBehaviour, IWeapon, IEquipable
{
    public abstract IReadOnlyDictionary<DamageType, float> BaseDamage { get; }
    public abstract float AttackSpeed { get; }
    public abstract void PerformAttack(Dictionary<DamageType, float> damage, float durationMultiplier);
    public abstract StatModifier GetStatModifier();
    [SerializeField] public readonly int id;
    [SerializeField] public Collider2D interactCollider;
}

public abstract class MeleeWeaponBase : WeaponBase, IPickable
{
    [SerializeField] private float speedValue;
    [SerializeField] private float attackSpeedValue;
    
    [OdinSerialize] private ISwing _swing;
    private SpriteRenderer _weaponSprite;
    [OdinSerialize] private Dictionary<DamageType, float> _baseDamage = new();
    public override IReadOnlyDictionary<DamageType, float> BaseDamage => _baseDamage;
    public override float AttackSpeed => attackSpeedValue;
    
    private StatModifier _speedMod;
    private void Awake()
    {
        _speedMod = new StatModifierBase(StatType.Speed, x => x + speedValue);
        _weaponSprite = _weaponSprite = gameObject.GetComponentInChildren<SpriteRenderer>();
    } 
    
    public virtual void Update()
    {
        _weaponSprite.flipX = transform.localRotation.eulerAngles.z is > 90f and < 270f;
        _weaponSprite.sortingOrder = transform.localRotation.eulerAngles.z is > 0f and < 180f ? 8 : 10;
    }
    
    public override void PerformAttack(Dictionary<DamageType, float> damageType, float durationModifier)
    {
        var merged = DictionaryUtils.MergeIntersection(_baseDamage, damageType, (x, y) => x + x * y / 100);
        var duration = Mathf.Max(0.1f, attackSpeedValue * durationModifier / 100);
        _swing.StartSwing(PlayerController.Instance.GetAngle(), merged, duration);
    }
    
    public override StatModifier GetStatModifier() => _speedMod;

    public void PickUp(Inventory context)
    {
        if (context.TryAdd(WeaponDatabase.Instance.GetWeaponByID(id)))
        {
            Destroy(gameObject);
        }
    }
}

