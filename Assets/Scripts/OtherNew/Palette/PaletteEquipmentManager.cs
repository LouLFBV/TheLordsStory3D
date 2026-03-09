using UnityEngine;

public class PaletteEquipmentManager : MonoBehaviour
{
    [SerializeField] private EquipmentLibrary equipmentLibrary;
    [SerializeField] private InteractSystem interactSystem;
    [SerializeField] private PaletteSlotManager slotManager;


    private void UseObject(int numberOfObject, PlayerController player)
    {
        slotManager.weaponSlots[0].isEquipped = false;
        slotManager.weaponSlots[1].isEquipped = false;
        player.Animator.SetBool("CarryingConsumable", true);
        player.Animator.SetBool("IsTwoHandedWeapon", false);
        player.Animator.SetBool("IsOneHandedWeapon", false);
        if (numberOfObject == 1)
        {
            EquipmentLibraryItem equipmentLibraryItem1 = equipmentLibrary.Get(slotManager.objectSlots[0].slotItemData);
            equipmentLibraryItem1.itemPrefab.SetActive(true);

            interactSystem.SetCurrentEquippedItem(equipmentLibraryItem1);

            EquipmentLibraryItem equipmentLibraryItem2 = equipmentLibrary.Get(slotManager.objectSlots[1].slotItemData);

            if (equipmentLibraryItem2 != null && equipmentLibraryItem2.itemData != slotManager.objects[0].itemData) equipmentLibraryItem2.itemPrefab.SetActive(false);


            DisableWeapon(slotManager.weaponSlots[0].slotItemData);
            DisableWeapon(slotManager.weaponSlots[1].slotItemData);

        }
        else
        {
            EquipmentLibraryItem equipmentLibraryItem2 = equipmentLibrary.Get(slotManager.objectSlots[1].slotItemData);
            equipmentLibraryItem2.itemPrefab.SetActive(true);
            interactSystem.SetCurrentEquippedItem(equipmentLibraryItem2);


            EquipmentLibraryItem equipmentLibraryItem1 = equipmentLibrary.Get(slotManager.objectSlots[0].slotItemData);
            if (equipmentLibraryItem1 != null && equipmentLibraryItem1.itemData != slotManager.objects[1].itemData) equipmentLibraryItem1.itemPrefab.SetActive(false);

            DisableWeapon(slotManager.weaponSlots[0].slotItemData);
            DisableWeapon(slotManager.weaponSlots[1].slotItemData);
        }
    }

    private void UseWeapon(int slot, PlayerController player)
    {
        ItemData itemToEquip = (slot == 1) ? slotManager.weaponSlots[1].slotItemData : slotManager.weaponSlots[0].slotItemData;
        player.PendingWeaponItem = itemToEquip;
        // 1. On cache les consommables
        DisableObject(slotManager.objectSlots[0].slotItemData);
        DisableObject(slotManager.objectSlots[1].slotItemData);
        slotManager.objectSlots[0].isEquipped = slotManager.objectSlots[1].isEquipped = false;
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

        ItemData currentItem = (numberOfWeapon == 1) ? slotManager.weaponSlots[0].slotItemData : slotManager.weaponSlots[1].slotItemData;

        //EquipmentLibraryItem lib = equipmentLibrary.Get(currentItem);

        // animations + model disable

        // remettre le slot visuellement vide
        if (numberOfWeapon == 1)
        {
            slotManager.weaponSlots[0].slotItemData = null;
            slotManager.weaponSlots[0].slotInEquipment.item = null;
            slotManager.weaponSlots[0].slotInEquipment.itemVisual.sprite = InventorySystem.instance.emptySlotVisual;
            slotManager.weaponSlots[0].SlotImage.sprite = InventorySystem.instance.emptySlotVisual;
            slotManager.weaponSlots[0].slotInEquipment.itemVisual.sprite = InventorySystem.instance.emptySlotVisual;

        }
        else
        {
            slotManager.weaponSlots[1].slotItemData = null;
            slotManager.weaponSlots[1].slotInEquipment.item = null;
            slotManager.weaponSlots[1].slotInEquipment.itemVisual.sprite = InventorySystem.instance.emptySlotVisual;
            slotManager.weaponSlots[1].SlotImage.sprite = InventorySystem.instance.emptySlotVisual;
            slotManager.weaponSlots[1].slotInEquipment.itemVisual.sprite = InventorySystem.instance.emptySlotVisual;
        }

        // remettre dans l'inventaire
        InventorySystem.instance.AddItem(currentItem);
        RemoveWeapon(numberOfWeapon);
        if(slotManager.weaponSlots[numberOfWeapon].isEquipped)
        {
            Debug.Log("Desequipping currently equipped weapon in slot 1");
        }
        slotManager.RefreshAffichage();
        slotManager.UpdateImageSeleted();
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
            currentItem = slotManager.objectSlots[0].slotItemData;
            slotManager.objectSlots[0].SlotImage.sprite = InventorySystem.instance.emptySlotVisual;
            slotManager.objectSlots[0].slotInEquipment.itemVisual.sprite = InventorySystem.instance.emptySlotVisual;
            slotManager.objectSlots[0].isEquipped = false;
        }
        else
        {
            currentItem = slotManager.objectSlots[1].slotItemData;
            slotManager.objectSlots[1].SlotImage.sprite = InventorySystem.instance.emptySlotVisual;
            slotManager.objectSlots[1].slotInEquipment.itemVisual.sprite = InventorySystem.instance.emptySlotVisual;
            slotManager.objectSlots[1].isEquipped = false;
        }



        EquipmentLibraryItem equipmentLibraryItem = equipmentLibrary.Get(currentItem);
        if (equipmentLibraryItem != null)
            equipmentLibraryItem?.itemPrefab.SetActive(false);

        if (currentItem)
        {
            InventorySystem.instance.AddItem(currentItem);
        }
        RemoveObject(numberOfObject);
        slotManager.RefreshAffichage();
        slotManager.UpdateImageSeleted();
    }


    public void ToggleWeapon(int slot, PlayerController player)
    {
        bool isCurrentlyEquipped = (slot == 1) ? slotManager.weaponSlots[1].isEquipped : slotManager.weaponSlots[0].isEquipped;
        ItemData item = (slot == 1) ? slotManager.weapons[1].itemData : slotManager.weapons[0].itemData;

        if (!isCurrentlyEquipped)
        {
            slotManager.weaponSlots[0].isEquipped = slot == 0;
            slotManager.weaponSlots[1].isEquipped = slot == 1;

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
        slotManager.UpdateImageSeleted();
    }

    public void ToggleObject(int slot, PlayerController player)
    {
        bool isCurrentlyEquipped = (slot == 1) ? slotManager.objectSlots[0].isEquipped : slotManager.objectSlots[1].isEquipped;

        if (!isCurrentlyEquipped)
        {
            slotManager.objectSlots[0].isEquipped = (slot == 1);
            slotManager.objectSlots[1].isEquipped = (slot == 2);

            player.StateMachine.ChangeState(PlayerStateType.Equip);
            UseObject(slot, player);
        }
        else
        {
            // Déséquipement de l'objet
            PaletteSlot slotData = slotManager.objectSlots[slot - 1];
            slotData.isEquipped = false;

            var item = (slot == 1) ? slotManager.objectSlots[0].slotItemData : slotManager.objectSlots[1].slotItemData;
            DisableObject(item);
            player.Animator.SetBool("CarryingConsumable", false);
        }
        slotManager.UpdateImageSeleted();
    }

    public void RemoveObject(int numberOfObject)
    {
        if (numberOfObject == 1)
        {

            if (slotManager.objects[0].count > 1)
            {
                slotManager.objects[0].count--;
                slotManager.objectSlots[0].countText.text = slotManager.objects[0].count.ToString();
                slotManager.objectSlots[0].slotInEquipment.countTexte.text = slotManager.objects[0].count.ToString();
            }
            else
            {
                slotManager.objects[0].itemData = null;
                slotManager.objects[0].count = 0;
                slotManager.objectSlots[0].slotItemData = null;

                slotManager.objectSlots[0].slotInEquipment.item = null;
                slotManager.objectSlots[0].slotInEquipment.countTexte.text = "";
            }
        }
        else
        {
            if (slotManager.objects[1].count > 1)
            {
                slotManager.objects[1].count--;
                slotManager.objectSlots[1].countText.text = slotManager.objects[1].count.ToString();
                slotManager.objectSlots[1].slotInEquipment.countTexte.text = slotManager.objects[1].count.ToString();
            }
            else
            {
                slotManager.objects[1].itemData = null;
                slotManager.objects[1].count = 0;
                slotManager.objectSlots[1].slotItemData = null;
                slotManager.objectSlots[1].slotInEquipment.item = null;
                slotManager.objectSlots[1].slotInEquipment.countTexte.text = "";
            }
        }
        slotManager.RefreshAffichage();
    }

    public void RemoveWeapon(int numberOfWeapon)
    {
        if (numberOfWeapon == 1)
        {
            slotManager.weaponSlots[0].slotItemData = null;
            slotManager.weapons[0].itemData = null;
            slotManager.weapons[0].count = 0;
            slotManager.weaponSlots[0].slotInEquipment.item = null;

        }
        else
        {
            slotManager.weaponSlots[1].slotItemData = null;
            slotManager.weapons[1].itemData = null;
            slotManager.weapons[1].count = 0;
            slotManager.weaponSlots[1].slotInEquipment.item = null;
        }
        slotManager.RefreshAffichage();
    }

    public void DisableObject(ItemData item)
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

    private void DesequipCurrentActiveWeapon(int slot, PlayerController player)
    {
        ItemData item = (slot == 1) ? slotManager.weaponSlots[0].slotItemData : slotManager.weaponSlots[1].slotItemData;
        if (item == null) return;

        PaletteSlot slotData = slotManager.weaponSlots[slot - 1];
        player.PendingUnequipType = item.handWeaponType;

        player.StateMachine.ChangeState(PlayerStateType.Unequip);
        slotData.isEquipped = false;

    }
}