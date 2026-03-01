using UnityEngine;
using UnityEngine.UI;

public class DayNightOverlay : MonoBehaviour
{
    [Header("Verknüpfungen")]
    public GameTimeManager timeManager;
    public Image overlayImage;

    [Header("Helligkeit")]
    [Tooltip("0.0 = Mitternacht, 0.5 = Mittag")]
    public float sunriseTime = 0.25f;  // 6:00 Uhr
    public float sunsetTime = 0.75f;   // 18:00 Uhr
    public float transitionDuration = 0.05f; // Wie schnell der Übergang ist

    [Header("Dunkelheit")]
    [Range(0, 1)] public float maxDarkness = 0.85f; // Wie dunkel die Nacht ist

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

        // Sanfter Übergang
        Color c = overlayImage.color;
        c.a = Mathf.Lerp(c.a, targetAlpha, transitionDuration);
        overlayImage.color = c;
    }

    float CalculateDarkness(float time)
    {
        // Tagsüber → hell
        if (time >= sunriseTime && time <= sunsetTime)
        {
            // Sonnenaufgang: dunkel → hell
            if (time < sunriseTime + transitionDuration * 2)
                return Mathf.Lerp(maxDarkness, 0, (time - sunriseTime) / (transitionDuration * 2));

            // Sonnenuntergang: hell → dunkel
            if (time > sunsetTime - transitionDuration * 2)
                return Mathf.Lerp(0, maxDarkness, (time - (sunsetTime - transitionDuration * 2)) / (transitionDuration * 2));

            return 0f; // Vollständig hell
        }

        return maxDarkness; // Nacht
    }
}