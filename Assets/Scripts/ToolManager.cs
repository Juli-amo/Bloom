using UnityEngine;
using System.Collections.Generic;

public class ToolManager : MonoBehaviour
{
    public static ToolManager I { get; private set; }
    private Inventory inventory;

    public int index 
    {
        get => inventory != null ? inventory.selectedSlotIndex : 0;
        set { if (inventory != null) inventory.SelectSlot(value); }
    }

    private void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        inventory = Object.FindFirstObjectByType<Inventory>();
    }

    public Tool Current
    {
        get
        {
            if (inventory == null) inventory = Object.FindFirstObjectByType<Inventory>();
            if (inventory == null) return Tool.None;

            var selectedStack = inventory.GetSelectedStack();
            if (selectedStack == null || selectedStack.item == null) return Tool.Hand;

            return MapItemToTool(selectedStack.item);
        }
    }

    // FIX für HotbarQuickSelect: Simuliert die alte Liste für das UI
    public List<ItemStack> HotbarEntries => inventory != null ? inventory.slots : new List<ItemStack>();

    private Tool MapItemToTool(ItemType item)
    {
        if (item == null || string.IsNullOrEmpty(item.itemName)) return Tool.None;
        string name = item.itemName.ToLower();

        if (name.Contains("hoe")) return Tool.HoeWood;
        if (name.Contains("watering")) return Tool.WateringCan;
        if (name.Contains("seed")) return Tool.Seeds;
        
        // Korrektur: Nutze nur existierende Enums aus Tool.cs
        if (name.Contains("axe")) return Tool.Axe; 
        if (name.Contains("spade")) return Tool.SpadeWood;
        if (name.Contains("sickle")) return Tool.Sickle;

        return Tool.None;
    }

    public System.Action OnChanged;
}