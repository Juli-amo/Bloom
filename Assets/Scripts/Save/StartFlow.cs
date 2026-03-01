using UnityEngine;
using UnityEngine.UI;

public class StartFlow : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "SampleScene"; // Trag hier Island Scene Namen ein
    
    [Header("UI")]
    [SerializeField] private Button continueButton; 

    private void Start()
    {
        // Prüfen, ob SaveManager existiert und ein Savegame hat, um Continue auszugrauen
        if (continueButton != null && SaveManager.I != null)
        {
            continueButton.interactable = SaveManager.I.HasSave();
        }
    }

    public void NewGame()
    {
        if (GameSceneManager.I == null) return;
        GameSceneManager.I.LoadNewGame(gameSceneName);
    }

    public void Continue()
    {
        if (GameSceneManager.I == null) return;
        GameSceneManager.I.LoadSavedGame(gameSceneName);
    }

    // --- NEUE METHODE FÜR DEN QUIT BUTTON ---
    public void QuitGame()
    {
        Debug.Log("[StartFlow] QuitGame aufgerufen! Das Spiel wird beendet.");
        
        // Beendet das gebaute Spiel (.exe, .apk, etc.)
        Application.Quit();

        // (Optional) Wenn du im Unity Editor bist, stoppt dies den Play Mode
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}