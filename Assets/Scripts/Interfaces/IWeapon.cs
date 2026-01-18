using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    int Damage { get; }
    Sprite WeaponSprite { get; }
    int SpeedModifier { get; }
    List<DamageType> DamageTypes { get; }
    void PerformAttack(float f);
}
