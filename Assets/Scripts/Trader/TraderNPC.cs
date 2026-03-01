using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class TraderNPC : MonoBehaviour, IInteractable
{
    [System.Serializable]
    public class SeedProgression
    {
        public string shopName; // Nur zur Übersicht im Inspector (z.B. "Tier 1: Basic Seed")
        public ItemType seedItem;
        
        [Tooltip("Wie viel Holz kostet dieser Samen?")]
        public int woodCost = 10;
        
        [Tooltip("Ist dieser Samen von Anfang an kaufbar? (Sollte bei Samen 1 auf True stehen)")]
        public bool isUnlocked = false; 

        [Header("Unlock Condition (Leer lassen, wenn von Beginn an frei)")]
        public ItemType requiredHarvest;
        public int requiredHarvestAmount = 4;

        [Header("Dialogues (Scriptable Objects)")]
        [Tooltip("Spielt ab, wenn der Spieler den Samen durch Ernte freischaltet.")]
        public DialogueSequence unlockSuccessDialog;
        
        [Tooltip("Spielt ab, wenn der Spieler die Ernte hält, aber noch nicht genug hat.")]
        public DialogueSequence unlockHintDialog;

        [Tooltip("Spielt ab, wenn der Spieler den Samen erfolgreich mit Holz kauft.")]
        public DialogueSequence buySuccessDialog;

        [Tooltip("Spielt ab, wenn der Spieler Holz hält, aber zu wenig hat.")]
        public DialogueSequence buyFailDialog;

        [Tooltip("Spielt ab, wenn man mit leerer Hand klickt (Infos zum aktuellen Angebot).")]
        public DialogueSequence offerInfoDialog;
    }

    [System.Serializable]
    public class DebugItem
    {
        public ItemType item;
        public int amount;
    }

    [Header("Story & Progression")]
    [Tooltip("Wird nur beim allerersten Anklicken abgespielt.")]
    public DialogueSequence firstMeetingDialog;
    private bool hasMetTrader = false;

    [Tooltip("Ziehe hier dein Holz-Item rein, damit der Trader Holz als Währung erkennt.")]
    public ItemType woodItem;
    public List<SeedProgression> seedShop;
    private int currentOfferIndex = 0;

    [Header("Debug Settings (Für Inspector Tests)")]
    public bool giveItemsOnStart = true;
    public List<DebugItem> debugStartItems;

    private void Start()
    {
        if (giveItemsOnStart)
        {
            Invoke(nameof(GiveDebugItems), 0.5f);
        }

        // Sicherheitscheck: Der allererste Samen muss kaufbar sein (Softlock-Schutz)
        if (seedShop.Count > 0 && !seedShop[0].isUnlocked)
        {
            seedShop[0].isUnlocked = true; 
        }
    }

    private void GiveDebugItems()
    {
        Inventory inv = Object.FindFirstObjectByType<Inventory>();
        if (inv != null)
        {
            foreach (var dbg in debugStartItems)
            {
                if (dbg.item != null)
                {
                    inv.AddItem(dbg.item, dbg.amount);
                    Debug.Log($"[Trader System] DEBUG: Gave player {dbg.amount}x {dbg.item.name} on Start.");
                }
            }
        }
    }

    public void Interact(Tool toolInHand)
    {
        // Blockiere Interaktion, wenn gerade schon ein Dialog läuft
        if (DialogueManager.I != null && DialogueManager.I.IsPlaying) 
            return;

        // 1. ERSTES ZUSAMMENTREFFEN
        if (!hasMetTrader && firstMeetingDialog != null)
        {
            hasMetTrader = true;
            PlayDialog(firstMeetingDialog);
            return;
        }

        Inventory inv = Object.FindFirstObjectByType<Inventory>();
        if (inv == null) 
        {
            Debug.LogError("[Trader System] ERROR: No Inventory found in scene!");
            return;
        }

        var selectedStack = inv.GetSelectedStack();
        ItemType heldItem = selectedStack != null ? selectedStack.item : null;

        // 2. UNLOCK NEUER SAMEN (Spieler hält eine Ernte in der Hand)
        if (heldItem != null)
        {
            foreach (var tier in seedShop)
            {
                if (!tier.isUnlocked && tier.requiredHarvest != null && heldItem == tier.requiredHarvest)
                {
                    if (HasEnoughItems(inv, tier.requiredHarvest, tier.requiredHarvestAmount))
                    {
                        RemoveItems(inv, tier.requiredHarvest, tier.requiredHarvestAmount);
                        tier.isUnlocked = true;
                        inv.AddItem(tier.seedItem, 1); // 1 Samen gratis als Belohnung!
                        
                        PlayDialog(tier.unlockSuccessDialog);
                        return;
                    }
                    else
                    {
                        PlayDialog(tier.unlockHintDialog);
                        return;
                    }
                }
            }
        }

        // 3. SAMEN KAUFEN (Spieler hält Holz in der Hand)
        if (heldItem != null && heldItem == woodItem)
        {
            if (seedShop.Count == 0) return;

            SeedProgression offer = seedShop[currentOfferIndex];
            
            if (HasEnoughItems(inv, woodItem, offer.woodCost))
            {
                RemoveItems(inv, woodItem, offer.woodCost);
                inv.AddItem(offer.seedItem, 1);
                PlayDialog(offer.buySuccessDialog);
            }
            else
            {
                PlayDialog(offer.buyFailDialog);
            }
            return;
        }

        // 4. ANGEBOTE DURCHSCHALTEN (Spieler hat leere Hand oder etwas anderes)
        CycleToNextUnlockedOffer();
        
        SeedProgression currentOffer = seedShop[currentOfferIndex];
        PlayDialog(currentOffer.offerInfoDialog);
    }

    // --- HILFSMETHODE FÜR DEN DIALOG-MANAGER ---
    private void PlayDialog(DialogueSequence seq)
    {
        if (seq != null && DialogueManager.I != null)
        {
            DialogueManager.I.Play(seq);
        }
        else if (seq == null)
        {
            Debug.LogWarning("[TraderNPC] Es fehlt ein ScriptableObject (DialogueSequence) im Inspector!");
        }
    }

    private void CycleToNextUnlockedOffer()
    {
        if (seedShop.Count == 0) return;

        int originalIndex = currentOfferIndex;
        for (int i = 0; i < seedShop.Count; i++)
        {
            currentOfferIndex = (currentOfferIndex + 1) % seedShop.Count;
            if (seedShop[currentOfferIndex].isUnlocked)
            {
                return; // Nächstes freigeschaltetes Angebot gefunden!
            }
        }
        currentOfferIndex = originalIndex; // Fallback
    }

    // --- HILFSMETHODEN FÜR DAS INVENTAR ---
    private bool HasEnoughItems(Inventory inv, ItemType item, int requiredAmount)
    {
        int totalFound = 0;
        foreach (var slot in inv.slots)
        {
            if (slot != null && slot.item == item)
            {
                totalFound += slot.amount;
            }
        }
        return totalFound >= requiredAmount;
    }

    private void RemoveItems(Inventory inv, ItemType item, int amountToRemove)
    {
        for (int i = 0; i < inv.slots.Count; i++)
        {
            if (inv.slots[i] != null && inv.slots[i].item == item)
            {
                int taken = Mathf.Min(amountToRemove, inv.slots[i].amount);
                inv.slots[i].amount -= taken;
                amountToRemove -= taken;

                if (inv.slots[i].amount <= 0)
                {
                    inv.slots[i] = null;
                }

                if (amountToRemove <= 0) break;
            }
        }
        inv.OnInventoryChanged?.Invoke();
    }
}