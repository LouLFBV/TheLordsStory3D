using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.InputSystem.HID;

public class UINavigationManager : MonoBehaviour
{
    public List<UISelectable> elements;
    public int currentIndex = 0;
    [SerializeField] private float offset = 40f;

   [Tooltip("Nombre de colonnes pour la grille de navigation")]
    public int columns = 4;

    [SerializeField] private float moveCooldown = 0.2f;
    private float lastMoveTime;

    [Header("Enfant/ Parent")]
    [SerializeField] private GameObject childToDisableWhenActive;

    [SerializeField] private PlayerInput playerInput;
    private Vector2 navigationInput;
    private bool isSubmitting = false;
    public System.Action onCancel;

    [SerializeField] private ScrollRect scrollRect; // drag ton ScrollRect dans l'inspecteur

    public bool isActive = true;

    #region Méthodes PlayerInput 
    private void Awake()
    {
        if (playerInput == null)
            playerInput = InputProvider.Instance.UIInput;
    }

    private void OnEnable()
    {
        var a = playerInput.actions;
        a["Navigate"].Enable();
        a["Submit"].Enable();
        a["Cancel"].Enable();
        a["Navigate"].performed += OnNavigatePerformed;
        a["Navigate"].canceled += OnNavigateCanceled;
        a["Submit"].performed += OnSubmitPerformed;
        a["Submit"].canceled += OnSubmitCanceled;
        a["Cancel"].performed += OnCancel;

        currentIndex = 0;

        CheckChildToChange(false);
        lastMoveTime = Time.unscaledTime;
    }

    private void OnDisable()
    {
        if (childToDisableWhenActive != null)
        {
            CheckChildToChange(true);
            return;
        }
        var a = playerInput.actions;
        a["Navigate"].Disable();
        a["Submit"].Disable();
        a["Cancel"].Disable();
        a["Navigate"].performed -= OnNavigatePerformed;
        a["Navigate"].canceled -= OnNavigateCanceled;
        a["Submit"].performed -= OnSubmitPerformed;
        a["Submit"].canceled -= OnSubmitCanceled;
        a["Cancel"].performed -= OnCancel;
    }

    private void OnDestroy()
    {
        if (playerInput != null)
        {
            var cancel = playerInput.actions["Cancel"];
            cancel.performed -= OnCancel;
        }
    }


    private void OnNavigatePerformed(InputAction.CallbackContext ctx)
    {
        navigationInput = ctx.ReadValue<Vector2>();
    }
    private void OnNavigateCanceled(InputAction.CallbackContext ctx)
    {
        navigationInput = Vector2.zero;
    }

    private void OnSubmitPerformed(InputAction.CallbackContext ctx)
    {
        isSubmitting = true;
    }
    private void OnSubmitCanceled(InputAction.CallbackContext ctx)
    {
        isSubmitting = false;
    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {
        Cancel();
    }
    #endregion
    void Update()
    {
        if (!isActive)
            return;

        if (GamepadDetector.DetectCurrentGamepad() == GamepadType.None)
            return;

        if (elements.Count == 0)
            return;


        // Déplacement
        if (Time.unscaledTime - lastMoveTime > moveCooldown)
        {
            if (navigationInput.x > 0.5f) MoveHorizontal(+1);
            else if (navigationInput.x < -0.5f) MoveHorizontal(-1);
            else if (navigationInput.y > 0.5f) MoveVertical(-1); // haut = index plus petit
            else if (navigationInput.y < -0.5f) MoveVertical(+1);
        }

        // Submit
        if (isSubmitting)
        {
            elements[currentIndex].OnSubmit();
            isSubmitting = false;
        }
    }

    void LateUpdate()
    {
        if (!isActive || elements.Count == 0 || GamepadDetector.DetectCurrentGamepad() == GamepadType.None)
            return;

        MoveCursorToCurrent();
    }

    #region SYSTÈME DE NAVIGATION OPTIMISÉ

    private void MoveHorizontal(int direction)
    {
        int newIndex = currentIndex;

        while (true)
        {
            newIndex += direction;

            if (newIndex < 0 || newIndex >= elements.Count)
                return;

            if (IsActiveSelectable(newIndex))
                break;
        }

        SetIndex(newIndex);
    }

    private void MoveVertical(int direction)
    {
        int newIndex = currentIndex + direction * columns;

        while (newIndex >= 0 && newIndex < elements.Count)
        {
            if (IsActiveSelectable(newIndex))
            {
                SetIndex(newIndex);
                return;
            }

            newIndex += direction * columns;
        }
    }

    private bool IsActiveSelectable(int index)
    {
        return elements[index] != null &&
               elements[index].gameObject.activeInHierarchy;
    }

    private void SetIndex(int newIndex)
    {
        currentIndex = newIndex;
        lastMoveTime = Time.unscaledTime;

        // Scroll automatique vers l'élément sélectionné
        if (elements[currentIndex].TryGetComponent<Selectable>(out var selectable) && scrollRect != null)
        {
            if (!IsVisible(selectable.GetComponent<RectTransform>()))
            {
                StartCoroutine(SmoothScrollTo(selectable));
            }
        }
    }

    #endregion

    #region CURSEUR / SUBMIT / CANCEL

    void MoveCursorToCurrent()
    {
        if (!IsActiveSelectable(currentIndex))
            return;

        Vector2 pos = elements[currentIndex].GetScreenPosition();
        Mouse.current.WarpCursorPosition(pos);
    }

    private void Cancel()
    {
        if (this == null) return;
        if (!gameObject) return;
        if (onCancel != null)
        {
            onCancel.Invoke();
            return;
        }
        gameObject.SetActive(false);
    }
    #endregion

    #region GESTION DE L’ACTIVATION DES ENFANTS

    private void CheckChildToChange(bool actived)
    {
        if (childToDisableWhenActive == null)
            return;
        if (childToDisableWhenActive.TryGetComponent<UINavigationManager>(out var nav))
            nav.isActive = actived;
        else
            Debug.LogWarning("Le GameObject assigné n'a pas de UINavigationManager.");
        if(actived)
        {
            var a = playerInput.actions;
            a["Navigate"].Enable();
            a["Submit"].Enable();
            a["Cancel"].Enable();
            a["Navigate"].performed += OnNavigatePerformed;
            a["Navigate"].canceled += OnNavigateCanceled;
            a["Submit"].performed += OnSubmitPerformed;
            a["Submit"].canceled += OnSubmitCanceled;
            a["Cancel"].performed += OnCancel;

            currentIndex = 0;
        }
    }

    #endregion

    public IEnumerator SmoothScrollTo(Selectable selectable, float duration = 0.2f)
    {
        if (scrollRect == null || selectable == null)
            yield break;

        Canvas.ForceUpdateCanvases();

        RectTransform content = scrollRect.content;
        RectTransform viewport = scrollRect.viewport;
        RectTransform item = selectable.GetComponent<RectTransform>();

        Vector3[] viewportCorners = new Vector3[4];
        Vector3[] itemCorners = new Vector3[4];

        viewport.GetWorldCorners(viewportCorners);
        item.GetWorldCorners(itemCorners);

        float viewportTop = viewportCorners[1].y;
        float viewportBottom = viewportCorners[0].y;

        float itemTop = itemCorners[1].y;
        float itemBottom = itemCorners[0].y;

        float delta = 0f;

        // Item au-dessus du viewport
        if (itemTop > viewportTop)
        {
            delta = itemTop - viewportTop + offset;
        }
        // Item en dessous du viewport
        else if (itemBottom < viewportBottom)
        {
            delta = itemBottom - viewportBottom - offset;
        }
        else
        {
            yield break; // déjà visible
        }

        float contentHeight = content.rect.height;
        float viewportHeight = viewport.rect.height;

        float normalizedDelta = delta / (contentHeight - viewportHeight);

        float startPos = scrollRect.verticalNormalizedPosition;
        float targetPos = Mathf.Clamp01(startPos + normalizedDelta);

        Debug.Log(
            $"[SmoothScrollTo FIX]\n" +
            $"Delta pixels: {delta}\n" +
            $"Normalized delta: {normalizedDelta}\n" +
            $"Start: {startPos} -> Target: {targetPos}"
        );

        float elapsed = 0f;

        while (elapsed < duration)
        {
            scrollRect.verticalNormalizedPosition =
                Mathf.Lerp(startPos, targetPos, elapsed / duration);

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        scrollRect.verticalNormalizedPosition = targetPos;
    }



    private bool IsVisible(RectTransform item)
    {
        RectTransform viewport = scrollRect.viewport;
        Vector3[] viewportCorners = new Vector3[4];
        viewport.GetWorldCorners(viewportCorners);

        Vector3[] itemCorners = new Vector3[4];
        item.GetWorldCorners(itemCorners);

        return itemCorners[0].y < viewportCorners[1].y && itemCorners[1].y > viewportCorners[0].y;
    }
}
