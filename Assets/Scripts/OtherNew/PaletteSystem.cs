using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

    #region Weapons And Objects Data
    [Header("Weapon 1")]

    public Button weapon1SlotDesequipButton;
    public ItemData equipmentWeapon1Item;
    public Image weapon1SlotImage;
    public Image weapon1SlotInInventory;
    public Image iconeInputWeapon1;
    public bool isEquippedWeapon1 = false;
    public GameObject weapon1ImageSelected;

    [Header("Weapon 2")]

    public Button weapon2SlotDesequipButton;
    public ItemData equipmentWeapon2Item;
    public Image weapon2SlotImage;
    public Image weapon2SlotInInventory;
    public Image iconeInputWeapon2;
    public bool isEquippedWeapon2 = false;
    public GameObject weapon2ImageSelected;

    [Header("Object 1")]

    public Button object1SlotDesequipButton;
    public ItemData equipmentObject1Item;
    public Image object1SlotImage;
    public Image object1SlotInInventory;
    public Image iconeInputObject1;
    public bool isEquippedObject1 = false;
    public TextMeshProUGUI object1CountText;
    public GameObject object1ImageSelected;


    [Header("Object 2")]

    public Button object2SlotDesequipButton;
    public ItemData equipmentObject2Item;
    public Image object2SlotImage;
    public Image object2SlotInInventory;
    public Image iconeInputObject2;
    public bool isEquippedObject2 = false;
    public TextMeshProUGUI object2CountText;
    public GameObject object2ImageSelected;

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
        if (player.StateMachine.CurrentState is not GroundedState || player.StateMachine.CurrentState is EquipState)
            return;

        if (player.Input.Weapon1Pressed && equipmentWeapon1Item != null)
        {
            ToggleWeapon(1, player);
        }
        else if (player.Input.Weapon2Pressed && equipmentWeapon2Item != null)
        {
            ToggleWeapon(2, player);
        }
        else if (player.Input.Object1Pressed && equipmentObject1Item != null)
        {
            ToggleObject(1, player);
        }
        else if (player.Input.Object2Pressed && equipmentObject2Item != null)
        {
            ToggleObject(2, player);
        }
    }

    private void ToggleWeapon(int slot, PlayerController player)
    {
        bool isCurrentlyEquipped = (slot == 1) ? isEquippedWeapon1 : isEquippedWeapon2;
        ItemData item = (slot == 1) ? equipmentWeapon1Item : equipmentWeapon2Item;

        if (!isCurrentlyEquipped)
        {
            if (slot == 1) isEquippedWeapon1 = true; else isEquippedWeapon2 = true;

            // On passe l'info au player et on change d'état
            player.PendingWeaponType = item.handWeaponType;
            player.PrepareEquip(item);
            player.StateMachine.ChangeState(PlayerStateType.Equip);

            UseWeapon(slot, player);
        }
        else
        {
            // Pour le déséquipement, on pourrait aussi créer un "UnequipState" 
            // ou simplement appeler la logique via le player
            DesequipCurrentActiveWeapon(slot, player);
        }
        UpdateImageSeleted();
    }
    private void DesequipCurrentActiveWeapon(int slot, PlayerController player)
    {
        ItemData item = (slot == 1) ? equipmentWeapon1Item : equipmentWeapon2Item;
        if (item == null) return;

        // On passe par une version qui prend le player pour l'animator
        if (slot == 1) ForceDesequipWeapon(item, ref isEquippedWeapon1, player);
        else ForceDesequipWeapon(item, ref isEquippedWeapon2, player);

        EquipmentLibraryItem lib = equipmentLibrary.content.First(x => x.itemData == item);
        lib.itemPrefab.SetActive(false);
    }

    private void ForceDesequipWeapon(ItemData weaponData, ref bool equippedFlag, PlayerController player)
    {
        if (weaponData == null) return;

        // On prépare les infos pour l'état
        player.PendingUnequipType = weaponData.handWeaponType;

        // On change d'état (Cela lancera l'animation proprement)
        player.StateMachine.ChangeState(PlayerStateType.Unequip);

        // On garde la logique de Data
        EquipmentLibraryItem lib = equipmentLibrary.content.First(x => x.itemData == weaponData);
        PlayerStats.instance.equipmentToDesequip = lib;

        equippedFlag = false;
    }
    private void ToggleObject(int slot, PlayerController player)
    {
        bool isCurrentlyEquipped = (slot == 1) ? isEquippedObject1 : isEquippedObject2;

        if (!isCurrentlyEquipped)
        {
            isEquippedObject1 = (slot == 1);
            isEquippedObject2 = (slot == 2);

            player.StateMachine.ChangeState(PlayerStateType.Equip);
            UseObject(slot, player);
        }
        else
        {
            // Déséquipement de l'objet
            if (slot == 1) isEquippedObject1 = false; else isEquippedObject2 = false;

            var item = (slot == 1) ? equipmentObject1Item : equipmentObject2Item;
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
        isEquippedWeapon1 = false;
        isEquippedWeapon2 = false;
        player.Animator.SetBool("CarryingConsumable", true);
        player.Animator.SetBool("IsTwoHandedWeapon", false);
        player.Animator.SetBool("IsOneHandedWeapon", false);
        if (numberOfObject == 1)
        {
            EquipmentLibraryItem equipmentLibraryItem1 = equipmentLibrary.content.Where(x => x.itemData == equipmentObject1Item).First();
            equipmentLibraryItem1.itemPrefab.SetActive(true);

            interactBehaviour.SetCurrentEquippedItem(equipmentLibraryItem1);

            EquipmentLibraryItem equipmentLibraryItem2 = equipmentLibrary.content.Where(x => x.itemData == equipmentObject2Item).FirstOrDefault();
            if (equipmentLibraryItem2 != null && equipmentLibraryItem2.itemData != objects[0].itemData) equipmentLibraryItem2.itemPrefab.SetActive(false);


            EquipmentLibraryItem equipmentLibraryWeapon1 = equipmentLibrary.content.Where(x => x.itemData == equipmentWeapon1Item).FirstOrDefault();
            equipmentLibraryWeapon1?.itemPrefab.SetActive(false);

            EquipmentLibraryItem equipmentLibraryWeapon2 = equipmentLibrary.content.Where(x => x.itemData == equipmentWeapon2Item).FirstOrDefault();
            equipmentLibraryWeapon2?.itemPrefab.SetActive(false);

        }
        else
        {
            EquipmentLibraryItem equipmentLibraryItem2 = equipmentLibrary.content.Where(x => x.itemData == equipmentObject2Item).First();
            equipmentLibraryItem2.itemPrefab.SetActive(true);
            interactBehaviour.SetCurrentEquippedItem(equipmentLibraryItem2);


            EquipmentLibraryItem equipmentLibraryItem1 = equipmentLibrary.content.Where(x => x.itemData == equipmentObject1Item).FirstOrDefault();
            if (equipmentLibraryItem1 != null && equipmentLibraryItem1.itemData != objects[1].itemData) equipmentLibraryItem1.itemPrefab.SetActive(false);

            EquipmentLibraryItem equipmentLibraryWeapon1 = equipmentLibrary.content.Where(x => x.itemData == equipmentWeapon1Item).FirstOrDefault();
            equipmentLibraryWeapon1?.itemPrefab.SetActive(false);

            EquipmentLibraryItem equipmentLibraryWeapon2 = equipmentLibrary.content.Where(x => x.itemData == equipmentWeapon2Item).FirstOrDefault();
            equipmentLibraryWeapon2?.itemPrefab.SetActive(false);
        }
    }

    private void UseWeapon(int slot, PlayerController player)
    {
        ItemData itemToEquip = (slot == 1) ? equipmentWeapon1Item : equipmentWeapon2Item;
        player.PendingWeaponItem = itemToEquip;
        // 1. On cache les consommables
        DisableObject(equipmentObject1Item);
        DisableObject(equipmentObject2Item);
        isEquippedObject1 = isEquippedObject2 = false;
        player.Animator.SetBool("CarryingConsumable", false);

        // 2. On force le rangement de l'AUTRE arme si elle était sortie
        if (slot == 1 && isEquippedWeapon2) ForceDesequipWeapon(equipmentWeapon2Item, ref isEquippedWeapon2, player);
        if (slot == 2 && isEquippedWeapon1) ForceDesequipWeapon(equipmentWeapon1Item, ref isEquippedWeapon1, player);

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
        if (Inventory.instance.IsFull())
        {
            Debug.LogWarning("Cannot desequip item, inventory is full.");
            return;
        }

        ItemData currentItem = (numberOfWeapon == 1) ? equipmentWeapon1Item : equipmentWeapon2Item;

        EquipmentLibraryItem lib = equipmentLibrary.content.First(x => x.itemData == currentItem);

        // animations + model disable

        // remettre le slot visuellement vide
        if (numberOfWeapon == 1)
        {
            equipmentWeapon1Item = null;
            weapon1SlotImage.sprite = InventorySystem.instance.emptySlotVisual;

        }
        else
        {
            equipmentWeapon2Item = null;
            weapon2SlotImage.sprite = InventorySystem.instance.emptySlotVisual;
        }

        // remettre dans l'inventaire
        InventorySystem.instance.AddItem(currentItem);
        RemoveWeapon(numberOfWeapon);
        RefreshAffichage();
        UpdateImageSeleted();
    }


    public void DesequipObject(int numberOfObject)
    {
        if (Inventory.instance.IsFull())
        {
            Debug.LogWarning("Cannot desequip item, inventory is full.");
            return;
        }

        ItemData currentItem = null;
        if (numberOfObject == 1)
        {
            currentItem = equipmentObject1Item;
            object1SlotImage.sprite = InventorySystem.instance.emptySlotVisual;
            isEquippedObject1 = false;
        }
        else
        {
            currentItem = equipmentObject2Item;
            object2SlotImage.sprite = InventorySystem.instance.emptySlotVisual;
            isEquippedObject2 = false;
        }



        EquipmentLibraryItem equipmentLibraryItem = equipmentLibrary.content.Where(x => x.itemData == currentItem).FirstOrDefault();

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
        weapon1SlotImage.sprite = equipmentWeapon1Item ? equipmentWeapon1Item.visual : InventorySystem.instance.emptySlotVisual;
        weapon2SlotImage.sprite = equipmentWeapon2Item ? equipmentWeapon2Item.visual : InventorySystem.instance.emptySlotVisual;

        weapon1SlotInInventory.sprite = equipmentWeapon1Item ? equipmentWeapon1Item.visual : InventorySystem.instance.emptySlotVisual;
        weapon2SlotInInventory.sprite = equipmentWeapon2Item ? equipmentWeapon2Item.visual : InventorySystem.instance.emptySlotVisual;

        object1SlotImage.sprite = equipmentObject1Item ? equipmentObject1Item.visual : InventorySystem.instance.emptySlotVisual;
        object1SlotInInventory.sprite = equipmentObject1Item ? equipmentObject1Item.visual : InventorySystem.instance.emptySlotVisual;
        object1CountText.gameObject.SetActive(equipmentObject1Item);

        object2SlotImage.sprite = equipmentObject2Item ? equipmentObject2Item.visual : InventorySystem.instance.emptySlotVisual;
        object2SlotInInventory.sprite = equipmentObject2Item ? equipmentObject2Item.visual : InventorySystem.instance.emptySlotVisual;
        object2CountText.gameObject.SetActive(equipmentObject2Item);
    }

    public void AddWeapon(ItemData item)
    {
        if (equipmentWeapon1Item == null)
        {
            equipmentWeapon1Item = item;
            ItemInInventory newWeapon1 =
                    new ItemInInventory
                    {
                        itemData = item,
                        count = 1
                    };
            weapons[0] = newWeapon1;
        }
        else if (equipmentWeapon2Item == null)
        {
            equipmentWeapon2Item = item;
            ItemInInventory newWeapon2 =
                    new ItemInInventory
                    {
                        itemData = item,
                        count = 1
                    };
            weapons[1] = newWeapon2;
        }
        else if (equipmentWeapon1Item == item || equipmentWeapon2Item == item)
        {
            // If the weapon is already equipped, we do nothing
            return;
        }
        RefreshAffichage();
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
        var equippedItem = slotIndex == 0 ? equipmentObject1Item : equipmentObject2Item;
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
        if (inventoryItem.itemData == null)
        {
            Debug.Log("Creating new ItemInInventory for slot " + slotIndex);
            objects[slotIndex] = new ItemInInventory { itemData = item, count = 1 };
            if (slotIndex == 0) equipmentObject1Item = item;
            else equipmentObject2Item = item;
        }
        else if (inventoryItem.itemData == item)
        {
            Debug.Log("Incrementing count for slot " + slotIndex);
            if (inventoryItem.count < item.maxStack)
            {
                inventoryItem.count++;
            }
        }

        UpdateSlotUI(slotIndex, objects[slotIndex].count);
    }

    private void UpdateSlotUI(int slotIndex, int count)
    {
        if (slotIndex == 0)
            object1CountText.text = count.ToString();
        else
            object2CountText.text = count.ToString();
    }

    public void RemoveWeapon(int numberOfWeapon)
    {
        if (numberOfWeapon == 1)
        {
            equipmentWeapon1Item = null;
            weapons[0].itemData = null;
            weapons[0].count = 0;
        }
        else
        {
            equipmentWeapon2Item = null;
            weapons[1].itemData = null;
            weapons[1].count = 0;
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
                object1CountText.text = objects[0].count.ToString();
            }
            else
            {
                objects[0].itemData = null;
                objects[0].count = 0;
                equipmentObject1Item = null;
            }
        }
        else
        {
            if (objects[1].count > 1)
            {
                objects[1].count--;
                object2CountText.text = objects[1].count.ToString();
            }
            else
            {
                objects[1].itemData = null;
                objects[1].count = 0;
                equipmentObject2Item = null;
            }
        }
        RefreshAffichage();
    }

    public void UpdateImageSeleted()
    {
        weapon1ImageSelected.SetActive(isEquippedWeapon1);
        weapon2ImageSelected.SetActive(isEquippedWeapon2);
        object1ImageSelected.SetActive(isEquippedObject1);
        object2ImageSelected.SetActive(isEquippedObject2);
    }
    public bool WeaponsAreFull()
    {
        return weapons.All(w => w.itemData != null);
    }

    public bool ObjectsAreFull(ItemData item)
    {
        return !Enumerable.Range(0, 2).Any(slot => IsValidForSlot(slot, item));
    }

    public bool IfPlayerHasWeaponEquipped()
    {
        return isEquippedWeapon1 || isEquippedWeapon2;
    }

    private void DisableObject(ItemData item)
    {
        if (item == null) return;

        EquipmentLibraryItem lib = equipmentLibrary.content
            .FirstOrDefault(x => x.itemData == item);

        lib?.itemPrefab.SetActive(false);
    }



    public PaletteSaveData GetSaveData()
    {
        return new PaletteSaveData
        {
            weapon1 = CreateSlotSave(equipmentWeapon1Item, weapons[0], isEquippedWeapon1),
            weapon2 = CreateSlotSave(equipmentWeapon2Item, weapons[1], isEquippedWeapon2),

            object1 = CreateSlotSave(equipmentObject1Item, objects[0], isEquippedObject1),
            object2 = CreateSlotSave(equipmentObject2Item, objects[1], isEquippedObject2),
        };
    }

    private PaletteSlotSave CreateSlotSave(ItemData item, ItemInInventory slot, bool equipped)
    {
        if (item == null)
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
        equipmentWeapon1Item = null;
        equipmentWeapon2Item = null;
        equipmentObject1Item = null;
        equipmentObject2Item = null;

        isEquippedWeapon1 = false;
        isEquippedWeapon2 = false;
        isEquippedObject1 = false;
        isEquippedObject2 = false;

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

        if (slot == 1)
        {
            equipmentWeapon1Item = item;
            isEquippedWeapon1 = save.isEquipped;
        }
        else
        {
            equipmentWeapon2Item = item;
            isEquippedWeapon2 = save.isEquipped;
        }
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

        if (slot == 1)
        {
            equipmentObject1Item = item;
            isEquippedObject1 = save.isEquipped;
        }
        else
        {
            equipmentObject2Item = item;
            isEquippedObject2 = save.isEquipped;
        }
        UpdateSlotUI(index, save.count);
    }

    private void ApplyEquippedStateAfterLoad()
    {
        // Priorité aux armes
        if (isEquippedWeapon1 && equipmentWeapon1Item != null)
        {
            EquipFromSave(equipmentWeapon1Item);
        }
        else if (isEquippedWeapon2 && equipmentWeapon2Item != null)
        {
            EquipFromSave(equipmentWeapon2Item);
        }
        // Sinon objets
        else if (isEquippedObject1 && equipmentObject1Item != null)
        {
            EquipObjectFromSave(equipmentObject1Item);
        }
        else if (isEquippedObject2 && equipmentObject2Item != null)
        {
            EquipObjectFromSave(equipmentObject2Item);
        }
    }

    private void EquipFromSave(ItemData item)
    {
        // Désactiver tout
        DisableObject(equipmentObject1Item);
        DisableObject(equipmentObject2Item);

        EquipmentLibraryItem libItem =
            equipmentLibrary.content.First(x => x.itemData == item);

        // Activer le prefab
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
        EquipmentLibraryItem libItem =
            equipmentLibrary.content.First(x => x.itemData == item);

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
