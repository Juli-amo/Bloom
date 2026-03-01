using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // WICHTIG für Coroutines (das Warten)

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager I { get; private set; }

    private bool loadSaveAfterTransition = false;

    private void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void LoadNewGame(string sceneName)
    {
        loadSaveAfterTransition = false;
        
        if (SaveManager.I != null) 
            SaveManager.I.NewGame(); 
            
        SceneManager.LoadScene(sceneName);
    }

    public void LoadSavedGame(string sceneName)
    {
        loadSaveAfterTransition = true;
        SceneManager.LoadScene(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (loadSaveAfterTransition)
        {
            // Wir starten einen Countdown, der einen Frame wartet
            StartCoroutine(DelayedLoadRoutine());
        }
    }

    private IEnumerator DelayedLoadRoutine()
    {
        // MAGIE: Wir warten exakt einen Frame. 
        // So können alle Doppelgänger gelöscht werden und alle Objekte sauber in der Welt aufwachen.
        yield return null; 

        if (SaveManager.I != null)
        {
            SaveManager.I.LoadGame();
        }
        
        loadSaveAfterTransition = false;
    }
}