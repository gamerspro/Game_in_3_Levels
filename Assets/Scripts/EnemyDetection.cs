using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    [Header("Vision Settings")]
    public float visionRange = 8f;
    public float visionAngle = 45f;
    
    [Header("References")]
    public Transform player;
    
    private StealthManager playerStealth;
    private bool hasDetected = false;
    
    void Start()
    {
        // Find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                playerStealth = playerObj.GetComponent<StealthManager>();
            }
            else
            {
                Debug.LogError("EnemyDetection: Could not find player with tag 'Player'");
            }
        }
        else
        {
            playerStealth = player.GetComponent<StealthManager>();
        }
        
        if (playerStealth == null)
        {
            Debug.LogError("EnemyDetection: Player does not have StealthManager component!");
        }
    }
    
    void Update()
    {
        if (player == null || playerStealth == null) return;
        
        // Check if player is in range
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        if (distanceToPlayer <= visionRange)
        {
            // Calculate direction to player
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            
            // Calculate angle between enemy's forward direction and direction to player
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            
            // Check if player is within vision angle
            if (angleToPlayer <= visionAngle)
            {
                // Check if player is hidden
                if (!playerStealth.isHidden)
                {
                    // Player detected!
                    DetectPlayer();
                }
            }
        }
    }
    
    void DetectPlayer()
    {
        if (!hasDetected)
        {
            hasDetected = true;
            Debug.Log("PLAYER DETECTED! Game Over! Press R to restart.");
            
            // Stop the game
            Time.timeScale = 0f;
        }
    }
    
    void OnDestroy()
    {
        // Make sure time scale is reset
        Time.timeScale = 1f;
    }
    
    // Draw vision cone in Scene view
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        
        // Draw vision range
        Gizmos.DrawWireSphere(transform.position, visionRange);
        
        // Draw vision cone
        Vector3 leftBoundary = Quaternion.Euler(0, -visionAngle, 0) * transform.forward * visionRange;
        Vector3 rightBoundary = Quaternion.Euler(0, visionAngle, 0) * transform.forward * visionRange;
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * visionRange);
    }
}