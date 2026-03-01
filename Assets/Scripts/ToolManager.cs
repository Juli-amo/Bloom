using System;
using System.Collections.Generic;
using UnityEngine;

public class ToolManager : MonoBehaviour
{
  
    [Header("Start-Reset")]
    [SerializeField] bool clearHotbarOnPlay = true;   // <-- NEU

    void Awake()
    {
        if (clearHotbarOnPlay)
        {
            hotbar.Clear();          // Hotbar leeren
            index = 0;
            backpack.Clear();        // (falls vorhanden)
            OnChanged?.Invoke();
        }

        // Sicherheitsnetz: doppelte Stacks zusammenziehen
        MergeStacks(Tool.Seeds);
        MergeStacks(Tool.Wood);
        MergeStacks(Tool.Stone);
    }
    public class Entry
    {
        public Tool tool;      // welches Item/Tool
        public int count;      // 1 bei Tools, 1..99 bei stackbaren Ressourcen
        public bool stackable; // Seeds/Wood/Stone = true
    }

    [Header("Hotbar")]
    public int maxSlots = 7;                 // sichtbare Plätze
    public List<Entry> hotbar = new();       // Reihenfolge = Slotreihenfolge
    public int index = 0;                    // aktuell ausgewählter Slot

    // Welche Items stapelbar sind
    static readonly HashSet<Tool> Stackables = new() { Tool.Seeds, Tool.Wood, Tool.Stone };

    // Einfaches „Backpack“ (Overflow, noch ohne UI)
    readonly Dictionary<Tool, int> backpack = new();

    public event Action OnChanged;

    public Tool Current =>
        (hotbar.Count > 0) ? hotbar[Mathf.Clamp(index, 0, hotbar.Count - 1)].tool : Tool.Hand;

    public IReadOnlyList<Entry> HotbarEntries => hotbar;
    public Entry GetEntry(Tool t) => hotbar.Find(e => e.tool == t);
    public int GetBackpack(Tool t) => backpack.TryGetValue(t, out var n) ? n : 0;

    // ------- API -------

    /// <summary>Einmalige Werkzeuge (Axe, Pickaxe, Hoe, WateringCan …)</summary>
    public void AddTool(Tool t)
    {
        if (hotbar.Exists(e => e.tool == t)) { OnChanged?.Invoke(); return; }
        TryAddToHotbar(new Entry { tool = t, count = 1, stackable = false });
    }

    /// <summary>Stackbare Items (Seeds/Wood/Stone). Stacks bis 99. Rest in Backpack.</summary>
    public void AddStackable(Tool t, int amount)
    {
        if (!Stackables.Contains(t) || amount <= 0) return;

        // 1) vorhandenen Stack auffüllen
        int pi = hotbar.FindIndex(x => x.tool == t && x.stackable);
        if (pi >= 0)
        {
            var p = hotbar[pi];
            int room = 99 - p.count;
            int add  = Mathf.Min(room, amount);
            p.count += add;
            amount  -= add;
        }

        // 2) falls noch Menge übrig und Platz in der Hotbar -> EIN neuer Stack
        if (amount > 0 && hotbar.Count < maxSlots)
        {
            int take = Mathf.Min(99, amount);
            TryAddToHotbar(new Entry { tool = t, count = take, stackable = true });
            amount -= take;
        }

        // 3) Rest ins Backpack (Overflow)
        if (amount > 0) backpack[t] = GetBackpack(t) + amount;

        // 4) Sicherheitsnetz: evtl. vorhandene Doppelstacks zusammenführen
        MergeStacks(t);

        OnChanged?.Invoke();
    }

    /// <summary>Verbraucht n Einheiten eines stackbaren Items aus der Hotbar (z. B. Seeds).</summary>
    public bool ConsumeStackable(Tool t, int amount)
    {
        if (!Stackables.Contains(t) || amount <= 0) return false;
        var e = hotbar.Find(x => x.tool == t && x.stackable);
        if (e == null || e.count < amount) return false;

        e.count -= amount;
        if (e.count <= 0)
        {
            int removedIndex = hotbar.IndexOf(e);
            hotbar.RemoveAt(removedIndex);
            index = Mathf.Clamp(index, 0, hotbar.Count - 1);
        }
        OnChanged?.Invoke();
        return true;
    }

    // ------- intern -------

    void TryAddToHotbar(Entry e)
    {
        if (hotbar.Count >= maxSlots)
        {
            if (e.stackable) backpack[e.tool] = GetBackpack(e.tool) + e.count;
            OnChanged?.Invoke();
            return;
        }

        if (e.stackable) e.count = Mathf.Clamp(e.count, 1, 99);
        hotbar.Add(e);                      // „nächstbester Platz“ = Ende der Liste
        index = Mathf.Clamp(index, 0, hotbar.Count - 1);
        OnChanged?.Invoke();
    }

    void MergeStacks(Tool t)
    {
        int primary = hotbar.FindIndex(e => e.tool == t && e.stackable);
        if (primary < 0) return;

        for (int i = hotbar.Count - 1; i >= 0; i--)
        {
            if (i == primary) continue;
            var e = hotbar[i];
            if (e.tool != t || !e.stackable) continue;

            int room = 99 - hotbar[primary].count;
            int move = Mathf.Min(room, e.count);
            hotbar[primary].count += move;
            e.count -= move;

            if (e.count <= 0) hotbar.RemoveAt(i);
        }
    }
}