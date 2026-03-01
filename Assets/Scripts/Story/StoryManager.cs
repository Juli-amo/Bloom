using System;
using System.Collections.Generic;
using UnityEngine;

public interface ICutscenePlayer
{
    void PlayIntro(Action onFinished);
}

public class StoryManager : MonoBehaviour, ISaveable, IResettableSaveable
{
    public static StoryManager I { get; private set; }

    [Header("State (saved)")]
    [SerializeField] private StoryState state;

    [Header("Save")]
    [Tooltip("Stable unique ID for this module in SaveManager.")]
    [SerializeField] private string uniqueSaveId = "BloomStory_State";

    [Header("Intro Cutscene Gate")]
    [SerializeField] private bool enableIntroCutscene = true;
    [SerializeField] private string introCutsceneFlagKey = "intro_cutscene_played";
    [SerializeField] private MonoBehaviour cutscenePlayerBehaviour; 
    [SerializeField] private bool saveAfterIntroCutscene = true;

    private ICutscenePlayer CutscenePlayer => cutscenePlayerBehaviour as ICutscenePlayer;

    // WICHTIG: Auf 'public' geändert für sichere JSON-Serialisierung!
    [Serializable]
    public class StorySave
    {
        public List<string> flags = new List<string>();
        public List<StoryState.IntEntry> counters = new List<StoryState.IntEntry>();
    }

    private void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;

        if (state == null)
        {
            state = ScriptableObject.CreateInstance<StoryState>();
        }
    }

    private void Start()
    {
        TryPlayIntroCutsceneIfNeeded();
    }

    public void TryPlayIntroCutsceneIfNeeded()
    {
        if (!enableIntroCutscene) return;
        if (state.HasFlag(introCutsceneFlagKey)) return;

        if (CutscenePlayer == null)
        {
            state.SetFlag(introCutsceneFlagKey);
            return;
        }

        CutscenePlayer.PlayIntro(() =>
        {
            state.SetFlag(introCutsceneFlagKey);
            if (saveAfterIntroCutscene && SaveManager.I != null) SaveManager.I.SaveGame();
        });
    }

    public void TryFireEvent(StoryEvent ev)
    {
        if (ev == null || ev.sequence == null) return;
        if (ev.playOnlyOnce && state.HasFlag(ev.eventId)) return;

        bool ok = ev.conditionType switch
        {
            StoryConditionType.None => true,
            StoryConditionType.CounterAtLeast => GetCounter(ev.counterKey) >= ev.counterMinValue,
            _ => true
        };

        if (!ok) return;

        if (ev.playOnlyOnce) state.SetFlag(ev.eventId);
        if (DialogueManager.I != null) DialogueManager.I.Play(ev.sequence);
        if (ev.autoSaveAfter && SaveManager.I != null) SaveManager.I.SaveGame();
    }

    public void OnEnterArea(string areaId, StoryEvent ev)
    {
        if (ev == null) return;
        if (ev.conditionType != StoryConditionType.EnterArea) return;
        if (!string.Equals(ev.triggerId, areaId, StringComparison.Ordinal)) return;
        TryFireEvent(ev);
    }

    public void OnTalkToNPC(string npcId, StoryEvent ev)
    {
        if (ev == null) return;
        if (ev.conditionType != StoryConditionType.TalkToNPC) return;
        if (!string.Equals(ev.triggerId, npcId, StringComparison.Ordinal)) return;
        TryFireEvent(ev);
    }

    public bool HasFlag(string key) => state.HasFlag(key);
    public void SetFlag(string key) => state.SetFlag(key);
    public int GetCounter(string key) => state.GetCounter(key);
    public void SetCounter(string key, int value) => state.SetCounter(key, value);
    public void AddCounter(string key, int delta) => state.AddCounter(key, delta);

    // -------- Saveable --------
    public string GetUniqueId() => uniqueSaveId;

    public string CaptureState()
    {
        var s = new StorySave
        {
            flags = new List<string>(state.flags),
            counters = new List<StoryState.IntEntry>(state.counters)
        };
        string json = JsonUtility.ToJson(s);
        Debug.Log($"[StoryManager] VERPACKE DATEN: {s.flags.Count} Flags gespeichert.");
        return json;
    }

    public void RestoreState(string stateJson)
    {
        if (string.IsNullOrWhiteSpace(stateJson)) return;

        var s = JsonUtility.FromJson<StorySave>(stateJson);
        if (s == null) return;

        state.ResetAll(); // Erstmal das Gedächtnis sauber löschen

        // Wir nutzen SetFlag, damit das ScriptableObject die Daten sauber schluckt
        if (s.flags != null)
        {
            foreach (var flag in s.flags)
            {
                state.SetFlag(flag);
            }
            Debug.Log($"[StoryManager] DATEN ENTPACKT: Habe {s.flags.Count} Flags wiederhergestellt!");
        }
        
        // Das Gleiche für Counters
        if (s.counters != null)
        {
            foreach (var c in s.counters)
            {
                state.SetCounter(c.key, c.value);
            }
        }
    }

    public void ResetToDefaults()
    {
        state.ResetAll();
    }
}