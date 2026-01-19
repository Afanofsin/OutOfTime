using System.Collections.Generic;
using Interfaces;
using UnityEngine;

public class Goblin : EnemyEntityBase, IDamageable
{
    public void TakeDamage(IReadOnlyDictionary<DamageType, float> damage)
    {
        foreach (var damageKvp in damage)
        {
            CurrentHealth -= Mathf.Max(0, damageKvp.Value - damageKvp.Value * (resists[damageKvp.Key] / 100));
        }
        React();
    }

    public override void React()
    {
        
    }
    public override void Action()
    {
        
    }
    
    public override void Heal(float amount)
    {
        
    }
}
