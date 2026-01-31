using UnityEngine;

public class WorldDisappearOnCollected : MonoBehaviour
{
    protected WorldObjectID worldID;

    protected virtual void Awake()
    {
        worldID = GetComponent<WorldObjectID>();
    }

    protected virtual void OnEnable()
    {
        if (WorldStateManager.Instance == null || worldID == null)
        {
            Debug.LogWarning($"<color=red>[{name}] WorldStateManager instance is null or WorldObjectID is missing. Cannot apply world state.</color>");
            return;
        }
        Debug.LogWarning($"<color=green>[{name}] Subscribing to OnWorldStateLoaded event.</color>");
        WorldStateManager.Instance.OnWorldStateLoaded += ApplyWorldState;
    }

    protected virtual void OnDisable()
    {
        if (WorldStateManager.Instance != null || worldID == null)
            WorldStateManager.Instance.OnWorldStateLoaded -= ApplyWorldState;
    }

    protected void ApplyWorldState()
    {
        if (worldID != null && WorldStateManager.Instance.IsCollected(worldID.UniqueID))
        {
            Destroy(transform.gameObject);
            Debug.LogWarning($"<color=orange>[{name}] checked world state: Collected = {WorldStateManager.Instance.IsCollected(worldID.UniqueID)}, with ID : {worldID.UniqueID}</color>");
        }
        else if (worldID != null)
        {
            Debug.LogWarning($"<color=cyan>[{name}] checked world state: Collected = {WorldStateManager.Instance.IsCollected(worldID.UniqueID)}, with ID : {worldID.UniqueID}</color>");
        }
    }
}
