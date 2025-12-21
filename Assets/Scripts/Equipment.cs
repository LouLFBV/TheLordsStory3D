using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Equipment : MonoBehaviour
{
    public static Equipment instance;

    [Header("Other Scripts References")]

    [SerializeField]private ItemActionsSystem itemActionsSystem;

    [SerializeField]private PlayerStats playerStats;

    [SerializeField] private Animator animator;

    [SerializeField] private Palette palette;

    [Header("Equipment Panel References")]

    [SerializeField]
    private EquipmentLibrary equipmentLibrary;

    [SerializeField]
    private Image headSlotImage, chestSlotImage, handsSlotImage, legslotImage, feetSlotImage, arrowSlotImage;

    [HideInInspector]
    public ItemData equipmentHeadItem, equipmentChestItem, equipmentHandsItem, equipmentLegsItem, equipmentFeetItem;

    [HideInInspector]
    public ItemInInventory arrowItemInInventory;

    [SerializeField] private TextMeshProUGUI arrowText;

    [SerializeField]
    private Button headSlotDesequipButton, chestSlotDesequipButton, handsSlotDesequipButton, legsSlotDesequipButton, feetSlotDesequipButton, arrowSlotDesequipButton;

    [HideInInspector]
    public AudioSource audioSource;

    [HideInInspector]
    public AudioClip equipSound;

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
    private void Start()
    {
        animator = playerStats.GetComponent<Animator>();
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
        switch (itemToDisable.armorType)
        {
            case DamageType.Percant:
                playerStats.currentArmourPointsPercant -= itemToDisable.armorPoints;
                break;
            case DamageType.Contendant:
                playerStats.currentArmourPointsContendant -= itemToDisable.armorPoints;
                break;
            case DamageType.Tranchant:
                playerStats.currentArmourPointsTranchant -= itemToDisable.armorPoints;
                break;
            case DamageType.Feu:
                playerStats.currentArmourPointsFire -= itemToDisable.armorPoints;
                break;
            case DamageType.Glace:
                playerStats.currentArmourPointsIce -= itemToDisable.armorPoints;
                break;
            case DamageType.Foudre:
                playerStats.currentArmourPointsElectric -= itemToDisable.armorPoints;
                break;
        }
        if (itemToDisable.equipmentType == EquipmentType.Arrow)
        {
            if (arrowItemInInventory.count > 0)
            {
                int count = arrowItemInInventory.count;
                arrowItemInInventory.count = 0;
                arrowItemInInventory.itemData = null;
                for (int i = 0; i < count; i++)
                {
                    Inventory.instance.AddItem(itemToDisable);
                }
            }
        }
        else
            Inventory.instance.AddItem(itemToDisable);
    }

    public void DesequipEquipment(EquipmentType equipmentType)
    {
        if (Inventory.instance.IsFull())
        {
            Debug.LogWarning("Cannot desequip item, inventory is full.");
            return;
        }

        ItemData currentItem = null;

        switch (equipmentType)
        {
            case EquipmentType.Head:
                currentItem = equipmentHeadItem;
                headSlotImage.sprite = Inventory.instance.emptySlotVisual;
                equipmentHeadItem = null;
                break;
            case EquipmentType.Chest:
                currentItem = equipmentChestItem;
                chestSlotImage.sprite = Inventory.instance.emptySlotVisual;
                equipmentChestItem = null;
                break;
            case EquipmentType.Hands:
                currentItem = equipmentHandsItem;
                handsSlotImage.sprite = Inventory.instance.emptySlotVisual;
                equipmentHandsItem = null;
                break;
            case EquipmentType.Legs:
                currentItem = equipmentLegsItem;
                legslotImage.sprite = Inventory.instance.emptySlotVisual;
                equipmentLegsItem = null;
                break;
            case EquipmentType.Feet:
                currentItem = equipmentFeetItem;
                feetSlotImage.sprite = Inventory.instance.emptySlotVisual;
                equipmentFeetItem = null;
                break;
            case EquipmentType.Arrow:
                currentItem = arrowItemInInventory.itemData;
                arrowSlotImage.sprite = Inventory.instance.emptySlotVisual;
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
        if (currentItem.handWeaponType == HandWeapon.TwoHanded)
        {
            animator.SetBool("IsTwoHandedWeapon", false);
        }
        if (currentItem)
        {
            switch (currentItem.armorType)
            {
                case DamageType.Percant:
                    playerStats.currentArmourPointsPercant -= currentItem.armorPoints;
                    break;
                case DamageType.Contendant:
                    playerStats.currentArmourPointsContendant -= currentItem.armorPoints;
                    break;
                case DamageType.Tranchant:
                    playerStats.currentArmourPointsTranchant -= currentItem.armorPoints;
                    break;
                case DamageType.Feu:
                    playerStats.currentArmourPointsFire -= currentItem.armorPoints;
                    break;
                case DamageType.Glace:
                    playerStats.currentArmourPointsIce -= currentItem.armorPoints;
                    break;
                case DamageType.Foudre:
                    playerStats.currentArmourPointsElectric -= currentItem.armorPoints;
                    break;
            }
            playerStats.UpddateArmorText();
            if (currentItem.equipmentType == EquipmentType.Arrow )
            {
                if (arrowItemInInventory.count > 0)
                {
                    for (int i = 0; i < arrowItemInInventory.count; i++)
                    {
                        Inventory.instance.AddItem(currentItem);
                    }
                }
                UpdateArrowsText();
            }
            else
                Inventory.instance.AddItem(currentItem);
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
                    break;
                case EquipmentType.Arrow:
                    DisablePreviousEquipedEquipment(arrowItemInInventory.itemData);
                    arrowSlotImage.sprite = itemToEquip.visual;
                    arrowItemInInventory.itemData = itemToEquip;
                    arrowItemInInventory.count = Inventory.instance.GetContent().Find(x=>x.itemData == itemToEquip).count;
                    for (int i = 0; i < arrowItemInInventory.count; i++)
                    {
                        Inventory.instance.RemoveItem(itemToEquip);
                    }
                    UpdateArrowsText();
                    ActiveItemVisuel(equipmentLibraryItem);
                    BowBehaviour.instance.UpdateQuiverVisual(arrowItemInInventory.count);
                    break;
            }
            if (itemToEquip.itemType == ItemType.Consumable || itemToEquip.itemType == ItemType.Key || itemToEquip.itemType == ItemType.QuestItem)
            {
                palette.AddObject(itemToEquip);
            }

            UpdateArmorText(itemToEquip);
            if (itemToEquip.equipmentType != EquipmentType.Arrow)
                 Inventory.instance.RemoveItem(itemToEquip);
            audioSource.PlayOneShot(equipSound);
        }
        else
        {
            Debug.LogWarning("Item not found in equipment library: " + itemToEquip.name);
        }

        itemActionsSystem.CloseActionPanel();
        UpdateEquipmentsDesequipButtons();
    }

    public void LoadEquipments(ItemData[] savedEquiments)
    {
        Inventory.instance.ClearContent();
        foreach (EquipmentType type in System.Enum.GetValues(typeof(EquipmentType)))
        {
            DesequipEquipment(type);
        }

        foreach (ItemData item in savedEquiments)
        {
            if (item)
            {
                EquipAction(item);
            }
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

    public void UpdateArmorText(ItemData produit)
    {
        switch (produit.armorType)
        {
            case DamageType.Percant:
                playerStats.currentArmourPointsPercant += produit.armorPoints;
                break;
            case DamageType.Contendant:
                playerStats.currentArmourPointsContendant += produit.armorPoints;
                break;
            case DamageType.Tranchant:
                playerStats.currentArmourPointsTranchant += produit.armorPoints;
                break;
            case DamageType.Feu:
                playerStats.currentArmourPointsFire += produit.armorPoints;
                break;
            case DamageType.Glace:
                playerStats.currentArmourPointsIce += produit.armorPoints;
                break;
            case DamageType.Foudre:
                playerStats.currentArmourPointsElectric += produit.armorPoints;
                break;
        }
        playerStats.UpddateArmorText();
    }
}
