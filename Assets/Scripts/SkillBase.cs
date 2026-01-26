using System;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class SkillBase : SerializedMonoBehaviour, IPickable, ISkill
{
    public readonly int id;
    [SerializeField] private float bloodCost;
    [SerializeField] private float coolDown;
    public readonly Sprite icon;
    public float CoolDown => coolDown;
    public float Cost => bloodCost;
    public void PickUp(Inventory context) => Destroy(gameObject);
    public abstract void Perform();
}
