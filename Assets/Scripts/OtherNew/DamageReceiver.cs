using UnityEngine;

public class DamageReceiver : MonoBehaviour, IDamageable
{
    private HealthSystem _health;
    private PoiseSystem _poise;
    private PlayerController _player; // Pour le joueur
    private EnemyParent _enemy;      // Pour l'IA (si tu as une classe de base IA)
    private ArmorSystem _armor;

    private void Awake()
    {
        _health = GetComponent<HealthSystem>();
        _poise = GetComponent<PoiseSystem>();
        _player = GetComponent<PlayerController>();
        _enemy = GetComponent<EnemyParent>();
        _armor = GetComponent<ArmorSystem>();
    }

    public void TakeDamage(float damage, float poiseDamage, DamageType type)
    {
        Debug.Log($"Received damage: {damage} of type {type}, with poise damage: {poiseDamage}");
        // 1. Calcul de l'armure
        float finalDamage = (_armor != null) ? _armor.CalculateReducedDamage(damage, type) : damage;

        // 2. Appliquer à la vie
        _health.TakeDamage(finalDamage);

        // 3. Logique de Stun (Poise)
        if (_poise != null && _poise.ApplyPoiseDamage(poiseDamage))
        {
            TriggerHitReaction();
        }
    }

    private void TriggerHitReaction()
    {
        // Si c'est le joueur, on force l'état Hit
        if (_player != null)
        {
            // Si le poise est totalement brisé (système de posture à 0)
            if (_poise.IsBroken)
            {
                _player.StateMachine.ChangeState(PlayerStateType.Stunned);
            }
            else
            {
                _player.StateMachine.ChangeState(PlayerStateType.Hit);
            }
        }

        // Si c'est une IA, on lui dit aussi de changer d'état
        //if (_enemy != null)
        //   _enemy.StateMachine.ChangeState(EnemyStateType.Hit);
    }
}