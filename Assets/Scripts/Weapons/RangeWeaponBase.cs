using System.Collections.Generic;
using PrimeTween;
using Sirenix.Serialization;
using UnityEngine;

public abstract class RangeWeaponBase : WeaponBase, IPickable
{
    [SerializeField] private float speedValue;
    [SerializeField] private float attackSpeedValue;
    [SerializeField] private Projectile bulletPrefab;
    [SerializeField] private Vector2 projectileAttenuation;
    [SerializeField] private float reloadTime;
    private ProjectilePool _projectilePool;
    private SpriteRenderer _weaponSprite;
    [OdinSerialize] private Dictionary<DamageType, float> _baseDamage = new();
    public override IReadOnlyDictionary<DamageType, float> BaseDamage => _baseDamage;
    public override float AttackSpeed => attackSpeedValue;
    private StatModifier _speedMod;
    
    [Header("Recoil")]
    private Tween _recoilTween;
    [SerializeField] private Ease ease;
    [SerializeField] private float recoilAngle;
    
    private void Awake()
    {
        _speedMod = new StatModifierBase(StatType.Speed, x => x + speedValue);
        _weaponSprite = gameObject.GetComponentInChildren<SpriteRenderer>();
        _projectilePool = ProjectilePool.Instance;
        _projectilePool.SetPrefab(bulletPrefab);
    }

    public virtual void Update()
    {
        _weaponSprite.flipY = transform.localRotation.eulerAngles.z is > 90f and < 270f;
        _weaponSprite.sortingOrder = transform.localRotation.eulerAngles.z is > 0f and < 180f ? 8 : 10;
    }
    
    public override void PerformAttack(Dictionary<DamageType, float> damageType, float durationModifier)
    {
        gameObject.transform.rotation = PlayerController.Instance.GetQuaternion();
        var radians = PlayerController.Instance.GetAngle() * Mathf.Deg2Rad;
        var dir = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));

        var projectile = _projectilePool.Get();
        var spawnPos = _weaponSprite.transform.TransformPoint(projectileAttenuation);
        projectile.transform.position = spawnPos;
        projectile.transform.rotation = PlayerController.Instance.GetQuaternion();
        var merged = DictionaryUtils.MergeIntersection(_baseDamage, damageType, (x, y) => x + x * y / 100);
        projectile.Launch(dir, merged);
        var flipAngle = !_weaponSprite.flipY ? recoilAngle : -recoilAngle;
        
        _recoilTween = Tween.Custom(
            PlayerController.Instance.GetAngle() + flipAngle,
            PlayerController.Instance.GetAngle(),
            Mathf.Max(0.1f, AttackSpeed * 0.25f),
            angle =>
            {
                transform.localRotation = Quaternion.Euler(0f, 0f, angle);
            },
            ease
        );
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
