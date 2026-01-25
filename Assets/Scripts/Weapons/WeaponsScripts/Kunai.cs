using UnityEngine;

public class Kunai : RangeWeaponBase
{
    public override void Update()
    {
        weaponSprite.flipX = transform.localRotation.eulerAngles.z is > 90f and < 270f;
        weaponSprite.sortingOrder = transform.localRotation.eulerAngles.z is > 45f and < 135f ? 8 : 11;
    }
}
