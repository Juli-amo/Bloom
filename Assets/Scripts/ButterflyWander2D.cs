using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ButterflyWander2D : MonoBehaviour
{
    [Header("Zone relativ zur Startposition")]
    public Vector2 areaSize = new(4f, 3f);

    [Header("Bewegung")]
    public float speed = 0.35f;
    public float dwellMin = 0.5f, dwellMax = 1.5f;
    public float hoverAmp = 0.04f, hoverFreq = 2.2f;
    public float stepRadius = 0.8f;

    [Header("Scheuchen")]
    public float fleeRadius = 1.2f;        // Ab dieser Distanz fliehen
    public float fleeSpeed = 1.5f;         // Fluchtgeschwindigkeit
    public float fleeDuration = 1.5f;      // Wie lange sie fliehen
    public string playerTag = "Player";    // Tag des Spielers

    Vector3 startPos, target;
    SpriteRenderer sr;
    Rigidbody2D rb;
    Transform player;
    bool waiting;
    bool fleeing;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        if (rb) { rb.gravityScale = 0f; rb.constraints = RigidbodyConstraints2D.FreezeRotation; }
        startPos = transform.position;

        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj) player = playerObj.transform;

        PickTarget(first: true);
    }

    void Update()
    {
        // Spieler in der Nähe? → Fliehen starten
        if (!fleeing && player != null)
        {
            float dist = Vector2.Distance(transform.position, player.position);
            if (dist < fleeRadius)
            {
                StartCoroutine(Flee());
                return;
            }
        }

        if (waiting || fleeing) return;

        var dir = (target - transform.position);
        float distToTarget = dir.magnitude;
        if (distToTarget < 0.05f) { StartCoroutine(WaitAndPick()); return; }

        dir /= Mathf.Max(distToTarget, 0.0001f);
        Vector3 step = dir * speed * Time.deltaTime;
        step.y += Mathf.Sin(Time.time * hoverFreq) * hoverAmp * Time.deltaTime;

        Vector3 newPos = transform.position + step;
        if (rb) rb.MovePosition(newPos); else transform.position = newPos;
        if (Mathf.Abs(dir.x) > 0.01f) sr.flipX = dir.x < 0;
    }

    IEnumerator Flee()
    {
        fleeing = true;
        waiting = false;
        StopCoroutine(nameof(WaitAndPick));

        float timer = 0f;
        while (timer < fleeDuration)
        {
            if (player != null)
            {
                // Weg vom Spieler fliegen
                Vector3 fleeDir = (transform.position - player.position).normalized;
                Vector3 newPos = transform.position + fleeDir * fleeSpeed * Time.deltaTime;
                newPos.y += Mathf.Sin(Time.time * hoverFreq) * hoverAmp * Time.deltaTime;

                if (rb) rb.MovePosition(newPos); else transform.position = newPos;

                // Umdrehen in Fluchtrichtung
                if (Mathf.Abs(fleeDir.x) > 0.01f) sr.flipX = fleeDir.x < 0;
            }
            timer += Time.deltaTime;
            yield return null;
        }

        // Nach der Flucht neue Startpos merken und weiterwandern
        startPos = transform.position;
        PickTarget(first: false);
        fleeing = false;
    }

    IEnumerator WaitAndPick()
    {
        waiting = true;
        yield return new WaitForSeconds(Random.Range(dwellMin, dwellMax));
        PickTarget(first: false);
        waiting = false;
    }

    void PickTarget(bool first)
    {
        Vector2 cand = (Vector2)transform.position + Random.insideUnitCircle * stepRadius;
        Vector2 center = (Vector2)startPos;
        float halfW = areaSize.x * 0.5f, halfH = areaSize.y * 0.5f;
        cand.x = Mathf.Clamp(cand.x, center.x - halfW, center.x + halfW);
        cand.y = Mathf.Clamp(cand.y, center.y - halfH, center.y + halfH);
        if (first) transform.position = new Vector3(cand.x, cand.y, transform.position.z);
        target = new Vector3(cand.x, cand.y, transform.position.z);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 0, 0.25f);
        Vector3 c = Application.isPlaying ? startPos : transform.position;
        Gizmos.DrawWireCube(c, new Vector3(areaSize.x, areaSize.y, 0));

        // Fluchtradius anzeigen
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, fleeRadius);
    }
}