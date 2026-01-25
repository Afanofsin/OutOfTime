using System;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;
using PrimeTween;

public class BloodArcSwing : SwingBase, ISwing
{
    [SerializeField] private float startAngle;
    [SerializeField] private float endAngle;
    [SerializeField] private Ease ease;
    [SerializeField] private TrailRenderer trail;
    
    private Tween _swingTween;
    
    private IReadOnlyDictionary<DamageType, float> _damage;
    private readonly HashSet<Collider2D> _hitTargets = new();

    public bool IsRunning => _swingTween.isAlive;

    public override void Awake()
    {
        
    }

    public void OnEnable()
    {
        trail.emitting = false;
    }

    public void StartSwing(float attackAngle, IReadOnlyDictionary<DamageType, float> damage, float duration)
    {
        if (_swingTween.isAlive) return;
        
        _hitTargets.Clear();
        
        
        float from;
        float to;
        
        if (attackAngle is > 90f and < 270f)
        {
            from = attackAngle - startAngle;
            to = attackAngle + endAngle;
        }
        else
        {
            from = attackAngle + startAngle;
            to = attackAngle - endAngle;
        }
        
        _damage = damage;
        
        trail.Clear();
        trail.emitting = true;
        trail.time = 0.2f;
        
        _swingTween = Tween.Custom(
            from,
            to,
            duration,
            angle =>
            {
                transform.rotation = Quaternion.Euler(0f, 0f, angle);
            },
            ease
        ).OnComplete(() =>
        { 
            _hitTargets.Clear();
            Destroy(gameObject);
        }); 
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<IDamageable>(out var damageable) || damageable is Player) return;

        if (!_hitTargets.Add(other)) return;
        
        damageable.TakeDamage(_damage);
    }
}