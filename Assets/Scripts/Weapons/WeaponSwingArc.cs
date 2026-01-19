using System;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;
using PrimeTween;
using Unity.VisualScripting;

public class WeaponSwing : MonoBehaviour
{
    [SerializeField] private float startAngle;
    [SerializeField] private float endAngle;
    [SerializeField] private float duration;
    [SerializeField] private Ease ease;
    [SerializeField] private LayerMask hitMask;
    private Tween _swingTween;
    
    private Collider2D _weaponCollider;
    private readonly HashSet<Collider2D> _hitTargets = new();

    private void Start()
    {
        _weaponCollider = gameObject.GetOrAddComponent<BoxCollider2D>();
        _weaponCollider.includeLayers = hitMask;
        _weaponCollider.excludeLayers -= hitMask;
    }

    public void StartSwing(float attackAngle)
    {   
        
        if(_swingTween.isAlive) return;
        
        _hitTargets.Clear();
        
        var from = attackAngle - startAngle;
        var to = attackAngle + endAngle;

        transform.localRotation = Quaternion.Euler(0f, 0f, from);
        
        _weaponCollider.enabled = true;
        
        _swingTween = Tween.Custom(
            from,
            to,
            duration,
            angle =>
            {
                transform.localRotation = Quaternion.Euler(0f, 0f, angle);
            },
            ease
        ).OnComplete(() => _weaponCollider.enabled = false); 
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_hitTargets.Add(other) && other.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(15);
        }

        if (_hitTargets.Add(other) && other.TryGetComponent<IAttackReactor>(out var reactor))
        {
            reactor.React();
        }
    }
}
