using UnityEngine;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;
    public InventorySlotUI slotPrefab;
    public Transform slotParent;

    private List<InventorySlotUI> slots = new();

    void Start()
    {
        inventory.OnInventoryChanged += Refresh;
        CreateSlots();
        Refresh();
    }

    void CreateSlots()
{
    for (int i = 0; i < inventory.maxSlots; i++)
    {
        var slot = Instantiate(slotPrefab, slotParent);
        slot.slotIndex = i;
        slot.inventory = inventory;
        slots.Add(slot);
    }
}


    public void Refresh()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (i < inventory.slots.Count)
                slots[i].Set(inventory.slots[i]);
            else
                slots[i].Set(null);
        }
    }
}
