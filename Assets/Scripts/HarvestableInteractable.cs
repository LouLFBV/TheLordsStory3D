using UnityEngine;

public class HarvestableInteractable : InteractableBase
{
    private Harvestable harvestable;

    private void Awake()
    {
        harvestable = GetComponent<Harvestable>();
    }


    public override void OnInteract(PlayerInteractor player)
    {
        Debug.Log("Interacting with Harvestable");
        var interactBehaviour = player.GetComponent<InteractBehaviour>();

        if (interactBehaviour == null || harvestable == null)
        {

            Debug.Log("Cannot harvest: missing InteractBehaviour or Harvestable component.");
            return;
        }

        // Vérification outil
        if ((harvestable.tool == Tool.Axe && !interactBehaviour.canAxe) ||
            (harvestable.tool == Tool.Pickaxe && !interactBehaviour.canPickaxe))
        {
            Debug.Log("Cannot harvest: incorrect tool.");
            return;
        }

        Debug.Log("Harvesting...");
        interactBehaviour.DoHarvest(harvestable);
    }
}
