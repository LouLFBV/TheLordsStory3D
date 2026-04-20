using UnityEngine;
using System.Collections.Generic;

public class ForgeronUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EquipmentSystem equipment;
    [SerializeField] private InventorySystem inventory;

    [Header("UI Elements")]
    [SerializeField] private List<SlotForgeronUI> slotForgeronUIs;

    private void Start()
    {
        if (equipment == null)
            equipment = EquipmentSystem.instance;
        if (inventory == null)
            inventory = InventorySystem.instance;
    }

    public void UpdateForgeronUI(EquipmentType equipmentType)
    {
        // Logique pour mettre à jour l'UI du forgeron en fonction du type d'équipement sélectionné
        // Par exemple, afficher les recettes disponibles pour ce type d'équipement
        Debug.Log("Mise à jour de l'UI du forgeron pour le type d'équipement : " + equipmentType);
        // Vous pouvez ajouter ici la logique pour afficher les recettes ou les options de forgeage spécifiques à ce type d'équipement

        List<ItemInInventory> items = inventory.GetContentEquipment();
        GetContentForEquipment(items, equipmentType);

        foreach (var slot in slotForgeronUIs)
        {
            foreach (var item in items)
            {
                if (item.itemData.equipmentType == equipmentType)
                {
                    slot.itemData = item.itemData;
                    slot.equipmentIcone.sprite = item.itemData.visual;
                }
            }
        }
    }
    private List<ItemInInventory> GetContentForEquipment(List<ItemInInventory> items, EquipmentType equipmentType)
    {
        List<ItemInInventory> filteredItems = new List<ItemInInventory>();

        foreach (ItemInInventory item in items)
        {
            if (item.itemData.equipmentType == equipmentType)
            {
                filteredItems.Add(item);
            }
        }
        return filteredItems;
    }
}
