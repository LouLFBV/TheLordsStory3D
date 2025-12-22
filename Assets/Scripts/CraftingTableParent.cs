using System.Collections.Generic;
using UnityEngine;

public  abstract class CraftingTableParent : InteractableBase
{
    public static CraftingTableParent instance;

    public List<RecipeData> recetteDeLObjectCrafting;
    public List<RecipeData> recetteDeLObjectCooking;

    public override void OnInteract(PlayerInteractor player)
    {
    }
}
