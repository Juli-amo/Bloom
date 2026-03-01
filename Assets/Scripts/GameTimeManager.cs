using UnityEngine;

public class GameTimeManager : MonoBehaviour
{
    [Header("Einstellungen")]
    [Tooltip("Wie viele echte Minuten dauert ein Tag im Spiel?")]
    public float dayDurationMinutes = 24f;

    [Header("Info (Nur lesen)")]
    [Range(0, 1)] 
    public float dayProgress = 0f; // 0.0 = Mitternacht, 0.5 = Mittag, 1.0 = Mitternacht
    public int hours;
    public int minutes;

    void Update()
    {
        // 1. Zeit fortschreiten lassen
        // Formel: DeltaTime / (Minuten * 60 Sekunden)
        dayProgress += Time.deltaTime / (dayDurationMinutes * 60f);

        // 2. Tag loopen (Wenn 100% erreicht, fang bei 0 an)
        if (dayProgress >= 1f)
        {
            dayProgress = 0f;
            // Hier könntest du später: daysPlayed++;
        }

        // 3. Umrechnung für Anzeige (0.0-1.0 in 0-24 Uhr)
        float dayInSeconds = dayProgress * 86400; // Ein Tag hat 86400 Sekunden
        hours = Mathf.FloorToInt(dayInSeconds / 3600);
        minutes = Mathf.FloorToInt((dayInSeconds % 3600) / 60);
    }

    // Hilfsfunktion für andere Skripte (Sonne, Uhr, NPCs)
    public float GetNormalizedTime()
    {
        return dayProgress;
    }
}