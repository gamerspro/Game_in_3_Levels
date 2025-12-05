using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Enhanced player movement with crouch, sprint, and improved physics
/// </summary>
public class PlayerMovement_Enhanced : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float groundCheckDistance = 0.3f;
    [SerializeField] private LayerMask groundLayer;
    
    [Header("Crouch Settings")]
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float crouchTransitionSpeed = 10f;
    
    [Header("Audio Settings")]
    [SerializeField] private float walkNoiseRadius = 3f;
    [SerializeField] private float sprintNoiseRadius = 8f;
    [SerializeField] private float crouchNoiseRadius = 1f;
    
    // Components
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private StealthManager stealthManager;
    
    // State
    private bool isGrounded = true;
    private bool isCrouching = false;
    private bool isSprinting = false;
    private Vector2 moveInput;
    private float currentSpeed;
    private float targetHeight;
    
    // Public properties
    public float CurrentNoiseRadius { get; private set; }
    public bool IsCrouching => isCrouching;
    public bool IsSprinting => isSprinting;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        stealthManager = GetComponent<StealthManager>();
        
        if (rb == null)
        {
            Debug.LogError("PlayerMovement: Rigidbody component required!");
        }
        
        if (capsuleCollider == null)
        {
            Debug.LogError("PlayerMovement: CapsuleCollider component required!");
        }
        
        targetHeight = standingHeight;
        currentSpeed = walkSpeed;
        CurrentNoiseRadius = walkNoiseRadius;
        
        // Configure Rigidbody
        rb.freezeRotation = true; // Prevent player from tipping over
    }
    
    void Update()
    {
        HandleInput();
        CheckGrounded();
        HandleCrouchTransition();
        UpdateNoiseRadius();
    }
    
    void FixedUpdate()
    {
        // Use FixedUpdate for physics-based movement
        MovePlayer();
    }
    
    private void HandleInput()
    {
        // Get movement input
        moveInput = Vector2.zero;
        
        if (Keyboard.current.wKey.isPressed) moveInput.y = 1f;
        if (Keyboard.current.sKey.isPressed) moveInput.y = -1f;
        if (Keyboard.current.aKey.isPressed) moveInput.x = -1f;
        if (Keyboard.current.dKey.isPressed) moveInput.x = 1f;
        
        // Normalize diagonal movement
        moveInput = moveInput.normalized;
        
        // Sprint (Shift key) - Can't sprint while crouching
        if (Keyboard.current.leftShiftKey.isPressed && !isCrouching && moveInput.magnitude > 0)
        {
            isSprinting = true;
            currentSpeed = sprintSpeed;
        }
        else
        {
            isSprinting = false;
            currentSpeed = isCrouching ? crouchSpeed : walkSpeed;
        }
        
        // Crouch (Ctrl or C key)
        if (Keyboard.current.leftCtrlKey.wasPressedThisFrame || Keyboard.current.cKey.wasPressedThisFrame)
        {
            ToggleCrouch();
        }
        
        // Jump (Space)
        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded && !isCrouching)
        {
            Jump();
        }
    }
    
    private void MovePlayer()
    {
        if (moveInput.magnitude > 0.01f)
        {
            // Calculate movement direction
            Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
            
            // Apply movement using Rigidbody (better for physics)
            Vector3 targetVelocity = moveDirection * currentSpeed;
            targetVelocity.y = rb.linearVelocity.y; // Preserve vertical velocity
            
            // Smooth velocity change
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, targetVelocity, Time.fixedDeltaTime * 10f);
            
            // Rotate player to face movement direction (optional)
            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
            }
        }
    }
    
    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
    }
    
    private void ToggleCrouch()
    {
        // Check if there's room to stand up
        if (isCrouching)
        {
            if (CanStandUp())
            {
                isCrouching = false;
                targetHeight = standingHeight;
            }
            else
            {
                Debug.Log("Not enough room to stand up!");
            }
        }
        else
        {
            isCrouching = true;
            targetHeight = crouchHeight;
        }
    }
    
    private bool CanStandUp()
    {
        // Raycast upward to check for obstacles
        float checkDistance = standingHeight - crouchHeight + 0.1f;
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position, Vector3.up, out hit, checkDistance))
        {
            return false; // Obstacle above
        }
        
        return true;
    }
    
    private void HandleCrouchTransition()
    {
        if (capsuleCollider != null)
        {
            // Smoothly transition height
            float newHeight = Mathf.Lerp(capsuleCollider.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);
            capsuleCollider.height = newHeight;
            
            // Adjust collider center
            capsuleCollider.center = new Vector3(0, newHeight / 2f, 0);
        }
    }
    
    private void CheckGrounded()
    {
        // Raycast downward to check if grounded
        RaycastHit hit;
        Vector3 rayStart = transform.position + Vector3.up * 0.1f;
        
        if (Physics.Raycast(rayStart, Vector3.down, out hit, groundCheckDistance + 0.1f, groundLayer))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
    
    private void UpdateNoiseRadius()
    {
        // Update noise radius based on movement state
        if (isSprinting)
        {
            CurrentNoiseRadius = sprintNoiseRadius;
        }
        else if (isCrouching)
        {
            CurrentNoiseRadius = crouchNoiseRadius;
        }
        else
        {
            CurrentNoiseRadius = walkNoiseRadius;
        }
        
        // Notify StealthManager if available
        if (stealthManager != null)
        {
            stealthManager.currentNoiseLevel = CurrentNoiseRadius;
        }
    }
    
    // Debug visualization
    void OnDrawGizmos()
    {
        // Draw ground check ray
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector3 rayStart = transform.position + Vector3.up * 0.1f;
        Gizmos.DrawLine(rayStart, rayStart + Vector3.down * (groundCheckDistance + 0.1f));
        
        // Draw noise radius
        Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
        Gizmos.DrawWireSphere(transform.position, CurrentNoiseRadius);
    }
}