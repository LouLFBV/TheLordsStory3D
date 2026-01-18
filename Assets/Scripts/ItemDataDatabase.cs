using UnityEngine;
using System.Collections.Generic;

public class ItemDataDatabase : MonoBehaviour
{
    public List<ItemData> items;

    public ItemData GetItemByID(string id)
    {
        return items.Find(item => item.itemID == id);
    }
}