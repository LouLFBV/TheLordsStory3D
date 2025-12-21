using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InteractableIconUI : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float scaleDuration = 0.2f;
    [SerializeField] private Vector3 popScale = Vector3.one * 1.2f;

    [Header("UI")]
    [SerializeField] private Image icone;
    [SerializeField] private Image iconeObject;
    [SerializeField] private InteractableIconDatabase iconDatabase;


    [Header("Icon Positioning")]
    public float distanceFromObject = 0.2f;
    public float heightOffset = 0.5f;

    private Vector3 baseScale;
    private bool isVisible = false;
    private DeviceType currentDevice;
    private Coroutine animationCoroutine;


    private Transform interactableTarget;
    private Transform playerRef;

    private void Awake()
    {
        baseScale = transform.localScale;
        transform.localScale = Vector3.zero;

        if (icone == null)
            icone = transform.GetChild(0).GetComponent<Image>();

        if (iconeObject == null)
            iconeObject = transform.GetChild(1).GetComponent<Image>();

    }

    private void OnEnable()
    {
        StartCoroutine(InitializeDeviceWatcher());
        StartCoroutine(UpdateIconWhenReady());
        InputRebindManager.OnRebindsChanged += OnRebindsChanged;
    }


    private void OnDisable()
    {
        if (DeviceWatcher.Instance != null)
            DeviceWatcher.Instance.OnDeviceChanged -= UpdateDevice;
        InputRebindManager.OnRebindsChanged -= OnRebindsChanged;
    }

    private void OnRebindsChanged()
    {
        StartCoroutine(UpdateIconWhenReady());
    }

    private IEnumerator InitializeDeviceWatcher()
    {
        // Attend que DeviceWatcher soit prêt
        while (DeviceWatcher.Instance == null)
            yield return null;

        DeviceWatcher.Instance.OnDeviceChanged += UpdateDevice;

        currentDevice = DeviceWatcher.Instance.CurrentDevice;
        UpdateIcon();
    }

    private IEnumerator UpdateIconWhenReady()
    {
        // Attend que InputProvider soit prêt
        while (InputProvider.Instance == null)
            yield return null;

        UpdateIcon();
    }

    private void UpdateDevice(DeviceType device)
    {
        currentDevice = device;
        StartCoroutine(UpdateIconWhenReady());
    }

    private void LateUpdate()
    {
        if (interactableTarget == null || playerRef == null || Camera.main == null)
            return;

        UpdateWorldPosition();

        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180f, 0);
    }

    private void UpdateWorldPosition()
    {
        Vector3 objectPos = interactableTarget.position;
        Vector3 dir = (playerRef.position - objectPos).normalized;

        Vector3 iconPos = objectPos + dir * distanceFromObject + Vector3.up * heightOffset;
        transform.position = iconPos;
    }

    public void Initialize(Transform interactable, Transform player)
    {
        interactableTarget = interactable;
        playerRef = player;
    }

    private void UpdateIcon()
    {
        if (icone == null || InputProvider.Instance == null)
            return;


        InputRebindManager.UpdateBindingDisplayForAction(
            InputProvider.Instance.UIInput.actions["Interact"],
            icone,
            currentDevice
        );
    }

    public void Show()
    {
        // IMPORTANT : annule toute disparition programmée
        CancelInvoke(nameof(DisableSelf));

        if (isVisible)
            return;

        isVisible = true;

        if (icone != null)
            icone.enabled = true;

        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        animationCoroutine = StartCoroutine(PopAnimation(baseScale));
    }


    public void Hide()
    {
        if (!isVisible)
            return;

        isVisible = false;

        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        animationCoroutine = StartCoroutine(ScaleDownAnimation());

        Invoke(nameof(DisableSelf), scaleDuration + 0.05f);
    }


    private void DisableSelf()
    {
        if (isVisible)
            return; // sécurité ultime

        if (icone != null)
            icone.enabled = false;
        if (iconeObject != null)
            iconeObject.enabled = false;

    }

    #region IEnumerators
    private IEnumerator PopAnimation(Vector3 targetScale)
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

    private IEnumerator ScaleDownAnimation()
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
    #endregion
    public void SetInteractable(InteractableBase interactable)
    {
        if (interactable == null)
            return;

        Sprite sprite = iconDatabase.Get(interactable.objectType);
        if (sprite == null)
        {
            iconeObject.enabled = false;
            return;
        }
        iconeObject.sprite = sprite;
        iconeObject.enabled = sprite != null;
    }
}
