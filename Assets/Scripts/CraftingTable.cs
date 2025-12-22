using UnityEngine;
using System.Collections.Generic;

public class CraftingTable : CraftingTableParent
{
    [Header("References")]
    [SerializeField] private CraftingSystem craftingSystem;

    [Header("Others")]
    [SerializeField] private GameObject craftPanel;
    public bool isCrafting, isCooking;

    public override void OnInteract(PlayerInteractor player)
    {
        if (craftPanel != null && !craftPanel.activeInHierarchy)
        {
            //craftingSystem.availableRecipes = list;
            craftPanel.SetActive(true);
            craftingSystem.textIsRecipeListEmpty.SetActive(craftingSystem.availableRecipes.Count == 0);
        }
    }
}
