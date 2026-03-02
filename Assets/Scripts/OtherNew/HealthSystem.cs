using UnityEngine;
using System;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f; 
    [SerializeField] private ParticleSystem healEffect;

    public float CurrentHealth { get; private set; }

    public event Action<float, float> OnHealthChanged;
    public event Action OnDeath;
    public event Action OnHit;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        OnHit?.Invoke();

        if (CurrentHealth <= 0)
            OnDeath?.Invoke();
    }
    public void Heal(float amount)
    {
        // Si on est déjà full vie, on peut choisir de ne pas consommer l'objet
        // Mais si on soigne, on lance la logique :
        if (CurrentHealth < maxHealth)
        {
            if (healEffect != null) healEffect.Play();

            CurrentHealth = Mathf.Min(CurrentHealth + amount, maxHealth);

            // C'est CA qui remplace "UpdateHealthBar" ! 
            // Ton script d'UI doit être abonné à cet event.
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        }
    }
}