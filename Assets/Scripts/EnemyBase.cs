using System.Collections.Generic;
using Interfaces;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public abstract class EnemyBase : MonoBehaviour, IEnemyAction, IHealth, IDamageable
{
    [SerializeField] private float maxHealth;
    [SerializeField, ReadOnly] private float _currentHealth;

    [SerializeField, ReadOnly] private GameObject target;
    public GameObject Target => target;
    public float CurrentHealth
    {
        get => _currentHealth;

        set
        {
            _currentHealth = value;

            if (_currentHealth <= 0)
            {
                Die();
            }
        }
    }

    public void SetTarget(GameObject targetGo)
    {
        target = targetGo;
    }
    
    public float MaxHealth => maxHealth;
    
    public abstract void Action();
    public abstract void Die();
    public abstract void Heal(int amount);
    public abstract void TakeDamage(Dictionary<DamageType, float> amount);

}
