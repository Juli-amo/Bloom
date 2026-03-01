using UnityEngine;

[DisallowMultipleComponent]
public class SaveableEntity : MonoBehaviour
{
    [SerializeField] private string uniqueId;

    public string UniqueId => uniqueId;

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Auto-ID, wenn leer (Editor only)
        if (string.IsNullOrWhiteSpace(uniqueId))
        {
            uniqueId = System.Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
#endif
}