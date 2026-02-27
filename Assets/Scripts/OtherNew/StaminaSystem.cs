using UnityEngine;
using System;

public class StaminaSystem : MonoBehaviour
{
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float regenRate = 15f;
    [SerializeField] private float regenDelay = 1f;
    public float consommationRate = 1f;

    public float CurrentStamina { get; private set; }

    private float regenTimer;

    public event Action<float, float> OnStaminaChanged;

    private void Awake()
    {
        CurrentStamina = maxStamina;
    }

    private void Update()
    {
        if (regenTimer > 0)
        {
            regenTimer -= Time.deltaTime;
            return;
        }

        if (CurrentStamina < maxStamina)
        {
            CurrentStamina += regenRate * Time.deltaTime;
            CurrentStamina = Mathf.Clamp(CurrentStamina, 0, maxStamina);
            OnStaminaChanged?.Invoke(CurrentStamina, maxStamina);
        }
    }

    //public bool CanSpend(float amount)
    //{
    //    return CurrentStamina >= amount;
    //}

    public void Spend(float amount)
    {
        CurrentStamina -= amount;
        CurrentStamina = Mathf.Clamp(CurrentStamina, 0, maxStamina);

        regenTimer = regenDelay;

        OnStaminaChanged?.Invoke(CurrentStamina, maxStamina);
    }   

    public bool HasStamina() => CurrentStamina > 0f;
}