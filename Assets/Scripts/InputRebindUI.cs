using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class InputRebindUI : MonoBehaviour
{
    [Header("Références")]
    public PlayerInput playerInput;
    public TextMeshProUGUI bindingText;
    public Image iconField;

    [Header("Action & Binding")]
    public string actionName;
    public int bindingIndex;

    private InputActionRebindingExtensions.RebindingOperation rebindOperation;

    private void Start()
    {
        RefreshDisplay();
    }

    public void StartRebind()
    {
        InputAction action = playerInput.actions[actionName];

        bindingText.text = "...";

        //  Désactiver toutes les actions pour autoriser le rebind
        playerInput.actions.Disable();

        rebindOperation = action.PerformInteractiveRebinding(bindingIndex)
            .WithCancelingThrough("<Keyboard>/escape")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation =>
            {
                operation.Dispose();
                playerInput.actions.Enable();   //  RÉACTIVER
                FinishRebind();
            })
            .OnCancel(operation =>
            {
                operation.Dispose();
                playerInput.actions.Enable();   // RÉACTIVER
                RefreshDisplay();
            });

        rebindOperation.Start();
    }


    private void FinishRebind()
    {
        // Sauvegarder automatiquement
        InputRebindManager.SaveRebinds(playerInput);

        RefreshDisplay();
    }

    public void RefreshDisplay()
    {
        InputAction action = playerInput.actions[actionName];
        InputBindingDisplay.UpdateDisplay(
        action,
        bindingIndex,
        bindingText,
        iconField
    );
    }

}
