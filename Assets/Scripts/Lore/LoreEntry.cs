using UnityEngine;

public enum LoreEntryType
{
    DiaryFragment,
    SurvivorHandbook,
    Ending
}

[CreateAssetMenu(menuName = "BloomStory/Lore Entry")]
public class LoreEntry : ScriptableObject
{
    [Header("Identity")]
    public string id;                 // e.g. D_01, HB_Intro, END_Symbiotic
    public LoreEntryType type;

    [Header("UI Text")]
    public string title;              // big title in UI
    public string subtitle;           // small line under title (e.g. Day 3, Mechanic 1, etc.)
    [TextArea(6, 18)] public string body;

    [Header("Optional UI")]
    [TextArea(2, 6)] public string unlockHint; // where/how it’s found (optional)
    public Sprite icon;               // optional later
}