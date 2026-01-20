using UnityEngine;

public class WorldObjectID : MonoBehaviour
{
    public string uniqueID;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(uniqueID))
            uniqueID = System.Guid.NewGuid().ToString();
    }
#endif
}
