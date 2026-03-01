using UnityEngine;

[CreateAssetMenu(menuName = "Bloom/Plant Data")]
public class PlantData : ScriptableObject
{
    [Header("Identity")]
    public string plantId; // e.g. "coral_fern"

    [Header("Seed & Harvest")]
    public ItemType seedItem; // <-- NEU: Welcher Samen pflanzt diese Pflanze?
    public ItemType harvestItem;
    public int harvestAmount = 1;

    [Header("Visuals")]
    public Sprite[] growthFrames;
    public Sprite[] decayFrames;

    [Header("Timing")]
    public float timeToGrow = 10f;
    public float timeToStartDecay = 6f;
    public float timeToFullyDecay = 8f;

    [Header("Talk Boost")]
    [Range(0, 100)] public float talkBoostPercent = 25f;
}