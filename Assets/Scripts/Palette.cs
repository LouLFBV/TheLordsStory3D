using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class Palette : MonoBehaviour
{
    public static Palette instance;

    [Header("Other References")]

    [SerializeField] private EquipmentLibrary equipmentLibrary;

    [SerializeField] private Equipment equipmentSystem;

    [SerializeField] private ItemActionsSystem itemActionsSystem;

    [SerializeField] private Animator animator;
    

    [Header("Palette Settings")]

    public ItemInInventory[] weapons = new ItemInInventory[2];

    public ItemInInventory[] objects = new ItemInInventory[2];

    [SerializeField] private KeyCode touchePourArme1, touchePourArme2, touchePourObjet1, touchePourObjet2;


    [Header("Weapon 1")]
        
    public Button weapon1SlotDesequipButton;
    public ItemData equipmentWeapon1Item;
    public Image weapon1SlotImage;
    public TextMeshProUGUI weapon1Text;
    public bool isEquippedWeapon1 = false;
    public GameObject weapon1ImageSelected;

    [Header("Weapon 2")]

    public Button weapon2SlotDesequipButton;
    public ItemData equipmentWeapon2Item;
    public Image weapon2SlotImage;
    public TextMeshProUGUI weapon2Text;
    public bool isEquippedWeapon2 = false;
    public GameObject weapon2ImageSelected;

    [Header("Object 1")]

    public Button object1SlotDesequipButton;
    public ItemData equipmentObject1Item;
    public Image object1SlotImage;
    public TextMeshProUGUI object1Text;
    public bool isEquippedObject1 = false;
    public TextMeshProUGUI object1CountText;
    public GameObject object1ImageSelected;


    [Header("Object 2")]

    public Button object2SlotDesequipButton;
    public ItemData equipmentObject2Item;
    public Image object2SlotImage;
    public TextMeshProUGUI object2Text;
    public bool isEquippedObject2 = false;
    public TextMeshProUGUI object2CountText;
    public GameObject object2ImageSelected;

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
    void Start()
    {
        weapon1Text.text = touchePourArme1.ToString();
        weapon2Text.text = touchePourArme2.ToString();
        object1Text.text = touchePourObjet1.ToString();
        object2Text.text = touchePourObjet2.ToString();
        UpdateEquipmentsDesequipButtons();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(touchePourArme1) && equipmentWeapon1Item != null && !PlayerStats.instance.isEquiping && !BowBehaviour.instance.chargeBow)
        {
            if (!isEquippedWeapon1)
            {
                isEquippedWeapon1 = true;
                UseWeapon(1);
                UpdateImageSeleted();
            }
            else
            {
                EquipmentLibraryItem equipmentLibraryItem = equipmentLibrary.content.Where(x => x.itemData == equipmentWeapon1Item).First();
                if (equipmentLibraryItem.itemData.handWeaponType == HandWeapon.Bow)
                {
                    PlayerStats.instance.equipmentToDesequip = equipmentLibraryItem;
                    animator.SetTrigger("DesequipBow");
                    animator.SetBool("BowEquipped", false);
                }
                else
                    equipmentLibraryItem.itemPrefab.SetActive(false);

                isEquippedWeapon1 = false;
                if (equipmentWeapon1Item.handWeaponType == HandWeapon.TwoHanded)
                {
                    animator.SetBool("IsTwoHandedWeapon", false);
                }
                else if (equipmentWeapon1Item.handWeaponType == HandWeapon.OneHanded)
                {
                    animator.SetBool("IsOneHandedWeapon", false);
                }
                UpdateImageSeleted();
            }
        }
        if (Input.GetKeyDown(touchePourArme2) && equipmentWeapon2Item != null && !PlayerStats.instance.isEquiping && !BowBehaviour.instance.chargeBow)
        {
            if (!isEquippedWeapon2)
            {
                isEquippedWeapon2 = true;
                UseWeapon(2);
                UpdateImageSeleted();
            }
            else
            {
                EquipmentLibraryItem equipmentLibraryItem = equipmentLibrary.content.Where(x => x.itemData == equipmentWeapon2Item).First();
                if (equipmentLibraryItem.itemData.handWeaponType == HandWeapon.Bow)
                {
                    PlayerStats.instance.equipmentToDesequip = equipmentLibraryItem;
                    animator.SetTrigger("DesequipBow");
                    animator.SetBool("BowEquipped", false);
                }
                else
                    equipmentLibraryItem.itemPrefab.SetActive(false);

                isEquippedWeapon2 = false;
                if (equipmentWeapon2Item.handWeaponType == HandWeapon.TwoHanded)
                {
                    animator.SetBool("IsTwoHandedWeapon", false);
                }
                else if (equipmentWeapon2Item.handWeaponType == HandWeapon.OneHanded)
                {
                    animator.SetBool("IsOneHandedWeapon", false);
                }
                UpdateImageSeleted();
            }
        }
        if (Input.GetKeyDown(touchePourObjet1) && equipmentObject1Item != null && !PlayerStats.instance.isEquiping)
        {
            if (!isEquippedObject1)
            {
                isEquippedObject1 = true;
                isEquippedObject2 = false;
                UseObject(1);
                UpdateImageSeleted();
            }
            else
            {
                EquipmentLibraryItem equipmentLibraryItem = equipmentLibrary.content.Where(x => x.itemData == equipmentObject1Item).First();
                equipmentLibraryItem.itemPrefab.SetActive(false);
                isEquippedObject1 = false;
                animator.SetBool("CarryingConsumable", false);
                UpdateImageSeleted();
            }
        }
        if (Input.GetKeyDown(touchePourObjet2) && equipmentObject2Item != null && !PlayerStats.instance.isEquiping)
        {
            if (!isEquippedObject2)
            {
                isEquippedObject2 = true;
                isEquippedObject1 = false;
                UseObject(2);
                UpdateImageSeleted();
            }
            else
            {
                EquipmentLibraryItem equipmentLibraryItem = equipmentLibrary.content.Where(x => x.itemData == equipmentObject2Item).First();
                equipmentLibraryItem.itemPrefab.SetActive(false);
                isEquippedObject2 = false;
                animator.SetBool("CarryingConsumable", false);
                UpdateImageSeleted();
            }
        }
    }

    private void UseObject(int numberOfObject)
    {
        isEquippedWeapon1 = false;
        isEquippedWeapon2 = false;
        animator.SetBool("CarryingConsumable", true);
        animator.SetBool("IsTwoHandedWeapon", false);
        animator.SetBool("IsOneHandedWeapon", false);
        if (numberOfObject == 1)
        {
            EquipmentLibraryItem equipmentLibraryItem1 = equipmentLibrary.content.Where(x => x.itemData == equipmentObject1Item).First();
            equipmentLibraryItem1.itemPrefab.SetActive(true);


            EquipmentLibraryItem equipmentLibraryItem2 = equipmentLibrary.content.Where(x => x.itemData == equipmentObject2Item).FirstOrDefault();
            if (equipmentLibraryItem2 != null && equipmentLibraryItem2.itemData != objects[0].itemData) equipmentLibraryItem2.itemPrefab.SetActive(false);


            EquipmentLibraryItem equipmentLibraryWeapon1 = equipmentLibrary.content.Where(x => x.itemData == equipmentWeapon1Item).FirstOrDefault();
            if (equipmentLibraryWeapon1 != null) equipmentLibraryWeapon1.itemPrefab.SetActive(false);

            EquipmentLibraryItem equipmentLibraryWeapon2 = equipmentLibrary.content.Where(x => x.itemData == equipmentWeapon2Item).FirstOrDefault();
            if (equipmentLibraryWeapon2 != null) equipmentLibraryWeapon2.itemPrefab.SetActive(false);

        }
        else
        {
            EquipmentLibraryItem equipmentLibraryItem2 = equipmentLibrary.content.Where(x => x.itemData == equipmentObject2Item).First();
            equipmentLibraryItem2.itemPrefab.SetActive(true);


            EquipmentLibraryItem equipmentLibraryItem1 = equipmentLibrary.content.Where(x => x.itemData == equipmentObject1Item).FirstOrDefault();
            if (equipmentLibraryItem1 != null && equipmentLibraryItem1.itemData != objects[1].itemData)equipmentLibraryItem1.itemPrefab.SetActive(false);

            EquipmentLibraryItem equipmentLibraryWeapon1 = equipmentLibrary.content.Where(x => x.itemData == equipmentWeapon1Item).FirstOrDefault();
            if (equipmentLibraryWeapon1 != null) equipmentLibraryWeapon1.itemPrefab.SetActive(false);

            EquipmentLibraryItem equipmentLibraryWeapon2 = equipmentLibrary.content.Where(x => x.itemData == equipmentWeapon2Item).FirstOrDefault();
            if (equipmentLibraryWeapon2 != null) equipmentLibraryWeapon2.itemPrefab.SetActive(false);
        }
    }

    private void UseWeapon(int numberOfWeapon)
    {
        isEquippedObject1 = false;
        isEquippedObject2 = false;
        animator.SetBool("CarryingConsumable", false);
        ItemData itemToEquip = null;
        if (numberOfWeapon == 1)
        {
            itemToEquip = equipmentWeapon1Item;
            if (equipmentWeapon1Item.handWeaponType == HandWeapon.TwoHanded)
            {
                animator.SetBool("IsTwoHandedWeapon", true);
            }
            else if (equipmentWeapon1Item.handWeaponType == HandWeapon.OneHanded)
            {
                animator.SetBool("IsOneHandedWeapon", true);
            }
            else
            {
                animator.SetBool("IsOneHandedWeapon", false);
                animator.SetBool("IsTwoHandedWeapon", false);
            }
        }
        else
        {
            itemToEquip = equipmentWeapon2Item;
            if (equipmentWeapon2Item.handWeaponType == HandWeapon.TwoHanded)
            {
                animator.SetBool("IsTwoHandedWeapon", true);
            }
            else if (equipmentWeapon2Item.handWeaponType == HandWeapon.OneHanded)
            {
                animator.SetBool("IsOneHandedWeapon", true);
            }
            else
            {
                animator.SetBool("IsOneHandedWeapon", false);
                animator.SetBool("IsTwoHandedWeapon", false);
            }
        }
        print("Equip item in palette : " + itemToEquip.name);

        
        //DESEQUIP
        if(itemToEquip == equipmentWeapon1Item && equipmentWeapon2Item != null)
        {
            EquipmentLibraryItem equipmentLibraryItemToDesequip = equipmentLibrary.content.Where(x => x.itemData == equipmentWeapon2Item).First();
            if (equipmentLibraryItemToDesequip.itemData.handWeaponType == HandWeapon.Bow && isEquippedWeapon2)
            {
                PlayerStats.instance.equipmentToDesequip = equipmentLibraryItemToDesequip;
                animator.SetTrigger("DesequipBow");
                animator.SetBool("BowEquipped", false);
            }
            else
                 equipmentLibraryItemToDesequip.itemPrefab.SetActive(false);
            isEquippedWeapon2 = false;
        }
        else if (itemToEquip == equipmentWeapon2Item && equipmentWeapon1Item != null)
        {
            EquipmentLibraryItem equipmentLibraryItemToDesequip = equipmentLibrary.content.Where(x => x.itemData == equipmentWeapon1Item).First();
            if (equipmentLibraryItemToDesequip.itemData.handWeaponType == HandWeapon.Bow && isEquippedWeapon1)
            {
                PlayerStats.instance.equipmentToDesequip = equipmentLibraryItemToDesequip;
                animator.SetTrigger("DesequipBow");
                animator.SetBool("BowEquipped", false);
            }
            else
                 equipmentLibraryItemToDesequip.itemPrefab.SetActive(false);
            isEquippedWeapon1 = false;
        }

        //OBJECTS
        EquipmentLibraryItem equipmentLibraryItem1 = equipmentLibrary.content.Where(x => x.itemData == equipmentObject1Item).FirstOrDefault();
        if (equipmentLibraryItem1 != null)
        {
            equipmentLibraryItem1.itemPrefab.SetActive(false);
        }

        EquipmentLibraryItem equipmentLibraryItem2 = equipmentLibrary.content.Where(x => x.itemData == equipmentObject2Item).FirstOrDefault();
        if (equipmentLibraryItem2 != null) 
        {
            
            equipmentLibraryItem2.itemPrefab.SetActive(false);
        }

        //EQUIP
        EquipmentLibraryItem equipmentLibraryItem = equipmentLibrary.content.Where(x => x.itemData == itemToEquip).First();
        if (equipmentLibraryItem.itemData.handWeaponType == HandWeapon.Bow)
        {
            PlayerStats.instance.equipmentToEquip = equipmentLibraryItem;
            animator.SetTrigger("EquipBow");
            animator.SetBool("BowEquipped", true);
        }
        else
            equipmentLibraryItem.itemPrefab.SetActive(true);
    }

    public void UpdateEquipmentsDesequipButtons()
    {
        weapon2SlotDesequipButton.onClick.RemoveAllListeners();
        weapon2SlotDesequipButton.onClick.AddListener(delegate { DesequipWeapon(2); });
        weapon2SlotDesequipButton.gameObject.SetActive(equipmentWeapon2Item != null && !isEquippedWeapon2);

        weapon1SlotDesequipButton.onClick.RemoveAllListeners();
        weapon1SlotDesequipButton.onClick.AddListener(delegate { DesequipWeapon(1); });
        weapon1SlotDesequipButton.gameObject.SetActive(equipmentWeapon1Item != null && !isEquippedWeapon1);

        object2SlotDesequipButton.onClick.RemoveAllListeners();
        object2SlotDesequipButton.onClick.AddListener(delegate { DesequipObject(2); });
        object2SlotDesequipButton.gameObject.SetActive(equipmentObject2Item != null && !isEquippedObject2);

        object1SlotDesequipButton.onClick.RemoveAllListeners();
        object1SlotDesequipButton.onClick.AddListener(delegate { DesequipObject(1); });
        object1SlotDesequipButton.gameObject.SetActive(equipmentObject1Item != null && !isEquippedObject1);
    }

    public void DesequipWeapon(int numberOfWeapon)
    {
        if (Inventory.instance.IsFull())
        {
            Debug.LogWarning("Cannot desequip item, inventory is full.");
            return;
        }

        ItemData currentItem = null;
        if(numberOfWeapon == 1)
        {
            currentItem = equipmentWeapon1Item;
            weapon1SlotImage.sprite = Inventory.instance.emptySlotVisual;
            equipmentWeapon1Item = null;
            isEquippedWeapon1 = false;
            RemoveWeapon(1);
        }
        else
        {
            currentItem = equipmentWeapon2Item;
            weapon2SlotImage.sprite = Inventory.instance.emptySlotVisual;
            equipmentWeapon2Item = null;
            isEquippedWeapon2 = false;
            RemoveWeapon(2);
        }
        

        EquipmentLibraryItem equipmentLibraryItem = equipmentLibrary.content.Where(x => x.itemData == currentItem).FirstOrDefault();
        if (currentItem.handWeaponType == HandWeapon.TwoHanded && equipmentLibraryItem.itemPrefab.activeSelf)
        {
            animator.SetBool("IsTwoHandedWeapon", false);
        }
        else if (currentItem.handWeaponType == HandWeapon.OneHanded && equipmentLibraryItem.itemPrefab.activeSelf)
        {
            animator.SetBool("IsOneHandedWeapon", false);
        }

        if (equipmentLibraryItem != null)
        {
            equipmentLibraryItem.itemPrefab.SetActive(false);
        }
       
        if (currentItem)
        {
            Inventory.instance.AddItem(currentItem);
        }
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
            object1SlotImage.sprite = Inventory.instance.emptySlotVisual;
            isEquippedObject1 = false;
        }
        else
        {
            currentItem = equipmentObject2Item;
            object2SlotImage.sprite = Inventory.instance.emptySlotVisual;
            isEquippedObject2 = false;
        }



        EquipmentLibraryItem equipmentLibraryItem = equipmentLibrary.content.Where(x => x.itemData == currentItem).FirstOrDefault();

        if (equipmentLibraryItem != null)
        {
            equipmentLibraryItem.itemPrefab.SetActive(false);
        }
        if (currentItem)
        {
            Inventory.instance.AddItem(currentItem);
        }
        RemoveObject(numberOfObject);
        RefreshAffichage();
        UpdateImageSeleted();
    }

    public void RefreshAffichage()
    {
        weapon1SlotImage.sprite = equipmentWeapon1Item ? equipmentWeapon1Item.visual : Inventory.instance.emptySlotVisual;
        weapon2SlotImage.sprite = equipmentWeapon2Item ? equipmentWeapon2Item.visual : Inventory.instance.emptySlotVisual;

        object1SlotImage.sprite = equipmentObject1Item ? equipmentObject1Item.visual : Inventory.instance.emptySlotVisual;
        object1CountText.gameObject.SetActive(equipmentObject1Item);

        object2SlotImage.sprite = equipmentObject2Item ? equipmentObject2Item.visual : Inventory.instance.emptySlotVisual;
        object2CountText.gameObject.SetActive(equipmentObject2Item);

        UpdateEquipmentsDesequipButtons();
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
        UpdateEquipmentsDesequipButtons();
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
}
