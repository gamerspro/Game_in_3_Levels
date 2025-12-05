using UnityEngine;

public class PlatformerPlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
   
    [Header("Ground Check")]
    public float groundCheckDistance = 0.3f;
    public LayerMask groundLayer;
   
    private Rigidbody rb;
    private bool isGrounded;
    private float horizontalInput;
   
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
   
    void Update()
    {
        // Get horizontal input
        horizontalInput = Input.GetAxis("Horizontal");
       
        // Ground check using raycast
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance + 0.5f, groundLayer);
       
        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            Debug.Log("Jump!");
        }
    }
   
    void FixedUpdate()
    {
        // Move player horizontally
        Vector3 movement = new Vector3(horizontalInput * moveSpeed, rb.linearVelocity.y, 0);
        rb.linearVelocity = movement;
    }
   
    void OnDrawGizmos()
    {
        // Draw ground check ray
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * (groundCheckDistance + 0.5f));
    }
}