using UnityEngine;

public class PickupItem : MonoBehaviour
{
    // WICHTIG: Hoe / Wood / Stone sind hier enthalten
    public enum Type { Seed, Can, Axe, Pickaxe, Hoe, Wood, Stone }

    [Header("Pickup")]
    public Type type;
    public int amount = 1;         // für Seeds/Wood/Stone

    void OnTriggerEnter2D(Collider2D c)
    {
        if (!c.CompareTag("Player")) return;

        var tm = Object.FindFirstObjectByType<ToolManager>();
        if (!tm) { Destroy(gameObject); return; }

        switch (type)
        {
            case Type.Seed:    tm.AddStackable(Tool.Seeds,  amount); break;
            case Type.Wood:    tm.AddStackable(Tool.Wood,   amount); break;
            case Type.Stone:   tm.AddStackable(Tool.Stone,  amount); break;

            case Type.Can:     tm.AddTool(Tool.WateringCan); break;
            case Type.Axe:     tm.AddTool(Tool.Axe);         break;
            case Type.Pickaxe: tm.AddTool(Tool.Pickaxe);     break;
            case Type.Hoe:     tm.AddTool(Tool.Hoe);         break;
        }

        Destroy(gameObject);
    }
}