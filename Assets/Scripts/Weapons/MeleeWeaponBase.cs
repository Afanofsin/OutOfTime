using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponBase : MonoBehaviour, IWeapon
{
    [SerializeField] private int damage;
    [SerializeField] private int speedModifier;
    [SerializeField] private WeaponSwing swing;
    [SerializeField] private List<DamageType> damageTypes;
    [SerializeField] private Sprite weaponSprite;
    
    public int Damage
    {
        get => damage;

        set
        {
            damage = value;
        }

    }
    
    public int SpeedModifier
    {
        get => speedModifier;

        set
        {
            speedModifier = value;
        }
    }
    
    public List<DamageType> DamageTypes
    {
        get => damageTypes;
        
        set
        {
            damageTypes = value;
        }
    }

    public Sprite WeaponSprite => weaponSprite;

    public void PerformAttack(float angle)
    {
        swing.StartSwing(angle);
    }
}
