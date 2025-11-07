using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class BookRecipe : MonoBehaviour
{
    [Header("References")]
    public GameObject canvas;
    [SerializeField] public TextMeshProUGUI itemNameText;
    [SerializeField] public Image itemIcon;

    public Item item;

    void Start()
    {
        item = GetComponent<Item>();
        canvas.transform.SetParent(null, false);
    }

    public void OpenCanvasRecipeBook()
    {
        canvas.SetActive(true);
        itemNameText.text = item.itemData.recipe.craftableItem.itemName;
        itemIcon.sprite = item.itemData.recipe.craftableItem.visual;
    }
}
