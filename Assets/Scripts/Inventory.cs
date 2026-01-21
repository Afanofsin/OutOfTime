using System.Collections.Generic;
using UnityEngine;
using Interfaces;

public class Inventory : MonoBehaviour, IKeyHolder
{
    [SerializeField] private int keys;
    [SerializeField] private int debt;
    
    [SerializeField] private InventorySlot[] inventorySlots;
    public bool Keys => keys > 0;
    public bool TryAdd(WeaponBase item)
    {
        foreach (var slot in inventorySlots)
        {
            if (slot.HeldItem == null || slot.HeldItem == WeaponDatabase.Instance.GetWeaponByID(item.id))
            {
                slot.SetItem(item);
                return true;
            }
        }
        return false;
    }

    public WeaponBase GetSlotItem(int slot)
    {
        if (inventorySlots[slot].HeldItem != null)
        {
            return inventorySlots[slot].HeldItem;
        }
        return WeaponDatabase.Instance.GetWeaponByID(0);
    }
}
