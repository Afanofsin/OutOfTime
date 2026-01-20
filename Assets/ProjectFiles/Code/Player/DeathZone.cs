using System.Collections.Generic;
using Interfaces;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace ProjectFiles.Code.Player
{
    public class DeathZone : SerializedMonoBehaviour
    {
        [OdinSerialize] private IReadOnlyDictionary<DamageType, float> _damage;
        private Collider2D _weaponCollider;
        private readonly HashSet<Collider2D> _hitTargets = new();
        
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
}