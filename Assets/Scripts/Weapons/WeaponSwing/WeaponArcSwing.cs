using System.Collections.Generic;
using Interfaces;
using UnityEngine;
using PrimeTween;

public class WeaponArcSwing : SwingBase, ISwing
{
    [SerializeField] private float startAngle;
    [SerializeField] private float endAngle;
    [SerializeField] private Ease ease;
    [SerializeField] private TrailRenderer trail;
    
    private Tween _swingTween;
    
    private IReadOnlyDictionary<DamageType, float> _damage;
    private readonly HashSet<Collider2D> _hitTargets = new();
    
    public void StartSwing(float attackAngle, IReadOnlyDictionary<DamageType, float> damage, float duration)
    {
        if (_swingTween.isAlive) return;
        
        _hitTargets.Clear();
        
        var from = attackAngle - startAngle;
        var to = attackAngle + endAngle;

        transform.localRotation = Quaternion.Euler(0f, 0f, from);
        weaponCollider.enabled = true;
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
            weaponCollider.enabled = false;
            _hitTargets.Clear();
        }); 
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IDamageable>(out var damageable) && damageable is not Player && _hitTargets.Add(other))
        {
            damageable.TakeDamage(_damage);
        }
    }
}
