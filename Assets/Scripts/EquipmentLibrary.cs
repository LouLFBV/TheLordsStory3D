using UnityEngine;
using System.Collections.Generic;

public class EquipmentLibrary : MonoBehaviour
{
    public List<EquipmentLibraryItem> content = new List<EquipmentLibraryItem>();
    private Dictionary<ItemData, EquipmentLibraryItem> lookup;

    void Awake()
    {
        lookup = new Dictionary<ItemData, EquipmentLibraryItem>();

        foreach (var item in content)
        {
            lookup[item.itemData] = item;
        }
    }

    public EquipmentLibraryItem Get(ItemData item)
    {
        lookup.TryGetValue(item, out var result);
        return result;
    }
}

[System.Serializable]
public class EquipmentLibraryItem
{
    public ItemData itemData;
    public GameObject itemPrefab;
    public GameObject[] elementsToDisable;
}