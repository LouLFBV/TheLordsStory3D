using System.Collections.Generic;
using UnityEngine;

public class WorldStateManager : MonoBehaviour
{
    public static WorldStateManager Instance;

    private HashSet<string> collectedObjects = new();

    public event System.Action OnWorldStateLoaded;
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
        Debug.Log($"<color=green>[WorldStateManager] Loaded {collectedObjects.Count} collected objects.</color>");
        foreach (var id in collectedObjects)
        {
            Debug.Log($"<color=green> - Collected Object ID: {id}</color>");
        }
        OnWorldStateLoaded?.Invoke();
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