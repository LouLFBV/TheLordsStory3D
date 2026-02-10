using UnityEngine;
using UnityEngine.UI;

public class ItemActionsSystem : MonoBehaviour
{

    [Header("Other Scripts References")]

    [SerializeField]
    private Equipment equipment;

    [SerializeField]
    private PlayerStats playerStats;

    [SerializeField] Palette palette;

    [Header("Action Panel References")]

    public GameObject actionPanel;

    [SerializeField]
    private Button useItemButton;

    [SerializeField]
    private Button equipmentItemButton;

    [SerializeField]
    private Button dropItemButton;

    [SerializeField]
    private Button destroyItemButton;

    [SerializeField]
    private Transform dropPoint;

    [HideInInspector]
    public ItemData itemCurrentlySelected;

    public void OpenActionPanel(ItemData item, Vector3 slotPosition)
    {
        itemCurrentlySelected = item;
        if (item == null)
        {
            actionPanel.SetActive(false);
            return;
        }
        switch (item.itemType)
        {
            case ItemType.Consumable:
                useItemButton.gameObject.SetActive(true);
                equipmentItemButton.gameObject.SetActive(!palette.ObjectsAreFull(item));
                dropItemButton.gameObject.SetActive(true);
                destroyItemButton.gameObject.SetActive(true);
                break;
            case ItemType.Equipment:
                useItemButton.gameObject.SetActive(false);
                if (item.equipmentType != EquipmentType.Weapon)
                {
                    equipmentItemButton.gameObject.SetActive(true);
                }
                else
                {
                    equipmentItemButton.gameObject.SetActive(!palette.WeaponsAreFull());
                }
                dropItemButton.gameObject.SetActive(true);
                destroyItemButton.gameObject.SetActive(true);
                break;
            case ItemType.QuestItem:
                useItemButton.gameObject.SetActive(false);
                equipmentItemButton.gameObject.SetActive(false);
                dropItemButton.gameObject.SetActive(false);
                destroyItemButton.gameObject.SetActive(false);
                break;
            case ItemType.Ressource:
                useItemButton.gameObject.SetActive(false);
                equipmentItemButton.gameObject.SetActive(false);
                dropItemButton.gameObject.SetActive(true);
                destroyItemButton.gameObject.SetActive(true);
                break;
            case ItemType.Key:
                useItemButton.gameObject.SetActive(false);
                equipmentItemButton.gameObject.SetActive(false);
                dropItemButton.gameObject.SetActive(true);
                destroyItemButton.gameObject.SetActive(true);
                break;
        }
        actionPanel.transform.position = slotPosition;
        actionPanel.SetActive(true);
    }

    public void CloseActionPanel()
    {
        actionPanel.SetActive(false);
        itemCurrentlySelected = null;
    }

    public void UseActionButton()
    {
        playerStats.ConsumeItem(itemCurrentlySelected.healthEffect);
        Inventory.instance.RemoveItem(itemCurrentlySelected);
        CloseActionPanel();
    }

    public void EquipActionButton()
    {
        equipment.EquipAction();
    }

    public void DropActionButton()
    {
        GameObject instantiatedItem = Instantiate(itemCurrentlySelected.prefab);
        instantiatedItem.transform.position = dropPoint.position;
        instantiatedItem.GetComponent<Item>().enableFloating = true;
        DestroyActionButton();
    }

    public void DestroyActionButton()
    {
        Inventory.instance.RemoveItem(itemCurrentlySelected);
        CloseActionPanel();
        Inventory.instance.RefreshContent();
    }
}
