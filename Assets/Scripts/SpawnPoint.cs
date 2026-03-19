using UnityEngine;

public sealed class SpawnPoint : MonoBehaviour
{
    [field: SerializeField]
    public string SpawnID { get; private set; }

    private void OnEnable()
    {
        GameManager.Instance.SpawnSystem.Register(this);
        //ThirdPersonOrbitCamBasic.Instance.UnlockCamera();
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.SpawnSystem.Unregister(this);
    }

#if UNITY_EDITOR
private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.3f);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward);
    }
#endif
}