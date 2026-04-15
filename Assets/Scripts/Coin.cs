using UnityEngine;

public class Coin : InteractableBase
{
    public int goldAmount = 1;

    public override void OnInteract(PlayerInteractor interactor)
    {
        // On passe par le Singleton du PlayerController
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.Wallet.AddGold(goldAmount);

            // Si tu as un système de feedback visuel/sonore plus global, 
            // c'est ici qu'on le triggerait.
            SetTargeted(false, interactor.transform); // On désélectionne la pièce avant de la détruire
            Destroy(gameObject);
        }
    }
}