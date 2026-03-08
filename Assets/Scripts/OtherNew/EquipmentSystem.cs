using System.Linq;
using UnityEngine;

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

    public Slot headSlot, chestSlot, handsSlot, legsSlot, feetSlot, arrowSlot;


    [HideInInspector]
    public ItemInInventory arrowItemInInventory;


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
        return headSlot.item == item ||
               chestSlot.item == item ||
               handsSlot.item == item ||
               legsSlot.item == item ||
               feetSlot.item == item ||
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
        if (InventorySystem.instance.IsFullEquipment())
        {
            Debug.LogWarning("Cannot desequip item, inventory is full.");
            return;
        }

        ItemData currentItem = null;

        switch (equipmentType)
        {
            case EquipmentType.Head:
                currentItem = headSlot.item;
                headSlot.itemVisual.sprite = InventorySystem.instance.emptySlotVisual;
                headSlot.item = null;
                break;
            case EquipmentType.Chest:
                currentItem = chestSlot.item;
                chestSlot.itemVisual.sprite = InventorySystem.instance.emptySlotVisual;
                chestSlot.item = null;
                break;
            case EquipmentType.Hands:
                currentItem = handsSlot.item;
                handsSlot.itemVisual.sprite = InventorySystem.instance.emptySlotVisual;
                handsSlot.item = null;
                break;
            case EquipmentType.Legs:
                currentItem = legsSlot.item;
                legsSlot.itemVisual.sprite = InventorySystem.instance.emptySlotVisual;
                legsSlot.item = null;
                break;
            case EquipmentType.Feet:
                currentItem = feetSlot.item;
                feetSlot.itemVisual.sprite = InventorySystem.instance.emptySlotVisual;
                feetSlot.item = null;
                break;
            case EquipmentType.Arrow:
                currentItem = arrowItemInInventory.itemData;
                arrowSlot.itemVisual.sprite = InventorySystem.instance.emptySlotVisual;
                arrowItemInInventory.itemData = null;
                break;
        }

        EquipmentLibraryItem equipmentLibraryItem = equipmentLibrary.Get(currentItem);

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
            //UpdateEquipmentsDesequipButtons();
        }
    }

    public void UpdateArrowsText()
    {
        arrowSlot.countTexte.gameObject.SetActive(arrowItemInInventory.itemData != null);
        arrowSlot.countTexte.text = arrowItemInInventory.count.ToString();
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
                    DisablePreviousEquipedEquipment(headSlot.item);
                    headSlot.itemVisual.sprite = itemToEquip.visual;
                    headSlot.item = itemToEquip;
                    headSlot.item = itemToEquip;
                    ActiveItemVisuel(equipmentLibraryItem);
                    break;
                case EquipmentType.Chest:
                    DisablePreviousEquipedEquipment(chestSlot.item);
                    chestSlot.itemVisual.sprite = itemToEquip.visual;
                    chestSlot.item = itemToEquip;
                    ActiveItemVisuel(equipmentLibraryItem);
                    break;
                case EquipmentType.Hands:
                    DisablePreviousEquipedEquipment(handsSlot.item);
                    handsSlot.itemVisual.sprite = itemToEquip.visual;
                    handsSlot.item = itemToEquip;
                    ActiveItemVisuel(equipmentLibraryItem);
                    break;
                case EquipmentType.Legs:
                    DisablePreviousEquipedEquipment(legsSlot.item);
                    legsSlot.itemVisual.sprite = itemToEquip.visual;
                    legsSlot.item = itemToEquip;
                    ActiveItemVisuel(equipmentLibraryItem);
                    break;
                case EquipmentType.Feet:
                    DisablePreviousEquipedEquipment(feetSlot.item);
                    feetSlot.itemVisual.sprite = itemToEquip.visual;
                    feetSlot.item = itemToEquip;
                    ActiveItemVisuel(equipmentLibraryItem);
                    break;
                case EquipmentType.Weapon:
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
                    arrowSlot.itemVisual.sprite = itemToEquip.visual;
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
        //UpdateEquipmentsDesequipButtons();
    }

    public EquipmentSaveData GetSaveData()
    {
        return new EquipmentSaveData
        {
            headID = headSlot.item ? headSlot.item.itemID : null,
            chestID = chestSlot.item ? chestSlot.item.itemID : null,
            handsID = handsSlot.item ? handsSlot.item.itemID : null,
            legsID = legsSlot.item ? legsSlot.item.itemID : null,
            feetID = feetSlot.item ? feetSlot.item.itemID : null,

            arrowID = arrowItemInInventory.itemData ? arrowItemInInventory.itemData.itemID : null,
            arrowCount = arrowItemInInventory.count
        };
    }

    public void LoadSaveData(EquipmentSaveData data)
    {
        isLoading = true;

        // Reset interne SANS toucher l'inventaire
        headSlot.item = null;
        chestSlot.item = null;
        handsSlot.item = null;
        legsSlot.item = null;
        feetSlot.item = null;

        arrowItemInInventory.itemData = null;
        arrowItemInInventory.count = 0;

        headSlot.itemVisual.sprite = InventorySystem.instance.emptySlotVisual;
        chestSlot.itemVisual.sprite = InventorySystem.instance.emptySlotVisual;
        handsSlot.itemVisual.sprite = InventorySystem.instance.emptySlotVisual;
        legsSlot.itemVisual.sprite = InventorySystem.instance.emptySlotVisual;
        feetSlot.itemVisual.sprite = InventorySystem.instance.emptySlotVisual;
        arrowSlot.itemVisual.sprite = InventorySystem.instance.emptySlotVisual;

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

            arrowSlot.itemVisual.sprite = arrow.visual;
            UpdateArrowsText();
            BowBehaviour.instance.UpdateQuiverVisual(data.arrowCount);
        }

        //UpdateEquipmentsDesequipButtons();

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
