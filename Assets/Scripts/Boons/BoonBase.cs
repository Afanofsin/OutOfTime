using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public abstract class BoonBase : SerializedMonoBehaviour, IBoon, IPickable
{   
    [OdinSerialize] private RarityType _rarityType;
    public RarityType BoonRarity => _rarityType;
    protected List<StatModifier> statModifiers = new();
    
    public List<StatModifier> GetBoonMods() => statModifiers;
    public void PickUp(Inventory context) => Destroy(gameObject);
}
