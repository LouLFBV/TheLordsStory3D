using UnityEngine;

[CreateAssetMenu(fileName = "NewAttack", menuName = "Combat/Attack")]
public class AttackSO : ScriptableObject
{
    public int AnimationHash => Animator.StringToHash(animationName);
    public string animationName;      // Le nom du clip (ou le paramètre trigger)
    public float damageMultiplier = 1f; // Multiplicateur de dégâts
    public float staminaCost = 15f;   // Coût en endurance

    [Header("Combo Logic")]
    public AttackSO nextAttack;       // L'attaque suivante si on reclique

    [Header("AI Conditions")]
    public float minDistance; // L'attaque ne se lance pas si trop près
    public float maxDistance; // L'attaque ne se lance pas si trop loin
    public float attackCooldown; // Temps à attendre avant de pouvoir réutiliser CETTE attaque
    [HideInInspector] public float lastUsedTime; // Pour gérer le cooldown interne
}