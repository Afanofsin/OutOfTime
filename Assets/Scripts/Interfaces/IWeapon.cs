using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    IReadOnlyDictionary<DamageType, float> BaseDamage { get; }
    void PerformAttack(Dictionary<DamageType, float> damage, float durationMultiplier);
}
