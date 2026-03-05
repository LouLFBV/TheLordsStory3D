using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;

public class InventorySystem : MonoBehaviour
{

    public static InventorySystem instance;

    public ItemDataDatabase itemDatabase;


    [Header("Other scripts References")]
    [SerializeField] private EquipmentSystem equipment;
    [SerializeField] private NewItemActionsSystem itemActionsSystem;
    [SerializeField] private PlayerController player;

    [Header("Inventory System Variables")]
    [SerializeField] private List<ItemInInventory> content = new List<ItemInInventory>();
    [SerializeField] private GameObject inventoryPanel;

    [SerializeField] private Transform inventoryRessourcesSlotsParent;

    [SerializeField] private Transform inventoryEquipmentSlotsParent;

    [SerializeField] private Transform inventoryCraftSlotsParent;

    public Sprite emptySlotVisual;

    [SerializeField] private UINavigationManager navManager;


    const int InventorySize = 32;

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
        //CloseInventory();
        RefreshContent();
    }


    public void OpenInventory()
    {
        inventoryPanel.SetActive(true);
        player.StateMachine.ChangeState(PlayerStateType.UI);
        RefreshContent();

        // On force la State Machine à passer dans un état "Menu" ou "Idle" 
        // pour empêcher le joueur de frapper/courir pendant qu'il trie ses objets
        //player.StateMachine.ChangeState(PlayerStateType.Idle);

        // Si tu as un état spécifique "UI" ou "Pause", c'est encore mieux :
        // player.StateMachine.ChangeState(PlayerStateType.InventoryOpen);

        PaletteSystem.instance.UpdateEquipmentsDesequipButtons();

        if (navManager != null) navManager.onCancel = CloseInventory;
    }

    public void CloseInventory()
    {
        if (itemActionsSystem.actionPanel.activeSelf) return;

        inventoryPanel.SetActive(false); 
        itemActionsSystem.actionPanel.SetActive(false);
        TooltipSystem.instance.Hide();

        PaletteSystem.instance.UpdateEquipmentsDesequipButtons();

        if (navManager != null) navManager.onCancel = null;
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


    public void RefreshContent()
    {
        RefreshCraftContent();
        RefreshRessourcesContent();
        RefreshEquipmentContent();
    }
    public void RefreshCraftContent()
    {
        //On vide tous les slots / visuels
        for (int i = 0; i < inventoryCraftSlotsParent.childCount; i++)
        {
            Slot currentSlot = inventoryCraftSlotsParent.GetChild(i).GetComponent<Slot>();

            currentSlot.item = null;
            currentSlot.itemVisual.sprite = emptySlotVisual;
            currentSlot.countTexte.enabled = false;
        }

        //On peuple le visuel des slots selon le contenu de l'inventaire
        for (int i = 0; i < content.Count; i++)
        {
            Slot currentSlot = inventoryCraftSlotsParent.GetChild(i).GetComponent<Slot>();
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
        }
        equipment.UpdateEquipmentsDesequipButtons();
    }
    public void RefreshRessourcesContent()
    {
        //On vide tous les slots / visuels
        for (int i = 0; i < inventoryRessourcesSlotsParent.childCount; i++)
        {
            Slot currentSlot = inventoryRessourcesSlotsParent.GetChild(i).GetComponent<Slot>();

            currentSlot.item = null;
            currentSlot.itemVisual.sprite = emptySlotVisual;
            currentSlot.countTexte.enabled = false;
        }

        //On peuple le visuel des slots selon le contenu de l'inventaire
        for (int i = 0; i < content.Count; i++)
        {
            Slot currentSlot = inventoryRessourcesSlotsParent.GetChild(i).GetComponent<Slot>();
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
        }
        equipment.UpdateEquipmentsDesequipButtons();
    }
    public void RefreshEquipmentContent()
    {
        //On vide tous les slots / visuels
        for (int i = 0; i < inventoryEquipmentSlotsParent.childCount; i++)
        {
            Slot currentSlot = inventoryEquipmentSlotsParent.GetChild(i).GetComponent<Slot>();

            currentSlot.item = null;
            currentSlot.itemVisual.sprite = emptySlotVisual;
            currentSlot.countTexte.enabled = false;
        }

        //On peuple le visuel des slots selon le contenu de l'inventaire
        for (int i = 0; i < content.Count; i++)
        {
            Slot currentSlot = inventoryEquipmentSlotsParent.GetChild(i).GetComponent<Slot>();
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
        }
        equipment.UpdateEquipmentsDesequipButtons();
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

    #region SaveSystem
    public InventorySaveData GetSaveData()
    {
        InventorySaveData data = new InventorySaveData();
        data.content = new List<ItemInInventorySave>();

        foreach (var item in content)
        {
            data.content.Add(new ItemInInventorySave
            {
                itemID = item.itemData.itemID,
                count = item.count
            });
        }

        return data;
    }


    public void LoadSaveData(InventorySaveData data)
    {
        if (data == null || data.content == null)
        {
            Debug.LogWarning("InventorySaveData is null");
            return;
        }

        content.Clear();

        foreach (var savedItem in data.content)
        {
            ItemData itemData = itemDatabase.GetItemByID(savedItem.itemID);
            if (itemData == null) continue;

            content.Add(new ItemInInventory
            {
                itemData = itemData,
                count = savedItem.count
            });
        }

        RefreshContent();
    }
    #endregion
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