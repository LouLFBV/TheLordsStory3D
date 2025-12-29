using UnityEngine;

public class CraftingTable : InteractableBase
{
    [Header("References")]
    [SerializeField] private AllRecipeData allRecipeData;
    [SerializeField] private CraftingSystem craftingSystem;

    [Header("Others")]
    [SerializeField] private GameObject craftPanel;
    public bool isCrafting, isCooking;

    public override void OnInteract(PlayerInteractor player)
    {
        if (craftPanel != null && !craftPanel.activeInHierarchy)
        {
            RefreshDisplay();
            craftPanel.SetActive(true);
            craftingSystem.textIsRecipeListEmpty.SetActive(craftingSystem.availableRecipes.Count == 0);
        }
    }

    private void RefreshDisplay()
    {
        if (isCooking)
        {
            craftingSystem.availableRecipes = allRecipeData.recetteDeLObjectCooking;
        }
        else if (isCrafting)
        {
            craftingSystem.availableRecipes = allRecipeData.recetteDeLObjectCrafting;
        }
        craftingSystem.UpdateDisplayRecipes();
    }
}
