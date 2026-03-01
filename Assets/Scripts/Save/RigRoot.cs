using UnityEngine;

public class RigBootstrapper : MonoBehaviour
{
    private static RigBootstrapper instance;

    private void Awake()
    {
        // 1. Singleton pattern: Destroy duplicates if we load back into this scene
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        // 2. Critical Fix: Force this object to be a root object before persisting it.
        // This guarantees you will NEVER get the "only works for root GameObjects" warning again,
        // even if you accidentally dragged StoryRig inside another object in the hierarchy.
        transform.SetParent(null);

        // 3. Persist the StoryRig and ALL of its children (your managers) across scenes
        DontDestroyOnLoad(gameObject);
        
        Debug.Log("[RigBootstrapper] StoryRig and all child managers are now persistent.");
    }
}