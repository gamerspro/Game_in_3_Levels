
using UnityEngine;

public class StealthManager : MonoBehaviour
{
    public bool isHidden = false;
    
    private Renderer playerRenderer;
    private Material playerMaterial;
    private Color originalColor;
    
    [Range(0f, 1f)]
    [Tooltip("Alpha value when hidden (0 = fully transparent, 1 = fully opaque)")]
    public float hiddenAlpha = 0.3f;
    
    void Start()
    {
        playerRenderer = GetComponent<Renderer>();
        
        if (playerRenderer != null)
        {
            playerMaterial = playerRenderer.material;
            originalColor = playerMaterial.color;
            SetMaterialTransparent(playerMaterial);
        }
        else
        {
            Debug.LogWarning("StealthManager: No Renderer found on player!");
        }
    }
    
    public void SetHidden(bool status)
    {
        isHidden = status;
        
        if (playerMaterial != null)
        {
            if (isHidden)
            {
                Color hiddenColor = originalColor;
                hiddenColor.a = hiddenAlpha;
                playerMaterial.color = hiddenColor;
                Debug.Log("Player is now hidden");
            }
            else
            {
                playerMaterial.color = originalColor;
                Debug.Log("Player is now visible");
            }
        }
    }
    
    private void SetMaterialTransparent(Material material)
    {
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;
    }
}