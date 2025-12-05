using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Vector3 pointA;
    [SerializeField] private Vector3 pointB;
    [SerializeField] private float speed = 2f;
    
    private Vector3 targetPosition;
    
    void Start()
    {
        pointA = transform.position;
        pointB = transform.position + new Vector3(0, 4, 0); // Moves up 4 units
        targetPosition = pointB;
    }
    
    void Update()
    {
        // Move platform
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        
        // Switch direction when reached
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            targetPosition = targetPosition == pointA ? pointB : pointA;
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }
    
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}