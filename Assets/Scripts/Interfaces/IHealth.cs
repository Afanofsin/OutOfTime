using UnityEngine;

public interface IHealth
{
    float CurrentHealth { get; }
    float MaxHealth { get; }
    void Heal(int amount); 
    void Die();
}
