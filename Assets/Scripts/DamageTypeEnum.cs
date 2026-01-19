using UnityEngine;

public enum DamageType
{
    Physical,
    Fire,
    Ice,
    Energy,
    Blood
}


[System.Serializable]
public struct DamageTypeEntry
{
    public DamageType key;
    public float value;
}