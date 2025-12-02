using UnityEngine;

public class LevelExit : MonoBehaviour
{
    public int requiredCollectibles = 3;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player reached exit!");
            
            // Find GameManager
            GameObject gameManagerObject = GameObject.Find("GameManager");
            
            if (gameManagerObject != null)
            {
                GameManager gameManager = gameManagerObject.GetComponent<GameManager>();
                
                if (gameManager != null)
                {
                    // Check if player collected enough items
                    if (gameManager.collectedItems >= requiredCollectibles)
                    {
                        Debug.Log("Level Complete! All intel collected!");
                        gameManager.CompleteLevel();
                    }
                    else
                    {
                        Debug.Log("Need to collect all intel first! (" + gameManager.collectedItems + "/" + requiredCollectibles + ")");
                    }
                }
            }
            else
            {
                Debug.LogError("GameManager not found!");
            }
        }
    }
}
