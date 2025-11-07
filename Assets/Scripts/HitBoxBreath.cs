using UnityEngine;

public class HitBoxAttack : MonoBehaviour
{
    [HideInInspector] public int damageAmount = 10;
    [HideInInspector] public DamageType damageType;
    [HideInInspector] public bool isFireBreath = false;
    [SerializeField] private GameObject fire;

    private bool fireSpawned = false; // éviter les multiples instantiations

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("HitBoxAttack OnTriggerEnter with " + other.name);

        // --- Gestion des dégâts sur le joueur ---
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<PlayerStats>(out var playerHealth))
            {
                playerHealth.TakeDamage(damageAmount, damageType);
            }
        }

        // --- Gestion des flammes du feu du dragon ---
        if (isFireBreath && !fireSpawned && other.CompareTag("Ground"))
        {
            Vector3 spawnPosition = transform.position;
            spawnPosition.y = other.ClosestPoint(transform.position).y + 0.05f; //  Juste au-dessus du sol

            if (fire != null)
            {
                Instantiate(fire, spawnPosition, Quaternion.identity);
                fireSpawned = true; //  éviter de spam le feu
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //  Permet de recréer du feu si on sort du sol puis on y revient
        if (other.CompareTag("Ground"))
        {
            fireSpawned = false;
        }
    }
}
