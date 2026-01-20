using UnityEngine;

public interface IHealth
{
    float CurrentHealth { get; }
    float MaxHealth { get; }
    void Heal(float amount); 
    void Die();
}
