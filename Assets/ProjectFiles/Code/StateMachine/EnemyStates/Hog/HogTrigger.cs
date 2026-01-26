using System;
using Interfaces;
using UnityEngine;

namespace ProjectFiles.Code.StateMachine.EnemyStates.Hog
{
    public class HogTrigger : MonoBehaviour
    {
        private Enemies.Hog hog;
        [SerializeField] protected LayerMask wallLayer;
        
        private void Awake()
        {
            hog = GetComponentInParent<Enemies.Hog>();
        }
        
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!hog.isCharging) return;
        
            // Hit player during charge
            var damageable = collision.gameObject.GetComponent<IDamageable>();
            if (damageable != null && collision.gameObject == hog.Target)
            {
                damageable.TakeDamage(hog.chargeDamageDict);
                Debug.Log($"{name}: Hit player during charge!");
                hog.chargeHitSomething = true;
                return;
            }
        
            // Hit wall (fallback if raycast missed)
            if (((1 << collision.gameObject.layer) & wallLayer) != 0)
            {
                Debug.Log($"{name}: Collision with wall!");
                hog.chargeHitSomething = true;
            }
        }
    }
}