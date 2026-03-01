using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [Header("Roots")]
    [SerializeField] private GameObject panelRoot;

    [Header("Background (optional)")]
    [SerializeField] private Image panelBackground;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI dialogueText;

    private string fullText = "";

    private int currentPage = 1;
    private int totalPages = 1;

    private void Awake()
    {
        if (panelRoot == null) panelRoot = gameObject;
        if (dialogueText == null) dialogueText = GetComponentInChildren<TextMeshProUGUI>(true);

        if (dialogueText != null)
        {
            dialogueText.overflowMode = TextOverflowModes.Page;
            dialogueText.pageToDisplay = 1;
        }

        Hide();
    }

    public void Show()
    {
        if (panelRoot != null) panelRoot.SetActive(true);
    }

    public void Hide()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
    }

    public void SetLine(DialogueLine line)
    {
        if (dialogueText == null)
        {
            Debug.LogError("[DialogueUI] dialogueText missing.");
            return;
        }

        fullText = line.text ?? "";
        currentPage = 1;

        dialogueText.text = fullText;
        dialogueText.pageToDisplay = 1;
        dialogueText.maxVisibleCharacters = int.MaxValue;
        dialogueText.ForceMeshUpdate();

        totalPages = Mathf.Max(1, dialogueText.textInfo.pageCount);
        dialogueText.pageToDisplay = currentPage;
    }

    /// <summary>
    /// Space behavior:
    /// - If there is another page: go next page
    /// - Else: return false so DialogueManager advances to next line / closes
    /// </summary>
    public bool NextPageOrDone()
    {
        if (dialogueText == null) return false;

        if (currentPage < totalPages)
        {
            currentPage++;
            dialogueText.pageToDisplay = currentPage;
            return true;
        }

        return false;
    }
}