using UnityEngine;
using UnityEngine.InputSystem;

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
}
