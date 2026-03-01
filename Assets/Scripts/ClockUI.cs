using UnityEngine;
using UnityEngine.UI;

public class ClockUI : MonoBehaviour
{
    [Header("Verknüpfungen")]
    public GameTimeManager timeManager;
    public Image clockImage;

    [Header("Animation")]
    public Sprite[] clockSprites; // Hier kommen deine 24 Bilder rein

    void Start()
    {
        if (timeManager == null) 
            timeManager = FindFirstObjectByType<GameTimeManager>();
    }

    void Update()
    {
        if (timeManager == null || clockImage == null || clockSprites.Length == 0) return;

        // Hole die aktuelle Zeit (0.0 bis 1.0)
        float time = timeManager.GetNormalizedTime();

        // Berechne welches Bild dran ist
        // Beispiel: Bei 0.5 (Mittag) * 24 Bilder = Index 12
        int spriteIndex = Mathf.FloorToInt(time * clockSprites.Length);

        // Sicherheit: Index darf nicht größer als Array sein
        spriteIndex = Mathf.Clamp(spriteIndex, 0, clockSprites.Length - 1);

        // Bild tauschen
        clockImage.sprite = clockSprites[spriteIndex];
    }
}