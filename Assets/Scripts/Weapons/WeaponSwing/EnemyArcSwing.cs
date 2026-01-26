using System.Collections.Generic;
using Interfaces;
using PrimeTween;
using UnityEngine;

namespace DefaultNamespace.Weapons.WeaponSwing
{
    public class EnemyArcSwing : SwingBase, ISwing
    {
        [SerializeField] private float startAngle;
        [SerializeField] private float endAngle;
        [SerializeField] private Ease ease;
        [SerializeField] private TrailRenderer trail;
    
        private Tween _swingTween;
    
        private IReadOnlyDictionary<DamageType, float> _damage;
        private readonly HashSet<Collider2D> _hitTargets = new();

        public bool IsRunning => _swingTween.isAlive;
    
        private void Start() => trail.emitting = false;
    
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
           
            weaponCollider.enabled = true;
            trail.emitting = true;
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
                trail.emitting = false;
                    
            }); 
        }
    
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent<IDamageable>(out var damageable)) return;

            if (!_hitTargets.Add(other)) return;
            
            if (damageable is Player)
            {
                damageable.TakeDamage(_damage);
            }
        }
    }
}