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

    private bool isGrounded;
    private bool wasGroundedLastFrame;

    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    private float moveInput;

    [Header("Jump Settings")]
    public float jumpForce = 14f;
    private bool jumpStarted;

    [Header("Jump Buffer")]
    public float jumpBufferTime = 0.15f;
    private float jumpBufferCounter;

    [Header("Coyote Time")]
    public float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    [Header("Apex Float")]
    public float hangTimeGravityScale = 0.5f;
    public float hangTimeVelocityThreshold = 0.25f;
    private float originalGravityScale;

    [Header("Jump Cut via Gravity")]
    public float jumpCutGravityMultiplier = 3f;
    public float jumpCutTimeWindow = 0.2f;
    private float jumpCutTimer;
    private bool jumpCutQueued;
    private bool jumpReleasedBeforeJump;

    [Header("Stress System (Temporarily Disabled)")]
    public bool isMediumStress = false;
    public bool isHighStress = false;
    public bool freezed = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        originalGravityScale = rb.gravityScale;
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

        // Coyote time
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        wasGroundedLastFrame = isGrounded;

        // Horizontal input
        moveInput = 0f;
        if (Input.GetKey(KeyCode.A)) moveInput = -1f;
        else if (Input.GetKey(KeyCode.D)) moveInput = 1f;

        // Jump input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }


        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (jumpStarted && jumpCutTimer > 0f)
            {
                jumpCutQueued = true;
            }

            // Released jump before jump executed (used for buffered jumps)
            if (!jumpStarted && jumpBufferCounter > 0f)
            {
                jumpReleasedBeforeJump = true;
            }
        }

        // Animation
        float animSpeed = Mathf.Abs(rb.linearVelocity.x);
        animator.SetFloat("Speed", animSpeed);
    }

    void FixedUpdate()
    {
        if (freezed) return;

        // Apply horizontal movement
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Handle jump
        if (jumpBufferCounter > 0f && (isGrounded || wasGroundedLastFrame || coyoteTimeCounter > 0f))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;

            jumpStarted = true;
            jumpCutTimer = jumpCutTimeWindow;

            // Reset state from previous jump
            jumpCutQueued = false;

            if (jumpReleasedBeforeJump)
            {
                jumpCutQueued = true;
                jumpReleasedBeforeJump = false;
            }
        }

        // Update jump cut timer
        if (jumpStarted && jumpCutTimer > 0f)
        {
            jumpCutTimer -= Time.fixedDeltaTime;
        }

        // Gravity handling
        if (jumpCutQueued && rb.linearVelocity.y > 0f)
        {
            rb.gravityScale = originalGravityScale * jumpCutGravityMultiplier;
        }
        else if (!isGrounded && Mathf.Abs(rb.linearVelocity.y) < hangTimeVelocityThreshold && Input.GetKey(KeyCode.Space) && !jumpCutQueued)
        {
            rb.gravityScale = hangTimeGravityScale; // Apex float
        }
        else
        {
            rb.gravityScale = originalGravityScale;
        }

        // Reset jump flags on landing
        if (isGrounded && rb.linearVelocity.y <= 0f)
        {
            jumpStarted = false;
            jumpCutQueued = false;
            jumpCutTimer = 0f;
            jumpReleasedBeforeJump = false;
        }
    }

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
