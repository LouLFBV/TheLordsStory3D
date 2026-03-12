using UnityEngine;

public class PaletteSlotManager : MonoBehaviour
{
    public ItemInInventory[] weapons = new ItemInInventory[2];
    public ItemInInventory[] objects = new ItemInInventory[2];

    public PaletteSlot[] weaponSlots = new PaletteSlot[2];
    public PaletteSlot[] objectSlots = new PaletteSlot[2];

    public PaletteSlot arrowSlot;


    public bool WeaponsAreFull()
    {
        return weapons[0].itemData != null && weapons[1].itemData != null;
    }


    public bool IfPlayerHasWeaponEquipped()
    {
        return weaponSlots[0].isEquipped || weaponSlots[1].isEquipped;
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

    public void AddArrow(ItemData item)
    {
        arrowSlot.slotItemData = item;
        arrowSlot.SlotImage.sprite = item.visual;
        arrowSlot.slotInEquipment.itemVisual.sprite = item.visual;
        arrowSlot.slotInEquipment.item = item;
        arrowSlot.countText.gameObject.SetActive(true); ;
    }
    public  void UpdateCountArrow(int count)
    {
        if (count == 0)
        {
            arrowSlot.slotItemData = null;
            arrowSlot.SlotImage.sprite = InventorySystem.instance.emptySlotVisual;
            arrowSlot.slotInEquipment.itemVisual.sprite = InventorySystem.instance.emptySlotVisual;
            arrowSlot.slotInEquipment.item = null;
            arrowSlot.countText.gameObject.SetActive(false);
        }
        arrowSlot.countText.text = count.ToString();
        arrowSlot.slotInEquipment.countTexte.text = count.ToString();
    }
    public void RefreshUI()
    {
        foreach (var slot in weaponSlots)
            RefreshSlot(slot);

        foreach (var slot in objectSlots)
            RefreshSlot(slot);
    }

    public void RefreshAffichage()
    {
        PaletteSlot[] allSlots =
        {
            weaponSlots[0],
            weaponSlots[1],
            objectSlots[0],
            objectSlots[1]
        };

        foreach (var slot in allSlots)
            RefreshSlot(slot);
    }

    public bool ObjectsAreFull(ItemData item)
    {
        return !IsValidForSlot(0, item) && !IsValidForSlot(1, item);
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
            slotData.SlotImage.sprite = item.visual;
            slotData.countText.text = "1";
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

    public void UpdateSlotUI(int slotIndex, int count)
    {
        PaletteSlot slotData = objectSlots[slotIndex];
        slotData.countText.text = count.ToString();
        slotData.slotInEquipment.countTexte.text = count.ToString();
    }

    public bool IsValidForSlot(int slotIndex, ItemData item)
    {
        var equippedItem = slotIndex == 0 ? objectSlots[0].slotItemData : objectSlots[1].slotItemData;
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



    public void UpdateImageSeleted()
    {
        weaponSlots[0].imageSelected.SetActive(weaponSlots[0].isEquipped);
        weaponSlots[1].imageSelected.SetActive(weaponSlots[1].isEquipped);
        objectSlots[0].imageSelected.SetActive(objectSlots[0].isEquipped);
        objectSlots[1].imageSelected.SetActive(objectSlots[1].isEquipped);
    }
    public void ClearPalette()
    {
        weaponSlots[0].slotItemData = null;
        weaponSlots[1].slotItemData = null;
        objectSlots[0].slotItemData = null;
        objectSlots[1].slotItemData = null;

        weaponSlots[0].isEquipped = false;
        weaponSlots[1].isEquipped = false;
        objectSlots[0].isEquipped = false;
        objectSlots[1].isEquipped = false;

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
}