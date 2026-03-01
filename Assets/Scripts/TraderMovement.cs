using UnityEngine;

public class TraderMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float minWaitTime = 2f;
    public float maxWaitTime = 5f;
    public float wanderRadius = 10f;

    // Ziehe hier deine 4 Sprites im Inspector rein
    public Sprite trader_0; // links hoch
    public Sprite trader_1; // rechts hoch
    public Sprite trader_2; // links runter
    public Sprite trader_3; // rechts runter

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 movement;
    private Vector2 targetPosition;
    private bool isWaiting = true;
    private bool isTalking = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        targetPosition = rb.position;
        StartCoroutine(WaitAndPickNewTarget());
    }

    void Update()
    {
        if (isTalking || isWaiting) return;

        Vector2 direction = targetPosition - rb.position;

        if (direction.magnitude < 0.1f)
        {
            movement = Vector2.zero;
            StartCoroutine(WaitAndPickNewTarget());
            return;
        }

        movement = direction.normalized;
        UpdateSprite();
    }

    void FixedUpdate()
    {
        if (isTalking || isWaiting) return;
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    void UpdateSprite()
    {
        if (movement.magnitude < 0.01f) return;

        // Links oder Rechts?
        bool goingRight = movement.x >= 0;
        // Hoch oder Runter?
        bool goingUp = movement.y >= 0;

        if (goingUp && !goingRight)
            spriteRenderer.sprite = trader_0; // links hoch
        else if (goingUp && goingRight)
            spriteRenderer.sprite = trader_1; // rechts hoch
        else if (!goingUp && !goingRight)
            spriteRenderer.sprite = trader_2; // links runter
        else
            spriteRenderer.sprite = trader_3; // rechts runter
    }

    System.Collections.IEnumerator WaitAndPickNewTarget()
    {
        isWaiting = true;
        movement = Vector2.zero;
        float waitTime = Random.Range(minWaitTime, maxWaitTime);
        yield return new WaitForSeconds(waitTime);

        Vector2 randomDirection = Random.insideUnitCircle * wanderRadius;
        targetPosition = rb.position + randomDirection;
        isWaiting = false;
    }

    public void SetTalking(bool talking)
    {
        isTalking = talking;
        if (talking)
        {
            movement = Vector2.zero;
            rb.linearVelocity = Vector2.zero;
        }
    }
}