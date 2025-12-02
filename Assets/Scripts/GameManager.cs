
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Game Stats")]
    public int collectedItems = 0;
    public int totalItemsInLevel = 3;
    public int currentLevel = 1;
    
    void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "Level1") currentLevel = 1;
        else if (sceneName == "Level2") currentLevel = 2;
        else if (sceneName == "Level3") currentLevel = 3;
        
        Debug.Log("GameManager started! Current Level: " + currentLevel);
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
        
        if (collectedItems >= totalItemsInLevel)
        {
            Debug.Log("All intel collected! Find the exit!");
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
        
        if (currentLevel >= 3)
        {
            Debug.Log("MISSION COMPLETE! YOU WON THE WAR!");
            Time.timeScale = 0f;
        }
        else
        {
            LoadNextLevel();
        }
    }
    
    void LoadNextLevel()
    {
        Time.timeScale = 1f;
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
            Debug.Log("Loading next level...");
        }
    }
}