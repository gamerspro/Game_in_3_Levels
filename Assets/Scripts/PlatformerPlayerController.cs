using UnityEngine;
using UnityEngine.SceneManagement;

public class AdvancedPlatformerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 10f;
    public float acceleration = 15f;
    public float deceleration = 20f;
    public float airControl = 0.8f;
   
    [Header("Jumping")]
    public float jumpForce = 15f;
    public float doubleJumpForce = 12f;
    public float coyoteTime = 0.15f;
    public float jumpBufferTime = 0.1f;
   
    [Header("Wall Jump")]
    public bool canWallJump = true;
    public float wallJumpForce = 12f;
    public float wallSlideSpeed = 2f;
    public float wallJumpXForce = 8f;
   
    [Header("Dash")]
    public bool canDash = true;
    public float dashSpeed = 25f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
   
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.3f;
    public LayerMask groundLayer;
   
    [Header("Wall Check")]
    public Transform wallCheckRight;
    public Transform wallCheckLeft;
    public float wallCheckDistance = 0.5f;
   
    [Header("Effects")]
    public ParticleSystem jumpParticles;
    public ParticleSystem dashParticles;
    public TrailRenderer trailRenderer;
   
    // Private variables
    private Rigidbody rb;
    private bool isGrounded;
    private bool wasGrounded;
    private bool isWallRight;
    private bool isWallLeft;
    private bool canDoubleJump;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private float horizontalInput;
    private float currentSpeed;
    private bool isDashing;
    private float dashTimeLeft;
    private float dashCooldownLeft;
   
    void Start()
    {
        rb = GetComponent<Rigidbody>();
       
        // Create ground check if not assigned
        if (groundCheck == null)
        {
            GameObject gc = new GameObject("GroundCheck");
            gc.transform.parent = transform;
            gc.transform.localPosition = new Vector3(0, -0.5f, 0);
            groundCheck = gc.transform;
        }
       
        // Create wall checks if not assigned
        if (wallCheckRight == null)
        {
            GameObject wcr = new GameObject("WallCheckRight");
            wcr.transform.parent = transform;
            wcr.transform.localPosition = new Vector3(0.5f, 0, 0);
            wallCheckRight = wcr.transform;
        }
       
        if (wallCheckLeft == null)
        {
            GameObject wcl = new GameObject("WallCheckLeft");
            wcl.transform.parent = transform;
            wcl.transform.localPosition = new Vector3(-0.5f, 0, 0);
            wallCheckLeft = wcl.transform;
        }
    }
   
    void Update()
    {
        // Input
        horizontalInput = Input.GetAxisRaw("Horizontal");
       
        // Ground check
        wasGrounded = isGrounded;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
       
        // Wall check
        if (canWallJump)
        {
            isWallRight = Physics.Raycast(wallCheckRight.position, Vector3.right, wallCheckDistance, groundLayer);
            isWallLeft = Physics.Raycast(wallCheckLeft.position, Vector3.left, wallCheckDistance, groundLayer);
        }
       
        // Coyote time (grace period after leaving ground)
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            canDoubleJump = true;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
       
        // Jump buffer (press jump slightly before landing)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
       
        // Jumping
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            Jump(jumpForce);
            jumpBufferCounter = 0f;
        }
        // Double jump
        else if (Input.GetKeyDown(KeyCode.Space) && !isGrounded && canDoubleJump && coyoteTimeCounter <= 0f)
        {
            Jump(doubleJumpForce);
            canDoubleJump = false;
            if (jumpParticles != null) jumpParticles.Play();
        }
        // Wall jump
        else if (canWallJump && Input.GetKeyDown(KeyCode.Space) && !isGrounded && (isWallRight || isWallLeft))
        {
            WallJump();
        }
       
        // Dash
        if (canDash && Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownLeft <= 0f && !isDashing)
        {
            StartDash();
        }
       
        // Update dash
        if (isDashing)
        {
            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0f)
            {
                EndDash();
            }
        }
       
        dashCooldownLeft -= Time.deltaTime;
       
        // Fall faster for better game feel
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * 1.5f * Time.deltaTime;
        }
    }
   
    void FixedUpdate()
    {
        if (isDashing)
        {
            // Dash movement
            rb.linearVelocity = new Vector3(transform.right.x * dashSpeed, 0, 0);
        }
        else
        {
            // Normal movement with acceleration
            float targetSpeed = horizontalInput * moveSpeed;
            float speedDif = targetSpeed - currentSpeed;
            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
           
            // Reduce control in air
            if (!isGrounded)
            {
                accelRate *= airControl;
            }
           
            currentSpeed += speedDif * accelRate * Time.fixedDeltaTime;
           
            // Apply movement
            rb.linearVelocity = new Vector3(currentSpeed, rb.linearVelocity.y, 0);
           
            // Wall slide
            if (canWallJump && !isGrounded && (isWallRight || isWallLeft) && rb.linearVelocity.y < 0)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, -wallSlideSpeed, 0);
            }
        }
    }
   
    void Jump(float force)
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, force, 0);
        if (jumpParticles != null) jumpParticles.Play();
    }
   
    void WallJump()
    {
        float jumpDirection = isWallRight ? -1f : 1f;
        rb.linearVelocity = new Vector3(jumpDirection * wallJumpXForce, wallJumpForce, 0);
        canDoubleJump = true;
        if (jumpParticles != null) jumpParticles.Play();
    }
   
    void StartDash()
    {
        isDashing = true;
        dashTimeLeft = dashDuration;
        dashCooldownLeft = dashCooldown;
       
        if (dashParticles != null) dashParticles.Play();
        if (trailRenderer != null) trailRenderer.emitting = true;
    }
   
    void EndDash()
    {
        isDashing = false;
        if (trailRenderer != null) trailRenderer.emitting = false;
    }
   
    void OnDrawGizmos()
    {
        // Ground check
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
       
        // Wall checks
        if (wallCheckRight != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(wallCheckRight.position, wallCheckRight.position + Vector3.right * wallCheckDistance);
        }
       
        if (wallCheckLeft != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(wallCheckLeft.position, wallCheckLeft.position + Vector3.left * wallCheckDistance);
        }
    }
}