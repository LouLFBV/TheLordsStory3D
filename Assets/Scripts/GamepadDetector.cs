using UnityEngine;
using UnityEngine.InputSystem;

public enum GamepadType
{
    None,
    Xbox,
    PlayStation,
    Switch
}

public enum DeviceType
{
    Keyboard,
    Gamepad
}

public static class GamepadDetector
{
    /// <summary>
    /// Retourne TRUE si un gamepad est connecté et reconnu par le InputSystem.
    /// </summary>
    public static bool IsGamepadConnected()
    {
        return Gamepad.current != null;
    }

    /// <summary>
    /// Retourne le type exact du gamepad connecté (Xbox, PS, Switch…)
    /// </summary>
    public static GamepadType DetectCurrentGamepad()
    {
        if (Gamepad.current == null)
            return GamepadType.None;

        string layout = Gamepad.current.layout.ToLower();
        string displayName = Gamepad.current.displayName.ToLower();

        if (displayName.Contains("dual") || displayName.Contains("ps") || layout.Contains("dualshock"))
            return GamepadType.PlayStation;

        if (displayName.Contains("xbox") || layout.Contains("xinput"))
            return GamepadType.Xbox;

        if (displayName.Contains("switch") || displayName.Contains("nintendoswitch"))
            return GamepadType.Switch;

        // fallback
        return GamepadType.PlayStation;
    }

    /// <summary>
    /// Fournit directement le DeviceType (Keyboard ou Gamepad)
    /// </summary>
    public static DeviceType GetDeviceType()
    {
        return IsGamepadConnected() ? DeviceType.Gamepad : DeviceType.Keyboard;
    }
}
