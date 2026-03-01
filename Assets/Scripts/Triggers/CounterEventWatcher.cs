using UnityEngine;

public class CounterEventWatcher : MonoBehaviour
{
    public static CounterEventWatcher I { get; private set; }

    [Header("CounterAtLeast events to evaluate")]
    [SerializeField] private StoryEvent[] eventsToCheck;

    private void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        
    }

    /// <summary>
    /// Call after you changed any counter (trees cut, plants dead, harvest, etc.)
    /// </summary>
    public void CheckNow()
    {
        if (StoryManager.I == null) return;
        if (eventsToCheck == null) return;

        foreach (var ev in eventsToCheck)
        {
            if (ev == null) continue;
            if (ev.conditionType != StoryConditionType.CounterAtLeast) continue;

            StoryManager.I.TryFireEvent(ev);
        }
    }

    /// <summary>
    /// Day/Night system should call this at the start of a new day.
    /// </summary>
    public void OnNewDayReset()
    {
        if (StoryManager.I == null) return;

        StoryManager.I.SetCounter("trees_cut_today", 0);
        // If you ever add plants_dead_today etc. you can reset them here.
    }
}