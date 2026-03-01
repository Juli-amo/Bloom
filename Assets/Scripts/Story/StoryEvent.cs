using UnityEngine;


    public enum StoryConditionType
    {
        None,
        EnterArea,
        TalkToNPC,
        CounterAtLeast
    }

    [CreateAssetMenu(menuName = "BloomStory/Story Event")]
    public class StoryEvent : ScriptableObject
    {
        [Header("Identity (unique)")]
        public string eventId;

        [Header("Condition")]
        public StoryConditionType conditionType = StoryConditionType.None;

        // for EnterArea / TalkToNPC
        public string triggerId;

        // for CounterAtLeast
        public string counterKey;
        public int counterMinValue = 1;

        [Header("Action")]
        public DialogueSequence sequence;

        [Header("Rules")]
        public bool playOnlyOnce = true;
        public bool autoSaveAfter = false;
    }
