using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PaletteSystem : MonoBehaviour
{
    public static PaletteSystem instance;

    [Header("Other References")]
    public PaletteInputHandler inputHandler;
    public PaletteSlotManager slotManager;
    public PaletteEquipmentManager equipmentManager;
    public PaletteSaveSystem saveSystem;

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

    //void OnEnable()
    //{

    //    if (DeviceWatcher.Instance != null)
    //        DeviceWatcher.Instance.OnDeviceChanged += UpdateBindingDisplay;
    //}
    //void OnDisable()
    //{

    //    if (DeviceWatcher.Instance != null)
    //        DeviceWatcher.Instance.OnDeviceChanged -= UpdateBindingDisplay;
    //}



    //private void Start()
    //{
    //    UpdateEquipmentsDesequipButtons();
    //    UpdateBindingDisplay(DeviceWatcher.Instance.CurrentDevice);
    //}

    public void HandlePaletteLogic(PlayerController player)
    {
        inputHandler.HandleInput(player);
    }

    public PaletteSaveData GetSaveData()
    {
        return saveSystem.GetSaveData();
    }

    public void LoadSaveData(PaletteSaveData data)
    {
        saveSystem.LoadSaveData(data);
    }

    //private void UpdateBindingDisplay(DeviceType currentDevice)
    //{
    //    //InputRebindManager.UpdateBindingDisplayForAction(playerInput.actions["Weapon1"], iconeInputWeapon1, currentDevice);
    //    //InputRebindManager.UpdateBindingDisplayForAction(playerInput.actions["Weapon2"], iconeInputWeapon2, currentDevice);
    //    //InputRebindManager.UpdateBindingDisplayForAction(playerInput.actions["Object1"], iconeInputObject1, currentDevice);
    //    //InputRebindManager.UpdateBindingDisplayForAction(playerInput.actions["Object2"], iconeInputObject2, currentDevice);
    //}

}

[System.Serializable]
public class PaletteSaveData
{
    public PaletteSlotSave weapon1;
    public PaletteSlotSave weapon2;

    public PaletteSlotSave object1;
    public PaletteSlotSave object2;
}

[System.Serializable]
public class PaletteSlotSave
{
    public string itemID;
    public int count;
    public bool isEquipped;
}

[System.Serializable]
public class PaletteSlot
{
    public ItemData slotItemData;
    public Image SlotImage;
    public Image iconeInput;
    public bool isEquipped = false;
    public TextMeshProUGUI countText;
    public GameObject imageSelected;
    public Slot slotInEquipment;
}