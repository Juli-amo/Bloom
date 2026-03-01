using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int maxSlots = 20;
    public List<ItemStack> slots = new List<ItemStack>();

    // NEU: Merkt sich, welcher Hotbar-Slot gerade aktiv ist (0 bis 4)
    public int selectedSlotIndex = 0; 
    
    public Action OnInventoryChanged;

    void Awake()
    {
        // Inventar mit leeren Slots füllen
        slots.Clear();
        for (int i = 0; i < maxSlots; i++)
            slots.Add(null);
    }

    // NEU: Wählt einen Slot in der Hotbar aus
    public void SelectSlot(int index)
    {
        selectedSlotIndex = index;
        OnInventoryChanged?.Invoke();
    }

    // NEU: Gibt uns das Item zurück, das wir gerade in der Hand halten
    public ItemStack GetSelectedStack()
    {
        if (selectedSlotIndex < slots.Count)
            return slots[selectedSlotIndex];
        return null;
    }

    // ---------------- ADD ITEMS ----------------
    public bool AddItem(ItemType item, int amount)
    {
        // 1. Existierende Stacks auffüllen
        for (int i = 0; i < maxSlots; i++)
        {
            var stack = slots[i];
            if (stack != null && stack.item == item && !stack.IsFull)
            {
                int space = item.maxStackSize - stack.amount;
                int add = Mathf.Min(space, amount);

                stack.amount += add;
                amount -= add;

                if (amount <= 0)
                {
                    OnInventoryChanged?.Invoke();
                    return true;
                }
            }
        }

        // 2. Neue Stacks in leeren Slots erstellen
        for (int i = 0; i < maxSlots && amount > 0; i++)
        {
            if (slots[i] == null)
            {
                int add = Mathf.Min(item.maxStackSize, amount);
                slots[i] = new ItemStack { item = item, amount = add };
                amount -= add;
            }
        }

        OnInventoryChanged?.Invoke();
        return amount <= 0;
    }

    // ---------------- DRAG & DROP ----------------
    public void HandleSlotDrop(int fromIndex, int toIndex)
    {
        if (fromIndex == toIndex) return;

        ItemStack fromStack = slots[fromIndex];
        ItemStack toStack = slots[toIndex];

        if (fromStack == null) return;

        if (toStack == null)
        {
            slots[toIndex] = fromStack;
            slots[fromIndex] = null;
        }
        else if (fromStack.item == toStack.item)
        {
            int space = toStack.item.maxStackSize - toStack.amount;
            int add = Mathf.Min(space, fromStack.amount);

            toStack.amount += add;
            fromStack.amount -= add;

            if (fromStack.amount <= 0) slots[fromIndex] = null;
        }
        else
        {
            slots[toIndex] = fromStack;
            slots[fromIndex] = toStack;
        }

        OnInventoryChanged?.Invoke();
    }
}