using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
    [SerializeField] private GameObject uiContainer; // Le parent de l'UI (pour l'afficher/cacher)
    [SerializeField] private Image healthBarFill;
    private HealthSystem health;

    public void Initialize(HealthSystem targetHealth)
    {
        health = targetHealth;
        uiContainer.SetActive(false); // Caché par défaut

        // On s'abonne à l'événement de changement de vie
        health.OnHealthEnemyChanged += UpdateUI;
    }

    private void UpdateUI(float current, float max)
    {
        uiContainer.SetActive(true); // On affiche dès qu'il prend un coup
        healthBarFill.fillAmount = current / max;

        // Optionnel : Changer la couleur selon la vie (ton ancien Lerp)
        healthBarFill.color = Color.Lerp(Color.red, Color.yellow, current / max);

        if (current <= 0) uiContainer.SetActive(false);
    }

    private void LateUpdate()
    {
        // Pour que la barre de vie regarde toujours la caméra
        transform.LookAt(transform.position + Camera.main.transform.forward);
    }
}