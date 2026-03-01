using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Collider2D))]
public class PlantSystem : MonoBehaviour, IInteractable
{
    [Header("Objekte (aus Hierarchie reinziehen)")]
    public GameObject soilBackObject;
    public GameObject soilFrontObject;
    public GameObject plantObject;

    [Header("Back Boden Sprites (volle Tiles)")]
    public Sprite dirt_default;
    public Sprite dirt_tilt;
    public Sprite dirt_planted;

    [Header("Frontlip (nur planted)")]
    public Sprite frontlip_planted;

    [Header("Datenbank: Alle verfügbaren Pflanzen")]
    public PlantData[] allAvailablePlants;

    [Header("Water Tint")]
    public Color wateredTint = new Color(0.6f, 0.6f, 1f, 1f);
    public bool tintFrontlipToo = false;
    public bool keepWoundedVisualAfterRescue = true;
    public float woundedHoldTime = 2f;

    [Header("Ernte Settings")]
    public bool enableHarvest = true;
    public bool resetToUntilledAfterHarvest = true;

    [Header("Status (Playmode)")]
    public bool isTilled = false;
    public bool hasSeed = false;
    public bool isWatered = false;
    
    private PlantData currentPlant;
    private SpriteRenderer soilBackSR;
    private SpriteRenderer soilFrontSR;
    private SpriteRenderer plantSR;

    private float growthTimer = 0f;
    private int growthStage = 0;
    private float dryTimer = 0f;
    private bool isDecaying = false;
    private float decayTimer = 0f;
    private bool isDead = false;
    private bool isWounded = false;
    private float woundedTimer = 0f;
    private int woundedDecayFrameIndex = 0;

    private void Awake()
    {
        if (soilBackObject != null) soilBackSR = soilBackObject.GetComponent<SpriteRenderer>();
        if (soilFrontObject != null) soilFrontSR = soilFrontObject.GetComponent<SpriteRenderer>();
        if (plantObject != null) plantSR = plantObject.GetComponent<SpriteRenderer>();
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void Start() => UpdateVisuals();

    private void Update()
    {
        if (!hasSeed || currentPlant == null) return;
        if (isWounded) {
            woundedTimer += Time.deltaTime;
            if (woundedTimer >= woundedHoldTime) { isWounded = false; woundedTimer = 0f; UpdateVisuals(); }
            return;
        }
        if (isDead) return;

        if (isWatered && !isDecaying) {
            dryTimer = 0f;
            if (currentPlant.growthFrames != null && growthStage < currentPlant.growthFrames.Length - 1) {
                growthTimer += Time.deltaTime;
                if (growthTimer >= currentPlant.timeToGrow) GrowToNextStage();
            }
        } else {
            dryTimer += Time.deltaTime;
            if (!isDecaying && dryTimer >= currentPlant.timeToStartDecay) { isDecaying = true; decayTimer = 0f; }
            if (isDecaying) {
                decayTimer += Time.deltaTime;
                UpdateDecayVisual();
                if (decayTimer >= currentPlant.timeToFullyDecay) KillPlant();
            }
        }
    }

    public void Interact(Tool toolInHand)
    {
        if (DialogueManager.I != null && DialogueManager.I.IsPlaying) return;

        bool isHoldingHoe = toolInHand == Tool.Hoe || toolInHand == Tool.HoeWood || toolInHand == Tool.HoeDense; 
        bool isHoldingWateringCan = toolInHand == Tool.WateringCan;
        bool isEmptyHand = toolInHand == Tool.None || toolInHand == Tool.Hand;
        bool isHoldingSeed = toolInHand == Tool.Seeds;

        if (enableHarvest && CanHarvest()) {
            if (isEmptyHand) Harvest();
            return;
        }
        if (!isTilled) {
            if (isHoldingHoe) Till();
            return;
        }
        if (isTilled && !hasSeed) {
            if (isHoldingSeed) {
                Inventory inv = Object.FindFirstObjectByType<Inventory>();
                var selectedStack = inv?.GetSelectedStack();
                if (selectedStack?.item != null) {
                    PlantData plantToGrow = FindPlantForSeed(selectedStack.item);
                    if (plantToGrow != null) {
                        PlantSeed(plantToGrow);
                        selectedStack.amount--;
                        if (selectedStack.amount <= 0) inv.slots[inv.selectedSlotIndex] = null;
                        inv.OnInventoryChanged?.Invoke();
                    }
                }
            }
            return;
        }
        if (hasSeed && !isWatered && !isDead) {
            if (isHoldingWateringCan) Water();
            return;
        }
        if (hasSeed && !isDead && !isDecaying && !isWounded && isEmptyHand) TalkBoost();
    }

    // --- Hilfsmethoden ---
    private PlantData FindPlantForSeed(ItemType seedItem) => allAvailablePlants?.FirstOrDefault(p => p.seedItem == seedItem);

    private void Till() { isTilled = true; hasSeed = false; isWatered = false; currentPlant = null; ResetLifecycle(); UpdateVisuals(); }

    private void PlantSeed(PlantData selectedPlant) { currentPlant = selectedPlant; hasSeed = true; isWatered = false; ResetLifecycle(); UpdateVisuals(); }

    private void Water() {
        isWatered = true;
        if (isDecaying) { woundedDecayFrameIndex = GetCurrentDecayIndex(); isDecaying = false; decayTimer = 0f; dryTimer = 0f; if (keepWoundedVisualAfterRescue) { isWounded = true; woundedTimer = 0f; } }
        else dryTimer = 0f;
        UpdateVisuals();
    }

    private void TalkBoost() {
        if (currentPlant == null || isDecaying) return;
        growthTimer += currentPlant.timeToGrow * (currentPlant.talkBoostPercent / 100f);
        if (currentPlant.growthFrames != null && growthStage < currentPlant.growthFrames.Length - 1) {
            if (growthTimer >= currentPlant.timeToGrow) GrowToNextStage();
        }
    }

    private void GrowToNextStage() { growthStage++; growthTimer = 0f; isWatered = false; UpdateVisuals(); }

    private void UpdateDecayVisual() {
        if (plantSR == null || currentPlant?.decayFrames == null || currentPlant.decayFrames.Length == 0) return;
        plantSR.sprite = currentPlant.decayFrames[GetCurrentDecayIndex()];
    }

    private int GetCurrentDecayIndex() {
        if (currentPlant?.decayFrames == null || currentPlant.decayFrames.Length == 0) return 0;
        float t = Mathf.Clamp01(decayTimer / Mathf.Max(0.0001f, currentPlant.timeToFullyDecay));
        return Mathf.Clamp(Mathf.FloorToInt(t * (currentPlant.decayFrames.Length - 1)), 0, currentPlant.decayFrames.Length - 1);
    }

    private void KillPlant() { isDead = true; isDecaying = false; isWounded = false; isWatered = false; UpdateVisuals(); }

    private bool CanHarvest() => hasSeed && !isDead && currentPlant != null && !isDecaying && !isWounded && growthStage >= currentPlant.growthFrames.Length - 1;

    private void Harvest() {
        Inventory inv = Object.FindFirstObjectByType<Inventory>();
        if (inv != null && currentPlant?.harvestItem != null) inv.AddItem(currentPlant.harvestItem, currentPlant.harvestAmount);
        hasSeed = false; isWatered = false; currentPlant = null;
        if (resetToUntilledAfterHarvest) isTilled = false;
        ResetLifecycle(); UpdateVisuals();
    }

    private void ResetLifecycle() { growthStage = 0; growthTimer = 0f; dryTimer = 0f; isDecaying = false; decayTimer = 0f; isDead = false; isWounded = false; woundedTimer = 0f; }

    private void UpdateVisuals() {
        if (soilBackSR != null) {
            soilBackSR.sprite = !isTilled ? dirt_default : (!hasSeed ? dirt_tilt : dirt_planted);
            soilBackSR.color = isWatered ? wateredTint : Color.white;
        }
        if (soilFrontSR != null && soilFrontObject != null) {
            bool showFront = hasSeed && frontlip_planted != null;
            soilFrontObject.SetActive(showFront);
            if (showFront) soilFrontSR.sprite = frontlip_planted;
            soilFrontSR.color = tintFrontlipToo ? (isWatered ? wateredTint : Color.white) : Color.white;
        }
        if (plantSR == null || plantObject == null) return;
        if (!hasSeed || currentPlant == null) { plantObject.SetActive(false); return; }
        plantObject.SetActive(true);
        if (isDead) plantSR.sprite = currentPlant.decayFrames?.LastOrDefault();
        else if (isWounded) plantSR.sprite = currentPlant.decayFrames?[Mathf.Clamp(woundedDecayFrameIndex, 0, currentPlant.decayFrames.Length - 1)];
        else if (isDecaying) UpdateDecayVisual();
        else plantSR.sprite = currentPlant.growthFrames?[Mathf.Clamp(growthStage, 0, currentPlant.growthFrames.Length - 1)];
    }
}