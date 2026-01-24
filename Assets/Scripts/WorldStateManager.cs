using System.Collections.Generic;
using UnityEngine;

public class WorldStateManager : MonoBehaviour
{
    public static WorldStateManager Instance;

    private HashSet<string> collectedObjects = new();
    private readonly List<WorldObjectPersistence> worldObjects = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // 🔹 Enregistrement
    public void RegisterWorldObject(WorldObjectPersistence obj)
    {
        if (!worldObjects.Contains(obj))
            worldObjects.Add(obj);
    }

    public void UnregisterWorldObject(WorldObjectPersistence obj)
    {
        worldObjects.Remove(obj);
    }

    // 🔹 Application globale
    public void ApplyWorldState()
    {
        foreach (var obj in worldObjects)
        {
            obj.ApplyWorldState();
        }
    }

    // --- Save / Load ---
    public WorldStateSaveData GetSaveData()
    {
        return new WorldStateSaveData
        {
            collectedObjectIDs = new List<string>(collectedObjects)
        };
    }

    public void LoadSaveData(WorldStateSaveData data)
    {
        collectedObjects = new HashSet<string>(data.collectedObjectIDs);
    }

    public void RegisterCollectedObject(string id)
    {
        collectedObjects.Add(id);
    }

    public bool IsCollected(string id)
    {
        return collectedObjects.Contains(id);
    }
}


[System.Serializable]
public class WorldStateSaveData
{
    public List<string> collectedObjectIDs = new List<string>();
}