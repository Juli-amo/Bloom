using UnityEngine;

public class AudioZone : MonoBehaviour
{
    public string zoneName;
    public string playerTag = "Player";

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        AmbientSoundManager.I?.EnterZone(zoneName);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        AmbientSoundManager.I?.ExitZone(zoneName);
    }
}