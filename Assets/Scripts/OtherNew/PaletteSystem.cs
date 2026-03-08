using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PaletteSystem : MonoBehaviour
{
    public static PaletteSystem instance;

    [Header("Other References")]

    [SerializeField] private EquipmentLibrary equipmentLibrary;

    [SerializeField] private EquipmentSystem equipmentSystem;

    [SerializeField] private NewItemActionsSystem itemActionsSystem;

    [SerializeField] private InteractSystem interactBehaviour;


    [Header("Palette Settings")]

    public ItemInInventory[] weapons = new ItemInInventory[2];

    public ItemInInventory[] objects = new ItemInInventory[2];

    [Header("Weapons And Objects Data")]
    public PaletteSlot weapon1Slot;
    public PaletteSlot weapon2Slot;
    public PaletteSlot object1Slot;
    public PaletteSlot object2Slot;

    private PaletteSlot[] weaponSlots;
    private PaletteSlot[] objectSlots;

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

        weaponSlots = new[] { weapon1Slot, weapon2Slot };
        objectSlots = new[] { object1Slot, object2Slot };
    }

    //void OnEnable()
    //{

    //    if (DeviceWatcher.Instance != null)
    //        DeviceWatcher.Instance.OnDeviceChanged += UpdateBindingDisplay;
    //}
    //void OnDisable()
    //{

    //    if (DeviceWatcher.Instance != null)
    //        DeviceWatcher.Instance.OnDeviceChanged -= UpdateBindingDisplay;
    //}



    //private void Start()
    //{
    //    UpdateEquipmentsDesequipButtons();
    //    UpdateBindingDisplay(DeviceWatcher.Instance.CurrentDevice);
    //}

    public void HandlePaletteLogic(PlayerController player)
    {
        // On ne permet pas le switch si on est déjà en train d'équiper ou si on n'est pas au sol
        if (player.StateMachine.CurrentState is not GroundedState ||
            player.StateMachine.CurrentState is EquipState)
            return;

        if (player.Input.Weapon1Pressed && weapon1Slot.slotItemData != null)
        {
            ToggleWeapon(1, player);
        }
        else if (player.Input.Weapon2Pressed && weapon2Slot.slotItemData != null)
        {
            ToggleWeapon(2, player);
        }
        else if (player.Input.Object1Pressed && object1Slot.slotItemData != null)
        {
            ToggleObject(1, player);
        }
        else if (player.Input.Object2Pressed && object2Slot.slotItemData != null)
        {
            ToggleObject(2, player);
        }
    }

    private void ToggleWeapon(int slot, PlayerController player)
    {
        bool isCurrentlyEquipped = (slot == 1) ? weapon1Slot.isEquipped : weapon2Slot.isEquipped;
        ItemData item = (slot == 1) ? weapon1Slot.slotItemData : weapon2Slot.slotItemData;

        if (!isCurrentlyEquipped)
        {
            weapon1Slot.isEquipped = slot == 1;
            weapon2Slot.isEquipped = slot == 2;

            // On passe l'info au player et on change d'état
            player.PendingWeaponType = item.handWeaponType;
            player.PrepareEquip(item);
            player.StateMachine.ChangeState(PlayerStateType.Equip);

            UseWeapon(slot, player);
        }
        else
        {
            DesequipCurrentActiveWeapon(slot, player);
        }
        UpdateImageSeleted();
    }
    private void DesequipCurrentActiveWeapon(int slot, PlayerController player)
    {
        ItemData item = (slot == 1) ? weapon1Slot.slotItemData : weapon2Slot.slotItemData;
        if (item == null) return;

        PaletteSlot slotData = weaponSlots[slot - 1];
        player.PendingUnequipType = item.handWeaponType;

        player.StateMachine.ChangeState(PlayerStateType.Unequip);
        slotData.isEquipped = false;

    }
    private void ToggleObject(int slot, PlayerController player)
    {
        bool isCurrentlyEquipped = (slot == 1) ? object1Slot.isEquipped : object2Slot.isEquipped;

        if (!isCurrentlyEquipped)
        {
            object1Slot.isEquipped = (slot == 1);
            object2Slot.isEquipped = (slot == 2);

            player.StateMachine.ChangeState(PlayerStateType.Equip);
            UseObject(slot, player);
        }
        else
        {
            // Déséquipement de l'objet
            PaletteSlot slotData = objectSlots[slot - 1];
            slotData.isEquipped = false;

            var item = (slot == 1) ? object1Slot.slotItemData : object2Slot.slotItemData;
            DisableObject(item);
            player.Animator.SetBool("CarryingConsumable", false);
        }
        UpdateImageSeleted();
    }


    private void UpdateBindingDisplay(DeviceType currentDevice)
    {
        //InputRebindManager.UpdateBindingDisplayForAction(playerInput.actions["Weapon1"], iconeInputWeapon1, currentDevice);
        //InputRebindManager.UpdateBindingDisplayForAction(playerInput.actions["Weapon2"], iconeInputWeapon2, currentDevice);
        //InputRebindManager.UpdateBindingDisplayForAction(playerInput.actions["Object1"], iconeInputObject1, currentDevice);
        //InputRebindManager.UpdateBindingDisplayForAction(playerInput.actions["Object2"], iconeInputObject2, currentDevice);
    }

    private void UseObject(int numberOfObject, PlayerController player)
    {
        weapon1Slot.isEquipped = false;
        weapon2Slot.isEquipped = false;
        player.Animator.SetBool("CarryingConsumable", true);
        player.Animator.SetBool("IsTwoHandedWeapon", false);
        player.Animator.SetBool("IsOneHandedWeapon", false);
        if (numberOfObject == 1)
        {
            EquipmentLibraryItem equipmentLibraryItem1 = equipmentLibrary.Get(object1Slot.slotItemData);
            equipmentLibraryItem1.itemPrefab.SetActive(true);

            interactBehaviour.SetCurrentEquippedItem(equipmentLibraryItem1);

            EquipmentLibraryItem equipmentLibraryItem2 = equipmentLibrary.Get(object2Slot.slotItemData);

            if (equipmentLibraryItem2 != null && equipmentLibraryItem2.itemData != objects[0].itemData) equipmentLibraryItem2.itemPrefab.SetActive(false);


            DisableWeapon(weapon1Slot.slotItemData);
            DisableWeapon(weapon2Slot.slotItemData);

        }
        else
        {
            EquipmentLibraryItem equipmentLibraryItem2 = equipmentLibrary.Get(object2Slot.slotItemData);
            equipmentLibraryItem2.itemPrefab.SetActive(true);
            interactBehaviour.SetCurrentEquippedItem(equipmentLibraryItem2);


            EquipmentLibraryItem equipmentLibraryItem1 = equipmentLibrary.Get(object1Slot.slotItemData);
            if (equipmentLibraryItem1 != null && equipmentLibraryItem1.itemData != objects[1].itemData) equipmentLibraryItem1.itemPrefab.SetActive(false);

            DisableWeapon(weapon1Slot.slotItemData);
            DisableWeapon(weapon2Slot.slotItemData);
        }
    }

    private void UseWeapon(int slot, PlayerController player)
    {
        ItemData itemToEquip = (slot == 1) ? weapon1Slot.slotItemData : weapon2Slot.slotItemData;
        player.PendingWeaponItem = itemToEquip;
        // 1. On cache les consommables
        DisableObject(object1Slot.slotItemData);
        DisableObject(object2Slot.slotItemData);
        object1Slot.isEquipped = object2Slot.isEquipped = false;
        player.Animator.SetBool("CarryingConsumable", false);


        //// 3. On prépare les data pour le script qui gère l'apparition du mesh
        //EquipmentLibraryItem libItem = equipmentLibrary.content.First(x => x.itemData == itemToEquip);
        //interactBehaviour.SetCurrentEquippedItem(libItem);
        //player.instance.equipmentToEquip = libItem;

        player.PrepareEquip(itemToEquip);
        // Note: Le mesh apparaîtra via un Animation Event ou la logique de ton EquipmentSystem

        //StartCoroutine(EquipAfterDesequip(itemToEquip.handWeaponType, 0.01f));


    }

    public void DesequipWeapon(int numberOfWeapon)
    {
        if (InventorySystem.instance.IsFullEquipment())
        {
            Debug.LogWarning("Cannot desequip item, inventory is full.");
            return;
        }

        ItemData currentItem = (numberOfWeapon == 1) ? weapon1Slot.slotItemData : weapon2Slot.slotItemData;

        //EquipmentLibraryItem lib = equipmentLibrary.Get(currentItem);

        // animations + model disable

        // remettre le slot visuellement vide
        if (numberOfWeapon == 1)
        {
            weapon1Slot.slotItemData = null;
            weapon1Slot.slotInEquipment.item = null;
            weapon1Slot.slotInEquipment.itemVisual.sprite = InventorySystem.instance.emptySlotVisual;
            weapon1Slot.SlotImage.sprite = InventorySystem.instance.emptySlotVisual;
            weapon1Slot.slotInEquipment.itemVisual.sprite = InventorySystem.instance.emptySlotVisual;

        }
        else
        {
            weapon2Slot.slotItemData = null;
            weapon2Slot.slotInEquipment.item = null;
            weapon2Slot.slotInEquipment.itemVisual.sprite = InventorySystem.instance.emptySlotVisual;
            weapon2Slot.SlotImage.sprite = InventorySystem.instance.emptySlotVisual;
            weapon2Slot.slotInEquipment.itemVisual.sprite = InventorySystem.instance.emptySlotVisual;
        }

        // remettre dans l'inventaire
        InventorySystem.instance.AddItem(currentItem);
        RemoveWeapon(numberOfWeapon);
        RefreshAffichage();
        UpdateImageSeleted();
    }


    public void DesequipObject(int numberOfObject)
    {
        if (InventorySystem.instance.IsFullEquipment())
        {
            Debug.LogWarning("Cannot desequip item, inventory is full.");
            return;
        }

        ItemData currentItem = null;
        if (numberOfObject == 1)
        {
            currentItem = object1Slot.slotItemData;
            object1Slot.SlotImage.sprite = InventorySystem.instance.emptySlotVisual;
            object1Slot.slotInEquipment.itemVisual.sprite = InventorySystem.instance.emptySlotVisual;
            object1Slot.isEquipped = false;
        }
        else
        {
            currentItem = object2Slot.slotItemData;
            object2Slot.SlotImage.sprite = InventorySystem.instance.emptySlotVisual;
            object2Slot.slotInEquipment.itemVisual.sprite = InventorySystem.instance.emptySlotVisual;
            object2Slot.isEquipped = false;
        }



        EquipmentLibraryItem equipmentLibraryItem = equipmentLibrary.Get(currentItem);
        if (equipmentLibraryItem != null)
            equipmentLibraryItem?.itemPrefab.SetActive(false);

        if (currentItem)
        {
            InventorySystem.instance.AddItem(currentItem);
        }
        RemoveObject(numberOfObject);
        RefreshAffichage();
        UpdateImageSeleted();
    }

    public void RefreshAffichage()
    {
        PaletteSlot[] allSlots =
        {
            weapon1Slot,
            weapon2Slot,
            object1Slot,
            object2Slot
        };

        foreach (var slot in allSlots)
            RefreshSlot(slot);
    }

    void RefreshSlot(PaletteSlot slot)
    {
        var item = slot.slotItemData;

        slot.SlotImage.sprite = item ? item.visual : InventorySystem.instance.emptySlotVisual;
        slot.slotInEquipment.itemVisual.sprite = slot.SlotImage.sprite;
        slot.slotInEquipment.item = item;

        if (slot.countText != null)
            slot.countText.gameObject.SetActive(item);
    }

    public void AddWeapon(ItemData item)
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i].slotItemData == null)
            {
                weaponSlots[i].slotItemData = item;

                weapons[i] = new ItemInInventory
                {
                    itemData = item,
                    count = 1
                };

                RefreshAffichage();
                return;
            }

            if (weaponSlots[i].slotItemData == item)
                return;
        }
    }

    public void AddObject(ItemData item)
    {
        // Slot 1 d'abord
        if (IsValidForSlot(0, item))
        {
            Debug.Log("Adding to slot 1");
            AddToSlot(0, item);
        }
        // Sinon slot 2
        else if (IsValidForSlot(1, item))
        {
            Debug.Log("Adding to slot 2");
            AddToSlot(1, item);
        }

        RefreshAffichage();
    }

    public bool IsValidForSlot(int slotIndex, ItemData item)
    {
        var equippedItem = slotIndex == 0 ? object1Slot.slotItemData : object2Slot.slotItemData;
        var obj = objects[slotIndex];

        // Si le slot est vide
        if (equippedItem == null) return true;

        // Si même item et pas au max
        if (equippedItem == item && obj.count < item.maxStack)
        {
            // Cas spécial : les clés doivent aussi matcher sur attackPoints
            if (item.itemType == ItemType.Key)
                return equippedItem.attackPoints == item.attackPoints;

            return true;
        }

        return false;
    }

    private void AddToSlot(int slotIndex, ItemData item)
    {
        var inventoryItem = objects[slotIndex];
        Debug.Log("Current item in slot " + slotIndex + ": " + (inventoryItem.itemData != null ? inventoryItem.itemData.name : "null"));
        PaletteSlot slotData = objectSlots[slotIndex];
        if (inventoryItem.itemData == null)
        {
            Debug.Log("Creating new ItemInInventory for slot " + slotIndex);
            objects[slotIndex] = new ItemInInventory { itemData = item, count = 1 };

            slotData.slotItemData = item;
            slotData.slotInEquipment.item = item;
            slotData.slotInEquipment.countTexte.text = "1";
        }
        else if (inventoryItem.itemData == item)
        {
            Debug.Log("Incrementing count for slot " + slotIndex);
            if (inventoryItem.count < item.maxStack)
            {
                inventoryItem.count++;
                slotData.slotInEquipment.countTexte.text = inventoryItem.count.ToString();
            }
        }

        UpdateSlotUI(slotIndex, objects[slotIndex].count);
    }

    private void UpdateSlotUI(int slotIndex, int count)
    {
        PaletteSlot slotData = objectSlots[slotIndex];
        slotData.countText.text = count.ToString();
        slotData.slotInEquipment.countTexte.text = count.ToString();
    }

    public void RemoveWeapon(int numberOfWeapon)
    {
        if (numberOfWeapon == 1)
        {
            weapon1Slot.slotItemData = null;
            weapons[0].itemData = null;
            weapons[0].count = 0;
            weapon1Slot.slotInEquipment.item = null;

        }
        else
        {
            weapon2Slot.slotItemData = null;
            weapons[1].itemData = null;
            weapons[1].count = 0;
            weapon2Slot.slotInEquipment.item = null;
        }
        RefreshAffichage();
    }

    public void RemoveObject(int numberOfObject)
    {
        if (numberOfObject == 1)
        {

            if (objects[0].count > 1)
            {
                objects[0].count--;
                object1Slot.countText.text = objects[0].count.ToString();
                object1Slot.slotInEquipment.countTexte.text = objects[0].count.ToString();
            }
            else
            {
                objects[0].itemData = null;
                objects[0].count = 0;
                object1Slot.slotItemData = null;

                object1Slot.slotInEquipment.item = null;
                object1Slot.slotInEquipment.countTexte.text = "";
            }
        }
        else
        {
            if (objects[1].count > 1)
            {
                objects[1].count--;
                object2Slot.countText.text = objects[1].count.ToString();
                object2Slot.slotInEquipment.countTexte.text = objects[1].count.ToString();
            }
            else
            {
                objects[1].itemData = null;
                objects[1].count = 0;
                object2Slot.slotItemData = null;
                object2Slot.slotInEquipment.item = null;
                object2Slot.slotInEquipment.countTexte.text = "";
            }
        }
        RefreshAffichage();
    }

    public void UpdateImageSeleted()
    {
        weapon1Slot.imageSelected.SetActive(weapon1Slot.isEquipped);
        weapon2Slot.imageSelected.SetActive(weapon2Slot.isEquipped);
        object1Slot.imageSelected.SetActive(object1Slot.isEquipped);
        object2Slot.imageSelected.SetActive(object2Slot.isEquipped);
    }
    public bool WeaponsAreFull()
    {
        return weapons[0].itemData != null && weapons[1].itemData != null;
    }

    public bool ObjectsAreFull(ItemData item)
    {
        return !IsValidForSlot(0, item) && !IsValidForSlot(1, item);
    }

    public bool IfPlayerHasWeaponEquipped()
    {
        return weapon1Slot.isEquipped || weapon2Slot.isEquipped;
    }

    private void DisableObject(ItemData item)
    {
        if (item == null) return;

        var lib = equipmentLibrary.Get(item);
        if (lib?.itemPrefab.activeSelf == true)
            lib.itemPrefab.SetActive(false);
    }

    private void DisableWeapon(ItemData item)
    {
        if (item == null) return;

        var lib = equipmentLibrary.Get(item);
        lib?.itemPrefab.SetActive(false);
    }

    public PaletteSaveData GetSaveData()
    {
        return new PaletteSaveData
        {
            weapon1 = CreateSlotSave(weapon1Slot.slotItemData, weapons[0], weapon1Slot.isEquipped),
            weapon2 = CreateSlotSave(weapon2Slot.slotItemData, weapons[1], weapon2Slot.isEquipped),

            object1 = CreateSlotSave(object1Slot.slotItemData, objects[0], object1Slot.isEquipped),
            object2 = CreateSlotSave(object2Slot.slotItemData, objects[1], object2Slot.isEquipped),
        };
    }

    private PaletteSlotSave CreateSlotSave(ItemData item, ItemInInventory slot, bool equipped)
    {
        if (item == null || slot == null)
            return null;

        return new PaletteSlotSave
        {
            itemID = item.itemID,
            count = slot.count,
            isEquipped = equipped
        };
    }



    public void LoadSaveData(PaletteSaveData data)
    {
        ClearPalette();

        LoadWeaponSlot(1, data.weapon1);
        LoadWeaponSlot(2, data.weapon2);

        LoadObjectSlot(1, data.object1);
        LoadObjectSlot(2, data.object2);

        RefreshAffichage();
        UpdateImageSeleted();

        ApplyEquippedStateAfterLoad();
    }

    private void ClearPalette()
    {
        weapon1Slot.slotItemData = null;
        weapon2Slot.slotItemData = null;
        object1Slot.slotItemData = null;
        object2Slot.slotItemData = null;

        weapon1Slot.isEquipped = false;
        weapon2Slot.isEquipped = false;
        object1Slot.isEquipped = false;
        object2Slot.isEquipped = false;

        weapons = new ItemInInventory[2]
        {
        new ItemInInventory(),
        new ItemInInventory()
        };

        objects = new ItemInInventory[2]
        {
        new ItemInInventory(),
        new ItemInInventory()
        };
    }


    private void LoadWeaponSlot(int slot, PaletteSlotSave save)
    {
        if (save == null) return;

        ItemData item = Inventory.instance.itemDatabase.GetItemByID(save.itemID);
        if (item == null) return;

        int index = slot - 1;

        weapons[index] = new ItemInInventory
        {
            itemData = item,
            count = save.count
        };

        PaletteSlot slotData = weaponSlots[slot - 1];
        slotData.slotItemData = item;
        slotData.isEquipped = save.isEquipped;
    }


    private void LoadObjectSlot(int slot, PaletteSlotSave save)
    {
        if (save == null) return;

        ItemData item = Inventory.instance.itemDatabase.GetItemByID(save.itemID);
        if (item == null) return;

        int index = slot - 1;

        objects[index] = new ItemInInventory
        {
            itemData = item,
            count = save.count
        };

        PaletteSlot slotData = objectSlots[slot - 1];
        slotData.slotItemData = item;
        slotData.isEquipped = save.isEquipped;
        UpdateSlotUI(index, save.count);
    }

    private void ApplyEquippedStateAfterLoad()
    {
        // Priorité aux armes
        if (weapon1Slot.isEquipped && weapon1Slot.slotItemData != null)
        {
            EquipFromSave(weapon1Slot.slotItemData);
        }
        else if (weapon2Slot.isEquipped && weapon2Slot.slotItemData != null)
        {
            EquipFromSave(weapon2Slot.slotItemData);
        }
        // Sinon objets
        else if (object1Slot.isEquipped && object1Slot.slotItemData != null)
        {
            EquipObjectFromSave(object1Slot.slotItemData);
        }
        else if (object2Slot.isEquipped && object2Slot.slotItemData != null)
        {
            EquipObjectFromSave(object2Slot.slotItemData);
        }
    }

    private void EquipFromSave(ItemData item)
    {
        // Désactiver tout
        DisableObject(object1Slot.slotItemData);
        DisableObject(object2Slot.slotItemData);

        EquipmentLibraryItem libItem = equipmentLibrary.Get(item);

        // Activer le prefab
        if (!libItem.itemPrefab.activeSelf)
            libItem.itemPrefab.SetActive(true);

        // Informer les systèmes
        interactBehaviour.SetCurrentEquippedItem(libItem);
        PlayerStats.instance.equipmentToEquip = libItem;

        // Animator
        // ApplyWeaponTypeToAnimator(item.handWeaponType);

        Debug.Log($"[SAVE] Equipped weapon from save: {item.name}");
    }

    private void EquipObjectFromSave(ItemData item)
    {

        EquipmentLibraryItem libItem = equipmentLibrary.Get(item);

        if (!libItem.itemPrefab.activeSelf)
            libItem.itemPrefab.SetActive(true);
        interactBehaviour.SetCurrentEquippedItem(libItem);

        //animator.SetBool("CarryingConsumable", true);

        Debug.Log($"[SAVE] Equipped object from save: {item.name}");
    }

}

//[System.Serializable]
//public class PaletteSaveData
//{
//    public PaletteSlotSave weapon1;
//    public PaletteSlotSave weapon2;

//    public PaletteSlotSave object1;
//    public PaletteSlotSave object2;
//}

//[System.Serializable]
//public class PaletteSlotSave
//{
//    public string itemID;
//    public int count;
//    public bool isEquipped;
//}

[System.Serializable]
public class PaletteSlot
{
    public ItemData slotItemData;
    public Image SlotImage;
    public Image iconeInput;
    public bool isEquipped = false;
    public TextMeshProUGUI countText;
    public GameObject imageSelected;
    public Slot slotInEquipment;
}