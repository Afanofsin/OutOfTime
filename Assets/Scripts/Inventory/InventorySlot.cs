using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class InventorySlot : SerializedMonoBehaviour
{
    [OdinSerialize] private WeaponBase _slotItem;
    public WeaponBase HeldItem => _slotItem;

    public void SetItem(WeaponBase item) => _slotItem = item;
    //private void Awake() => gameObject.GetComponentInChildren<SpriteRenderer>().sprite = _slotItem.PickableSprite;
}