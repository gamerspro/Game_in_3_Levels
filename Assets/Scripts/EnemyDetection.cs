using UnityEngine;
using System.Collections;

/// <summary>
/// Enhanced enemy detection with gradual awareness, alert states, and sound detection
/// </summary>
public class EnemyDetection_Enhanced : MonoBehaviour
{
    [Header("Vision Settings")]
    [SerializeField] private float visionRange = 10f;
    [SerializeField] private float visionAngle = 60f;
    [SerializeField] private LayerMask obstacleMask; // For line-of-sight checking
    
    [Header("Detection Settings")]
    [SerializeField] private float detectionSpeed = 1f; // How fast detection fills
    [SerializeField] private float detectionDecaySpeed = 0.5f; // How fast detection decreases
    [SerializeField] private float maxDetection = 100f;
    [SerializeField] private float detectionThreshold = 80f; // When player is fully detected
    
    [Header("Sound Detection")]
    [SerializeField] private bool canHearPlayer = true;
    [SerializeField] private float maxHearingDistance = 15f;
    
    [Header("Alert Settings")]
    [SerializeField] private float alertDuration = 5f;
    [SerializeField] private float searchRadius = 10f;
    [SerializeField] private float investigationSpeed = 3f;
    
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform headTransform; // For accurate vision position
    
    [Header("Visual Feedback")]
    [SerializeField] private GameObject detectionIndicator; // UI element above enemy
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color suspiciousColor = Color.yellow;
    [SerializeField] private Color alertColor = Color.red;
    
    // Components
    private EnemyPatrol patrolScript;
    private StealthManager playerStealth;
    private Renderer enemyRenderer;
    
    // Detection state
    private float currentDetection = 0f;
    private bool isPlayerDetected = false;
    private bool isInvestigating = false;
    private Vector3 lastKnownPosition;
    
    // Alert state
    public enum AlertState { Patrol, Suspicious, Alert, Searching }
    private AlertState currentState = AlertState.Patrol;
    
    // Properties
    public float DetectionPercentage => (currentDetection / maxDetection) * 100f;
    public AlertState CurrentState => currentState;
    
    void Start()
    {
        InitializeComponents();
        
        if (headTransform == null)
        {
            headTransform = transform; // Use body if no head specified
        }
    }
    
    private void InitializeComponents()
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
        }
        
        patrolScript = GetComponent<EnemyPatrol>();
        enemyRenderer = GetComponentInChildren<Renderer>();
        
        if (detectionIndicator != null)
        {
            detectionIndicator.SetActive(false);
        }
    }
    
    void Update()
    {
        if (player == null) return;
        
        switch (currentState)
        {
            case AlertState.Patrol:
                PatrolBehavior();
                break;
                
            case AlertState.Suspicious:
                SuspiciousBehavior();
                break;
                
            case AlertState.Alert:
                AlertBehavior();
                break;
                
            case AlertState.Searching:
                SearchingBehavior();
                break;
        }
        
        UpdateVisualFeedback();
    }
    
    private void PatrolBehavior()
    {
        // Check for visual detection
        bool canSeePlayer = CheckVisualDetection();
        
        // Check for sound detection
        bool canHearPlayerNoise = CheckSoundDetection();
        
        if (canSeePlayer || canHearPlayerNoise)
        {
            // Increase detection
            currentDetection += detectionSpeed * Time.deltaTime;
            
            if (currentDetection >= detectionThreshold * 0.3f)
            {
                // Become suspicious
                ChangeState(AlertState.Suspicious);
            }
        }
        else
        {
            // Decrease detection
            currentDetection -= detectionDecaySpeed * Time.deltaTime;
            currentDetection = Mathf.Max(0, currentDetection);
        }
        
        // Check if fully detected
        if (currentDetection >= detectionThreshold)
        {
            DetectPlayer();
        }
    }
    
    private void SuspiciousBehavior()
    {
        // Stop patrolling and look around
        if (patrolScript != null)
        {
            patrolScript.enabled = false;
        }
        
        // Look towards player's last known position
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f);
        
        bool canSeePlayer = CheckVisualDetection();
        bool canHearPlayerNoise = CheckSoundDetection();
        
        if (canSeePlayer || canHearPlayerNoise)
        {
            currentDetection += detectionSpeed * 1.5f * Time.deltaTime; // Detect faster when suspicious
            
            if (currentDetection >= detectionThreshold)
            {
                DetectPlayer();
            }
        }
        else
        {
            currentDetection -= detectionDecaySpeed * 0.5f * Time.deltaTime; // Decay slower
            
            if (currentDetection <= 0)
            {
                // Return to patrol
                ChangeState(AlertState.Patrol);
            }
        }
    }
    
    private void AlertBehavior()
    {
        // Chase player or investigate last known position
        lastKnownPosition = player.position;
        
        // Move towards player
        Vector3 direction = (lastKnownPosition - transform.position).normalized;
        transform.position += direction * investigationSpeed * Time.deltaTime;
        
        // Look at player
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        
        // Keep checking if we can still see player
        if (!CheckVisualDetection())
        {
            // Lost sight, start searching
            ChangeState(AlertState.Searching);
            StartCoroutine(SearchRoutine());
        }
    }
    
    private void SearchingBehavior()
    {
        // Search around last known position
        // This is handled by SearchRoutine coroutine
    }
    
    private IEnumerator SearchRoutine()
    {
        float searchTime = 0f;
        Vector3 searchCenter = lastKnownPosition;
        
        while (searchTime < alertDuration && currentState == AlertState.Searching)
        {
            // Move to random positions around last known location
            Vector3 randomOffset = Random.insideUnitSphere * searchRadius;
            randomOffset.y = 0;
            Vector3 searchPoint = searchCenter + randomOffset;
            
            // Move towards search point
            float moveTime = 0f;
            while (moveTime < 2f && currentState == AlertState.Searching)
            {
                Vector3 direction = (searchPoint - transform.position).normalized;
                transform.position += direction * investigationSpeed * Time.deltaTime;
                
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
                
                // Check if we found the player again
                if (CheckVisualDetection())
                {
                    ChangeState(AlertState.Alert);
                    yield break;
                }
                
                moveTime += Time.deltaTime;
                yield return null;
            }
            
            // Look around
            yield return new WaitForSeconds(1f);
            
            searchTime += 3f;
        }
        
        // Give up search, return to patrol
        currentDetection = 0f;
        ChangeState(AlertState.Patrol);
    }
    
    private bool CheckVisualDetection()
    {
        if (playerStealth != null && playerStealth.isHidden)
        {
            return false; // Player is hidden
        }
        
        float distanceToPlayer = Vector3.Distance(headTransform.position, player.position);
        
        if (distanceToPlayer <= visionRange)
        {
            Vector3 directionToPlayer = (player.position - headTransform.position).normalized;
            float angleToPlayer = Vector3.Angle(headTransform.forward, directionToPlayer);
            
            if (angleToPlayer <= visionAngle)
            {
                // Check line of sight
                RaycastHit hit;
                if (Physics.Raycast(headTransform.position, directionToPlayer, out hit, distanceToPlayer, ~obstacleMask))
                {
                    if (hit.transform == player)
                    {
                        return true; // Can see player
                    }
                }
            }
        }
        
        return false;
    }
    
    private bool CheckSoundDetection()
    {
        if (!canHearPlayer) return false;
        
        PlayerMovement_Enhanced playerMovement = player.GetComponent<PlayerMovement_Enhanced>();
        if (playerMovement == null) return false;
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        float playerNoiseRadius = playerMovement.CurrentNoiseRadius;
        
        return distanceToPlayer <= playerNoiseRadius;
    }
    
    private void DetectPlayer()
    {
        if (!isPlayerDetected)
        {
            isPlayerDetected = true;
            ChangeState(AlertState.Alert);
            
            Debug.Log("PLAYER DETECTED BY " + gameObject.name);
            
            // Notify GameManager or UIManager
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowGameOver();
            }
            
            // Don't freeze time immediately, give player a chance
            StartCoroutine(GameOverDelay());
        }
    }
    
    private IEnumerator GameOverDelay()
    {
        yield return new WaitForSeconds(2f);
        Time.timeScale = 0f;
    }
    
    private void ChangeState(AlertState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
            
            switch (newState)
            {
                case AlertState.Patrol:
                    if (patrolScript != null)
                    {
                        patrolScript.enabled = true;
                    }
                    if (detectionIndicator != null)
                    {
                        detectionIndicator.SetActive(false);
                    }
                    break;
                    
                case AlertState.Suspicious:
                    if (detectionIndicator != null)
                    {
                        detectionIndicator.SetActive(true);
                    }
                    break;
                    
                case AlertState.Alert:
                    if (patrolScript != null)
                    {
                        patrolScript.enabled = false;
                    }
                    break;
            }
        }
    }
    
    private void UpdateVisualFeedback()
    {
        // Update enemy color based on state
        if (enemyRenderer != null)
        {
            Color targetColor = normalColor;
            
            switch (currentState)
            {
                case AlertState.Patrol:
                    targetColor = normalColor;
                    break;
                case AlertState.Suspicious:
                    targetColor = suspiciousColor;
                    break;
                case AlertState.Alert:
                case AlertState.Searching:
                    targetColor = alertColor;
                    break;
            }
            
            enemyRenderer.material.color = Color.Lerp(enemyRenderer.material.color, targetColor, Time.deltaTime * 3f);
        }
        
        // Update detection indicator if exists
        if (detectionIndicator != null && detectionIndicator.activeSelf)
        {
            // Scale indicator based on detection percentage
            float scale = DetectionPercentage / 100f;
            detectionIndicator.transform.localScale = Vector3.one * scale;
        }
    }
    
    void OnDrawGizmos()
    {
        if (headTransform == null) headTransform = transform;
        
        // Vision range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(headTransform.position, visionRange);
        
        // Vision cone
        Vector3 leftBoundary = Quaternion.Euler(0, -visionAngle, 0) * headTransform.forward * visionRange;
        Vector3 rightBoundary = Quaternion.Euler(0, visionAngle, 0) * headTransform.forward * visionRange;
        
        Gizmos.color = GetStateColor();
        Gizmos.DrawLine(headTransform.position, headTransform.position + leftBoundary);
        Gizmos.DrawLine(headTransform.position, headTransform.position + rightBoundary);
        Gizmos.DrawLine(headTransform.position, headTransform.position + headTransform.forward * visionRange);
        
        // Hearing range
        if (canHearPlayer)
        {
            Gizmos.color = new Color(0f, 1f, 1f, 0.2f);
            Gizmos.DrawWireSphere(transform.position, maxHearingDistance);
        }
    }
    
    private Color GetStateColor()
    {
        switch (currentState)
        {
            case AlertState.Patrol: return Color.green;
            case AlertState.Suspicious: return Color.yellow;
            case AlertState.Alert: return Color.red;
            case AlertState.Searching: return Color.magenta;
            default: return Color.white;
        }
    }
    
    void OnDestroy()
    {
        Time.timeScale = 1f; // Reset time scale
    }
}