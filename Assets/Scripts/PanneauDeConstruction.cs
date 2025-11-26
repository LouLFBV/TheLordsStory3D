using System.Collections.Generic;
using UnityEngine;

public class PanneauDeConstruction : MonoBehaviour
{
    public CraftingSystem craftingSystem;

    [Header("Others")]
    [SerializeField] private GameObject craftPanel;

    [SerializeField] private RecipeData recetteDeLObject;
    private UIManager uiManager;

    public void OpenPanel()
    {
        if (uiManager == null)
        {
            uiManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<UIManager>();
            uiManager.AddPanel(craftPanel);
        }
        if (craftPanel != null && !craftPanel.activeInHierarchy)
        {
            craftingSystem.availableRecipes = new List<RecipeData> { recetteDeLObject };
            craftingSystem.UpdateDisplayRecipes();
            craftingSystem.textIsRecipeListEmpty.SetActive(false);
            craftPanel.SetActive(true);
            if (craftingSystem.uiNavigationManager != null)
            {
                craftingSystem.uiNavigationManager.onCancel = craftingSystem.ClosePanel;
            }
        }
    }
}
