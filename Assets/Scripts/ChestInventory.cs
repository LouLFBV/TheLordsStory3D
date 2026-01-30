using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class ChestInventory : MonoBehaviour
{
    public static ChestInventory Instance;
    
    [Header ("Chest inventory")]

    public Transform inventoryChestSlotsParent;

    public List<ItemInInventory> contentChest = new List<ItemInInventory>();

    [Header("Player Inventory")]

    public Transform inventoryPlayerSlotsParent;

    public List<ItemInInventory> content = new List<ItemInInventory>();


    [Header("Others")]

    [SerializeField] private GameObject objectsToDisable;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void OnEnable()
    {
        objectsToDisable.SetActive(false);
    }

    public void RefreshContentChestInventory()
    {
        //On vide tous les slots / visuels
        for (int i = 0; i < inventoryChestSlotsParent.childCount; i++)
        {
            SlotChest currentSlot = inventoryChestSlotsParent.GetChild(i).GetComponent<SlotChest>();

            currentSlot.item = null;
            currentSlot.itemVisual.sprite = Inventory.instance.emptySlotVisual;
            currentSlot.countTexte.enabled = false;
            currentSlot.desequipButton.gameObject.SetActive(false);
        }

        //On peuple le visuel des slots selon le contenu de l'inventaire
        for (int i = 0; i < contentChest.Count; i++)
        {
            SlotChest currentSlot = inventoryChestSlotsParent.GetChild(i).GetComponent<SlotChest>();
            currentSlot.item = contentChest[i].itemData;
            currentSlot.itemVisual.sprite = contentChest[i].itemData.visual;
            currentSlot.desequipButton.gameObject.SetActive(content.Count != inventoryPlayerSlotsParent.childCount);

            DesequipButtonInventoryChest(currentSlot, i);

            if (currentSlot.item.stackable)
            {
                currentSlot.countTexte.text = contentChest[i].count.ToString();
                currentSlot.countTexte.enabled = true;
            }
        }
    }

    public void RefreshContentInventory()
    {
        content = Inventory.instance.GetContent();
        //On vide tous les slots / visuels
        for (int i = 0; i < inventoryPlayerSlotsParent.childCount; i++)
        {
            SlotChest currentSlot = inventoryPlayerSlotsParent.GetChild(i).GetComponent<SlotChest>();

            currentSlot.item = null;
            currentSlot.itemVisual.sprite = Inventory.instance.emptySlotVisual;
            currentSlot.countTexte.enabled = false;
            currentSlot.desequipButton.gameObject.SetActive(false);
        }

        //On peuple le visuel des slots selon le contenu de l'inventaire
        for (int i = 0; i < content.Count; i++)
        {
            SlotChest currentSlot = inventoryPlayerSlotsParent.GetChild(i).GetComponent<SlotChest>();
            currentSlot.item = content[i].itemData;
            currentSlot.itemVisual.sprite = content[i].itemData.visual;
            currentSlot.desequipButton.gameObject.SetActive(contentChest.Count != inventoryChestSlotsParent.childCount);

            DesepquipButtonInventoryPlayer(currentSlot, i);

            if (currentSlot.item.stackable)
            {
                currentSlot.countTexte.text = content[i].count.ToString();
                currentSlot.countTexte.enabled = true;
            }
        }
    }

    private void DesequipButtonInventoryChest(SlotChest currentSlot, int index)
    {
        currentSlot.desequipButton.onClick.RemoveAllListeners();
        currentSlot.desequipButton.onClick.AddListener(delegate
        {
            Inventory.instance.AddItem(currentSlot.item);
            RemoveFromChest(index);
        });
    }
    
    private void DesepquipButtonInventoryPlayer(SlotChest currentSlot, int index)
    {
        currentSlot.desequipButton.onClick.RemoveAllListeners();
        ItemInInventory item =
                    new ItemInInventory
                    {
                        itemData = currentSlot.item,
                        count = 1
                    };
        currentSlot.desequipButton.onClick.AddListener(delegate
        {
            RemoveFromInventory(index);
            AddToChest(item);
        });
    }
    private void AddToChest(ItemInInventory itemToAdd)
    {
        ItemInInventory[] itemInInventory = contentChest.Where(i => i.itemData == itemToAdd.itemData).ToArray();

        bool itemAdded = false;

        if (itemInInventory.Length > 0 && itemToAdd.itemData.stackable)
        {
            for (int i = 0; i < itemInInventory.Length; i++)
            {
                if (itemInInventory[i].count < itemToAdd.itemData.maxStack)
                {
                    itemAdded = true;
                    itemInInventory[i].count++;
                    break;
                }
            }

            if (!itemAdded)
            {
                contentChest.Add(
                    new ItemInInventory
                    {
                        itemData = itemToAdd.itemData,
                        count = 1
                    }
                );
            }
        }
        else
        {
            contentChest.Add(
                    new ItemInInventory
                    {
                        itemData = itemToAdd.itemData,
                        count = 1
                    }
                );
        }
        RefreshContentChestInventory();
        RefreshContentInventory();
    }
    private void RemoveFromChest(int index)
    {
        ItemInInventory itemInInventory = contentChest[index];

        if (itemInInventory != null && itemInInventory.count > 1)
        {
            itemInInventory.count--;
        }
        else
        {
            contentChest.Remove(itemInInventory);
        }
        RefreshContentChestInventory();
        RefreshContentInventory();
    }

    private void RemoveFromInventory(int index)
    {
        ItemInInventory itemInInventory = content[index];
        if (itemInInventory != null && itemInInventory.count > 1)
        {
            itemInInventory.count--;
        }
        else
        {
            content.Remove(itemInInventory);
        }
        RefreshContentChestInventory();
        RefreshContentInventory();
    }

    public List<ItemInInventory> RefreshItems(List<ItemInInventory> content)
    {
        List<ItemInInventory> refreshedContent = new List<ItemInInventory>();

        foreach (var item in content)
        {
            AddToList(item, refreshedContent);
        }

        return refreshedContent;
    }

    private void AddToList(ItemInInventory itemToAdd, List<ItemInInventory> list)
    {
        if (itemToAdd.itemData.stackable)
        {
            int toAdd = itemToAdd.count;

            foreach (var existing in list.Where(i => i.itemData == itemToAdd.itemData))
            {
                int space = itemToAdd.itemData.maxStack - existing.count;
                int amount = Mathf.Min(space, toAdd);
                existing.count += amount;
                toAdd -= amount;

                if (toAdd <= 0)
                    break;
            }

            while (toAdd > 0)
            {
                int amount = Mathf.Min(itemToAdd.itemData.maxStack, toAdd);
                list.Add(new ItemInInventory
                {
                    itemData = itemToAdd.itemData,
                    count = amount
                });
                toAdd -= amount;
            }
        }
        else
        {
            for (int i = 0; i < itemToAdd.count; i++)
            {
                list.Add(new ItemInInventory
                {
                    itemData = itemToAdd.itemData,
                    count = 1
                });
            }
        }
    }

    public ChestInventoryData GetSaveData()
    {
        ChestInventoryData data = new ChestInventoryData();
        data.items = new List<ItemInInventory>();
        foreach (var item in contentChest)
        {
            data.items.Add(new ItemInInventory
            {
                itemData = item.itemData,
                count = item.count
            });
        }
        return data;
    }

    public ChestInventoryData LoadSavaData(ChestInventoryData chestInventoryData)
    {
        ChestInventoryData data = new ChestInventoryData();
        data.items = new List<ItemInInventory>();
        foreach (var item in chestInventoryData.items)
        {
            data.items.Add(new ItemInInventory
            {
                itemData = item.itemData,
                count = item.count
            });
        }
        return data;
    }
}

[System.Serializable]
public class ChestInventoryData
{
    public List<ItemInInventory> items = new List<ItemInInventory>();
}
