using UnityEngine;

[CreateAssetMenu(menuName = "BloomStory/Dialogue Sequence")]
public class DialogueSequence : ScriptableObject
{
    public DialogueLine[] lines;
}

[System.Serializable]
public class DialogueLine
{
    [TextArea(3, 8)]
    public string text;

    // Optional for later:
    public Sprite portrait;
    public bool showPortrait;
}