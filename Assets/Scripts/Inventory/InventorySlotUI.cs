using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image icon;
    public TMP_Text amountText;

    public Image highlightFrame; 

    [HideInInspector] public int slotIndex;
    [HideInInspector] public Inventory inventory;

    private Canvas canvas;
    private GameObject dragObj;
    private Image dragIcon;
    
    ItemStack CurrentStack =>
        (inventory != null && slotIndex < inventory.slots.Count) ? inventory.slots[slotIndex] : null;

    void Start()
    {
        // Sucht sicher nach der Leinwand für Drag & Drop
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null) canvas = Object.FindFirstObjectByType<Canvas>();

        // MAGIE: Wir zwingen das Bild HIER per Code, im Slot zu bleiben!
        if (icon != null)
        {
            icon.preserveAspect = true; // Verhindert Verzerrung
            
            // Anchors auf "Stretch" stellen (füllt den Eltern-Slot aus)
            icon.rectTransform.anchorMin = Vector2.zero;
            icon.rectTransform.anchorMax = Vector2.one;
            
            // Winziger Abstand zum Rand (Padding), damit es gut aussieht
            icon.rectTransform.offsetMin = new Vector2(5, 5);
            icon.rectTransform.offsetMax = new Vector2(-5, -5);
        }
    }


    public void Set(ItemStack stack)
    {
        if (stack == null || stack.item == null || stack.amount <= 0)
        {
            icon.sprite = null;
            icon.color = new Color(1, 1, 1, 0); // Unsichtbar
            amountText.text = "";
            return;
        }

        icon.sprite = stack.item.icon;
        icon.color = Color.white; // Sichtbar
        amountText.text = stack.amount > 1 ? stack.amount.ToString() : "";
    }

        // ---------- DETAIL ANSICHT LOGIK ----------
    public void OnPointerClick(PointerEventData eventData) => UpdateDetailUI();
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (highlightFrame != null) highlightFrame.enabled = true;
        UpdateDetailUI(); 
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (highlightFrame != null) highlightFrame.enabled = false;
    }

    private void UpdateDetailUI()
    {
        InventoryUI ui = Object.FindFirstObjectByType<InventoryUI>();
        if (ui != null && CurrentStack != null && CurrentStack.item != null)
        {
            ui.UpdateDetailView(CurrentStack.item.icon, CurrentStack.item.itemName);
        }
    }

    // ---------- DRAG ----------
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (CurrentStack == null || CurrentStack.item == null) return;
        if (canvas == null) return; // Wenn Canvas fehlt, brich ab anstatt abzustürzen

        // Wir erschaffen ein sauberes Geister-Bild
        dragObj = new GameObject("DragIcon");
        dragObj.transform.SetParent(canvas.transform, false);
        dragObj.transform.SetAsLastSibling(); // Ganz nach vorne holen
        
        dragIcon = dragObj.AddComponent<Image>();
        dragIcon.sprite = icon.sprite;
        dragIcon.preserveAspect = true;
        dragIcon.raycastTarget = false;
        
        // Geisterbild fest auf 64x64 zwingen
        dragIcon.rectTransform.sizeDelta = new Vector2(64, 64);

        icon.color = new Color(1, 1, 1, 0); // Original verstecken
        amountText.enabled = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIcon != null) dragIcon.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragObj != null) Destroy(dragObj);

        // Original wieder anzeigen, wenn noch was drin ist
        if (CurrentStack != null && CurrentStack.item != null)
            icon.color = Color.white;
        else
            icon.color = new Color(1, 1, 1, 0);

        amountText.enabled = true;
    }

    // ---------- DROP ----------
    public void OnDrop(PointerEventData eventData)
    {
        var fromSlot = eventData.pointerDrag?.GetComponent<InventorySlotUI>();
        if (fromSlot == null || fromSlot == this) return;
        
        inventory.HandleSlotDrop(fromSlot.slotIndex, slotIndex);
    }
}