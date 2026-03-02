using UnityEngine;
using UnityEngine.UI;

public class DayNightOverlay : MonoBehaviour
{
    [Header("Verknüpfungen")]
    public GameTimeManager timeManager;
    public Image overlayImage;

    [Header("Tageszeiten")]
    public float sunriseTime = 0.25f;  // 6:00 Uhr
    public float sunsetTime = 0.75f;   // 18:00 Uhr

    [Header("Dunkelheit")]
    [Range(0, 1)] public float maxDarkness = 0.85f;
    public float fadeSpeed = 0.5f; // Übergangsgeschwindigkeit

    void Start()
    {
        if (timeManager == null)
            timeManager = FindFirstObjectByType<GameTimeManager>();
    }

    void Update()
    {
        if (timeManager == null || overlayImage == null) return;

        float time = timeManager.GetNormalizedTime();
        float targetAlpha = CalculateDarkness(time);

        Color c = overlayImage.color;
        c.a = Mathf.MoveTowards(c.a, targetAlpha, fadeSpeed * Time.deltaTime);
        overlayImage.color = c;
    }

    float CalculateDarkness(float time)
    {
        float transitionLength = 0.05f; // 5% des Tages = ca. 1 Stunde Übergang

        // Sonnenaufgang
        if (time >= sunriseTime && time <= sunriseTime + transitionLength)
        {
            float t = (time - sunriseTime) / transitionLength;
            return Mathf.Lerp(maxDarkness, 0f, t);
        }

        // Voller Tag
        if (time > sunriseTime + transitionLength && time < sunsetTime - transitionLength)
            return 0f; // Komplett transparent

        // Sonnenuntergang
        if (time >= sunsetTime - transitionLength && time <= sunsetTime)
        {
            float t = (time - (sunsetTime - transitionLength)) / transitionLength;
            return Mathf.Lerp(0f, maxDarkness, t);
        }

        // Nacht
        return maxDarkness;
    }
}