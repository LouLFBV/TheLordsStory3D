using UnityEngine;

public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    [SerializeField] protected InteractableIconUI interactUI;

    public void SetTargeted(bool targeted, Transform player)
    {
        if (targeted)
        {
            // On initialise la position de l'icône avant de l'afficher
            interactUI.Initialize(transform, player);
            interactUI.Show();
        }
        else
        {
            interactUI.Hide();
        }
    }


    public abstract void OnInteract(PlayerInteractor player);
}
