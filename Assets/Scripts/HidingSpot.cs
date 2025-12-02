
using UnityEngine;

public class HidingSpot : MonoBehaviour
{
    [Tooltip("Tag to identify the player (default: 'Player')")]
    public string playerTag = "Player";
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            StealthManager stealthManager = other.GetComponent<StealthManager>();
            
            if (stealthManager != null)
            {
                stealthManager.SetHidden(true);
            }
            else
            {
                Debug.LogWarning("HidingSpot: Player does not have a StealthManager component!");
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            StealthManager stealthManager = other.GetComponent<StealthManager>();
            
            if (stealthManager != null)
            {
                stealthManager.SetHidden(false);
            }
            else
            {
                Debug.LogWarning("HidingSpot: Player does not have a StealthManager component!");
            }
        }
    }
}