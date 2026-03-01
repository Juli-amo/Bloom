using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimDriver : MonoBehaviour
{
    public float dead = 0.05f;     // treat very small input as 0
    public float bias = 0.15f;     // hysteresis: new axis must exceed old by this
    public float damp = 0.06f;     // animator damping for smooth params

    Animator anim;
    Vector2 face = Vector2.down;   // last facing for Idle
    Vector2 lastCard = Vector2.down;

    void Awake() => anim = GetComponent<Animator>();

    void Update()
    {
        // Read input (replace with your movement vector if you have one)
        Vector2 v = new Vector2(Input.GetAxisRaw("Horizontal"),
                                Input.GetAxisRaw("Vertical"));

        // dead zone
        if (Mathf.Abs(v.x) < dead) v.x = 0;
        if (Mathf.Abs(v.y) < dead) v.y = 0;

        Vector2 card = Snap4(v);           // snap to 4 directions (with hysteresis)
        float speed = v.sqrMagnitude;      // use raw magnitude for Idle/Walk switch

        if (card != Vector2.zero) face = card;

        // Feed Animator (damped to avoid spikes)
        anim.SetFloat("MoveX", card.x, damp, Time.deltaTime);
        anim.SetFloat("MoveY", card.y, damp, Time.deltaTime);
        anim.SetFloat("Speed", speed);
        anim.SetFloat("FaceX", face.x);
        anim.SetFloat("FaceY", face.y);
    }

    Vector2 Snap4(Vector2 v)
    {
        if (v == Vector2.zero) return Vector2.zero;

        // decide dominant axis with bias; keep last axis if it's close (hysteresis)
        float ax = Mathf.Abs(v.x);
        float ay = Mathf.Abs(v.y);

        // prefer previous axis unless the other exceeds by "bias"
        if (Mathf.Abs(lastCard.x) > 0.5f && ax >= ay - bias)
            return lastCard = new Vector2(Mathf.Sign(v.x), 0);
        if (Mathf.Abs(lastCard.y) > 0.5f && ay >= ax - bias)
            return lastCard = new Vector2(0, Mathf.Sign(v.y));

        // otherwise pick clearly dominant axis
        if (ax > ay + bias)  return lastCard = new Vector2(Mathf.Sign(v.x), 0);
        if (ay > ax + bias)  return lastCard = new Vector2(0, Mathf.Sign(v.y));

        // tie → keep last
        return lastCard;
    }
}