using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public static class InputRebindManager
{
    private const string RebindsKey = "input_rebinds";

    public static void SaveRebinds(PlayerInput playerInput)
    {
        string json = playerInput.actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString(RebindsKey, json);
    }

    public static void LoadRebinds(PlayerInput playerInput)
    {
        if (!PlayerPrefs.HasKey(RebindsKey))
            return;

        string json = PlayerPrefs.GetString(RebindsKey);
        playerInput.actions.LoadBindingOverridesFromJson(json);
    }

    public static void ResetRebinds(PlayerInput playerInput)
    {
        PlayerPrefs.DeleteKey(RebindsKey);
        playerInput.actions.RemoveAllBindingOverrides();
    }

    public static void UpdateBindingDisplayForAction(InputAction action, Image iconField, DeviceType type)
    {
        InputBinding binding = default;

        if (type == DeviceType.Gamepad)
        {
            binding = action.bindings.FirstOrDefault(b =>
                !string.IsNullOrEmpty(b.effectivePath) &&
                b.effectivePath.Contains("<Gamepad>")
            );
        }
        else // Keyboard + Mouse
        {
            binding = action.bindings.FirstOrDefault(b =>
                !string.IsNullOrEmpty(b.effectivePath) &&
                (b.effectivePath.Contains("<Keyboard>") || b.effectivePath.Contains("<Mouse>"))
            );
        }

        if (binding != default)
        {
            Sprite icon = InputIconDatabase.instance.GetIcon(binding.effectivePath);

            if (icon != null)
            {
                iconField.sprite = icon;
                iconField.enabled = true;
                return;
            }
        }

        // Fallback sécurité
        iconField.enabled = false;
    }
}
