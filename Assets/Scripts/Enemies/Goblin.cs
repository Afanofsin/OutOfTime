using System;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

public class Goblin : EnemyBase, IAttackReactor
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

    public override void TakeDamage(Dictionary<DamageType, float> damage)
    {
        foreach (var damageValue in damage.Values)
        {
            CurrentHealth -= damageValue;
        }
        
        React();
    }

    public void React()
    {
        
    }
}
