using UnityEngine;

public class BaseballBat : MeleeWeaponBase
{
    public override void Update()
    {
        if (swing.IsRunning) return;
        
        weaponSprite.sortingOrder = transform.localRotation.eulerAngles.z is > 0f and < 180f ? 8 : 11;
    }
}
