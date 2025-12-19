using UnityEngine;

public class Coin : InteractableBase
{
    public int goldAmount = 1;

    public void Collect(PlayerStats playerStats)
    {
        if (playerStats != null)
        {
            playerStats.AddGold(goldAmount);
            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("PlayerStats component not found on the player.");
        }
    }

    public override void OnInteract(PlayerInteractor player)
    {
        Collect(player.PlayerTransform.GetComponent<PlayerStats>());
    }
}
