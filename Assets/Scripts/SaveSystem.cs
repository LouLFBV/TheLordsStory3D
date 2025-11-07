using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class SaveSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Transform playerTransform;

    [SerializeField]
    private Equipment equipmentSystem;

    [SerializeField]
    private PlayerStats playerStats;

    [SerializeField]
    private Menu menu;
    private void Start()
    {
        if(Menu.loadSaveData)
        {
            LoadData();
        }
    }
    public void SaveData()
    {
        SavedData saveData = new SavedData
        {
            playerPositions = playerTransform.position,
            inventoryContent = Inventory.instance.GetContent(),
            equipmentHeadItem = equipmentSystem.equipmentHeadItem,
            equipmentChestItem = equipmentSystem.equipmentChestItem,
            equipmentHandsItem = equipmentSystem.equipmentHandsItem,
            equipmentLegsItem = equipmentSystem.equipmentLegsItem,
            equipmentFeetItem = equipmentSystem.equipmentFeetItem,
            currentHealth = playerStats.currentHealth,
            currentHunger = playerStats.currentHunger,
            currentThirst = playerStats.currentThirst,
        };

        string jsonData = JsonUtility.ToJson(saveData);
        string filePath = Application.persistentDataPath + "/SaveData.json";
        Debug.Log("Saving data to: " + filePath);
        System.IO.File.WriteAllText(filePath, jsonData);
        Debug.Log("Data saved successfully.");

        menu.loadGameButton.interactable = true;
        menu.clearSavedDataButton.interactable = true;
    }

    public void LoadData()
    {
        string filePath = Application.persistentDataPath + "/SaveData.json";
        string jsonData = System.IO.File.ReadAllText(filePath);
        
        SavedData savedData = JsonUtility.FromJson<SavedData>(jsonData);

        playerTransform.position = savedData.playerPositions;

        equipmentSystem.LoadEquipments(new ItemData[]
        {
            savedData.equipmentHeadItem,
            savedData.equipmentChestItem,
            savedData.equipmentHandsItem,
            savedData.equipmentLegsItem,
            savedData.equipmentFeetItem,
            savedData.equipmentWeaponItem
        });

        Inventory.instance.LoadData(savedData.inventoryContent);

        playerStats.currentHealth = savedData.currentHealth;
        playerStats.currentHunger = savedData.currentHunger;
        playerStats.currentThirst = savedData.currentThirst;
        playerStats.UpdateHealthBar();

        Debug.Log("Data loaded successfully. Player position set to: " + playerTransform.position);
    }
}

public class SavedData
{
    public Vector3 playerPositions;
    public List<ItemInInventory> inventoryContent;
    public ItemData equipmentHeadItem, equipmentChestItem, equipmentHandsItem, equipmentLegsItem, equipmentFeetItem, equipmentWeaponItem;
    public float currentHealth, currentHunger, currentThirst;
}