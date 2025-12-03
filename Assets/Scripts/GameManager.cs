using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Game Stats")]
    public int collectedItems = 0;
    public int totalItemsInLevel = 3;
    public int currentLevel = 1;
    public int totalLevels = 3;
    
    void Start()
    {
        // Determine current level from scene name
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "Level1_Jungle") currentLevel = 1;
        else if (sceneName == "Level2_Tunnel") currentLevel = 2;
        else if (sceneName == "Level3_Escape") currentLevel = 3;
        
        Debug.Log("GameManager started! Current Level: " + currentLevel);
        
        // Update UI
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateCollectibles(collectedItems, totalItemsInLevel);
            UIManager.Instance.UpdateLevel(currentLevel);
        }
    }
    
    void Update()
    {
        // Press R to restart
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            RestartGame();
        }
    }
    
    public void CollectItem()
    {
        collectedItems++;
        Debug.Log("Collected: " + collectedItems + "/" + totalItemsInLevel);
        
        // Update UI
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateCollectibles(collectedItems, totalItemsInLevel);
            
            if (collectedItems >= totalItemsInLevel)
            {
                UIManager.Instance.UpdateStatus("All intel collected! Find the exit!");
            }
        }
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("Restarting game...");
    }
    
    public void CompleteLevel()
    {
        Debug.Log("Level " + currentLevel + " Complete!");
        
        // Check if this is the final level
        if (currentLevel >= totalLevels)
        {
            Debug.Log("MISSION COMPLETE! YOU WON!");
            
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowVictory();
            }
            
            Time.timeScale = 0f;
        }
        else
        {
            // Load next level
            LoadNextLevel();
        }
    }
    
    void LoadNextLevel()
    {
        Time.timeScale = 1f;
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        
        // Check if next scene exists in build settings
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log("Loading next level...");
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogError("No next level found in build settings!");
        }
    }
}