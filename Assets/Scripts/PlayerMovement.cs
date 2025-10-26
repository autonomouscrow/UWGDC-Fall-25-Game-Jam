using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    public Vector2 movement;

    private Rigidbody2D rb;
    private Animator animator;
    

    private const string horiz = "Horizontal";
    private const string vert = "Vertical";
    private const string lastHoriz = "LastHorizontal";
    private const string lastVert = "LastVertical";

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        rb.gravityScale = 0; // disable gravity
        rb.freezeRotation = true; // prevent spinning
    }

    void Update()
    {
        movement.Set(InputManager.Movement.x, InputManager.Movement.y);

        // Move with collision detection
        rb.linearVelocity = movement * moveSpeed;

        animator.SetFloat(horiz, movement.x);
        animator.SetFloat(vert, movement.y);


        if (movement != Vector2.zero)
        {
            animator.SetFloat(lastHoriz, movement.x);
            animator.SetFloat(lastVert, movement.y);
        }
    }
}