using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))] // Sorgt dafür, dass immer ein SpriteRenderer da ist
public class ItemPickupInteractable : MonoBehaviour, IInteractable
{
    [Header("Welches Item ist das?")]
    public ItemType item;
    public int amount = 1;

    [Header("Einstellungen")]
    public bool destroyAfterPickup = true;

    private Inventory inventory;
    private SpriteRenderer sr;

    private void Awake()
    {
        inventory = Object.FindFirstObjectByType<Inventory>();
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
        sr = GetComponent<SpriteRenderer>();
    }

    // --- UNITY MAGIE: Passiert direkt im Editor, wenn du etwas änderst ---
    private void OnValidate()
    {
        if (item != null)
        {
            sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                // Er nutzt das worldSprite aus deiner Rezeptkarte. 
                // Wenn du keins hast, nimmt er automatisch das normale Icon!
                sr.sprite = item.worldSprite != null ? item.worldSprite : item.icon;
            }
            
            // Benennt das Objekt in deiner Hierarchie automatisch um!
            gameObject.name = "Pickup_" + item.name; 
        }
    }

    // --- AUFHEBEN (Mit der E-Taste) ---
    public void Interact(Tool tool)
    {
        if (item == null)
        {
            Debug.LogWarning("Diesem Pickup fehlt ein ItemType!");
            return;
        }

        if (inventory == null)
            inventory = Object.FindFirstObjectByType<Inventory>();

        if (inventory != null)
        {
            bool success = inventory.AddItem(item, amount);
            if (success && destroyAfterPickup)
            {
                Destroy(gameObject); // Item aus der Welt löschen
            }
        }
    }
}