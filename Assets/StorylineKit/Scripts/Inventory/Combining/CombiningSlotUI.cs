using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CombiningSlotUI : MonoBehaviour, IPointerClickHandler, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI Elemente")]
    public Image icon;
    public TMP_Text amountText;

    [Header("Platzhalter Visuals")]
    public Sprite placeholderSprite;
    public Color placeholderColor = Color.white; 

    [Header("Referenzen")]
    public CombiningSlot slot;
    public Combiner combiner;
    public Canvas canvas;

    private GameObject dragObj;
    private Image dragIcon;

    void Update()
    {
        if (slot == null || icon == null) return;
        Refresh();
    }

    void Refresh()
    {
        // LEERER SLOT
        if (slot.stack == null || slot.stack.item == null || slot.stack.amount <= 0)
        {
            icon.sprite = placeholderSprite;
            icon.color = (placeholderSprite == null) ? new Color(1, 1, 1, 0) : placeholderColor;
            
            if (amountText != null) amountText.text = "";
            return;
        }

        // BELEGTER SLOT
        icon.sprite = slot.stack.item.icon;
        icon.color = Color.white;

        if (amountText != null)
        {
            // Zeige die Zahl an, wenn mehr als 1 Item im Mixer-Slot liegt
            amountText.text = slot.stack.amount > 1 ? slot.stack.amount.ToString() : "";
        }
    }

    // ---------- KLICK: Nur 1 Item zurück ins Inventar geben ----------
    public void OnPointerClick(PointerEventData eventData)
    {
        if (slot == null || slot.isOutput || slot.stack == null || slot.stack.item == null || combiner == null) return;

        // Wir versuchen genau 1 Item dem Inventar hinzuzufügen
        if (combiner.playerInventory != null && combiner.playerInventory.AddItem(slot.stack.item, 1))
        {
            slot.stack.amount--;
            
            // Wenn der Stack im Mixer nun 0 ist, räumen wir ihn auf
            if (slot.stack.amount <= 0)
            {
                slot.Clear();
            }
            
            combiner.Recalculate();
        }
    }

    // ---------- DROP: Nur 1 Item vom Inventar hier reinlegen ----------
    public void OnDrop(PointerEventData eventData)
    {
        if (slot == null || slot.isOutput || combiner == null) return;

        var invSlot = eventData.pointerDrag?.GetComponent<InventorySlotUI>();
        if (invSlot == null) return;

        var invStack = invSlot.inventory.slots[invSlot.slotIndex];
        if (invStack == null || invStack.item == null) return;

        // Logik für den Transfer:
        
        // 1. Falls im Mixer-Slot ein ANDERES Item liegt, geben wir es komplett zurück
        if (slot.stack != null && slot.stack.item != null && slot.stack.item != invStack.item)
        {
            invSlot.inventory.AddItem(slot.stack.item, slot.stack.amount);
            slot.Clear();
        }

        // 2. Jetzt 1 Item aus dem Inventar-Stack nehmen und in den Mixer legen
        if (slot.stack == null || slot.stack.item == null)
        {
            // Neuer Stack im Mixer mit Menge 1
            slot.stack = new ItemStack { item = invStack.item, amount = 1 };
        }
        else
        {
            // Vorhandenen Stack im Mixer um 1 erhöhen
            slot.stack.amount++;
        }

        // 3. Im Inventar 1 abziehen
        invStack.amount--;
        if (invStack.amount <= 0)
        {
            invSlot.inventory.slots[invSlot.slotIndex] = null;
        }

        // Inventar-UI aktualisieren und Mixer neu berechnen
        invSlot.inventory.OnInventoryChanged?.Invoke();
        combiner.Recalculate();
    }

    // ---------- DRAG: Ergebnis rausziehen ----------
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (slot == null || !slot.isOutput || slot.stack == null) return;

        if (combiner.TakeOutput())
        {
            if (canvas != null)
            {
                dragObj = new GameObject("DragIcon_Mixer");
                dragObj.transform.SetParent(canvas.transform, false);
                dragObj.transform.SetAsLastSibling();
                dragIcon = dragObj.AddComponent<Image>();
                dragIcon.sprite = icon.sprite;
                dragIcon.preserveAspect = true;
                dragIcon.raycastTarget = false;
                dragIcon.rectTransform.sizeDelta = new Vector2(40, 40);
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIcon != null) dragIcon.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragObj != null) Destroy(dragObj);
    }
}