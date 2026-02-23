using UnityEngine;
using System.Linq;

public class LockOnSystem : MonoBehaviour
{
    [SerializeField] private float lockRadius = 15f;
    [SerializeField] private LayerMask enemyLayer;

    public Transform CurrentTarget { get; private set; }

    public void ToggleLock()
    {
        if (CurrentTarget != null)
        {
            CurrentTarget = null;
            return;
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, lockRadius, enemyLayer);

        if (hits.Length == 0)
            return;

        CurrentTarget = hits
            .OrderBy(h => Vector3.Distance(transform.position, h.transform.position))
            .First()
            .transform;
    }
}