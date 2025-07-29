using UnityEngine;
using System.Collections;

public class Chr_Movement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Coyote Time")]
    public float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    private bool isGrounded;

    [Header("Movement Settings")]
    public float moveSpeed = 8f;

    [Header("Jump Settings")]
    public float jumpForce = 14f;

    private float moveInput;
    private bool jumpPressed;

    [Header("Stress System (Temporarily Disabled)")]
    public bool isMediumStress = false;
    public bool isHighStress = false;

    public bool freezed = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (freezed)
        {
            animator.SetFloat("Speed", 0f);
            return;
        }

        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;

        // Get horizontal input
        moveInput = 0f;
        if (Input.GetKey(KeyCode.A)) moveInput = -1f;
        else if (Input.GetKey(KeyCode.D)) moveInput = 1f;

        // Handle jump input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpPressed = true;
        }

        // Update animation speed based on horizontal movement
        float animSpeed = Mathf.Abs(rb.linearVelocity.x);
        animator.SetFloat("Speed", animSpeed);
    }

    void FixedUpdate()
    {
        if (freezed) return;

        // Apply horizontal movement
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Apply jump
        if (jumpPressed && coyoteTimeCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            coyoteTimeCounter = 0f;
        }
        jumpPressed = false;
    }

    // Future stress system (currently disabled)
    /*
    void TryTriggerFreeze()
    {
        int chance = -1;

        if (isHighStress) chance = 5;
        else if (isMediumStress) chance = 10;

        if (chance > 0)
        {
            int roll = Random.Range(1, chance + 1);
            if (roll == 1)
            {
                StartCoroutine(FreezeForSeconds(5f));
            }
        }
    }
    */

    IEnumerator FreezeForSeconds(float seconds)
    {
        freezed = true;
        rb.linearVelocity = Vector2.zero;
        animator.SetFloat("Speed", 0f);
        yield return new WaitForSeconds(seconds);
        freezed = false;
    }

    void OnDrawGizmosSelected()
{
    if (groundCheck != null)
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
}
