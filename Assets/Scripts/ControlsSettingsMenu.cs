using UnityEngine;
using UnityEngine.InputSystem;

public class ControlsSettingsMenu : MonoBehaviour
{
    public PlayerInput playerInput;
    public InputRebindUI[] rebindButtons;

    private void OnEnable()
    {
        InputRebindManager.LoadRebinds(playerInput);

        foreach (var rb in rebindButtons)
            rb.RefreshDisplay();
    }

    public void ResetToDefault()
    {
        InputRebindManager.ResetRebinds(playerInput);

        foreach (var rb in rebindButtons)
            rb.RefreshDisplay();
    }
}
