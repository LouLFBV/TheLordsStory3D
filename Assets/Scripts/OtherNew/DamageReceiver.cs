using UnityEngine;

public class DamageReceiver : MonoBehaviour
{
    private HealthSystem health;
    private PoiseSystem poise;
    private PlayerController player;
    private ArmorSystem armor; // Nouveau : pour les calculs de réduction

    private void Awake()
    {
        health = GetComponent<HealthSystem>();
        poise = GetComponent<PoiseSystem>();
        player = GetComponent<PlayerController>();
        armor = GetComponent<ArmorSystem>();
    }

    // On ajoute le DamageType pour savoir quelle armure utiliser
    public void ReceiveDamage(float damage, float poiseDamage, DamageType type)
    {
        float finalDamage = damage;

        // 1. Calcul de la réduction d'armure
        if (armor != null)
        {
            finalDamage = armor.CalculateReducedDamage(damage, type);
        }

        // 2. Application à la santé
        health.TakeDamage(finalDamage);

        // 3. Gestion du Poise (Équilibre)
        if (poise != null && poise.ApplyPoiseDamage(poiseDamage))
        {
            // Si le poise est brisé, on passe en HitState
            if (player != null)
                player.StateMachine.ChangeState(PlayerStateType.Hit);
        }
    }
}