using UnityEngine;

public class Collectible : MonoBehaviour
{
    public int pointValue = 1;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Intel touched by player!");
            
            // Find GameManager by name
            GameObject gameManagerObject = GameObject.Find("GameManager");
            
            if (gameManagerObject != null)
            {
                GameManager gameManager = gameManagerObject.GetComponent<GameManager>();
                
                if (gameManager != null)
                {
                    gameManager.CollectItem();
                    Debug.Log("Item collected successfully!");
                }
                else
                {
                    Debug.LogError("GameManager component not found on GameManager object!");
                }
            }
            else
            {
                Debug.LogError("GameManager object not found in scene!");
            }
            
            // Destroy this collectible
            Destroy(gameObject);
        }
    }
    
    void Update()
    {
        // Rotate for visual appeal
        transform.Rotate(Vector3.up * 50f * Time.deltaTime);
    }
}