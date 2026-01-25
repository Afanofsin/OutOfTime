using System;
using System.Collections.Generic;
using Interfaces;
using PrimeTween;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public abstract class EntityBase : SerializedMonoBehaviour, IHealth, IAttackReactor
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    [OdinSerialize] protected HitEffectType hitEffect;
    [HideInInspector] protected FlashEffect flashEffect;
    [OdinSerialize] protected IReadOnlyDictionary<DamageType, float> resists;
    private Tween deathTween;
    public float MaxHealth => maxHealth;
    
    public Action onEntityDeath;
    public Action<float> onHealthChanged;
    
    public float CurrentHealth
    {
        get => currentHealth;

        set
        {
            currentHealth = value;
            onHealthChanged?.Invoke(currentHealth);   
            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    public virtual void Awake()
    {
        flashEffect = GetComponent<FlashEffect>();
        CurrentHealth = maxHealth;
    }

    public virtual void Die()
    {
        deathTween = Tween.Scale(gameObject.transform, gameObject.transform.localScale, new Vector3(0.1f, 0.1f, 0.1f),
            0.35f, Ease.InQuart).OnComplete(() =>
        {
            onEntityDeath?.Invoke();
            Destroy(gameObject);
        });
    }

    public virtual void React()
    {
        var instance = PoolManager.Instance.hitPools[hitEffect].Get();
        instance.transform.position = transform.position;
        flashEffect.FlashEffects();
    }
    public virtual void Heal(float amount) => CurrentHealth += amount;
}