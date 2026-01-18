using UnityEngine;

public interface IHealth
{
    int CurrentHealth { get; }
    int MaxHealth { get; }
    void Heal(int amount); 
    void Die();
}
