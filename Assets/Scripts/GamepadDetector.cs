using UnityEngine;
using UnityEngine.InputSystem;

public enum GamepadType
{
    None,
    Xbox,
    PlayStation,
    Switch
}
public enum DeviceType { Keyboard, Gamepad }

public static class GamepadDetector
{
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

        if (displayName.Contains("switch") || layout.Contains("nintendoswitch"))
            return GamepadType.Switch;

        // fallback Xbox si inconnue
        return GamepadType.PlayStation;
    }
}
