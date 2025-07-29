using UnityEngine;
using System.Collections;

public class Chr_Movement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private bool wasGroundedLastFrame;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Jump Buffer")]
    public float jumpBufferTime = 0.15f;
    private float jumpBufferCounter;

    [Header("Coyote Time")]
    public float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    private bool isGrounded;

    [Header("Movement Settings")]
    public float moveSpeed = 8f;

    [Header("Jump Settings")]
    public float jumpForce = 14f;
    [Tooltip("Multiplier to reduce upward velocity when jump is released early")]
    public float jumpCutMultiplier = 0.5f;

    private float moveInput;

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

        // Track if grounded last frame
        wasGroundedLastFrame = isGrounded;
        // Get horizontal input
        moveInput = 0f;
        if (Input.GetKey(KeyCode.A)) moveInput = -1f;
        else if (Input.GetKey(KeyCode.D)) moveInput = 1f;

        // Handle jump input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Variable Jump Height (Jump Cut)
        if (Input.GetKeyUp(KeyCode.Space) && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
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
        if (jumpBufferCounter > 0f && (isGrounded || wasGroundedLastFrame || coyoteTimeCounter > 0f))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        }

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
