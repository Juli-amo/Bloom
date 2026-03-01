using UnityEngine;
using System.Collections.Generic;

public class HotbarUI : MonoBehaviour
{
    public Inventory inventory;
    
    [Header("Ziehe deine 5 Slots aus der Hierarchie hier rein!")]
    public List<InventorySlotUI> slots = new List<InventorySlotUI>();

    void Start()
    {
        if (inventory != null)
        {
            inventory.OnInventoryChanged += Refresh;
            
            // Wir sagen deinen manuell platzierten Slots, wer sie sind (Index 0 bis 4)
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i] != null)
                {
                    slots[i].slotIndex = i;
                    slots[i].inventory = inventory;
                }
            }
            Refresh();
        }
    }

    void Update()
    {
        if (inventory == null) return;
        
        // Tasten 1-5 zur Auswahl
        if (Input.GetKeyDown(KeyCode.Alpha1)) inventory.SelectSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) inventory.SelectSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) inventory.SelectSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) inventory.SelectSlot(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) inventory.SelectSlot(4);
    }

    void Refresh()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] == null) continue;

            if (i < inventory.slots.Count)
                slots[i].Set(inventory.slots[i]);
            else
                slots[i].Set(null);

            // Gelbe Markierung für den ausgewählten Slot
            var bgImage = slots[i].GetComponent<UnityEngine.UI.Image>();
            if (bgImage != null)
            {
                bgImage.color = (i == inventory.selectedSlotIndex) ? Color.yellow : Color.white;
            }
        }
    }
}