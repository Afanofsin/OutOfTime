using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private uint bandages;
    [SerializeField] private int debt;
    [SerializeField] private InventorySlot[] inventorySlots;
    private int ItemCount
    {
        get
        {
            int count = 0;

            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (inventorySlots[i].HeldItem != null)
                    count++;
            }

            return count;
        }
    }

    public bool Bandages => bandages > 0;

    private int _currentSlot;
    public int CurrentSlot => _currentSlot;
    public WeaponBase CurrentItem => GetSlotItem(_currentSlot);

    private void Awake()
    {
        _currentSlot = 0;
        bandages = 0;
    }
    
    public WeaponBase Next()
    {
        var startSlot = _currentSlot;

        do
        {
            _currentSlot = (_currentSlot + 1) % inventorySlots.Length;

            if (!IsSlotEmpty(_currentSlot))
                return inventorySlots[_currentSlot].HeldItem;

        } while (_currentSlot != startSlot);
        return CurrentItem;
    }

    public WeaponBase Previous()
    {
        var startSlot = _currentSlot;

        do
        {
            _currentSlot--;
            if (_currentSlot < 0)
                _currentSlot = inventorySlots.Length - 1;

            if (!IsSlotEmpty(_currentSlot))
                return inventorySlots[_currentSlot].HeldItem;

        } while (_currentSlot != startSlot);

        return CurrentItem;
    }
    

    public void SelectSlot(int slot)
    {
        if (slot < 0 || slot >= inventorySlots.Length)
            return;

        _currentSlot = slot;
    }
    public bool TryAdd(WeaponBase item)
    {
        foreach (var slot in inventorySlots)
        {
            if (slot.HeldItem == null)
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
        return null;
    }
    private bool IsSlotEmpty(int index)
    {
        var item = inventorySlots[index].HeldItem;
        return item == null;
    }
    public WeaponBase TryDropCurrent()
    {
        if (ItemCount <= 1) return null;

        var droppedItem = inventorySlots[_currentSlot].HeldItem;
        inventorySlots[_currentSlot].SetItem(null);
        
        Next();

        return droppedItem;
    }

    public void AddBandage(uint amount) => bandages += amount;
    public void SubtractBandage() => bandages--;
}
