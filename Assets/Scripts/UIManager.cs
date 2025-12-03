
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    [Header("UI References")]
    public TextMeshProUGUI collectiblesText;
    public TextMeshProUGUI statusText;
    public GameObject gameOverPanel;
    public GameObject victoryPanel;
    public TextMeshProUGUI levelText;
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
        UpdateStatus("Collect all intel and reach the exit!");
    }
    
    public void UpdateCollectibles(int collected, int total)
    {
        if (collectiblesText != null)
        {
            collectiblesText.text = "Intel: " + collected + "/" + total;
        }
    }
    
    public void UpdateStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }
    
    public void UpdateLevel(int level)
    {
        if (levelText != null)
        {
            levelText.text = "Level " + level;
        }
    }
    
    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        UpdateStatus("DETECTED! Press R to Restart");
    }
    
    public void ShowVictory()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }
        UpdateStatus("MISSION COMPLETE!");
    }
    
    public void ShowHiddenStatus(bool isHidden)
    {
        if (isHidden)
        {
            UpdateStatus("HIDDEN - Safe from detection");
        }
        else
        {
            UpdateStatus("EXPOSED - Stay out of sight!");
        }
    }
}