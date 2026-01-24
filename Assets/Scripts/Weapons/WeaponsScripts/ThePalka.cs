using UnityEngine;

public class ThePalka : RangeWeaponBase
{
    public override void Update()
    {
        weaponSprite.flipX = transform.localRotation.eulerAngles.z is > 90f and < 270f;
        weaponSprite.sortingOrder = transform.localRotation.eulerAngles.z is > 0f and < 180f ? 8 : 10;
    }
}
