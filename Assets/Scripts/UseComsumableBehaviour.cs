using UnityEngine;
using System.Linq;

public class UseComsumableBehaviour : MonoBehaviour
{
    [Header ("References")]

    [SerializeField] private Palette palette;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private EquipmentLibrary equipmentLibrary;

    private Animator animator;

    private PlayerControls controls;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        animator = player.GetComponent<Animator>();
        controls = new PlayerControls();
    }
    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();
    // Update is called once per frame
    void Update()
    {
        if (controls.Player.Attack.triggered && CanEat())
        {
            if (palette.isEquippedObject1 && palette.equipmentObject1Item.itemType == ItemType.Consumable)
            {
                UseConsumable(1);
            }
            else if (palette.isEquippedObject2 && palette.equipmentObject2Item.itemType == ItemType.Consumable)
            {
                UseConsumable(2);
            }
        }
    }

    private void UseConsumable(int numberOfObject)
    {
        if (numberOfObject == 1)
        {
            playerStats.ConsumeItem(palette.equipmentObject1Item.healthEffect, palette.equipmentObject1Item.hungerEffect, palette.equipmentObject1Item.thirstEffect);


            ItemInInventory itemInInventory = palette.objects.Where(x=> x.itemData == palette.equipmentObject1Item).First();
            if (itemInInventory.count == 1)
            {
                EquipmentLibraryItem equipmentLibraryItem = equipmentLibrary.content.Where(x => x.itemData == palette.equipmentObject1Item).FirstOrDefault();

                if (equipmentLibraryItem != null)
                {
                    equipmentLibraryItem.itemPrefab.SetActive(false);
                }
                animator.SetBool("CarryingConsumable", false);
                palette.isEquippedObject1 = false;
            }

            palette.RemoveObject(numberOfObject);
        }
        else
        {
            playerStats.ConsumeItem(palette.equipmentObject2Item.healthEffect, palette.equipmentObject2Item.hungerEffect, palette.equipmentObject2Item.thirstEffect);
            


            ItemInInventory itemInInventory = palette.objects.Where(x => x.itemData == palette.equipmentObject2Item).First();
            if (itemInInventory.count == 1)
            {
                EquipmentLibraryItem equipmentLibraryItem = equipmentLibrary.content.Where(x => x.itemData == palette.equipmentObject2Item).FirstOrDefault();

                if (equipmentLibraryItem != null)
                {
                    equipmentLibraryItem.itemPrefab.SetActive(false);
                }
                animator.SetBool("CarryingConsumable", false);
                palette.isEquippedObject2 = false;
            }

            palette.RemoveObject(numberOfObject);
        }
        palette.UpdateImageSeleted();
    }

    private bool CanEat()
    {
        return ((palette.isEquippedObject1 && palette.equipmentObject1Item.itemType == ItemType.Consumable) || (palette.isEquippedObject2 && palette.equipmentObject2Item.itemType == ItemType.Consumable));
    }
}
