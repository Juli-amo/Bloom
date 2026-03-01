using UnityEngine;

    public interface ISaveable
    {
        // Must be stable across sessions (do not generate at runtime!)
        string GetUniqueId();

        // Return JSON string
        string CaptureState();

        // Apply JSON string
        void RestoreState(string stateJson);
    }

    // Optional: implement if you want explicit reset on NewGame
    public interface IResettableSaveable
    {
        void ResetToDefaults();
    }
