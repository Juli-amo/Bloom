using UnityEngine;

public class SimplePlayerMovement : MonoBehaviour
{
    public float moveSpeed = 4f;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;
    private Vector2 lastDirection = Vector2.down; // Startrichtung

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Bewegung nur wenn keine Aktion läuft
        bool isDoingAction = animator.GetBool("IsAxing") || 
                             animator.GetBool("IsSickling") || 
                             animator.GetBool("IsWatering");

        if (!isDoingAction)
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
            movement.Normalize();

            if (movement != Vector2.zero)
            {
                lastDirection = movement;
                animator.SetFloat("MoveX", movement.x);
                animator.SetFloat("MoveY", movement.y);
                animator.SetBool("IsMoving", true);
            }
            else
            {
                animator.SetBool("IsMoving", false);
            }
        }
        else
        {
            movement = Vector2.zero;
        }

        // Aktionen testen (später durch dein Tool-System ersetzt)
        if (Input.GetKeyDown(KeyCode.Alpha1))
            animator.SetBool("IsAxing", true);
        if (Input.GetKeyUp(KeyCode.Alpha1))
            animator.SetBool("IsAxing", false);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            animator.SetBool("IsSickling", true);
        if (Input.GetKeyUp(KeyCode.Alpha2))
            animator.SetBool("IsSickling", false);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            animator.SetBool("IsWatering", true);
        if (Input.GetKeyUp(KeyCode.Alpha3))
            animator.SetBool("IsWatering", false);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}