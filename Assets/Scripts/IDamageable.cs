public interface IDamageable
{
    // On passe tout ce qui est nécessaire pour un calcul complet
    void TakeDamage(float damage, float poiseDamage, DamageType type);
}