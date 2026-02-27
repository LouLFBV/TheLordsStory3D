using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private PlayerController player; // Pour trouver les systèmes

    [Header("Health UI")]
    [SerializeField] private Image healthBarFill;
    [SerializeField] private Animator healthBarAnimator;

    [Header("Stamina UI")]
    [SerializeField] private Image staminaBarFill;
    [SerializeField] private Animator staminaBarAnimator;

    [Header("Colors")]
    private Color colorFull = new Color32(0x2F, 0x62, 0x26, 0xFF);   // Vert
    private Color colorMedium = new Color32(0xC0, 0x88, 0x34, 0xFF); // Orange
    private Color colorLow = new Color32(0x99, 0x46, 0x46, 0xFF);    // Rouge

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // On s'abonne aux événements des systèmes
        player.Health.OnHealthChanged += UpdateHealthBar;
        player.Stamina.OnStaminaChanged += UpdateStaminaBar;

        // On s'abonne aussi au "Hit" pour l'animation de la barre
        player.Health.OnHit += () => healthBarAnimator.SetTrigger("TakeDamage");
    }

    private void UpdateHealthBar(float current, float max)
    {
        float ratio = current / max;
        healthBarFill.fillAmount = ratio;

        // Ta logique de dégradé de couleurs
        if (ratio >= 0.6f)
            healthBarFill.color = Color.Lerp(colorMedium, colorFull, (ratio - 0.6f) / 0.4f);
        else if (ratio >= 0.2f)
            healthBarFill.color = Color.Lerp(colorLow, colorMedium, (ratio - 0.2f) / 0.4f);
        else
            healthBarFill.color = colorLow;
    }

    private void UpdateStaminaBar(float current, float max)
    {
        staminaBarFill.fillAmount = current / max;

        // Si stamina très basse, on peut trigger l'anim "StaminaLow"
        if (current <= 0)
            staminaBarAnimator.SetTrigger("StaminaLow");
    }
}