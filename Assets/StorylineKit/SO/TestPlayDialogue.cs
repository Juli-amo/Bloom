using UnityEngine;

public class TestPlayDialogue : MonoBehaviour
{
    public DialogueSequence seq;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            DialogueManager.I.Play(seq);
    }
}