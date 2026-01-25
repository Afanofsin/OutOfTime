using System.Collections.Generic;
using Sirenix.Serialization;

public class SpeedBoon : BoonBase
{
    [OdinSerialize] private Dictionary<StatType, float> _mods;
    public Dictionary<StatType, float> BoonBuffs => _mods;
    
    private void Awake()
    {
        foreach (var mod in _mods)
        {
            statModifiers.Add(mod.Key is StatType.AttackSpeed or StatType.DashCooldown ? 
                new StatModifierBase(mod.Key, x => x - mod.Value) 
                : new StatModifierBase(mod.Key, x => x + mod.Value));
        }
    }
}
