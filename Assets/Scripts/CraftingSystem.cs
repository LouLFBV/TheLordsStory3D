using UnityEngine;
using System.Collections.Generic;

public class CraftingSystem : MonoBehaviour
{
    public List<RecipeData> availableRecipes;

    [SerializeField]
    private GameObject recipeUiPrefab;

    [SerializeField]
    private Transform recipesParent;

    public GameObject craftPanel;
    public GameObject textIsRecipeListEmpty;
    void Start()
    {
        UpdateDisplayRecipes();
    }
    public void UpdateDisplayRecipes()
    {
        foreach (Transform child in recipesParent)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < availableRecipes.Count; i++)
        {
            GameObject currentRecipe = Instantiate(recipeUiPrefab, recipesParent);
            currentRecipe.GetComponent<Recipe>().Configure(availableRecipes[i]);
        }
    }
}
