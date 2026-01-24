using UnityEngine;

public class WorldObjectPersistence : MonoBehaviour
{
    private WorldObjectID id;

    private void Awake()
    {
        id = GetComponent<WorldObjectID>();
        WorldStateManager.Instance?.RegisterWorldObject(this);
    }

    private void OnDestroy()
    {
        if (WorldStateManager.Instance != null)
            WorldStateManager.Instance.UnregisterWorldObject(this);
    }

    public void ApplyWorldState()
    {
        if (id != null && WorldStateManager.Instance.IsCollected(id.uniqueID))
        {
            Destroy(gameObject);
        }
    }
}
