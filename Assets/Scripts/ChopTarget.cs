using UnityEngine;

public class ChopTarget : MonoBehaviour, IInteractable
{
    [Header("Einstellungen")]
    public int hitsToFell = 3;
    public int woodYield = 3;
    
    [Header("Abgabe")]
    public ItemType woodItem; // Hier das ItemType-Asset (z.B. Wood) im Inspector reinziehen

    private int hp;

    void Awake()
    {
        hp = Mathf.Max(1, hitsToFell);
    }

    public void Interact(Tool tool)
    {
        // Prüfung, ob das gehaltene Werkzeug eine Axt ist
        if (!IsAxe(tool)) 
        {
            Debug.Log("Ich brauche eine Axt, um diesen Baum zu fällen!");
            return;
        }

        hp--;
        Debug.Log($"Baum getroffen! HP: {hp}");

        if (hp <= 0)
        {
            FellTree();
        }
    }

    void FellTree()
    {
        // Zugriff auf das globale Inventar
        Inventory inv = Object.FindFirstObjectByType<Inventory>();
        
        if (inv != null && woodItem != null)
        {
            inv.AddItem(woodItem, woodYield);
            Debug.Log($"Baum gefällt! {woodYield}x {woodItem.itemName} erhalten.");
        }
        else if (woodItem == null)
        {
            Debug.LogWarning("[ChopTarget] Kein WoodItem (ItemType) im Inspector zugewiesen!");
        }

        // === STORY COUNTERS ===
        if (StoryManager.I != null)
        {
            StoryManager.I.AddCounter("trees_cut_today", 1);
            StoryManager.I.AddCounter("trees_cut_total", 1);
        }

        if (CounterEventWatcher.I != null)
        {
            CounterEventWatcher.I.CheckNow();
        }

        // Speichern nach dem Erfolg
        if (SaveManager.I != null)
        {
            SaveManager.I.SaveGame();
        }

        Destroy(gameObject);
    }

    private bool IsAxe(Tool t)
    {
        // Hier wurden nur die Tools eingetragen, die auch in deiner Tool.cs stehen
        return t == Tool.Axe || 
               t == Tool.AxeDense || 
               t == Tool.AxeCrystal;
    }
}
