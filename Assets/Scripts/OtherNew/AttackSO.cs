using UnityEngine;

[CreateAssetMenu(fileName = "NewAttack", menuName = "Combat/Attack")]
public class AttackSO : ScriptableObject
{
    public int AnimationHash => Animator.StringToHash(animationName);
    public string animationName;      // Le nom du clip (ou le paramètre trigger)
    public float damageMultiplier = 1f; // Multiplicateur de dégâts
    //public float staminaCost = 15f;   // Coût en endurance

    [Header("Combo Logic")]
    public AttackSO nextAttack;       // L'attaque suivante si on reclique

    [Header("AI Conditions")]
    public float minDistance;
    public float maxDistance;
    public float attackCooldown; 
    public float postAttackDelay = 0.5f; 
    [Range(1, 100)] public int weight = 50;

    [Header("Audio")]
    public AudioClip attackSound;

    [HideInInspector] public float nextAttackTime; 
}