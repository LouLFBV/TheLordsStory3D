using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Recipe : MonoBehaviour
{
    private RecipeData currentRecipe;

    [SerializeField]
    private Image craftableItemImage;
    public UISelectable craftableItemImageGO;

    [SerializeField]
    private GameObject elementRequiredPrefab;

    [SerializeField]
    private Transform elementRequiredParent;

    [SerializeField]
    private Button craftButton;
    public UISelectable craftButtonGO;

    [SerializeField]
    private Sprite canBuildIcon;

    [SerializeField]
    private Sprite cantBuildIcon;

    [SerializeField]
    private Color missingColor;

    [SerializeField]
    private Color availableColor;

    [HideInInspector] public CraftingSystem craftingSystem;

    public void Configure(RecipeData recipe)
    {
        currentRecipe = recipe;
        craftableItemImage.sprite = recipe.craftableItem.visual;
        craftableItemImage.transform.parent.GetComponent<Slot>().item = recipe.craftableItem;


        bool canCraft = true;


        for (int i = 0; i < recipe.requiredItems.Length; i++)
        {

            GameObject requiredItemGO = Instantiate(elementRequiredPrefab, elementRequiredParent);
            Image requiredItemImage = requiredItemGO.GetComponent<Image>();
            ItemData requiredItem = recipe.requiredItems[i].itemData;
            ElementRequired elementRequired = requiredItemGO.GetComponent<ElementRequired>();

            requiredItemGO.GetComponent<Slot>().item = requiredItem;

            ItemInInventory[] itemInInventory = Inventory.instance.GetContent().Where(item => item.itemData == requiredItem).ToArray();

            int totalRequiredItemQuantityInInventory = 0;

            for (int y = 0; y < itemInInventory.Length; y++)
            {
                totalRequiredItemQuantityInInventory += itemInInventory[y].count;
            }

            if (totalRequiredItemQuantityInInventory >= recipe.requiredItems[i].count)
            {
                requiredItemImage.color = availableColor;
            }
            else 
            {
                requiredItemImage.color = missingColor;
                canCraft = false;
            }
            elementRequired.elementImage.sprite = recipe.requiredItems[i].itemData.visual;
            elementRequired.elementCountText.text = recipe.requiredItems[i].count.ToString();
        }

        craftButton.image.sprite = canCraft ? canBuildIcon : cantBuildIcon;
        craftButton.enabled = canCraft;
        ResizeElementRequiredParent();
    }

    private void ResizeElementRequiredParent()
    {
        Canvas.ForceUpdateCanvases();
        elementRequiredParent.GetComponent<ContentSizeFitter>().enabled = false;
        elementRequiredParent.GetComponent<ContentSizeFitter>().enabled = true;
    }

    public void CraftItem()
    {
        for (int i = 0; i < currentRecipe.requiredItems.Length; i++)
        {
            for (int y = 0; y < currentRecipe.requiredItems[i].count; y++)
            {
                Inventory.instance.RemoveItem(currentRecipe.requiredItems[i].itemData);
            }
        }
        
        if (currentRecipe.craftableItem.itemType == ItemType.Constructible)
        {
            // METTRE L'OBJET A ACTIVE COMME PREMIER ENFANT 
            GameObject found = GameObject.Find(currentRecipe.craftableItem.ToString());
            Debug.Log("Crafting item: " + currentRecipe.craftableItem.ToString());
            if (found != null)
            {
                found.transform.GetChild(0).gameObject.SetActive(true);

                if (craftingSystem.gameObject.TryGetComponent<WorldObjectID>(out var worldID))
                {
                    WorldStateManager.Instance.RegisterActivedBuilding(worldID.UniqueID);
                    Debug.Log($"<color=green>Registered collected object: {worldID.UniqueID}</color");
                }

                craftingSystem.craftPanel.SetActive(false);
            }
            else
            {
                Debug.LogWarning("Objet non trouvé dans la scène !");
            }
        }
        else if (currentRecipe.craftableItem.itemType == ItemType.Destructible)
        {
            GameObject found = GameObject.Find(currentRecipe.craftableItem.ToString());
            if (found != null)
            {
                found.transform.GetChild(0).gameObject.SetActive(false);

                if (craftingSystem.gameObject.TryGetComponent<WorldObjectID>(out var worldID))
                {
                    WorldStateManager.Instance.RegisterCollectedObject(worldID.UniqueID);
                    Debug.Log($"<color=green>Registered collected object: { worldID.UniqueID}</color");
                }

                craftingSystem.craftPanel.SetActive(false);
            }
            else
            {
                Debug.LogWarning("Objet non trouvé dans la scène !");
            }
        }
        else
        {
            Inventory.instance.AddItem(currentRecipe.craftableItem);
            QuestManager.instance.UpdateQuestProgress("", 1, currentRecipe.craftableItem);
        }
    }


    private void Update()
    {
        HorizontalLayoutGroup layoutGroup = transform.GetComponent<HorizontalLayoutGroup>();
        if (layoutGroup != null)
        {
            layoutGroup.enabled = false;
            layoutGroup.enabled = true;
        }
    }
}
