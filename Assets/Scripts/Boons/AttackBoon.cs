using System.Collections.Generic;
using Sirenix.Serialization;

public class AttackBoon : BoonBase
{
    [OdinSerialize] private Dictionary<DamageType, float> _mods;
    public Dictionary<DamageType, float> BoonBuffs => _mods;
    
    private void Awake()
    {
        foreach (var mod in _mods)
        {
            statModifiers.Add(new StatModifierBase(StatType.Attack, x => x + mod.Value, mod.Key));
        }
    }
}