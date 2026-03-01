using UnityEngine;

public class AutoSaveTimer : MonoBehaviour
{
    [Tooltip("Seconds between autosaves")]
    [SerializeField] private float intervalSeconds = 300f; // 5 minutes

    private float t;

    private void Update()
    {
        if (SaveManager.I == null) return;

        t += Time.unscaledDeltaTime;
        if (t >= intervalSeconds)
        {
            t = 0f;
            SaveManager.I.SaveGame();
            Debug.Log("[AutoSave] Saved.");
        }
    }
}