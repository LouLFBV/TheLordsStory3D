using UnityEngine;
using System;

public class StaminaSystem : MonoBehaviour
{
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float regenRate = 15f;
    [SerializeField] private float regenDelay = 1f;
    [SerializeField] private float regenDelayEmpty = 3f; // Pénalité si vide
    [SerializeField] private float minimumStaminaToRestart = 10f;

    public float CurrentStamina { get; private set; }
    public float consommationRate = 1f;
    private float _regenTimer;
    private bool _isExhausted; // Plus clair que _canToRestartRun

    public event Action<float, float> OnStaminaChanged;

    private void Awake() => CurrentStamina = maxStamina;

    private void Update()
    {
        if (_regenTimer > 0)
        {
            _regenTimer -= Time.deltaTime;
            return;
        }

        if (CurrentStamina < maxStamina)
        {
            CurrentStamina += regenRate * Time.deltaTime;
            CurrentStamina = Mathf.Clamp(CurrentStamina, 0, maxStamina);

            // Si on récupère assez de stamina, on n'est plus épuisé
            if (_isExhausted && CurrentStamina >= minimumStaminaToRestart)
            {
                _isExhausted = false;
            }

            OnStaminaChanged?.Invoke(CurrentStamina, maxStamina);
        }
    }

    public void Spend(float amount)
    {
        if (amount <= 0) return;

        CurrentStamina -= amount;

        if (CurrentStamina <= 0)
        {
            CurrentStamina = 0;
            _isExhausted = true;
            _regenTimer = regenDelayEmpty; // On applique la grosse pénalité UNE SEULE FOIS
        }
        else
        {
            // On ne reset le timer que si on n'est pas déjà en pénalité "Empty"
            _regenTimer = Mathf.Max(_regenTimer, regenDelay);
        }

        OnStaminaChanged?.Invoke(CurrentStamina, maxStamina);
    }

    public bool HasStamina()
    {
        // On ne peut pas consommer si on est épuisé (en dessous du seuil)
        return !_isExhausted && CurrentStamina > 0;
    }


    //public bool CanSpend(float amount)
    //{
    //    return CurrentStamina >= amount;
    //}
}