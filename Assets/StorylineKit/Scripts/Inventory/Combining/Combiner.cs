using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Combiner : MonoBehaviour
{
    [Header("UI Slots")]
    public List<CombiningSlot> inputSlots;
    public CombiningSlot outputSlot;

    [Header("Daten")]
    public List<CombineRecipe> recipes;
    public Inventory playerInventory;

    // Hilfsmethode um alle gültigen Inputs aus den Slots zu sammeln
    private List<ItemStack> GetCurrentInputs()
    {
        return inputSlots
            .Where(s => s != null && s.stack != null && s.stack.item != null && s.stack.amount > 0)
            .Select(s => s.stack)
            .ToList();
    }

    public void Recalculate()
    {
        if (outputSlot == null) return;
        outputSlot.Clear();

        var currentInputs = GetCurrentInputs();
        if (currentInputs.Count == 0) return;

        foreach (var recipe in recipes)
        {
            if (recipe == null) continue;

            if (Matches(recipe, currentInputs))
            {
                outputSlot.stack = new ItemStack
                {
                    item = recipe.output,
                    amount = recipe.outputAmount
                };
                return;
            }
        }
    }

    bool Matches(CombineRecipe recipe, List<ItemStack> inputs)
    {
        // 1. Check: Hat das Rezept die gleiche Anzahl an verschiedenen Item-Typen wie unsere Eingabe?
        var distinctInputs = inputs.Select(i => i.item).Distinct().ToList();
        if (distinctInputs.Count != recipe.inputs.Count) return false;

        // 2. Check: Sind alle benötigten Items in der richtigen Menge da?
        foreach (var req in recipe.inputs)
        {
            int totalInMixer = inputs
                .Where(i => i.item == req.item)
                .Sum(i => i.amount);

            if (totalInMixer < req.amount) return false;
        }

        return true;
    }

    public bool TakeOutput()
    {
        if (outputSlot.stack == null || outputSlot.stack.item == null) return false;

        // 1. Rezept finden, das aktuell aktiv ist
        var currentInputs = GetCurrentInputs();
        CombineRecipe matchedRecipe = recipes.FirstOrDefault(r => Matches(r, currentInputs));

        if (matchedRecipe == null)
        {
            Debug.LogWarning("[Mixer] Kein passendes Rezept zum Verbrauch gefunden!");
            return false;
        }

        // 2. Versuche das Ergebnis ins Inventar zu legen
        if (!playerInventory.AddItem(outputSlot.stack.item, outputSlot.stack.amount))
        {
            Debug.LogWarning("Kein Platz im Inventar für das Ergebnis!");
            return false;
        }

        // 3. Zutaten verbrauchen
        foreach (var req in matchedRecipe.inputs)
        {
            int toRemove = req.amount;
            
            // Wir gehen die Slots durch und ziehen die Menge ab
            foreach (var slot in inputSlots)
            {
                if (slot.stack != null && slot.stack.item == req.item)
                {
                    int taken = Mathf.Min(toRemove, slot.stack.amount);
                    slot.stack.amount -= taken;
                    toRemove -= taken;

                    // Wenn der Stapel im Slot leer ist, Slot komplett leeren
                    if (slot.stack.amount <= 0)
                    {
                        slot.Clear();
                    }

                    if (toRemove <= 0) break;
                }
            }
        }

        // 4. Alles aktualisieren
        Recalculate();
        return true;
    }
}