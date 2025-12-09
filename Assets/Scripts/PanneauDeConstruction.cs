using System.Collections.Generic;
using UnityEngine;

public class PanneauDeConstruction : InteractableBase
{
    public CraftingSystem craftingSystem;

    [Header("Others")]
    [SerializeField] private GameObject craftPanel;

    [SerializeField] private RecipeData recetteDeLObject;
    private UIManager uiManager;

    public override void OnInteract(PlayerInteractor player)
    {
        OpenPanel();
    }

    public void OpenPanel()
    {
        if (uiManager == null)
        {
            uiManager = UIManager.instance;
            uiManager.AddPanel(craftPanel);
        }
        if (craftPanel != null && !craftPanel.activeInHierarchy)
        {
            craftingSystem.availableRecipes = new List<RecipeData> { recetteDeLObject };
            craftingSystem.UpdateDisplayRecipes();
            craftingSystem.textIsRecipeListEmpty.SetActive(false);
            craftPanel.SetActive(true);
            SetTargeted(false);
            if (craftingSystem.uiNavigationManager != null)
            {
                craftingSystem.uiNavigationManager.onCancel = craftingSystem.ClosePanel;
            }
        }
    }
}
