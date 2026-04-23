using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class ForgeronUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EquipmentSystem equipment;
    [SerializeField] private InventorySystem inventory;

    [Header("UI Elements")]
    [SerializeField] private GameObject forgeronUIPanel;
    [SerializeField] private List<SlotForgeronUI> slotForgeronUIs;

    [Header("Icones level")]
    [SerializeField] private Sprite iconeLevel1;
    [SerializeField] private Sprite iconeLevel2;
    [SerializeField] private Sprite iconeLevel3;

    [Header("Upgrade Panel")]
    [SerializeField] private GameObject upgradePanel;

    [SerializeField] private Image iconeLevelItem;
    [SerializeField] private TextMeshProUGUI nameItem;

    [SerializeField] private TextMeshProUGUI levelItem;
    [SerializeField] private TextMeshProUGUI levelItemUpgrade;

    [SerializeField] private TextMeshProUGUI resistanceItem;
    [SerializeField] private TextMeshProUGUI resistanceItemUpgrade;

    [SerializeField] private TextMeshProUGUI prixUpgradeItem;


    [SerializeField] private TextMeshProUGUI amountMetal;
    [SerializeField] private Image iconeItem;

    private void Start()
    {
        if (equipment == null)
            equipment = EquipmentSystem.instance;
        if (inventory == null)
            inventory = InventorySystem.instance;
    }

    public void OpenForgeonUI()
    {
        forgeronUIPanel.SetActive(true);
        upgradePanel.SetActive(false);
        UpdateForgeronUI(EquipmentType.Weapon);
    }

    public void CloseForgeronUI()
    {
        forgeronUIPanel.SetActive(false);
        upgradePanel.SetActive(false);
        PlayerController.Instance.StateMachine.ChangeState(PlayerStateType.Idle);
    }

    public void UpdateForgeronUI(EquipmentType equipmentType)
    {
        Debug.Log("Mise à jour de l'UI du forgeron pour le type d'équipement : " + equipmentType);

        List<ItemInInventory> items = inventory.GetContentEquipment();
        items = GetContentForEquipment(items, equipmentType);

        CleanForgeronUI();
        for (int i = 0; i < slotForgeronUIs.Count; i++)
        {
            var slot = slotForgeronUIs[i];
            if (i >= items.Count)
            {
                Debug.Log("Pas assez d'items pour remplir tous les slots du forgeron UI");
                slot.itemData = null;
                slot.equipmentIcone.enabled = false;
                slot.equipmentIcone.sprite = null;
            }
            else
            {
                Debug.Log("Remplissage du slot " + i + " avec l'item : " + items[i].itemData.itemName);
                slot.itemData = items[i].itemData;
                slot.equipmentIcone.enabled = true;
                slot.equipmentIcone.sprite = items[i].itemData.visual;
            }
        }
    }
    private void CleanForgeronUI()
    {
        foreach (var slot in slotForgeronUIs)
        {
            slot.itemData = null;
            slot.equipmentIcone.sprite = null;
        }
    }
    private List<ItemInInventory> GetContentForEquipment(List<ItemInInventory> items, EquipmentType equipmentType)
    {
        List<ItemInInventory> filteredItems = new List<ItemInInventory>();

        foreach (ItemInInventory item in items)
        {
            if (item.itemData.equipmentType == equipmentType)
            {
                filteredItems.Add(item);
            }
        }
        return filteredItems;
    }

    public void UpdateUpgradePanel(ItemData itemData)
    {
        if (itemData == null)
        {
            upgradePanel.SetActive(false);
            return;
        }
        upgradePanel.SetActive(true);

        switch(itemData.levelAmelioration)
        {
            case 0:
                iconeLevelItem.sprite = iconeLevel1;
                break;
            case 1:
                iconeLevelItem.sprite = iconeLevel2;
                break;
            case 2:
                iconeLevelItem.sprite = iconeLevel3;
                break;
            default:
                Debug.LogWarning("Niveau d'amélioration inattendu : " + itemData.levelAmelioration);
                break;
        }

        nameItem.text = itemData.itemName;
        iconeItem.sprite = itemData.visual;
        levelItem.text = itemData.levelAmelioration.ToString()+"/3";
        levelItemUpgrade.text = (itemData.levelAmelioration+1).ToString() + "/3";
        resistanceItem.text = itemData.armorPoints.ToString();
        resistanceItemUpgrade.text = (itemData.armorPoints+10).ToString();
        prixUpgradeItem.text = (itemData.prix * (itemData.levelAmelioration + 1)).ToString();
        amountMetal.text = itemData.metalCost.ToString();
    }

    public void UpgradeItem(ItemData itemData)
    {
        if (itemData == null)
        {
            Debug.LogWarning("Aucun item sélectionné pour l'amélioration");
            return;
        }
        if (itemData.levelAmelioration >= 3)
        {
            Debug.LogWarning("L'item est déjà au niveau maximum d'amélioration");
            return;
        }
        //if (!inventory.HasEnoughMetal(itemData.metalCost))
        //{
        //    Debug.LogWarning("Pas assez de métal pour améliorer cet item");
        //    return;
        //}
        //inventory.ConsumeMetal(itemData.metalCost);
        itemData.levelAmelioration++;
        itemData.armorPoints += 10; // Exemple d'amélioration des points d'armure
        UpdateUpgradePanel(itemData);
    }

    public void DestroyItem(ItemData itemData)
    {
        if (itemData == null)
        {
            Debug.LogWarning("Aucun item sélectionné pour la destruction");
            return;
        }
        //inventory.AddMetal(itemData.metalCost / 2); // Récupère la moitié du coût en métal
        inventory.RemoveItem(itemData);
        UpdateForgeronUI(itemData.equipmentType);
        upgradePanel.SetActive(false);
    }
}
