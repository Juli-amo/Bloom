using UnityEngine;
public class HarvestTarget : MonoBehaviour, IInteractable
{
    [Header("Einstellungen")]
    public int hitsToHarvest = 3;
    public int grassYield = 2;

    [Header("Abgabe")]
    public ItemType grassItem; // Hier das ItemType-Asset (z.B. TallGrass) im Inspector reinziehen

    private int hp;

    void Awake()
    {
        hp = Mathf.Max(1, hitsToHarvest);
    }

    public void Interact(Tool tool)
    {
        if (!IsSickle(tool))
        {
            Debug.Log("Ich brauche eine Sichel, um das Gras zu ernten!");
            return;
        }

        hp--;
        Debug.Log($"Gras getroffen! HP: {hp}");

        if (hp <= 0)
        {
            HarvestGrass();
        }
    }

    void HarvestGrass()
    {
        Inventory inv = Object.FindFirstObjectByType<Inventory>();
        if (inv != null && grassItem != null)
        {
            inv.AddItem(grassItem, grassYield);
            Debug.Log($"Gras geerntet! {grassYield}x {grassItem.itemName} erhalten.");
        }
        else if (grassItem == null)
        {
            Debug.LogWarning("[HarvestTarget] Kein GrassItem (ItemType) im Inspector zugewiesen!");
        }

        // === STORY COUNTERS ===
        if (StoryManager.I != null)
        {
            StoryManager.I.AddCounter("grass_harvested_today", 1);
            StoryManager.I.AddCounter("grass_harvested_total", 1);
        }

        if (CounterEventWatcher.I != null)
        {
            CounterEventWatcher.I.CheckNow();
        }

        if (SaveManager.I != null)
        {
            SaveManager.I.SaveGame();
        }

        Destroy(gameObject);
    }

    private bool IsSickle(Tool t)
    {
        return t == Tool.Sickle; // Passe den Namen an deine Tool.cs an!
    }
}