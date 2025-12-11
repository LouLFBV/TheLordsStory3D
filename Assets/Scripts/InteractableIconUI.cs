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

    [Header("Icon Positioning")]
    public float distanceFromObject = 0.6f;
    public float heightOffset = 1.8f;

    private Vector3 baseScale;
    private bool isVisible = false;
    private DeviceType currentDevice;

    private Transform interactableTarget;
    private Transform playerRef;

    private void Awake()
    {
        baseScale = transform.localScale;
        transform.localScale = Vector3.zero;

        if (icone == null)
            icone = GetComponent<Image>();

        currentDevice = GamepadDetector.GetDeviceType();
        UpdateIcon();
    }

    private void OnEnable()
    {
        if (DeviceWatcher.Instance != null)
            DeviceWatcher.Instance.OnDeviceChanged += UpdateDevice;

        // Met à jour l’icône au cas où on activerait le panel après un switch
        UpdateDevice(DeviceWatcher.Instance.CurrentDevice);
    }

    private void OnDisable()
    {
        if (DeviceWatcher.Instance != null)
            DeviceWatcher.Instance.OnDeviceChanged -= UpdateDevice;
    }

    private void UpdateDevice(DeviceType device)
    {
        currentDevice = device;
        UpdateIcon(); // Ton UpdateIcon existant
    }


    private void LateUpdate()
    {
        if (interactableTarget == null || playerRef == null || Camera.main == null)
            return;

        UpdateWorldPosition();

        // Toujours regarder la caméra
        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180f, 0);
    }

    /// <summary>
    /// Positionne l’icône du côté du joueur
    /// </summary>
    private void UpdateWorldPosition()
    {
        Vector3 objectPos = interactableTarget.position;
        Vector3 dir = (playerRef.position - objectPos).normalized;

        Vector3 iconPos =
            objectPos +
            dir * distanceFromObject +
            Vector3.up * heightOffset;

        transform.position = iconPos;
    }

    /// <summary>
    /// Appelé depuis InteractBehaviour pour donner les références
    /// </summary>
    public void Initialize(Transform interactable, Transform player)
    {
        interactableTarget = interactable;
        playerRef = player;
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

        enabled = true;

        StopAllCoroutines();
        StartCoroutine(PopAnimation(baseScale));
    }

    public void Hide()
    {
        if (!isVisible) return;
        isVisible = false;

        StopAllCoroutines();
        StartCoroutine(ScaleDownAnimation());
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
