using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;
    
    private Transform currentTarget;
    
    void Start()
    {
        currentTarget = pointB;
    }
    
    void Update()
    {
        if (pointA == null || pointB == null)
        {
            Debug.LogWarning("EnemyPatrol: Patrol points not assigned!");
            return;
        }
        
        // Calculate direction to target
        Vector3 direction = (currentTarget.position - transform.position).normalized;
        
        // Rotate enemy to face the direction
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
        }
        
        // Move toward target
        transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);
        
        // Switch target when reached
        if (Vector3.Distance(transform.position, currentTarget.position) < 0.1f)
        {
            currentTarget = (currentTarget == pointA) ? pointB : pointA;
        }
    }
}