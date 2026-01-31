using UnityEngine;

public class BuildingState : MonoBehaviour
{
    private WorldObjectID worldID;

    [SerializeField] private GameObject buildingVisual;

    private void Awake()
    {
        worldID = GetComponent<WorldObjectID>();
    }

    private void OnEnable()
    {
        if (WorldStateManager.Instance != null)
            WorldStateManager.Instance.OnWorldStateLoaded += ApplyWorldState;
    }

    private void OnDisable()
    {
        if (WorldStateManager.Instance != null)
            WorldStateManager.Instance.OnWorldStateLoaded -= ApplyWorldState;
    }

    private void ApplyWorldState()
    {
        Debug.LogWarning($"<color=green>[BuildingState] Applying world state for building {worldID.UniqueID}.</color>");
        if (WorldStateManager.Instance.IsActived(worldID.UniqueID))
        {
            buildingVisual.SetActive(true);
            Debug.Log($"<color=green>[BuildingState] Activated building {worldID.UniqueID}.</color>");
        } 
        else if (WorldStateManager.Instance.IsCollected(worldID.UniqueID))
        {
            buildingVisual.SetActive(false);
            Debug.Log($"<color=green>[BuildingState] Collected and destroyed building {worldID.UniqueID}.</color>");
        }
        else
        {
            Debug.Log($"<color=yellow>[BuildingState] Building {worldID.UniqueID} remains inactive.</color>");
        }
    }
}
