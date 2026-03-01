using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueSequence sequence;
    private bool playerInside;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerInside = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerInside = false;
    }

    private void Update()
    {
        if (!playerInside) return;
        if (DialogueManager.I != null && DialogueManager.I.IsPlaying) return;

        if (Input.GetKeyDown(KeyCode.E))
            DialogueManager.I.Play(sequence);
    }
}