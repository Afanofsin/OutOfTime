using UnityEngine;

public class Axe : MeleeWeaponBase
{
    public override void Update()
    {
        if (swing.IsRunning) return;
        
        weaponSprite.flipX = transform.localRotation.eulerAngles.z is <= 90f or >= 270f;
        weaponSprite.sortingOrder = transform.localRotation.eulerAngles.z is > 30f and < 150f ? 8 : 11;
    }
}
