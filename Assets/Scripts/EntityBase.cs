using System;
using System.Collections.Generic;
using Interfaces;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public abstract class EntityBase : SerializedMonoBehaviour, IHealth, IAttackReactor
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    [OdinSerialize] protected HitEffectType hitEffect;
    [OdinSerialize] protected IReadOnlyDictionary<DamageType, float> resists;
    public float MaxHealth => maxHealth;
    
    public Action onEntityDeath;
    
    public float CurrentHealth
    {
        get => currentHealth;

        set
        {
            currentHealth = value;

            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    public virtual void Awake() => CurrentHealth = maxHealth;

    public virtual void Die()
    {
        onEntityDeath?.Invoke();
        Destroy(gameObject);
    }

    public virtual void React()
    {
        var instance = PoolManager.Instance.pools[hitEffect].Get();
        instance.transform.position = transform.position;
    }
    public virtual void Heal(float amount) => CurrentHealth += amount;
}