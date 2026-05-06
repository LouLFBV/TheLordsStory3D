using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] private PlayerInputHandler playerInputHandler;

    [Header("Cursor Settings")]
    [SerializeField] private float cursorSpeed = 1000f;


    private void Update()
    {
        if (GamepadDetector.DetectCurrentGamepad() == GamepadType.None) return;
        Debug.Log(".");
        Vector2 stickValue = playerInputHandler.NavigationInput;
        Debug.Log($"Stick Raw Value: {stickValue} | Magnitude: {stickValue.magnitude}");

        // 1. Déplacement (On garde ton code, il est parfait)
        if (stickValue.magnitude > 0.1f)
        {
            Debug.Log("ZZ");
            Vector2 currentMousePos = Mouse.current.position.ReadValue();
            Vector2 newMousePos = currentMousePos + (stickValue * cursorSpeed * Time.unscaledDeltaTime);
            newMousePos.x = Mathf.Clamp(newMousePos.x, 0, Screen.width);
            newMousePos.y = Mathf.Clamp(newMousePos.y, 0, Screen.height);
            Mouse.current.WarpCursorPosition(newMousePos);
        }

        // 2. Clic (Plus permissif)
        if (playerInputHandler.SubmitPressed)
        {
            // On simule le clic systématiquement si le curseur est affiché
            // (Sauf si tu as un système de navigation par flèches en parallèle 
            // qui tourne sur un autre script, mais même là, cliquer "là où est la souris" est plus safe)
            SimulateMouseClick();
            Debug.Log("Click");

            playerInputHandler.UseSubmitInput();
        }
    }

    private void SimulateMouseClick()
    {
        // 1. Créer une donnée d'événement de pointeur
        PointerEventData eventData = new PointerEventData(EventSystem.current);

        // 2. Lui donner la position actuelle de la souris
        eventData.position = Mouse.current.position.ReadValue();

        // 3. Faire un Raycast sur l'UI pour voir ce qu'il y a sous la souris
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        if (results.Count > 0)
        {
            // On prend le premier objet touché (le plus en avant)
            GameObject clickedObject = results[0].gameObject;

            // 4. Simuler le clic (PointerDown + PointerUp = Click)
            ExecuteEvents.Execute(clickedObject, eventData, ExecuteEvents.pointerClickHandler);

            // Optionnel : Forcer le focus de l'EventSystem sur cet objet
            EventSystem.current.SetSelectedGameObject(clickedObject);
        }
    }
}