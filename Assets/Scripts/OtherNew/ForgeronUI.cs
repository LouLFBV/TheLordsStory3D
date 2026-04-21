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
        items = GetContentForEquipment(items, equipmentType);

        CleanForgeronUI();
        //foreach (var slot in slotForgeronUIs)
        for (int i = 0; i < slotForgeronUIs.Count; i++)
        {
            var slot = slotForgeronUIs[i];
            if (i >= items.Count)
            {
                Debug.Log("Pas assez d'items pour remplir tous les slots du forgeron UI");
                slot.itemData = null;
                slot.equipmentIcone.enabled = false;
                slot.equipmentIcone.sprite = null;
            }
            else
            {
                Debug.Log("Remplissage du slot " + i + " avec l'item : " + items[i].itemData.itemName);
                slot.itemData = items[i].itemData;
                slot.equipmentIcone.enabled = true;
                slot.equipmentIcone.sprite = items[i].itemData.visual;
            }
        }
    }
    private void CleanForgeronUI()
    {
        foreach (var slot in slotForgeronUIs)
        {
            slot.itemData = null;
            slot.equipmentIcone.sprite = null;
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
