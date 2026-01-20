using System.Collections.Generic;
using Interfaces;
using UnityEngine;
using PrimeTween;

public class WeaponArcSwing : MonoBehaviour, ISwing
{
    [SerializeField] private float startAngle;
    [SerializeField] private float endAngle;
    [SerializeField] private Ease ease;
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private TrailRenderer trail;
    private Tween _swingTween;
    private IReadOnlyDictionary<DamageType, float> _damage;
    private Collider2D _weaponCollider;
    private readonly HashSet<Collider2D> _hitTargets = new();
    
    private void Start()
    {
        _weaponCollider = gameObject.GetComponent<Collider2D>();
        _weaponCollider.includeLayers = hitMask;
        _weaponCollider.excludeLayers -= hitMask;
    }

    public void StartSwing(float attackAngle, IReadOnlyDictionary<DamageType, float> damage, float duration)
    {
        
        if (_swingTween.isAlive) return;
        
        _hitTargets.Clear();
        
        var from = attackAngle - startAngle;
        var to = attackAngle + endAngle;

        transform.localRotation = Quaternion.Euler(0f, 0f, from);
        _weaponCollider.enabled = true;
        _damage = damage;
        
        trail.Clear();
        _swingTween = Tween.Custom(
            from,
            to,
            duration,
            angle =>
            {
                transform.localRotation = Quaternion.Euler(0f, 0f, angle);
            },
            ease
        ).OnComplete(() =>
        { 
            _weaponCollider.enabled = false;
        }); 
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_hitTargets.Add(other)) return;

        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(_damage);
        }
        
        if (other.TryGetComponent<IAttackReactor>(out var reactor))
        {
            reactor.React();
        }
    }
}
