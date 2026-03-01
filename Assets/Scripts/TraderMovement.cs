using UnityEngine;
using System.Collections;

public class TraderMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float minWaitTime = 3f;
    public float maxWaitTime = 7f;
    public float wanderRadius = 5f;

    [Header("Home Base")]
    public Vector2 homePosition = Vector2.zero;
    [Range(0, 1)] public float chanceToReturnHome = 0.3f;

    [Header("Sprites")]
    public Sprite trader_0; // links hoch
    public Sprite trader_1; // rechts hoch
    public Sprite trader_2; // links runter
    public Sprite trader_3; // rechts runter

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 movement;
    private Vector2 targetPosition;
    private bool isWaiting = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        targetPosition = rb.position;
        StartCoroutine(WaitAndPickNewTarget());
    }

    void Update()
    {
        // AUTOMATISCHER STOPP: Wenn ein Dialog läuft, bewegen wir uns nicht.
        if (DialogueManager.I != null && DialogueManager.I.IsPlaying)
        {
            StopMovementAndFacePlayer();
            return;
        }
        
        if (isWaiting) return;

        Vector2 direction = targetPosition - rb.position;

        // Ziel erreicht?
        if (direction.magnitude < 0.1f)
        {
            movement = Vector2.zero;
            StartCoroutine(WaitAndPickNewTarget());
            return;
        }

        movement = direction.normalized;
        UpdateSprite(movement);
    }

    void FixedUpdate()
    {
        // Im FixedUpdate auch prüfen, ob ein Dialog läuft
        if (DialogueManager.I != null && DialogueManager.I.IsPlaying)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (isWaiting)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    // Hilfsfunktion: Hält an und dreht sich zum Spieler
    private void StopMovementAndFacePlayer()
    {
        movement = Vector2.zero;
        rb.linearVelocity = Vector2.zero;

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Vector2 lookDir = ((Vector2)player.transform.position - rb.position).normalized;
            UpdateSprite(lookDir);
        }
    }

    void UpdateSprite(Vector2 dir)
    {
        if (dir.magnitude < 0.01f) return;

        bool goingRight = dir.x >= 0;
        bool goingUp = dir.y >= 0;

        if (goingUp && !goingRight) spriteRenderer.sprite = trader_0;
        else if (goingUp && goingRight) spriteRenderer.sprite = trader_1;
        else if (!goingUp && !goingRight) spriteRenderer.sprite = trader_2;
        else spriteRenderer.sprite = trader_3;
    }

    IEnumerator WaitAndPickNewTarget()
    {
        isWaiting = true;
        movement = Vector2.zero;
        yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));

        // Wenn ein Dialog läuft, warten wir einfach weiter, bis er fertig ist
        while (DialogueManager.I != null && DialogueManager.I.IsPlaying)
        {
            yield return null;
        }

        // Entscheiden: Zurück zur Homebase (0,0,0) oder zufällig wandern?
        if (Random.value < chanceToReturnHome && Vector2.Distance(rb.position, homePosition) > 2f)
        {
            targetPosition = homePosition;
        }
        else
        {
            Vector2 randomPoint = Random.insideUnitCircle * wanderRadius;
            targetPosition = homePosition + randomPoint;
        }

        isWaiting = false;
    }
}