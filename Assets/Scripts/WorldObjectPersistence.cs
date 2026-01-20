using UnityEngine;

public class WorldObjectPersistence : MonoBehaviour
{
    private void Start()
    {
        var id = GetComponent<WorldObjectID>();
        if (id != null && WorldStateManager.Instance.IsCollected(id.uniqueID))
        {
            Destroy(gameObject);
        }
    }
}
