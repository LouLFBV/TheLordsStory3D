using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InteractableIconUI : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float scaleDuration = 0.2f;
    [SerializeField] private Vector3 popScale = Vector3.one * 1.2f;

    [Header("UI")]
    [SerializeField] private Image icone;

    private Vector3 baseScale;
    private bool isVisible = false;
    private DeviceType currentDevice;

    private void Awake()
    {
        baseScale = transform.localScale;
        transform.localScale = Vector3.zero;

        if (icone == null)
            icone = GetComponent<Image>();

        currentDevice = GamepadDetector.GetDeviceType();
        UpdateIcon();

        // S’abonner au changement de device
        InputSystem.onDeviceChange += OnDeviceChanged;
    }

    private void OnDestroy()
    {
        // Désabonnement obligatoire sinon erreurs
        InputSystem.onDeviceChange -= OnDeviceChanged;
    }

    private void OnDeviceChanged(InputDevice device, InputDeviceChange change)
    {
        if (change == InputDeviceChange.Added ||
            change == InputDeviceChange.Enabled ||
            change == InputDeviceChange.Disconnected ||
            change == InputDeviceChange.Reconnected)
        {
            DeviceType newDevice = GamepadDetector.GetDeviceType();
            if (newDevice != currentDevice)
            {
                currentDevice = newDevice;
                UpdateIcon();
            }
        }
    }

    private void LateUpdate()
    {
        if (Camera.main == null) return;

        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180f, 0);
    }

    private void UpdateIcon()
    {
        if (icone == null) return;
        if (InputProvider.Instance == null) return;

        InputRebindManager.UpdateBindingDisplayForAction(
            InputProvider.Instance.UIInput.actions["Interact"],
            icone,
            currentDevice
        );
    }

    public void Show()
    {
        if (isVisible) return;
        isVisible = true;

        enabled = true; // <-- RÉACTIVE les updates

        StopAllCoroutines();
        StartCoroutine(PopAnimation(baseScale));
    }

    public void Hide()
    {
        if (!isVisible) return;
        isVisible = false;

        StopAllCoroutines();
        StartCoroutine(ScaleDownAnimation());

        // Désactive ce script après disparition pour éviter UpdateIcon()
        // si l'objet est détruit juste après
        Invoke(nameof(DisableSelf), scaleDuration + 0.05f);
    }

    private void DisableSelf() => enabled = false;

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
