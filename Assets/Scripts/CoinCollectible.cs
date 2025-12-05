using UnityEngine;

public class CoinCollectible : MonoBehaviour
{
    public int pointValue = 1;
   
    void Update()
    {
        // Rotate for visual appeal
        transform.Rotate(Vector3.up * 100f * Time.deltaTime);
    }
   
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Coin collected!");
           
            // Find GameManager
            GameManager gm = FindObjectOfType<GameManager>();
            if (gm != null)
            {
                gm.CollectItem();
            }
           
            Destroy(gameObject);
        }
    }
}