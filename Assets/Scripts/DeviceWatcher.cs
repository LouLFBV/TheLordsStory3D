using UnityEngine;
using UnityEngine.InputSystem;

public class DeviceWatcher : MonoBehaviour
{
    private void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (device is Gamepad &&
            (change == InputDeviceChange.Added || change == InputDeviceChange.Disconnected))
        {
            GamepadIconDatabase.instance.UpdateActiveSet();
        }
    }
}
