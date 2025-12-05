using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{

    public static Inventory instance;


    [Header("Other scripts References")]

    [SerializeField]
    private Equipment equipment;

    [SerializeField]
    private ItemActionsSystem itemActionsSystem;

    [SerializeField]
    private CraftingSystem craftingSystem;

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
        if (equipment.arrowItemInInventory.itemData != null)
        {
            if (item.damageType == equipment.arrowItemInInventory.itemData.damageType)
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

    public void LoadData(List<ItemInInventory> loadedContent)
    {
        content = loadedContent;
        RefreshContent();
    }

    public void ClearContent()
    {
        content.Clear();
    }
}

[System.Serializable]
public class ItemInInventory
{
    public ItemData itemData;
    public int count;
}