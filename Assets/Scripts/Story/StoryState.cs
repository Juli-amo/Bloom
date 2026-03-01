using System;
using System.Collections.Generic;
using UnityEngine;


    [CreateAssetMenu(menuName = "BloomStory/Story State")]
    public class StoryState : ScriptableObject
    {
        [Serializable]
        public class IntEntry
        {
            public string key;
            public int value;
        }

        public List<string> flags = new List<string>();
        public List<IntEntry> counters = new List<IntEntry>();

        public bool HasFlag(string key) => flags.Contains(key);

        public void SetFlag(string key)
        {
            if (!flags.Contains(key))
                flags.Add(key);
        }

        public int GetCounter(string key)
        {
            var e = counters.Find(x => x.key == key);
            return e != null ? e.value : 0;
        }

        public void SetCounter(string key, int value)
        {
            var e = counters.Find(x => x.key == key);
            if (e != null) e.value = value;
            else counters.Add(new IntEntry { key = key, value = value });
        }

        public void AddCounter(string key, int delta)
        {
            SetCounter(key, GetCounter(key) + delta);
        }

        public void ResetAll()
        {
            flags.Clear();
            counters.Clear();
        }
    }
