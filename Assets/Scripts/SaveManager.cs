using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    private int currentSlot = 1;

    private string GetSavePath(int slot)
    {
        return Application.persistentDataPath + $"/save_slot_{slot}.json";
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 🔹 appelé depuis le menu ou le save book
    public void SetCurrentSlot(int slot)
    {
        currentSlot = slot;
    }

    public void SaveGame()
    {
        string path = GetSavePath(currentSlot);

        SaveData data = new SaveData();
        data.playerStats = PlayerStats.instance.GetSaveData();
        data.inventory = Inventory.instance.GetSaveData();
        data.palette = Palette.instance.GetSaveData();
        data.sceneName = SceneManager.GetActiveScene().name;

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);

        Debug.Log($"Game Saved in slot {currentSlot}");
    }

    public void LoadGame(int slot)
    {
        string path = GetSavePath(slot);

        if (!File.Exists(path))
        {
            Debug.LogWarning($"No save file in slot {slot}");
            return;
        }

        currentSlot = slot;

        string json = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        StartCoroutine(LoadRoutine(data));
    }

    private IEnumerator LoadRoutine(SaveData data)
    {
        yield return SceneManager.LoadSceneAsync(data.sceneName);
        yield return null;

        PlayerStats.instance.LoadSaveData(data.playerStats);
        if (data.inventory != null)
            Inventory.instance.LoadSaveData(data.inventory);
        if (data.palette != null)
            Palette.instance.LoadSaveData(data.palette);



        Debug.Log("Game Loaded");
    }
    public void DeleteSave(int slot)
    {
        string path = GetSavePath(slot);

        if (!File.Exists(path))
        {
            Debug.LogWarning($"No save file to delete in slot {slot}");
            return;
        }

        File.Delete(path);

        Debug.Log($"Save slot {slot} deleted");
    }

    public bool HasSave(int slot)
    {
        return File.Exists(GetSavePath(slot));
    }
}



[System.Serializable]
public class SaveData
{
    public PlayerStatsSaveData playerStats;

    // À venir :
    public InventorySaveData inventory;
    public PaletteSaveData palette;
    // public QuestSaveData quests;
    // public WorldStateSaveData world;

    public string sceneName;
    public int saveVersion = 1;
}
