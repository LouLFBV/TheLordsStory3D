using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections.Generic;

public class UINavigationManager : MonoBehaviour
{
    public List<UISelectable> elements;
    public int currentIndex = 0;

    [Tooltip("Nombre de colonnes pour la grille de navigation")]
    public int columns = 4;

    [SerializeField] private float moveCooldown = 0.2f;
    private float lastMoveTime;

    [Header("Enfant/ Parent")]
    [SerializeField] private GameObject[] childToDisableWhenActive;

    private PlayerControls controls;
    public System.Action onCancel;

    public bool isActive = true;
    [SerializeField] private bool canCancel = true;
    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.UI.Enable();
        Debug.Log($"[NAV] OnEnable() UI ENABLED sur {name}");

        CheckChildToDesactive();
        lastMoveTime = Time.unscaledTime;
    }

    private void OnDisable()
    {
        controls.UI.Disable();
        Debug.Log($"[NAV] OnDisable() UI DISABLED sur {name}");

        CheckChildToActive();
    }

    void Update()
    {
        if (!isActive)
            return;

        if (GamepadDetector.DetectCurrentGamepad() == GamepadType.None)
            return;

        if (elements.Count == 0)
            return;

        Vector2 nav = controls.UI.Navigate.ReadValue<Vector2>();

        // Déplacement
        if (Time.unscaledTime - lastMoveTime > moveCooldown)
        {
            if (nav.x > 0.5f) MoveHorizontal(+1);
            else if (nav.x < -0.5f) MoveHorizontal(-1);
            else if (nav.y > 0.5f) MoveVertical(-1); // haut = index plus petit
            else if (nav.y < -0.5f) MoveVertical(+1);
        }

        // Submit
        if (controls.UI.Submit.triggered)
            elements[currentIndex].OnSubmit();

        // Cancel
        if (controls.UI.Cancel.triggered)
            Cancel();
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
        if (!canCancel)
            return;
        if (onCancel != null)
        {
            onCancel.Invoke();
            return;
        }

        gameObject.SetActive(false);
    }
    #endregion

    #region GESTION DE L’ACTIVATION DES ENFANTS

    private void CheckChildToDesactive()
    {
        foreach (var child in childToDisableWhenActive)
        {
            if (!child) continue;

            var nav = child.GetComponent<UINavigationManager>();
            if (nav != null)
                nav.isActive = false;
        }
    }

    private void CheckChildToActive()
    {
        foreach (var child in childToDisableWhenActive)
        {
            if (!child) continue;

            var nav = child.GetComponent<UINavigationManager>();
            if (nav != null)
                nav.isActive = true;
        }
    }
    #endregion
}
