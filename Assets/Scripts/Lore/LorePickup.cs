using UnityEngine;

public class LorePickup : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private LoreEntry entry;

    [Header("Interaction")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private bool inRange;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) inRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) inRange = false;
    }

    private void Update()
    {
        if (!inRange) return;
        if (entry == null) return;

        // prevent interaction while dialogue is open
        if (DialogueManager.I != null && DialogueManager.I.IsPlaying) return;

        if (Input.GetKeyDown(interactKey))
        {
            TryPickup();
        }
    }

    private void TryPickup()
    {
        // Use StoryManager flags to ensure "read once"
        // If you removed namespace, these are global singletons.
        if (StoryManager.I != null)
        {
            string flag = "lore_" + entry.id + "_unlocked";
            if (StoryManager.I.HasFlag(flag))
                return;

            StoryManager.I.SetFlag(flag);
        }

        // Show the lore text using your dialogue UI (fast)
        // We create a temporary DialogueSequence at runtime (no asset needed).
        var seq = ScriptableObject.CreateInstance<DialogueSequence>();
        seq.lines = new DialogueLine[1];
        seq.lines[0] = new DialogueLine
        {
            text = FormatEntry(entry),
            showPortrait = false,
            portrait = null
        };

        if (DialogueManager.I != null)
            DialogueManager.I.Play(seq);

        // Save right away
        if (SaveManager.I != null)
            SaveManager.I.SaveGame();

        // Remove pickup from world
        Destroy(gameObject);
    }

    private string FormatEntry(LoreEntry e)
    {
        // UI-friendly formatting for your dialogue box
        // (Later your journal UI can use title/subtitle/body separately)
        if (!string.IsNullOrWhiteSpace(e.subtitle))
            return $"{e.title}\n{e.subtitle}\n\n{e.body}";

        return $"{e.title}\n\n{e.body}";
    }
}