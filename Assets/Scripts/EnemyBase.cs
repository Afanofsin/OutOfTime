using Interfaces;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public abstract class EnemyBase : MonoBehaviour, IEnemyAction, IHealth, IDamageable
{
    [SerializeField] private int maxHealth;
    [SerializeField, ReadOnly] private int _currentHealth;

    [SerializeField, ReadOnly] private GameObject target;
    public GameObject Target => target;
    public int CurrentHealth
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
    
    public int MaxHealth => maxHealth;
    
    public abstract void Action();
    public abstract void Die();
    public abstract void Heal(int amount);
    public abstract void TakeDamage(int amount);

}
