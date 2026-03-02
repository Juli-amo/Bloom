using System.Collections.Generic;
using UnityEngine;

public class AmbientSoundManager : MonoBehaviour
{
    public static AmbientSoundManager I { get; private set; }

    [Header("Tageszeit Sounds")]
    public AudioSource daySound;
    public AudioSource nightSound;

    [Header("Zonen Sounds")]
    public AudioSource beachSound;
    public AudioSource riverSound;

    [Header("Einstellungen")]
    public GameTimeManager timeManager;
    public float fadeSpeed = 1.5f;
    public float dayStart = 0.25f;  // 6:00 Uhr
    public float dayEnd = 0.75f;    // 18:00 Uhr

    private string currentZone = "";
    private bool isDay = true;
    private Dictionary<string, int> zoneCount = new Dictionary<string, int>();

    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
    }

    void Start()
    {
        if (timeManager == null)
            timeManager = FindFirstObjectByType<GameTimeManager>();

        StartAll();
        UpdateDayNight();
    }

    void Update()
    {
        UpdateDayNight();
        UpdateVolumes();
        CheckAudioSources();
    }

    void UpdateDayNight()
    {
        if (timeManager == null) return;
        float time = timeManager.GetNormalizedTime();
        isDay = time >= dayStart && time <= dayEnd;
    }

    void UpdateVolumes()
    {
        float targetBeach = currentZone == "Beach" ? 1f : 0f;
        float targetRiver = currentZone == "River" ? 1f : 0f;
        float targetDay   = currentZone == "" && isDay  ? 1f : 0f;
        float targetNight = currentZone == "" && !isDay ? 1f : 0f;

        beachSound.volume = Mathf.MoveTowards(beachSound.volume, targetBeach, fadeSpeed * Time.deltaTime);
        riverSound.volume = Mathf.MoveTowards(riverSound.volume, targetRiver, fadeSpeed * Time.deltaTime);
        daySound.volume   = Mathf.MoveTowards(daySound.volume,   targetDay,   fadeSpeed * Time.deltaTime);
        nightSound.volume = Mathf.MoveTowards(nightSound.volume, targetNight, fadeSpeed * Time.deltaTime);
    }

    public void EnterZone(string zoneName)
    {
        if (!zoneCount.ContainsKey(zoneName)) zoneCount[zoneName] = 0;
        zoneCount[zoneName]++;
        currentZone = zoneName;
    }

    public void ExitZone(string zoneName)
    {
        if (!zoneCount.ContainsKey(zoneName)) return;
        zoneCount[zoneName]--;

        if (zoneCount[zoneName] <= 0)
        {
            zoneCount[zoneName] = 0;
            if (currentZone == zoneName)
                currentZone = "";
        }
    }

    void CheckAudioSources()
    {
        if (daySound != null && !daySound.isPlaying) daySound.Play();
        if (nightSound != null && !nightSound.isPlaying) nightSound.Play();
        if (beachSound != null && !beachSound.isPlaying) beachSound.Play();
        if (riverSound != null && !riverSound.isPlaying) riverSound.Play();
    }

    void StartAll()
    {
        PlaySilent(daySound);
        PlaySilent(nightSound);
        PlaySilent(beachSound);
        PlaySilent(riverSound);
    }

    void PlaySilent(AudioSource source)
    {
        if (source == null) return;
        source.volume = 0f;
        source.loop = true;
        source.Play();
    }
}
