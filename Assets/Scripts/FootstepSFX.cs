using UnityEngine;

[RequireComponent(typeof(AudioSource)), RequireComponent(typeof(Animator))]
public class FootstepSFX : MonoBehaviour
{
    public AudioClip grassClip;
    public Vector2 volumeRange = new(0.85f, 1.0f);
    public Vector2 pitchRange  = new(0.95f, 1.05f);
    public float minInterval = 0.12f; // avoid double fires

    AudioSource src;
    Animator anim;
    float lastStep;

    void Awake() {
        src = GetComponent<AudioSource>();
        src.playOnAwake = false; src.loop = false; src.spatialBlend = 0f;
        anim = GetComponent<Animator>();
    }

    // Called by Animation Events on Walk_* clips
    public void OnFootstep() {
        // Ensure we are still in the walk blend-state (not already idling)
        var st = anim.GetCurrentAnimatorStateInfo(0);
        if (!st.IsName("WalkBT")) return;

        if (Time.time - lastStep < minInterval) return;
        lastStep = Time.time;

        if (!grassClip) return;
        src.volume = Random.Range(volumeRange.x, volumeRange.y);
        src.pitch  = Random.Range(pitchRange.x,  pitchRange.y);
        src.PlayOneShot(grassClip);
    }
}