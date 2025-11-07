using System.Collections.Generic;
using UnityEngine;

public class CraftingTableParent : MonoBehaviour
{
    public static CraftingTableParent instance;

    public List<RecipeData> recetteDeLObjectCrafting;
    public List<RecipeData> recetteDeLObjectCooking;
}
