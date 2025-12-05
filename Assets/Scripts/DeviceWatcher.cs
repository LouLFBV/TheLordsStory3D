using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class DeviceWatcher : MonoBehaviour
{
    private void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
        InputSystem.onEvent += OnInputEvent;
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
        InputSystem.onEvent -= OnInputEvent;
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (device is Gamepad &&
            (change == InputDeviceChange.Added || change == InputDeviceChange.Disconnected))
        {
            InputIconDatabase.instance.UpdateGamepadSet();
        }
    }

    private void OnInputEvent(InputEventPtr eventPtr, InputDevice device)
    {
        // On met à jour le set si le device actif change
        if (device is Gamepad || device is Keyboard || device is Mouse)
        {
            InputIconDatabase.instance.UpdateGamepadSet();
        }
    }
}
