using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    float SpeedModifier { get; }
    Dictionary<DamageType, float> Damage { get; }
    void PerformAttack(float f);
}
