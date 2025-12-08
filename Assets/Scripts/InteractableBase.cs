using UnityEngine;

public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    [SerializeField] protected InteractableIconUI interactUI;

    public void SetTargeted(bool targeted)
    {
        if (targeted)
            interactUI.Show();
        else
            interactUI.Hide();
    }

    public abstract void OnInteract(PlayerInteractor player);
}
