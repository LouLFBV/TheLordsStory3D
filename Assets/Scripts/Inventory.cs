using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{

    public static Inventory instance;

    public ItemDataDatabase itemDatabase;


    [Header("Other scripts References")]

    [SerializeField]
    private Equipment equipment;

    [SerializeField]
    private ItemActionsSystem itemActionsSystem;

    [Header("Inventory System Variables")]

    [SerializeField]
    private List<ItemInInventory> content = new List<ItemInInventory>();

    [SerializeField]
    private GameObject inventoryPanel;

    [SerializeField]
    private Transform inventorySlotsParent;

    public Sprite emptySlotVisual;

    [SerializeField] private UINavigationManager navManager;


    const int InventorySize = 32;
    public bool isOpen = false;


    [SerializeField] private PlayerInput playerInput;

    private InputAction inventoryAction;
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
        CloseInventory();
        RefreshContent();
    }
    private void OnEnable()
    {
        inventoryAction = playerInput.actions["Inventory"]; // UI/Inventory

        inventoryAction.performed += OnInventory;
        inventoryAction.Enable();
    }

    private void OnDisable()
    {
        inventoryAction.performed -= OnInventory;
        inventoryAction.Disable();
    }

    private void OnInventory(InputAction.CallbackContext ctx)
    {
        if (!isOpen)
            OpenInventory();
        else
            CloseInventory();
    }

    public void AddItem(ItemData item)
    {
        Debug.Log("Adding item: " + item.itemName);
        if (equipment.arrowItemInInventory.itemData != null)
        {
            Debug.Log("Checking for arrow stacking.");
            if (item.damageType == equipment.arrowItemInInventory.itemData.damageType && item.equipmentType == EquipmentType.Arrow)
            {
                equipment.arrowItemInInventory.count++;
                equipment.UpdateArrowsText();
                BowBehaviour.instance.UpdateQuiverVisual(equipment.arrowItemInInventory.count);
                return;
            }
        }
        ItemInInventory[] itemInInventory = content.Where(i => i.itemData == item).ToArray();

        bool itemAdded = false;

        if (itemInInventory.Length > 0 && item.stackable)
        {
            Debug.Log("Item already in inventory, trying to stack.");
            for (int i = 0; i < itemInInventory.Length; i++)
            {
                if (itemInInventory[i].count < item.maxStack)
                {
                    itemAdded = true;
                    itemInInventory[i].count++;
                    break;
                }
            }

            if (!itemAdded)
            {
                Debug.Log("All stacks full, adding new stack.");
                content.Add(
                    new ItemInInventory
                    {
                        itemData = item,
                        count = 1
                    }
                );
            }
        }
        else
        {
            Debug.Log("Item not in inventory, adding new item.");
            content.Add(
                    new ItemInInventory
                    {
                        itemData = item,
                        count = 1
                    }
                );
        }
        RefreshContent();
    }

    public void RemoveItem(ItemData item)
    {
        ItemInInventory itemInInventory = content.Where(i => i.itemData == item).FirstOrDefault();

        if (itemInInventory != null && itemInInventory.count > 1)
        {
            itemInInventory.count--;
        }
        else
        {
            content.Remove(itemInInventory);
        }
        RefreshContent();
    }

    public List<ItemInInventory> GetContent()
    {
        return content;
    }

    public void SetContent(List<ItemInInventory> newContent)
    {
        content = newContent;
    }

    private void OpenInventory()
    {
        RefreshContent();
        inventoryPanel.SetActive(true);
        isOpen = true;
        Palette.instance.UpdateEquipmentsDesequipButtons();

        if (navManager != null)
        {
            navManager.onCancel = CloseInventory;
        }
    }
    public void CloseInventory()
    {
        if (itemActionsSystem.actionPanel.activeSelf) return;
        inventoryPanel.SetActive(false);
        itemActionsSystem.actionPanel.SetActive(false);

        isOpen = false;
        TooltipSystem.instance.Hide();
        Palette.instance.UpdateEquipmentsDesequipButtons();

        // Retirer l’action pour éviter les callbacks fantômes
        if (navManager != null)
        {
            navManager.onCancel = null;
        }
    }


    public void RefreshContent()
    {
        //On vide tous les slots / visuels
        for (int i = 0; i < inventorySlotsParent.childCount; i++)
        {
            Slot currentSlot = inventorySlotsParent.GetChild(i).GetComponent<Slot>();

            currentSlot.item = null;
            currentSlot.itemVisual.sprite = emptySlotVisual;
            currentSlot.countTexte.enabled = false;
        }

        //On peuple le visuel des slots selon le contenu de l'inventaire
        for (int i = 0; i < content.Count; i++)
        {
            Slot currentSlot = inventorySlotsParent.GetChild(i).GetComponent<Slot>();
            if (content[i].itemData == null)
            {
                RemoveItem(content[i].itemData);
                continue;
            }
            currentSlot.item = content[i].itemData;
            currentSlot.itemVisual.sprite = content[i].itemData.visual;

            if (currentSlot.item.stackable)
            {
                currentSlot.countTexte.text = content[i].count.ToString();
                currentSlot.countTexte.enabled = true;
            }
            equipment.UpdateEquipmentsDesequipButtons();
        }
    }

    public bool IsFull()
    {
        return content.Count == InventorySize;
    }

    public void ClearContent()
    {
        content.Clear();
    }

    public bool KeyIsInInventory(ItemData itemData)
    {
        return content.Any(i => i.itemData == itemData);    
    }

    //public InventorySaveData GetSaveData()
    //{
    //    InventorySaveData data = new InventorySaveData();
    //    data.content = new List<ItemInInventorySave>();

    //    foreach (var item in content)
    //    {
    //        data.content.Add(new ItemInInventorySave
    //        {
    //            itemID = item.itemData.itemID,
    //            count = item.count
    //        });
    //    }

    //    return data;
    //}


    //public void LoadSaveData(InventorySaveData data)
    //{
    //    if (data == null || data.content == null)
    //    {
    //        Debug.LogWarning("InventorySaveData is null");
    //        return;
    //    }

    //    content.Clear();

    //    foreach (var savedItem in data.content)
    //    {
    //        ItemData itemData = itemDatabase.GetItemByID(savedItem.itemID);
    //        if (itemData == null) continue;

    //        content.Add(new ItemInInventory
    //        {
    //            itemData = itemData,
    //            count = savedItem.count
    //        });
    //    }

    //    RefreshContent();
    //}

}

//[System.Serializable]
//public class ItemInInventory
//{
//    public ItemData itemData;
//    public int count;
//}

//[System.Serializable]
//public class ItemInInventorySave
//{
//    public string itemID;
//    public int count;
//}

//[System.Serializable]
//public class InventorySaveData
//{
//    public List<ItemInInventorySave> content;
//}