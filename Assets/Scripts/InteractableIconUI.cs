using UnityEngine;

public class InteractableIconUI : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float scaleDuration = 0.2f;
    [SerializeField] private Vector3 popScale = Vector3.one * 1.2f;

    private Vector3 baseScale;
    private bool isVisible = false;

    private void Awake()
    {
        baseScale = transform.localScale;
        transform.localScale = Vector3.zero;
    }

    private void LateUpdate()
    {
        if (Camera.main != null)
        {
            transform.LookAt(Camera.main.transform);
            transform.Rotate(0, 180f, 0);
        }
    }

    public void Show()
    {
        if (isVisible) return;
        isVisible = true;
        StopAllCoroutines();
        StartCoroutine(PopAnimation(baseScale));
    }

    public void Hide()
    {
        if (!isVisible) return;
        isVisible = false;
        StopAllCoroutines();
        StartCoroutine(ScaleDownAnimation());
    }

    private System.Collections.IEnumerator PopAnimation(Vector3 targetScale)
    {
        float timer = 0f;
        Vector3 startScale = Vector3.zero;

        while (timer < scaleDuration)
        {
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, popScale, timer / scaleDuration);
            yield return null;
        }

        transform.localScale = targetScale;
    }

    private System.Collections.IEnumerator ScaleDownAnimation()
    {
        float timer = 0f;
        Vector3 startScale = transform.localScale;

        while (timer < scaleDuration)
        {
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, timer / scaleDuration);
            yield return null;
        }

        transform.localScale = Vector3.zero;
    }
}
