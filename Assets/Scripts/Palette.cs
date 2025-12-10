using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEditorInternal.VersionControl;

public class Palette : MonoBehaviour
{
    public static Palette instance;

    [Header("Other References")]

    [SerializeField] private EquipmentLibrary equipmentLibrary;

    [SerializeField] private Equipment equipmentSystem;

    [SerializeField] private ItemActionsSystem itemActionsSystem;

    [SerializeField] private Animator animator;
    [SerializeField] private InteractBehaviour interactBehaviour;


    [Header("Palette Settings")]

    public ItemInInventory[] weapons = new ItemInInventory[2];

    public ItemInInventory[] objects = new ItemInInventory[2];

    #region Weapons And Objects Data
    [Header("Weapon 1")]

    public Button weapon1SlotDesequipButton;
    public ItemData equipmentWeapon1Item;
    public Image weapon1SlotImage;
    public Image iconeInputWeapon1;
    public bool isEquippedWeapon1 = false;
    public GameObject weapon1ImageSelected;

    [Header("Weapon 2")]

    public Button weapon2SlotDesequipButton;
    public ItemData equipmentWeapon2Item;
    public Image weapon2SlotImage;
    public Image iconeInputWeapon2;
    public bool isEquippedWeapon2 = false;
    public GameObject weapon2ImageSelected;

    [Header("Object 1")]

    public Button object1SlotDesequipButton;
    public ItemData equipmentObject1Item;
    public Image object1SlotImage;
    public Image iconeInputObject1;
    public bool isEquippedObject1 = false;
    public TextMeshProUGUI object1CountText;
    public GameObject object1ImageSelected;


    [Header("Object 2")]

    public Button object2SlotDesequipButton;
    public ItemData equipmentObject2Item;
    public Image object2SlotImage;
    public Image iconeInputObject2;
    public bool isEquippedObject2 = false;
    public TextMeshProUGUI object2CountText;
    public GameObject object2ImageSelected;

    #endregion

    #region PlayerInput

    [SerializeField] private PlayerInput playerInput;
    private bool takingWeapon1;
    private bool takingWeapon2;
    private bool takingObject1;
    private bool takingObject2;
    private DeviceType currentDevice;
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
    void OnEnable()
    {
        playerInput.actions["Weapon1"].Enable();
        playerInput.actions["Weapon1"].performed += OnWeaponPerformed;
        playerInput.actions["Weapon1"].canceled += OnWeaponCanceled;

        playerInput.actions["Weapon2"].Enable();
        playerInput.actions["Weapon2"].performed += OnWeaponPerformed;
        playerInput.actions["Weapon2"].canceled += OnWeaponCanceled;

        playerInput.actions["Object1"].Enable();
        playerInput.actions["Object1"].performed += OnObjectPerformed;
        playerInput.actions["Object1"].canceled += OnObjectCanceled;

        playerInput.actions["Object2"].Enable();
        playerInput.actions["Object2"].performed += OnObjectPerformed;
        playerInput.actions["Object2"].canceled += OnObjectCanceled;


        DeviceWatcher.Instance.OnDeviceChanged += OnDeviceChanged;
    }
    void OnDisable()
    {
        playerInput.actions["Weapon1"].Disable();
        playerInput.actions["Weapon1"].performed -= OnWeaponPerformed;
        playerInput.actions["Weapon1"].canceled -= OnWeaponCanceled;

        playerInput.actions["Weapon2"].Disable();
        playerInput.actions["Weapon2"].performed -= OnWeaponPerformed;
        playerInput.actions["Weapon2"].canceled -= OnWeaponCanceled;

        playerInput.actions["Object1"].Disable();
        playerInput.actions["Object1"].performed -= OnObjectPerformed;
        playerInput.actions["Object1"].canceled -= OnObjectCanceled;

        playerInput.actions["Object2"].Disable();
        playerInput.actions["Object2"].performed -= OnObjectPerformed;
        playerInput.actions["Object2"].canceled -= OnObjectCanceled;

        DeviceWatcher.Instance.OnDeviceChanged -= OnDeviceChanged;
    }

    private void OnWeaponPerformed(InputAction.CallbackContext ctx)
    {
        if (ctx.action.name == "Weapon1") takingWeapon1 = true;
        else if (ctx.action.name == "Weapon2") takingWeapon2 = true;
    }

    private void OnWeaponCanceled(InputAction.CallbackContext ctx)
    {
        if (ctx.action.name == "Weapon1") takingWeapon1 = false;
        else if (ctx.action.name == "Weapon2") takingWeapon2 = false;
    }

    private void OnObjectPerformed(InputAction.CallbackContext ctx)
    {
        if (ctx.action.name == "Object1") takingObject1 = true;
        else if (ctx.action.name == "Object2") takingObject2 = true;
    }

    private void OnObjectCanceled(InputAction.CallbackContext ctx)
    {
        if (ctx.action.name == "Object1") takingObject1 = false;
        else if (ctx.action.name == "Object2") takingObject2 = false;
    }



    private void Start()
    {
        UpdateEquipmentsDesequipButtons();
        currentDevice = Gamepad.current != null ? DeviceType.Gamepad : DeviceType.Keyboard;
        UpdateBindingDisplay();
    }

    void Update()
    {
        if (takingWeapon1 && equipmentWeapon1Item != null && !PlayerStats.instance.isEquiping && !BowBehaviour.instance.chargeBow)
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
                PlayerStats.instance.equipmentToDesequip = equipmentLibraryItem;
                if (equipmentLibraryItem.itemData.handWeaponType == HandWeapon.Bow)
                {
                    animator.SetTrigger("DesequipBow");
                    animator.SetBool("BowEquipped", false);
                }

                isEquippedWeapon1 = false;
                if (equipmentWeapon1Item.handWeaponType == HandWeapon.TwoHanded)
                {
                    animator.SetBool("IsTwoHandedWeapon", false);
                    animator.SetTrigger("DesequipLongSword");
                }
                else if (equipmentWeapon1Item.handWeaponType == HandWeapon.OneHanded)
                {
                    animator.SetBool("IsOneHandedWeapon", false);
                    animator.SetTrigger("DesequipSword");
                }
                UpdateImageSeleted();
            }
            takingWeapon1 = false;
        }
        if (takingWeapon2 && equipmentWeapon2Item != null && !PlayerStats.instance.isEquiping && !BowBehaviour.instance.chargeBow)
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
                PlayerStats.instance.equipmentToDesequip = equipmentLibraryItem;
                if (equipmentLibraryItem.itemData.handWeaponType == HandWeapon.Bow)
                {
                    animator.SetTrigger("DesequipBow");
                    animator.SetBool("BowEquipped", false);
                }

                isEquippedWeapon2 = false;
                if (equipmentWeapon2Item.handWeaponType == HandWeapon.TwoHanded)
                {
                    animator.SetBool("IsTwoHandedWeapon", false);
                    animator.SetTrigger("DesequipLongSword");
                }
                else if (equipmentWeapon2Item.handWeaponType == HandWeapon.OneHanded)
                {
                    animator.SetBool("IsOneHandedWeapon", false);
                    animator.SetTrigger("DesequipSword");
                }
                UpdateImageSeleted();
            }
            takingWeapon2 = false;
        }
        if (takingObject1 && equipmentObject1Item != null && !PlayerStats.instance.isEquiping)
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
            takingObject1 = false;
        }
        if (takingObject2 && equipmentObject2Item != null && !PlayerStats.instance.isEquiping)
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
            takingObject2 = false;
        }

    }

    private void OnDeviceChanged(DeviceType device)
    {
        currentDevice = device;
        UpdateBindingDisplay();
    }

    private void UpdateBindingDisplay()
    {
        InputRebindManager.UpdateBindingDisplayForAction(playerInput.actions["Weapon1"], iconeInputWeapon1, currentDevice);
        InputRebindManager.UpdateBindingDisplayForAction(playerInput.actions["Weapon2"], iconeInputWeapon2, currentDevice);
        InputRebindManager.UpdateBindingDisplayForAction(playerInput.actions["Object1"], iconeInputObject1, currentDevice);
        InputRebindManager.UpdateBindingDisplayForAction(playerInput.actions["Object2"], iconeInputObject2, currentDevice);
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
        UpdateEquipmentsDesequipButtons();
    }

    private void UseWeapon(int slot)
    {
        ItemData itemToEquip = (slot == 1) ? equipmentWeapon1Item : equipmentWeapon2Item;
        Debug.Log("Equip item in palette : " + itemToEquip.name);

        // 1. Désactiver tous les objets
        DisableObject(equipmentObject1Item);
        DisableObject(equipmentObject2Item);

        isEquippedObject1 = false;
        isEquippedObject2 = false;
        animator.SetBool("CarryingConsumable", false);

        // 2. Appliquer le type d’arme
        ApplyWeaponTypeToAnimator(itemToEquip.handWeaponType);

        // 3. Forcer le déséquipement de l'autre weapon
        if (slot == 1 && isEquippedWeapon2)
            ForceDesequipWeapon(equipmentWeapon2Item, ref isEquippedWeapon2);

        if (slot == 2 && isEquippedWeapon1)
            ForceDesequipWeapon(equipmentWeapon1Item, ref isEquippedWeapon1);

        // 4. Équiper maintenant
        EquipmentLibraryItem libItem = equipmentLibrary.content.First(x => x.itemData == itemToEquip);
        interactBehaviour.SetCurrentEquippedItem(libItem);
        PlayerStats.instance.equipmentToEquip = libItem;

        StartCoroutine(EquipAfterDesequip(itemToEquip.handWeaponType, 0.01f));


        UpdateEquipmentsDesequipButtons();
    }



    public void UpdateEquipmentsDesequipButtons()
    {
        weapon2SlotDesequipButton.onClick.RemoveAllListeners();
        weapon2SlotDesequipButton.onClick.AddListener(delegate { DesequipWeapon(2); });
        weapon2SlotDesequipButton.gameObject.SetActive((equipmentWeapon2Item != null && !isEquippedWeapon2) && Inventory.instance.isOpen);

        weapon1SlotDesequipButton.onClick.RemoveAllListeners();
        weapon1SlotDesequipButton.onClick.AddListener(delegate { DesequipWeapon(1); });
        weapon1SlotDesequipButton.gameObject.SetActive((equipmentWeapon1Item != null && !isEquippedWeapon1) && Inventory.instance.isOpen);

        object2SlotDesequipButton.onClick.RemoveAllListeners();
        object2SlotDesequipButton.onClick.AddListener(delegate { DesequipObject(2); });
        object2SlotDesequipButton.gameObject.SetActive((equipmentObject2Item != null && !isEquippedObject2) && Inventory.instance.isOpen);

        object1SlotDesequipButton.onClick.RemoveAllListeners();
        object1SlotDesequipButton.onClick.AddListener(delegate { DesequipObject(1); });
        object1SlotDesequipButton.gameObject.SetActive((equipmentObject1Item != null && !isEquippedObject1) && Inventory.instance.isOpen);
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
            weapon1SlotImage.sprite = Inventory.instance.emptySlotVisual;
            
        }
        else
        {
            equipmentWeapon2Item = null;
            weapon2SlotImage.sprite = Inventory.instance.emptySlotVisual;
        }

        // remettre dans l'inventaire
        Inventory.instance.AddItem(currentItem);
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
        equipmentLibraryItem?.itemPrefab.SetActive(false);
        
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

    private void DisableObject(ItemData item)
    {
        if (item == null) return;

        EquipmentLibraryItem lib = equipmentLibrary.content
            .FirstOrDefault(x => x.itemData == item);

        lib?.itemPrefab.SetActive(false);
    }

    private void ForceDesequipWeapon(ItemData weaponData, ref bool equippedFlag)
    {
        if (weaponData == null) return;

        EquipmentLibraryItem lib = equipmentLibrary.content.First(x => x.itemData == weaponData);

        // Informer PlayerStats
        PlayerStats.instance.equipmentToDesequip = lib;

        // Gérer animations
        switch (weaponData.handWeaponType)
        {
            case HandWeapon.Bow:
                animator.SetTrigger("DesequipBow");
                animator.SetBool("BowEquipped", false);
                break;

            case HandWeapon.TwoHanded:
                animator.SetTrigger("DesequipLongSword");
                animator.SetBool("IsTwoHandedWeapon", false);
                break;

            case HandWeapon.OneHanded:
                animator.SetTrigger("DesequipSword");
                animator.SetBool("IsOneHandedWeapon", false);
                break;
        }
        Debug.LogWarning("Forcing desequip of weapon: " + weaponData.name);
        equippedFlag = false;
    }

    private void ApplyWeaponTypeToAnimator(HandWeapon type)
    {
        animator.SetBool("IsOneHandedWeapon", type == HandWeapon.OneHanded);
        animator.SetBool("IsTwoHandedWeapon", type == HandWeapon.TwoHanded);
    }

    private void PlayEquipAnimation(HandWeapon type)
    {
        Debug.LogWarning("Playing equip animation for weapon type: " + type);

        switch (type)
        {
            case HandWeapon.Bow:
                animator.SetTrigger("EquipBow");
                animator.SetBool("BowEquipped", true);
                break;

            case HandWeapon.TwoHanded:
                animator.SetTrigger("EquipLongSword");
                break;

            case HandWeapon.OneHanded:
                animator.SetTrigger("EquipSword");
                break;
        }
    }

    IEnumerator EquipAfterDesequip(HandWeapon type, float desequipDuration)
    {
        yield return new WaitForSeconds(desequipDuration);
        PlayEquipAnimation(type);
    }

}
