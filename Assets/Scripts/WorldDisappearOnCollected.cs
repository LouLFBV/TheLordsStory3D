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
        if (WorldStateManager.Instance == null || worldID == null) return;
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

    protected void OnDestroy()
    {
        Debug.Log($"<color=red>[DESTROYED] {name}, with ID : {worldID.UniqueID}</color>");
    }
}
