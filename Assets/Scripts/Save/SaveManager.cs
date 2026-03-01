using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager I { get; private set; }

    // --- DAS NEUE MEGAFON ---
    // Dieses Event wird gefeuert, wenn der Spielstand fertig entpackt wurde.
    public event Action OnSaveLoaded;

    [Header("File")]
    [SerializeField] private string fileName = "bloom_save.json";

    [Header("Debug")]
    [SerializeField] private bool logMessages = true;

    private string SavePath => Path.Combine(Application.persistentDataPath, fileName);

    [Serializable]
    public class SaveBlob
    {
        public string id;
        public string json;
    }

    [Serializable]
    public class SaveGameData
    {
        public int version = 1;
        public long savedAtUnix;
        public List<SaveBlob> blobs = new List<SaveBlob>();
    }

    private void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
    }

    public bool HasSave() => File.Exists(SavePath);

    public void DeleteSave()
    {
        if (File.Exists(SavePath))
            File.Delete(SavePath);

        if (logMessages) Debug.Log($"[BloomStory.SaveManager] Deleted save: {SavePath}");
    }

    public void NewGame()
    {
        DeleteSave();

        // Reset all systems that support it
        foreach (var r in FindAllResettables())
            r.ResetToDefaults();
            
        SaveGame(); // Speichert den frischen Zustand direkt ab
    }

    // Brücke für den StartMenuController
    public void ContinueGame()
    {
        LoadGame();
    }

    public void SaveGame()
    {
        var data = new SaveGameData
        {
            version = 1,
            savedAtUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        foreach (var s in FindAllSaveables())
        {
            var id = s.GetUniqueId();
            if (string.IsNullOrWhiteSpace(id)) continue;

            data.blobs.Add(new SaveBlob
            {
                id = id,
                json = s.CaptureState() ?? ""
            });
        }

        try
        {
            var json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SavePath, json);
            if (logMessages) Debug.Log($"[BloomStory.SaveManager] Saved ({data.blobs.Count} blobs) -> {SavePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[BloomStory.SaveManager] Save failed: {e.Message}");
        }
    }

    public void LoadGame()
    {
        if (!HasSave())
        {
            if (logMessages) Debug.LogWarning("[BloomStory.SaveManager] No save file found.");
            return;
        }

        SaveGameData data;
        try
        {
            var json = File.ReadAllText(SavePath);
            data = JsonUtility.FromJson<SaveGameData>(json);
        }
        catch (Exception e)
        {
            Debug.LogError($"[BloomStory.SaveManager] Load failed: {e.Message}");
            return;
        }

        if (data == null || data.blobs == null) return;

        // Build lookup for fast apply
        var map = data.blobs
            .Where(b => b != null && !string.IsNullOrWhiteSpace(b.id))
            .GroupBy(b => b.id)
            .ToDictionary(g => g.Key, g => g.Last().json);

        var saveables = FindAllSaveables();
        int applied = 0;

        foreach (var s in saveables)
        {
            var id = s.GetUniqueId();
            if (string.IsNullOrWhiteSpace(id)) continue;

            if (map.TryGetValue(id, out var stateJson))
            {
                s.RestoreState(stateJson);
                applied++;
            }
        }

        if (logMessages) Debug.Log($"[BloomStory.SaveManager] Loaded (applied {applied}/{saveables.Length}) from {SavePath}");
        
        // --- DER WICHTIGE RUF ---
        // Sagt allen Skripten (wie dem Tagebuch), dass die Daten jetzt bereitliegen!
        OnSaveLoaded?.Invoke();
    }

    // Nutzt das moderne, schnellere FindObjectsByType
    private ISaveable[] FindAllSaveables()
    {
        return FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<ISaveable>().ToArray();
    }

    private IResettableSaveable[] FindAllResettables()
    {
        return FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<IResettableSaveable>().ToArray();
    }
}