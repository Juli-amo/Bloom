using System.Collections;
using UnityEngine;

public class PlantPlot : MonoBehaviour, IInteractable
{
    public enum State { Grass, Tilled, Seeded, Growing, Mature }
    public State state = State.Grass;

    [Header("Renderers (zuweisen)")]
    public SpriteRenderer soilSR;     // Boden (grün/braun)
    public SpriteRenderer plantSR;    // Overlay für Pflanze
    public SpriteRenderer waterFXSR;  // kurze Gießkannen-Animation

    [Header("Sprites (zuweisen)")]
    public Sprite soilGrass;   // grünes Tile
    public Sprite soilTilled;  // braunes Tile
    public Sprite plantSeed;   // kleine grüne Details
    public Sprite plantGrowing;// optional mittlere Pflanze
    public Sprite plantMature; // reif

    [Header("Timing")]
    public float growthSeconds = 20f;   // <- Prototyp: 20 s ab dem Gießen
    public float waterFXTime   = 0.35f;

    Coroutine growCo;

    void Reset()
    {
        if (!soilSR)    soilSR    = GetComponent<SpriteRenderer>();
        if (!plantSR)   plantSR   = transform.Find("PlantVisual")?.GetComponent<SpriteRenderer>();
        if (!waterFXSR) waterFXSR = transform.Find("WaterFX")?.GetComponent<SpriteRenderer>();
    }

    void Awake()
    {
        if (!soilSR) soilSR = GetComponent<SpriteRenderer>();
        if (waterFXSR) { var c = waterFXSR.color; c.a = 0f; waterFXSR.color = c; waterFXSR.enabled = false; }
        Refresh();
    }

    // --------- E-Interaktion, abhängig vom aktiven Hotbar-Item ----------
    public void Interact(Tool tool)
    {
        var tm = FindFirstObjectByType<ToolManager>();

        switch (tool)
        {
            case Tool.Hoe:
                if (state == State.Grass)
                {
                    state = State.Tilled;
                    Refresh();
                }
                break;

            case Tool.Seeds:
                if (state == State.Tilled && tm != null && tm.ConsumeStackable(Tool.Seeds, 1))
                {
                    state = State.Seeded;
                    Refresh();
                }
                break;

            case Tool.WateringCan:
                if (state == State.Seeded || state == State.Growing)
                {
                    ShowWaterFX();
                    if (growCo != null) StopCoroutine(growCo);
                    growCo = StartCoroutine(GrowRoutine());     // 20 s bis reif
                    if (state == State.Seeded) { state = State.Growing; Refresh(); }
                }
                break;

            case Tool.Hand:
                if (state == State.Mature)
                {
                    if (tm != null) tm.AddStackable(Tool.Seeds, 2); // Prototyp: Ernte = 2 Seeds
                    state = State.Tilled; // Feld bleibt gelockert
                    Refresh();
                }
                break;
        }
    }
    // -------------------------------------------------------------------

    IEnumerator GrowRoutine()
    {
        yield return new WaitForSeconds(growthSeconds);
        state = State.Mature;
        Refresh();
        growCo = null;
    }

    void ShowWaterFX()
    {
        if (!waterFXSR) return;
        StartCoroutine(WaterFXFade());
    }
    System.Collections.IEnumerator WaterFXFade()
    {
        waterFXSR.enabled = true;
        var t = 0f;
        while (t < 0.1f) { t += Time.deltaTime; SetAlpha(waterFXSR, Mathf.Lerp(0,1,t/0.1f)); yield return null; }
        yield return new WaitForSeconds(waterFXTime);
        t = 0f;
        while (t < 0.1f) { t += Time.deltaTime; SetAlpha(waterFXSR, Mathf.Lerp(1,0,t/0.1f)); yield return null; }
        waterFXSR.enabled = false;
    }

    void SetAlpha(SpriteRenderer sr, float a){ var c = sr.color; c.a = a; sr.color = c; }

    void Refresh()
    {
        // Boden
        soilSR.sprite = (state == State.Grass) ? soilGrass : soilTilled;

        // Pflanze
        switch (state)
        {
            case State.Seeded:  plantSR.enabled = true;  plantSR.sprite = plantSeed;    break;
            case State.Growing: plantSR.enabled = true;  plantSR.sprite = plantGrowing ? plantGrowing : plantSeed; break;
            case State.Mature:  plantSR.enabled = true;  plantSR.sprite = plantMature;  break;
            default:            plantSR.enabled = false; break;
        }
    }
}