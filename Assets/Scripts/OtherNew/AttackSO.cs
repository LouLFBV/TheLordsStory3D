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
    public float comboWindowStart = 0.5f; // Moment où on peut "buffer" l'attaque suivante
    public float comboWindowEnd = 0.8f;   // Moment où c'est trop tard
}