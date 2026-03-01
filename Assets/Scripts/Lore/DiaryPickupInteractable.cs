using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DiaryPickupInteractable : MonoBehaviour, IInteractable
{
    [Header("Lore")]
    [SerializeField] private LoreEntry entry;

    [Header("One-time")]
    [SerializeField] private bool playOnlyOnce = true;
    [SerializeField] private string onceFlagIdOverride = "";

    [Header("Portrait (optional)")]
    [SerializeField] private bool showPortrait = false;
    [SerializeField] private Sprite portraitOverride;

    [Header("After reading")]
    [SerializeField] private bool destroyAfterRead = true;

    private void Awake()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void Start()
    {
        // 1. Versuch: Vielleicht sind die Daten schon da?
        CheckIfRead();
        
        // 2. Absicherung: Wir abonnieren das Megafon des SaveManagers!
        if (SaveManager.I != null)
        {
            SaveManager.I.OnSaveLoaded += CheckIfRead;
        }
    }

    private void OnDestroy()
    {
        // WICHTIG: Wenn das Buch zerstört wird, muss es aufhören zuzuhören, 
        // sonst wirft Unity Fehler.
        if (SaveManager.I != null)
        {
            SaveManager.I.OnSaveLoaded -= CheckIfRead;
        }
    }

    // Wir haben die Prüf-Logik in eine eigene Methode ausgelagert
    private void CheckIfRead()
    {
        string flagId = GetFlagId();
        
        if (playOnlyOnce && StoryManager.I != null)
        {
            if (StoryManager.I.HasFlag(flagId))
            {
                Debug.Log($"[DiaryPickup] GEFUNDEN: Flag '{flagId}' ist aktiv! Ich zerstöre mich selbst.");
                
                if (destroyAfterRead)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    public void Interact(Tool toolInHand)
    {
        if (entry == null) return;
        if (DialogueManager.I != null && DialogueManager.I.IsPlaying) return;

        string flagId = GetFlagId();
        
        if (playOnlyOnce && StoryManager.I != null)
        {
            if (StoryManager.I.HasFlag(flagId)) return;
            StoryManager.I.SetFlag(flagId);
            Debug.Log($"[DiaryPickup] INTERACT: Flag '{flagId}' wurde jetzt in den StoryManager geschrieben!");
        }

        var seq = ScriptableObject.CreateInstance<DialogueSequence>();
        seq.lines = new DialogueLine[1];
        seq.lines[0] = new DialogueLine
        {
            text = FormatEntry(entry),
            showPortrait = showPortrait && (portraitOverride != null),
            portrait = portraitOverride
        };

        DialogueManager.I?.Play(seq);
        SaveManager.I?.SaveGame();

        if (destroyAfterRead)
        {
            Destroy(gameObject);
        }
    }

    private string GetFlagId()
    {
        if (!string.IsNullOrWhiteSpace(onceFlagIdOverride)) return onceFlagIdOverride;
        if (entry != null && !string.IsNullOrWhiteSpace(entry.id)) return "lore_" + entry.id;
        if (entry != null) return "lore_" + entry.title;
        return "lore_unknown";
    }

    private string FormatEntry(LoreEntry e)
    {
        string t = e.title ?? "";
        string s = e.subtitle ?? "";
        string b = e.body ?? "";
        if (!string.IsNullOrWhiteSpace(s)) return $"{t}\n{s}\n\n{b}";
        return $"{t}\n\n{b}";
    }
}