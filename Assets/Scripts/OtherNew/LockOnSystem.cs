using UnityEngine;
using System.Linq;

public class LockOnSystem : MonoBehaviour
{
    [SerializeField] private float lockRadius = 15f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float maxLockDistance = 20f; // Distance de rupture

    public Transform CurrentTarget { get; private set; }
    public bool IsLocked => CurrentTarget != null;

    public void ToggleLock()
    {
        if (IsLocked)
        {
            CurrentTarget = null;
        }
        else
        {
            SearchTarget();
        }
    }

    private void SearchTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, lockRadius, enemyLayer);

        if (hits.Length == 0) return;

        // On prend le plus proche
        CurrentTarget = hits
            .OrderBy(h => Vector3.Distance(transform.position, h.transform.position))
            .First()
            .transform;
    }

    public void Update()
    {
        if (!IsLocked) return;

        // Sécurité : Si l'ennemi meurt ou s'éloigne trop
        float distance = Vector3.Distance(transform.position, CurrentTarget.position);
        if (distance > maxLockDistance)
        {
            CurrentTarget = null;
            return;
        }

        // Optionnel : On peut ajouter ici un changement de cible avec le stick droit
    }
    private void OnDrawGizmosSelected()
    {
        // Dessine le rayon de détection en bleu
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, lockRadius);

        // Dessine une ligne rouge vers la cible si verrouillé
        if (IsLocked && CurrentTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, CurrentTarget.position);
            Gizmos.DrawWireSphere(CurrentTarget.position, 1f);
        }
    }
}