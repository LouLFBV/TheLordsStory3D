using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class ZoneNameTrigger : MonoBehaviour
{
    [Header("UI Settings")]
    public TextMeshProUGUI zoneTextUI;
    [TextArea]
    public string zoneName = "Nom de la zone";

    [Header("Timing")]
    public float fadeDuration = 1f;
    public float displayDuration = 3f;
    public float cooldownDuration = 5f;

    [Header("Audio (Optionnel)")]
    public AudioClip zoneEnterSound;
    public AudioSource audioSource;

    private bool isOnCooldown = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isOnCooldown)
        {
            StartCoroutine(ShowZoneName());
        }
    }

    private IEnumerator ShowZoneName()
    {
        isOnCooldown = true;

        // Play sound if assigned
        if (zoneEnterSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(zoneEnterSound);
        }

        // Setup UI
        Color color = zoneTextUI.color;
        color.a = 0;
        zoneTextUI.text = zoneName;
        zoneTextUI.color = color;

        // Fade in
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, t / fadeDuration);
            zoneTextUI.color = color;
            yield return null;
        }

        // Wait visible
        yield return new WaitForSeconds(displayDuration);

        // Fade out
        t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(1, 0, t / fadeDuration);
            zoneTextUI.color = color;
            yield return null;
        }

        zoneTextUI.text = "";

        yield return new WaitForSeconds(cooldownDuration);
        isOnCooldown = false;
    }
}
