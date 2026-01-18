using System;
using Interfaces;
using UnityEngine;

public class Goblin : EnemyBase
{
    private void Update()
    {
        
    }

    private void Start()
    {
        CurrentHealth = MaxHealth;
    }
    
    public override void Action()
    {
        
    }

    public override void Die()
    {
        Destroy(gameObject);
    }

    public override void Heal(int amount)
    {
        
    }

    public override void TakeDamage(int amount)
    {
        CurrentHealth -= amount;
        Debug.Log(CurrentHealth);
    }
}
