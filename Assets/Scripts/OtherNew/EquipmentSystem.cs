using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSystem : MonoBehaviour
{
    #region Champs
    public static EquipmentSystem instance;

    [Header("Other Scripts References")]

    [SerializeField] private NewItemActionsSystem itemActionsSystem;

    [SerializeField] private PlayerController player;

    [SerializeField] private PaletteSystem palette;

    [Header("Equipment Panel References")]

    [SerializeField] private EquipmentLibrary equipmentLibrary;

    [SerializeField] private Image headSlotImage, chestSlotImage, handsSlotImage, legslotImage, feetSlotImage, arrowSlotImage;

    [HideInInspector]
    public ItemData equipmentHeadItem, equipmentChestItem, equipmentHandsItem, equipmentLegsItem, equipmentFeetItem;

    [HideInInspector]
    public ItemInInventory arrowItemInInventory;

    [SerializeField] private TextMeshProUGUI arrowText;

    [SerializeField] private Button headSlotDesequipButton, chestSlotDesequipButton, handsSlotDesequipButton, legsSlotDesequipButton, feetSlotDesequipButton, arrowSlotDesequipButton;

    [HideInInspector] public AudioSource audioSource;

    [HideInInspector] public AudioClip equipSound;

    private bool isLoading = false;

    #endregion

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public bool IsEquipped(ItemData item)
    {
        return equipmentHeadItem == item ||
               equipmentChestItem == item ||
               equipmentHandsItem == item ||
               equipmentLegsItem == item ||
               equipmentFeetItem == item ||
               arrowItemInInventory.itemData == item;
    }
    private void DisablePreviousEquipedEquipment(ItemData itemToDisable)
    {
        if (itemToDisable == null)
        {
            return;
        }

        EquipmentLibraryItem equipmentLibraryItem = equipmentLibrary.content.Where(x => x.itemData == itemToDisable).First();

        if (equipmentLibraryItem != null)
        {
            for (int i = 0; i < equipmentLibraryItem.elementsToDisable.Length; i++)
            {
                equipmentLibraryItem.elementsToDisable[i].SetActive(true);
            }
            equipmentLibraryItem.itemPrefab.SetActive(false);
        }
        player.Armor.UpdateArmor(itemToDisable.armorType, itemToDisable.armorPoints, false);
        if (itemToDisable.equipmentType == EquipmentType.Arrow)
        {
            if (arrowItemInInventory.count > 0)
            {
                int count = arrowItemInInventory.count;
                arrowItemInInventory.count = 0;
                arrowItemInInventory.itemData = null;
                for (int i = 0; i < count; i++)
                {
                    InventorySystem.instance.AddItem(itemToDisable);
                }
            }
        }
        else
            InventorySystem.instance.AddItem(itemToDisable);
    }

    public void DesequipEquipment(EquipmentType equipmentType)
    {
        if (InventorySystem.instance.IsFull())
        {
            Debug.LogWarning("Cannot desequip item, inventory is full.");
            return;
        }

        ItemData currentItem = null;

        switch (equipmentType)
        {
            case EquipmentType.Head:
                currentItem = equipmentHeadItem;
                headSlotImage.sprite = InventorySystem.instance.emptySlotVisual;
                equipmentHeadItem = null;
                break;
            case EquipmentType.Chest:
                currentItem = equipmentChestItem;
                chestSlotImage.sprite = InventorySystem.instance.emptySlotVisual;
                equipmentChestItem = null;
                break;
            case EquipmentType.Hands:
                currentItem = equipmentHandsItem;
                handsSlotImage.sprite = InventorySystem.instance.emptySlotVisual;
                equipmentHandsItem = null;
                break;
            case EquipmentType.Legs:
                currentItem = equipmentLegsItem;
                legslotImage.sprite = InventorySystem.instance.emptySlotVisual;
                equipmentLegsItem = null;
                break;
            case EquipmentType.Feet:
                currentItem = equipmentFeetItem;
                feetSlotImage.sprite = InventorySystem.instance.emptySlotVisual;
                equipmentFeetItem = null;
                break;
            case EquipmentType.Arrow:
                currentItem = arrowItemInInventory.itemData;
                arrowSlotImage.sprite = InventorySystem.instance.emptySlotVisual;
                arrowItemInInventory.itemData = null;
                break;
        }

        EquipmentLibraryItem equipmentLibraryItem = equipmentLibrary.content.Where(x => x.itemData == currentItem).FirstOrDefault();

        if (equipmentLibraryItem != null)
        {
            foreach (GameObject element in equipmentLibraryItem.elementsToDisable)
            {
                element.SetActive(true);
            }
            equipmentLibraryItem.itemPrefab.SetActive(false);
        }
        if (currentItem)
        {
            if (currentItem.handWeaponType == HandWeapon.TwoHanded)
            {
                player.Animator.SetBool("IsTwoHandedWeapon", false);
            }
            player.Armor.UpdateArmor(currentItem.armorType, currentItem.armorPoints, false);
            if (currentItem.equipmentType == EquipmentType.Arrow)
            {
                if (arrowItemInInventory.count > 0)
                {
                    for (int i = 0; i < arrowItemInInventory.count; i++)
                    {
                        InventorySystem.instance.AddItem(currentItem);
                    }
                }
                UpdateArrowsText();
            }
            else
                InventorySystem.instance.AddItem(currentItem);
            UpdateEquipmentsDesequipButtons();
        }
    }

    public void UpdateArrowsText()
    {
        arrowText.gameObject.SetActive(arrowItemInInventory.itemData != null);
        arrowText.text = arrowItemInInventory.count.ToString();
    }

    public void UpdateEquipmentsDesequipButtons()
    {
        headSlotDesequipButton.onClick.RemoveAllListeners();
        headSlotDesequipButton.onClick.AddListener(delegate { DesequipEquipment(EquipmentType.Head); });
        headSlotDesequipButton.gameObject.SetActive(equipmentHeadItem);

        chestSlotDesequipButton.onClick.RemoveAllListeners();
        chestSlotDesequipButton.onClick.AddListener(delegate { DesequipEquipment(EquipmentType.Chest); });
        chestSlotDesequipButton.gameObject.SetActive(equipmentChestItem);

        handsSlotDesequipButton.onClick.RemoveAllListeners();
        handsSlotDesequipButton.onClick.AddListener(delegate { DesequipEquipment(EquipmentType.Hands); });
        handsSlotDesequipButton.gameObject.SetActive(equipmentHandsItem);

        legsSlotDesequipButton.onClick.RemoveAllListeners();
        legsSlotDesequipButton.onClick.AddListener(delegate { DesequipEquipment(EquipmentType.Legs); });
        legsSlotDesequipButton.gameObject.SetActive(equipmentLegsItem);

        feetSlotDesequipButton.onClick.RemoveAllListeners();
        feetSlotDesequipButton.onClick.AddListener(delegate { DesequipEquipment(EquipmentType.Feet); });
        feetSlotDesequipButton.gameObject.SetActive(equipmentFeetItem);

        arrowSlotDesequipButton.onClick.RemoveAllListeners();
        arrowSlotDesequipButton.onClick.AddListener(delegate { DesequipEquipment(EquipmentType.Arrow); });
        arrowSlotDesequipButton.gameObject.SetActive(arrowItemInInventory.itemData);
    }

    public void EquipAction(ItemData equipment = null)
    {

        ItemData itemToEquip = equipment ? equipment : itemActionsSystem.itemCurrentlySelected;
        print("Equip item : " + itemToEquip.name);

        EquipmentLibraryItem equipmentLibraryItem = equipmentLibrary.content.Where(x => x.itemData == itemToEquip).FirstOrDefault();

        if (equipmentLibraryItem != null)
        {
            switch (itemToEquip.equipmentType)
            {
                case EquipmentType.Head:
                    DisablePreviousEquipedEquipment(equipmentHeadItem);
                    headSlotImage.sprite = itemToEquip.visual;
                    equipmentHeadItem = itemToEquip;
                    ActiveItemVisuel(equipmentLibraryItem);
                    break;
                case EquipmentType.Chest:
                    DisablePreviousEquipedEquipment(equipmentChestItem);
                    chestSlotImage.sprite = itemToEquip.visual;
                    equipmentChestItem = itemToEquip;
                    ActiveItemVisuel(equipmentLibraryItem);
                    break;
                case EquipmentType.Hands:
                    DisablePreviousEquipedEquipment(equipmentHandsItem);
                    handsSlotImage.sprite = itemToEquip.visual;
                    equipmentHandsItem = itemToEquip;
                    ActiveItemVisuel(equipmentLibraryItem);
                    break;
                case EquipmentType.Legs:
                    DisablePreviousEquipedEquipment(equipmentLegsItem);
                    legslotImage.sprite = itemToEquip.visual;
                    equipmentLegsItem = itemToEquip;
                    ActiveItemVisuel(equipmentLibraryItem);
                    break;
                case EquipmentType.Feet:
                    DisablePreviousEquipedEquipment(equipmentFeetItem);
                    feetSlotImage.sprite = itemToEquip.visual;
                    equipmentFeetItem = itemToEquip;
                    ActiveItemVisuel(equipmentLibraryItem);
                    break;
                case EquipmentType.Weapon:
                    palette.AddWeapon(itemToEquip);
                    // 1. On l'ajoute à la palette (logique de données)
                    palette.AddWeapon(itemToEquip);

                    //// 2. Si on n'est pas en train de charger une sauvegarde, on déclenche l'animation
                    //if (!isLoading)
                    //{
                    //    // On récupère le PlayerController (via les stats ou un singleton)
                    //    PlayerController player = playerStats.GetComponent<PlayerController>();

                    //    // On lui donne l'item à équiper pour que l'état sache quoi faire
                    //    player.PendingWeaponItem = itemToEquip;

                    //    // ON FERME L'INVENTAIRE (pour voir l'animation !)
                    //    Inventory.instance.CloseInventory();

                    //    // ON CHANGE D'ÉTAT
                    //    player.StateMachine.ChangeState(PlayerStateType.Equip);
                    //}
                    break;
                case EquipmentType.Arrow:
                    DisablePreviousEquipedEquipment(arrowItemInInventory.itemData);
                    arrowSlotImage.sprite = itemToEquip.visual;
                    arrowItemInInventory.itemData = itemToEquip;
                    arrowItemInInventory.count = InventorySystem.instance.GetContent().Find(x => x.itemData == itemToEquip).count;
                    for (int i = 0; i < arrowItemInInventory.count; i++)
                    {
                        InventorySystem.instance.RemoveItem(itemToEquip);
                    }
                    UpdateArrowsText();
                    ActiveItemVisuel(equipmentLibraryItem);
                    BowBehaviour.instance.UpdateQuiverVisual(arrowItemInInventory.count);
                    break;
            }
            if (itemToEquip.itemType == ItemType.Consumable)
            {
                palette.AddObject(itemToEquip);
            }

            if (!isLoading && itemToEquip.equipmentType != EquipmentType.Arrow)
                InventorySystem.instance.RemoveItem(itemToEquip);
            if (!isLoading)
                audioSource.PlayOneShot(equipSound);
        }
        else
        {
            Debug.LogWarning("Item not found in equipment library: " + itemToEquip.name);
        }

        itemActionsSystem.CloseActionPanel();
        UpdateEquipmentsDesequipButtons();
    }

    public EquipmentSaveData GetSaveData()
    {
        return new EquipmentSaveData
        {
            headID = equipmentHeadItem ? equipmentHeadItem.itemID : null,
            chestID = equipmentChestItem ? equipmentChestItem.itemID : null,
            handsID = equipmentHandsItem ? equipmentHandsItem.itemID : null,
            legsID = equipmentLegsItem ? equipmentLegsItem.itemID : null,
            feetID = equipmentFeetItem ? equipmentFeetItem.itemID : null,

            arrowID = arrowItemInInventory.itemData ? arrowItemInInventory.itemData.itemID : null,
            arrowCount = arrowItemInInventory.count
        };
    }

    public void LoadSaveData(EquipmentSaveData data)
    {
        isLoading = true;

        // Reset interne SANS toucher l'inventaire
        equipmentHeadItem = null;
        equipmentChestItem = null;
        equipmentHandsItem = null;
        equipmentLegsItem = null;
        equipmentFeetItem = null;

        arrowItemInInventory.itemData = null;
        arrowItemInInventory.count = 0;

        headSlotImage.sprite = InventorySystem.instance.emptySlotVisual;
        chestSlotImage.sprite = InventorySystem.instance.emptySlotVisual;
        handsSlotImage.sprite = InventorySystem.instance.emptySlotVisual;
        legslotImage.sprite = InventorySystem.instance.emptySlotVisual;
        feetSlotImage.sprite = InventorySystem.instance.emptySlotVisual;
        arrowSlotImage.sprite = InventorySystem.instance.emptySlotVisual;

        if (data == null)
        {
            isLoading = false;
            return;
        }

        EquipByID(data.headID);
        EquipByID(data.chestID);
        EquipByID(data.handsID);
        EquipByID(data.legsID);
        EquipByID(data.feetID);

        if (!string.IsNullOrEmpty(data.arrowID))
        {
            ItemData arrow = ItemDataDatabase.Instance.GetItemByID(data.arrowID);
            arrowItemInInventory.itemData = arrow;
            arrowItemInInventory.count = data.arrowCount;

            arrowSlotImage.sprite = arrow.visual;
            UpdateArrowsText();
            BowBehaviour.instance.UpdateQuiverVisual(data.arrowCount);
        }

        UpdateEquipmentsDesequipButtons();
        //playerStats.UpddateArmorText();

        isLoading = false;
    }


    private void EquipByID(string id)
    {
        if (string.IsNullOrEmpty(id)) return;

        ItemData item = ItemDataDatabase.Instance.GetItemByID(id);
        if (item != null)
        {
            EquipAction(item);
        }
    }

    private void ActiveItemVisuel(EquipmentLibraryItem equipmentLibraryItem)
    {
        foreach (GameObject element in equipmentLibraryItem.elementsToDisable)
        {
            element.SetActive(false);
        }
        equipmentLibraryItem.itemPrefab.SetActive(true);
    }
}

//[System.Serializable]
//public class EquipmentSaveData
//{
//    public string headID;
//    public string chestID;
//    public string handsID;
//    public string legsID;
//    public string feetID;

//    public string arrowID;
//    public int arrowCount;
//}
