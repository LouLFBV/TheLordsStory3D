using UnityEngine;
using System.Collections.Generic;

public class ItemDataDatabase : MonoBehaviour
{
    public static ItemDataDatabase Instance;

    public List<ItemData> items;

    private Dictionary<string, ItemData> itemLookup;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        BuildDatabase();
    }

    void BuildDatabase()
    {
        itemLookup = new Dictionary<string, ItemData>();

        foreach (var item in items)
        {
            if (!itemLookup.ContainsKey(item.itemID))
            {
                itemLookup.Add(item.itemID, item);
            }
            else
            {
                Debug.LogWarning($"Duplicate ItemID detected: {item.itemID}");
            }
        }
    }

    public ItemData GetItemByID(string id)
    {
        if (itemLookup.TryGetValue(id, out ItemData item))
        {
            return item;
        }

        Debug.LogWarning($"Item with ID {id} not found in database.");
        return null;
    }
}