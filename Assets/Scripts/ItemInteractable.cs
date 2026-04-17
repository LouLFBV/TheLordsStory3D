using UnityEngine;

public class ItemInteractable : InteractableBase
{
    private Item item;

    private void Awake()
    {
        item = GetComponent<Item>();
        if (item == null)
            Debug.LogError($"ItemInteractable requires an Item component! : {gameObject.name}");
    }

    public override void OnInteract(PlayerInteractor player)
    {
        //var interactBehaviour = player.GetComponent<InteractBehaviour>();
        var interactBehaviour = player.GetComponent<InteractSystem>();
        interactBehaviour.DoPickUp(item);
    }
}
