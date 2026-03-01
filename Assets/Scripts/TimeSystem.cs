using UnityEngine;
using System;

public class TimeSystem : MonoBehaviour
{
    [Header("30 sec = 1 hour")]
    public float secondsPerHour = 30f;

    [Range(0,23)] public int hour = 8;   // start morning
    [Range(0,59)] public int minute = 0;
    public int day = 1;

    float acc;

    public event Action<int,int> OnMinute;  // hour, minute
    public event Action<int> OnHour;        // hour
    public event Action<int> OnDay;         // day

    void Update()
    {
        acc += Time.deltaTime;
        var secPerMinute = secondsPerHour / 60f;
        while (acc >= secPerMinute)
        {
            acc -= secPerMinute;
            minute++;
            if (minute >= 60) { minute = 0; hour++; OnHour?.Invoke(hour % 24); }
            if (hour >= 24) { hour = 0; day++; OnDay?.Invoke(day); }
            OnMinute?.Invoke(hour, minute);
        }
    }

    public float DayFraction() => ((hour % 24) + minute/60f) / 24f; // 0..1
}