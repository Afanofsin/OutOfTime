using NaughtyAttributes;

public interface IModifier{}

public enum WeaponModifierType
{
    Damage,
    AttackSpeed,
    WalkSpeed
}

[System.Serializable]
public struct WeaponModifier
{
    public WeaponModifierType modifiers;
    [EnableIf("modifiers", WeaponModifierType.Damage), AllowNesting] public DamageType damageModifierType;
    public float modifierValue;
}

