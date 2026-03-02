using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI I;

    public Inventory inventory;
    public InventorySlotUI slotPrefab;
    
    [Header("Standard Slot Parents")]
    public Transform hotbarParent;   
    public Transform backpackParent; 

    [Header("Combining Page (Optional)")]
    public Transform combininghotbarParent; 
    public Transform combiningBackpackParent; 

    [Header("Item Details")]
    public GameObject detailPanel;   
    public Image detailIcon;         
    public TMP_Text detailName; 

    private List<InventorySlotUI> mainSlots = new List<InventorySlotUI>();
    private List<InventorySlotUI> combiningSlots = new List<InventorySlotUI>();

    private void Awake()
    {
        I = this;
    }

    void Start()
    {
        if (inventory != null)
        {
            inventory.OnInventoryChanged += Refresh;
            CreateSlots();
            Refresh();
        }
        ClearDetailView();
    }

    void CreateSlots()
    {
        // Säubern
        foreach (var s in mainSlots) if (s != null) Destroy(s.gameObject);
        foreach (var s in combiningSlots) if (s != null) Destroy(s.gameObject);
        mainSlots.Clear();
        combiningSlots.Clear();

        // 1. Haupt-Inventar erstellen (Hotbar + Rucksack)
        for (int i = 0; i < inventory.maxSlots; i++)
        {
            Transform parent = (i < 6) ? hotbarParent : backpackParent;
            if (parent == null) continue;

            var slot = Instantiate(slotPrefab, parent);
            slot.slotIndex = i;
            slot.inventory = inventory;
            mainSlots.Add(slot);
        }

        // 2. Sekundäres Inventar für Combining-Seite erstellen 
        for (int i = 0; i < inventory.maxSlots; i++)
        {
            Transform targetParent = (i < 6) ? combininghotbarParent : combiningBackpackParent;

            if (targetParent != null)
            {
                var slot = Instantiate(slotPrefab, targetParent);
                slot.slotIndex = i;
                slot.inventory = inventory;
                combiningSlots.Add(slot);
            }
        }
    }

    public void Refresh()
    {
        // Haupt-Slots aktualisieren
        for (int i = 0; i < mainSlots.Count; i++)
        {
            UpdateSlotVisuals(mainSlots[i], i);
        }

        // Combining-Slots aktualisieren
        for (int i = 0; i < combiningSlots.Count; i++)
        {
            // Achtung: Der Index im Inventory ist combiningSlots[i].slotIndex (also ab 6)
            UpdateSlotVisuals(combiningSlots[i], combiningSlots[i].slotIndex);
        }
    }

    void UpdateSlotVisuals(InventorySlotUI uiSlot, int index)
    {
        if (index < inventory.slots.Count)
            uiSlot.Set(inventory.slots[index]);
        else
            uiSlot.Set(null);
    }

    public void UpdateDetailView(Sprite itemIcon, string itemName) 
    {
        if (itemIcon != null)
        {
            if (detailPanel != null) detailPanel.SetActive(true);
            if (detailIcon != null) { detailIcon.sprite = itemIcon; detailIcon.enabled = true; detailIcon.color = Color.white; }
            if (detailName != null) detailName.text = itemName;
        }
        else ClearDetailView();
    }

    public void ClearDetailView()
    {
        if (detailPanel != null) detailPanel.SetActive(false);
        if (detailIcon != null) detailIcon.enabled = false;
        if (detailName != null) detailName.text = "";
    }
}