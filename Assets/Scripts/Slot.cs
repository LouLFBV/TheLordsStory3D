using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ItemData item;
    public Image itemVisual;
    public Text countTexte;

    [SerializeField]
    private ItemActionsSystem itemActionsSystem;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
        TooltipSystem.instance.Show(item.description, item.itemName);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipSystem.instance.Hide();
    }

    public void ClickOnSlot()
    {
        itemActionsSystem.OpenActionPanel(item, transform.position - new Vector3(0, 15, 0));
    }
}
