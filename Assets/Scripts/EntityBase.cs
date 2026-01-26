using System;
using System.Collections.Generic;
using Interfaces;
using PrimeTween;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using Random = UnityEngine.Random;

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
    private bool _isDead = false;
    
    
    public Action<float> onHealthChanged;
    public virtual float CurrentHealth
    {
        get => currentHealth;

        set
        {
            currentHealth = Mathf.Clamp(value, 0f, maxHealth);
            
            onHealthChanged?.Invoke(currentHealth);

            if (_isDead) return;

            if (currentHealth <= 0f)
            {
                _isDead = true;
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
            0.35f, Ease.InQuart).OnComplete( () =>
        {
            onEntityDeath?.Invoke();
            Destroy(gameObject);
            SoundManager.Instance.Play(SoundId.Death);
        });
    }

    public virtual void React()
    {
        var instance = PoolManager.Instance.hitPools[hitEffect].Get();
        instance.transform.position = transform.position;
        flashEffect.FlashEffects();
        SoundManager.Instance.Play(SoundId.EnemyHit);
    }
    public virtual void Heal(float amount) => CurrentHealth += amount;
}