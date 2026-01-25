using UnityEngine;

public class BaseballBat : MeleeWeaponBase
{
    public override void Update()
    {
        if (swing.IsRunning) return;
        
        weaponSprite.sortingOrder = transform.localRotation.eulerAngles.z is > 45f and < 135f ? 8 : 11;
    }
}
