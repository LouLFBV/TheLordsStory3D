using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class WorldObjectID : MonoBehaviour
{
    [SerializeField] private string uniqueID;
    public string UniqueID => uniqueID;

    private void Awake()
    {
        if (string.IsNullOrEmpty(uniqueID))
            Debug.LogWarning($"{name} has no WorldObjectID!");
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // ❌ Ne jamais toucher au prefab asset
        if (PrefabUtility.IsPartOfPrefabAsset(this))
            return;

        // 🔍 Récupère le prefab source (s’il existe)
        var prefabSource = PrefabUtility.GetCorrespondingObjectFromSource(this);

        // Cas 1 : instance sans ID
        if (string.IsNullOrEmpty(uniqueID))
        {
            GenerateID();
            return;
        }

        // Cas 2 : instance dupliquée → même ID que le prefab
        if (prefabSource != null)
        {
            var sourceID = prefabSource.GetComponent<WorldObjectID>();
            if (sourceID != null && sourceID.uniqueID == uniqueID)
            {
                GenerateID();
            }
        }
    }

    private void GenerateID()
    {
        uniqueID = System.Guid.NewGuid().ToString();
        EditorUtility.SetDirty(this);
    }
#endif

}
