using UnityEngine;
using System.Collections.Generic;

public class ItemDataDatabase : MonoBehaviour
{
    public static ItemDataDatabase Instance;

    public List<ItemData> items;


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
    public ItemData GetItemByID(string id)
    {
        return items.Find(item => item.itemID == id);
    }
}