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
    [SerializeField] private bool jumpStarted;

    [Header("Jump Buffer")]
    public float jumpBufferTime = 0.15f;
    private float jumpBufferCounter;

    [Header("Coyote Time")]
    public float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    [Header("Jump Cut via Gravity")]
    public float jumpCutGravityMultiplier = 3f;
    private bool jumpCutQueued;
    private bool jumpReleasedBeforeJump;

    private float originalGravityScale;

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

        HandleGroundCheck();
        HandleInput();
        HandleAnimations();
    }

    void FixedUpdate()
    {
        

        HandleMovement();
        HandleJump();
        HandleGravity();
        HandleLandingReset();
    }

    // === UPDATE METHODS ===

    void HandleGroundCheck()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;

        wasGroundedLastFrame = isGrounded;
    }

    void HandleInput()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

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
            if (jumpStarted)
            {
                jumpCutQueued = true;
            }

            if (!jumpStarted && jumpBufferCounter > 0f)
            {
                jumpReleasedBeforeJump = true;
            }
        }
    }

    void HandleAnimations()
    {
        float animSpeed = Mathf.Abs(rb.linearVelocity.x);
        animator.SetFloat("Speed", animSpeed);
    }

    // === FIXEDUPDATE METHODS ===

    void HandleMovement()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    void HandleJump()
    {
        if (jumpBufferCounter > 0f && (isGrounded || wasGroundedLastFrame || coyoteTimeCounter > 0f))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;

            jumpStarted = true;
            jumpCutQueued = false;

            if (jumpReleasedBeforeJump)
            {
                jumpCutQueued = true;
                jumpReleasedBeforeJump = false;
            }
        }
    }

    void HandleGravity()
    {
        float targetGravity = originalGravityScale;

        if (jumpCutQueued && rb.linearVelocity.y > 0f)
        {
            targetGravity = originalGravityScale * jumpCutGravityMultiplier;
        }

        if (rb.gravityScale != targetGravity)
        {
            rb.gravityScale = targetGravity;
        }
    }

    void HandleLandingReset()
    {
        if (isGrounded && rb.linearVelocity.y <= 0f)
        {
            jumpStarted = false;
            jumpCutQueued = false;
            jumpReleasedBeforeJump = false;
        }
    }

    // === FREEZE SYSTEM ===

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
