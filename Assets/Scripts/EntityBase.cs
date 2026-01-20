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
    [OdinSerialize] public IReadOnlyDictionary<DamageType, float> resists;
    public float MaxHealth => maxHealth;
    
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
    public virtual void Die() => Destroy(gameObject);
    public abstract void React();
    public abstract void Heal(float amount);
}