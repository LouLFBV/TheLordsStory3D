using UnityEngine;

public class PaletteSaveSystem : MonoBehaviour
{
    
    [SerializeField] private EquipmentLibrary equipmentLibrary;
    [SerializeField] private InteractSystem interactSystem;
    [SerializeField] private PaletteSlotManager slotManager;
    [SerializeField] private PaletteEquipmentManager equipmentManager;
    public PaletteSaveData GetSaveData()
    {
        return new PaletteSaveData
        {
            weapon1 = CreateSave(slotManager.weapons[0]),
            weapon2 = CreateSave(slotManager.weapons[1]),
            object1 = CreateSave(slotManager.objects[0]),
            object2 = CreateSave(slotManager.objects[1]),
        };
    }

    private PaletteSlotSave CreateSave(ItemInInventory slot)
    {
        if (slot.itemData == null)
            return null;

        return new PaletteSlotSave
        {
            itemID = slot.itemData.itemID,
            count = slot.count
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
        slotManager.ClearPalette();

        LoadWeaponSlot(1, data.weapon1);
        LoadWeaponSlot(2, data.weapon2);

        LoadObjectSlot(1, data.object1);
        LoadObjectSlot(2, data.object2);

        slotManager.RefreshAffichage();
        slotManager.UpdateImageSeleted();

        ApplyEquippedStateAfterLoad();
    }



    private void LoadWeaponSlot(int slot, PaletteSlotSave save)
    {
        if (save == null) return;

        ItemData item = InventorySystem.instance.itemDatabase.GetItemByID(save.itemID);
        if (item == null) return;

        int index = slot - 1;

        slotManager.weapons[index] = new ItemInInventory
        {
            itemData = item,
            count = save.count
        };

        PaletteSlot slotData = slotManager.weaponSlots[slot - 1];
        slotData.slotItemData = item;
        slotData.isEquipped = save.isEquipped;
    }


    private void LoadObjectSlot(int slot, PaletteSlotSave save)
    {
        if (save == null) return;

        ItemData item = InventorySystem.instance.itemDatabase.GetItemByID(save.itemID);
        if (item == null) return;

        int index = slot - 1;

        slotManager.objects[index] = new ItemInInventory
        {
            itemData = item,
            count = save.count
        };

        PaletteSlot slotData = slotManager.objectSlots[slot - 1];
        slotData.slotItemData = item;
        slotData.isEquipped = save.isEquipped;
        slotManager.UpdateSlotUI(index, save.count);
    }

    private void ApplyEquippedStateAfterLoad()
    {
        // Priorité aux armes
        if (slotManager.weaponSlots[0].isEquipped && slotManager.weaponSlots[0].slotItemData != null)
        {
            EquipFromSave(slotManager.weaponSlots[0].slotItemData);
        }
        else if (slotManager.weaponSlots[1].isEquipped && slotManager.weaponSlots[1].slotItemData != null)
        {
            EquipFromSave(slotManager.weaponSlots[1].slotItemData);
        }
        // Sinon objets
        else if (slotManager.objectSlots[0].isEquipped && slotManager.objectSlots[0].slotItemData != null)
        {
            EquipObjectFromSave(slotManager.objectSlots[0].slotItemData);
        }
        else if (slotManager.objectSlots[1].isEquipped && slotManager.objectSlots[1].slotItemData != null)
        {
            EquipObjectFromSave(slotManager.objectSlots[1].slotItemData);
        }
    }

    private void EquipFromSave(ItemData item)
    {
        // Désactiver tout
        equipmentManager.DisableObject(slotManager.objectSlots[0].slotItemData);
        equipmentManager.DisableObject(slotManager.objectSlots[1].slotItemData);

        EquipmentLibraryItem libItem = equipmentLibrary.Get(item);

        // Activer le prefab
        if (!libItem.itemPrefab.activeSelf)
            libItem.itemPrefab.SetActive(true);

        // Informer les systèmes
        //interactSystem.SetCurrentEquippedItem(libItem);
        //PlayerStats.instance.equipmentToEquip = libItem;

        // Animator
        // ApplyWeaponTypeToAnimator(item.handWeaponType);

        Debug.Log($"[SAVE] Equipped weapon from save: {item.name}");
    }

    private void EquipObjectFromSave(ItemData item)
    {

        EquipmentLibraryItem libItem = equipmentLibrary.Get(item);

        if (!libItem.itemPrefab.activeSelf)
            libItem.itemPrefab.SetActive(true);
        interactSystem.SetCurrentEquippedItem(libItem);

        //animator.SetBool("CarryingConsumable", true);

        Debug.Log($"[SAVE] Equipped object from save: {item.name}");
    }
}