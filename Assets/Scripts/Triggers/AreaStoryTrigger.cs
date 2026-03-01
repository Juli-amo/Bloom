using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AreaStoryTrigger : MonoBehaviour
{
    [SerializeField] private string areaId = "area_01";
    [SerializeField] private StoryEvent storyEvent;

    private void Start()
    {
        // Beim Laden der Map: Prüfen, ob dieses Event schon in der Vergangenheit
        // abgespielt wurde. Wenn ja, zerstören wir den Trigger direkt.
        if (storyEvent != null && storyEvent.playOnlyOnce)
        {
            if (StoryManager.I != null && StoryManager.I.HasFlag(storyEvent.eventId))
            {
                Destroy(gameObject);
            }
        }
    }

    private void Reset()
    {
        var c = GetComponent<Collider2D>();
        c.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (StoryManager.I == null) return;

        StoryManager.I.OnEnterArea(areaId, storyEvent);

        // Zerstöre den Trigger, nachdem der Spieler ihn ausgelöst hat, 
        // damit er nicht bei jedem Durchlaufen erneut gefeuert wird.
        if (storyEvent != null && storyEvent.playOnlyOnce)
        {
            Destroy(gameObject);
        }
    }
}