using UnityEngine;

public class Coin : MonoBehaviour
{
    public int goldAmount = 1;
    
    private PlayerStats playerStats;

    private void Start()
    {
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
    }

    public void Collect()
    {
        if (playerStats != null)
        {
            playerStats.AddGold(goldAmount);
            Destroy(gameObject);
        }
    }
}
