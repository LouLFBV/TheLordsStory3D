using UnityEngine;
using TMPro;
using System.Collections;
public class PopupDescription : MonoBehaviour
{
    [SerializeField] private GameObject popupDescriptionPanel;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private string description;


    [Header("UI Animation")]
    [SerializeField] private CanvasGroup popupCanvasGroup;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float displayDuration = 5f;

    private bool _isUsed = false;
    private void OnEnable()
    {
        PopupEvent.OnPopupRequested += ShowDescriptionPanel;
    }

    private void OnDisable()
    {
        PopupEvent.OnPopupRequested -= ShowDescriptionPanel;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_isUsed)
        {
            ShowDescriptionPanel(description);
            _isUsed = true;
        }
    }
    private void ShowDescriptionPanel(string desc)
    {
        popupDescriptionPanel.SetActive(true);
        descriptionText.text = desc;

        StopAllCoroutines();
        StartCoroutine(FadeDescriptionPanel());
    }
    private IEnumerator FadeDescriptionPanel()
    {
        popupCanvasGroup.alpha = 0;

        // --- FADE IN ---
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            popupCanvasGroup.alpha = t / fadeDuration;
            yield return null;
        }

        popupCanvasGroup.alpha = 1;

        // --- ATTENTE ---
        yield return new WaitForSeconds(displayDuration);

        // --- FADE OUT ---
        t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            popupCanvasGroup.alpha = 1 - (t / fadeDuration);
            yield return null;
        }

        popupCanvasGroup.alpha = 0;
        popupDescriptionPanel.SetActive(false);
    }
}
