using UnityEngine;
using System.Collections;

public class DelayedAppearance : MonoBehaviour
{
    [Header("Zeit-Steuerung")]
    public bool useTimeDelay = true;
    [Tooltip("Nach wie vielen Sekunden soll das Objekt erscheinen?")]
    public float delayInSeconds = 60f;

    [Header("Fortschritts-Steuerung")]
    public bool useProgressTrigger = false;
    [Tooltip("Welcher Samen muss beim Trader freigeschaltet sein, damit dieses Buch erscheint?")]
    public ItemType triggerSeed;

    [Header("Effekte")]
    public bool usePopEffect = true;
    public float popDuration = 0.5f;

    private SpriteRenderer spriteRenderer;
    private Collider2D col;
    private Vector3 originalScale;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        originalScale = transform.localScale;

        // Am Anfang unsichtbar und inaktiv machen
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        if (col != null) col.enabled = false;
        if (usePopEffect) transform.localScale = Vector3.zero;
    }

    void Start()
    {
        StartCoroutine(AppearanceRoutine());
    }

    private IEnumerator AppearanceRoutine()
    {
        // 1. Warten auf Zeit (falls aktiviert)
        if (useTimeDelay)
        {
            yield return new WaitForSeconds(delayInSeconds);
        }

        // 2. Warten auf Spielfortschritt (falls aktiviert)
        if (useProgressTrigger && triggerSeed != null)
        {
            bool isUnlocked = false;
            while (!isUnlocked)
            {
                // Wir suchen den Trader in der Szene
                TraderNPC trader = Object.FindFirstObjectByType<TraderNPC>();
                if (trader != null)
                {
                    // Wir suchen in der Liste des Traders nach unserem Trigger-Samen
                    var entry = trader.seedShop.Find(s => s.seedItem == triggerSeed);
                    if (entry != null && entry.isUnlocked)
                    {
                        isUnlocked = true;
                    }
                }
                
                // Wir prüfen nur einmal pro Sekunde, um Performance zu sparen
                yield return new WaitForSeconds(1f);
            }
        }

        // 3. Erscheinen!
        ShowObject();
    }

    private void ShowObject()
    {
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (col != null) col.enabled = true;

        if (usePopEffect)
        {
            StartCoroutine(PopEffectRoutine());
        }
        else
        {
            transform.localScale = originalScale;
        }

        Debug.Log($"[Story] {gameObject.name} ist nun erschienen (Trigger erfüllt).");
    }

    private IEnumerator PopEffectRoutine()
    {
        float elapsed = 0;
        while (elapsed < popDuration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / popDuration;
            // Sanftes Einblenden mit Sinus-Kurve
            float curve = Mathf.Sin(percent * Mathf.PI * 0.5f); 
            transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, curve);
            yield return null;
        }
        transform.localScale = originalScale;
    }
}