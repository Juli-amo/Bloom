using UnityEngine;
using UnityEngine.UI;

public class DayNightIndicator : MonoBehaviour
{
    public TimeSystem timeRef;
    public RectTransform sunHand, moonHand;  // assign in Inspector
    public float startAngleDeg = -90f;       // -90 = 6 o'clock at bottom

    void Awake(){ if (!timeRef) timeRef = FindFirstObjectByType<TimeSystem>(); }

    void Update()
    {
        if (!timeRef) return;
        float t = timeRef.DayFraction();             // 0..1
        float angle = startAngleDeg + t * 360f;
        if (sunHand)  sunHand.localEulerAngles  = new Vector3(0,0, angle);
        if (moonHand) moonHand.localEulerAngles = new Vector3(0,0, angle + 180f);
    }
}