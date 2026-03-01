using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager I { get; private set; }

    [SerializeField] private DialogueUI ui;

    private DialogueSequence current;
    private int index;
    private bool playing;

    private void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
    }

    private void Start()
    {
        if (ui == null)
        {
            ui = Object.FindFirstObjectByType<DialogueUI>();
        }

        if (ui == null)
        {
            Debug.LogError("[DialogueManager] No DialogueUI found. Put DialoguePanel (with DialogueUI) in the scene.");
            return;
        }

        ui.Hide();
    }

    private void Update()
    {
        if (!playing || ui == null) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // If your DialogueUI supports paging:
            if (ui.NextPageOrDone())
                return;

            NextLine();
        }
    }

    public void Play(DialogueSequence seq)
    {
        if (ui == null)
            ui = Object.FindFirstObjectByType<DialogueUI>();

        if (ui == null)
        {
            Debug.LogError("[DialogueManager] Cannot Play() because DialogueUI is missing.");
            return;
        }

        if (seq == null || seq.lines == null || seq.lines.Length == 0) return;

        current = seq;
        index = 0;
        playing = true;

        ui.Show();
        ui.SetLine(current.lines[index]);
    }

    private void NextLine()
    {
        index++;
        if (current == null || current.lines == null || index >= current.lines.Length)
        {
            Stop();
            return;
        }

        ui.SetLine(current.lines[index]);
    }

    public void Stop()
    {
        playing = false;
        current = null;
        if (ui != null) ui.Hide();
    }

    public bool IsPlaying => playing;
}