using System.Collections.Generic;
using UnityEngine;

public class WorldStateManager : MonoBehaviour
{
    public static WorldStateManager Instance;

    private HashSet<string> collectedObjects = new HashSet<string>();

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterCollectedObject(string id)
    {
        collectedObjects.Add(id);
    }

    public bool IsCollected(string id)
    {
        return collectedObjects.Contains(id);
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
    }
}
