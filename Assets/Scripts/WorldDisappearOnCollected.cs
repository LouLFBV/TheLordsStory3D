using UnityEngine;

public class WorldDisappearOnCollected : MonoBehaviour
{
    protected WorldObjectID worldID;

    protected virtual void Awake()
    {
        worldID = GetComponent<WorldObjectID>();
        Debug.Log($"[Awake] {name} activeSelf={gameObject.activeSelf} activeInHierarchy={gameObject.activeInHierarchy}");
    }

    protected virtual void OnEnable()
    {
        Debug.Log($"[OnEnable] {name}");

        if (worldID == null || WorldStateManager.Instance == null)
            return;

        WorldStateManager.Instance.Subscribe(ApplyWorldState);
    }


    protected virtual void OnDisable()
    {
        if (WorldStateManager.Instance != null)
            WorldStateManager.Instance.Unsubscribe(ApplyWorldState);
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
        else
        {
            Debug.LogWarning($"<color=red>[{name}] has no WorldObjectID component!, with ID : {worldID.UniqueID}</color>\"");
        }
    }
}
