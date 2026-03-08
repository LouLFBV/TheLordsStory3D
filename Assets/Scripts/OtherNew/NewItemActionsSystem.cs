using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewItemActionsSystem : MonoBehaviour
{

    [Header("Other Scripts References")]

    [SerializeField] private EquipmentSystem equipment;
    [SerializeField] private PlayerController player;

    [SerializeField] private PaletteSystem palette;

    [Header("Action Panel References")]

    public GameObject actionPanel;

    [SerializeField] private Button useItemButton;

    [SerializeField] private Button equipmentItemButton;

    [SerializeField] private Button dropItemButton;

    [SerializeField] private Button destroyItemButton;

    [SerializeField] private Button desequipmentItemButton;

    [SerializeField] private Transform dropPoint;

    [HideInInspector] public ItemData itemCurrentlySelected;

    [Header("item Description")]
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;

    [SerializeField] private TextMeshProUGUI itemDegatsText;
    [SerializeField] private TextMeshProUGUI itemEffetText;
    [SerializeField] private TextMeshProUGUI itemTypeDeResistanceText;


    public void OpenActionPanel(ItemData item, bool isEquipped)
    {
        itemCurrentlySelected = item;

        if (item == null)
        {
            actionPanel.SetActive(false);
            return;
        }

        itemDegatsText.gameObject.SetActive(false);
        itemEffetText.gameObject.SetActive(false);
        itemTypeDeResistanceText.gameObject.SetActive(false);

        if (!isEquipped)
        {
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
                case ItemType.Craft:
                case ItemType.Key:
                    useItemButton.gameObject.SetActive(false);
                    equipmentItemButton.gameObject.SetActive(false);
                    dropItemButton.gameObject.SetActive(true);
                    destroyItemButton.gameObject.SetActive(true);
                    break;
            }
            desequipmentItemButton.gameObject.SetActive(false);
        }
        else
        {
            useItemButton.gameObject.SetActive(false);
            equipmentItemButton.gameObject.SetActive(false);
            dropItemButton.gameObject.SetActive(false);
            destroyItemButton.gameObject.SetActive(false);
            desequipmentItemButton.gameObject.SetActive(true);
        }
        //actionPanel.transform.position = slotPosition;
        itemNameText.text = item.itemName;
        itemDescriptionText.text = item.description;

        if (item.attackPoints > 0)
        {
            itemDegatsText.text = $"Degats : {item.attackPoints}";
            itemDegatsText.gameObject.SetActive(true);

            itemEffetText.text = $"Effet : {item.damageType}";
            itemEffetText.gameObject.SetActive(true);
        }
        if (item.armorPoints > 0)
        {

            itemDegatsText.text = $"Pourcentage de reduction : {item.attackPoints}";
            itemDegatsText.gameObject.SetActive(true);
            itemTypeDeResistanceText.text = $"Type de Resistance : {item.armorType}";
            itemTypeDeResistanceText.gameObject.SetActive(true);
            itemEffetText.gameObject.SetActive(false);
        }
        if (item.healthEffect > 0)
        {
            itemDegatsText.text = $"Capacité de soin : +{item.healthEffect} PV";
            itemDegatsText.gameObject.SetActive(true);
            itemEffetText.gameObject.SetActive(false);
            itemTypeDeResistanceText.gameObject.SetActive(false);
        }
        actionPanel.SetActive(true);
    }

    public void CloseActionPanel()
    {
        actionPanel.SetActive(false);
        itemCurrentlySelected = null;
    }

    public void UseActionButton()
    {
        Debug.Log("Using item: " + itemCurrentlySelected.itemName);
        player.Health.Heal(itemCurrentlySelected.healthEffect);
        InventorySystem.instance.RemoveItem(itemCurrentlySelected);
        CloseActionPanel();
    }

    public void EquipActionButton()
    {
        Debug.Log("Equiping item: " + itemCurrentlySelected.itemName);
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
        InventorySystem.instance.RemoveItem(itemCurrentlySelected);
        CloseActionPanel();
        InventorySystem.instance.RefreshContent();
    }

    public void DesequipActionButton()
    {
        Debug.Log("Desequiping item: " + itemCurrentlySelected.itemName);
        if (itemCurrentlySelected.equipmentType == EquipmentType.Weapon)
        {
            if (palette.weapon1Slot.slotItemData == itemCurrentlySelected)            
                palette.DesequipWeapon(1);
            else
                palette.DesequipWeapon(2);
            
        }
        else if (itemCurrentlySelected.itemType == ItemType.Consumable)
        {
           if (palette.object1Slot.slotItemData == itemCurrentlySelected)
                palette.DesequipObject(1);
           else 
                palette.DesequipObject(2);
        }
        else
            equipment.DesequipEquipment(itemCurrentlySelected.equipmentType);

        CloseActionPanel();
    }
}