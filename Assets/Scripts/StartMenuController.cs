using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton;

    [Header("Scenes")]
    [Tooltip("Name deiner großen Insel-Scene (muss in Build Settings sein).")]
    [SerializeField] private string islandSceneName = "Island";

    private bool pendingLoad;

    private void Awake()
    {
        // Button Events verdrahten (falls du's lieber im Inspector machst, kannst du das entfernen)
        if (newGameButton != null)
            newGameButton.onClick.AddListener(OnNewGameClicked);

        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueClicked);
    }

    private void OnEnable()
    {
        RefreshContinueButton();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void RefreshContinueButton()
    {
        bool canContinue = SaveManager.I != null && SaveManager.I.HasSave();
        if (continueButton != null)
            continueButton.interactable = canContinue;
    }

    public void OnNewGameClicked()
    {
        if (SaveManager.I == null)
        {
            Debug.LogError("[StartMenu] SaveManager missing in scene. Put your Rig prefab into StartScene.");
            return;
        }

        // Reset & create fresh save state
        SaveManager.I.NewGame();

        pendingLoad = false; // no special load after scene load
        SceneManager.LoadScene(islandSceneName);
    }

    public void OnContinueClicked()
    {
        if (SaveManager.I == null)
        {
            Debug.LogError("[StartMenu] SaveManager missing in scene. Put your Rig prefab into StartScene.");
            return;
        }

        if (!SaveManager.I.HasSave())
        {
            RefreshContinueButton();
            return;
        }

        // Load island scene first, then apply save once objects exist.
        pendingLoad = true;
        SceneManager.LoadScene(islandSceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!pendingLoad) return;
        if (scene.name != islandSceneName) return;

        pendingLoad = false;

        if (SaveManager.I != null)
            SaveManager.I.ContinueGame(); // loads save into scene objects
    }
}