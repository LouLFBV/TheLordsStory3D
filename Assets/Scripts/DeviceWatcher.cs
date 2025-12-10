using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class DeviceWatcher : MonoBehaviour
{
    public static DeviceWatcher Instance;

    public DeviceType CurrentDevice { get; private set; }

    public event System.Action<DeviceType> OnDeviceChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        // Device par défaut
        CurrentDevice = Gamepad.current != null ? DeviceType.Gamepad : DeviceType.Keyboard;
    }

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
        // Branche/débranche un gamepad
        if (device is Gamepad &&
            (change == InputDeviceChange.Added || change == InputDeviceChange.Reconnected))
        {
            SwitchTo(DeviceType.Gamepad);
        }

        if (device is Gamepad &&
            (change == InputDeviceChange.Disconnected))
        {
            SwitchTo(DeviceType.Keyboard);
        }
    }

    private void OnInputEvent(InputEventPtr eventPtr, InputDevice device)
    {
        if (device is Keyboard || device is Mouse)
            SwitchTo(DeviceType.Keyboard);

        else if (device is Gamepad)
            SwitchTo(DeviceType.Gamepad);
    }

    private void SwitchTo(DeviceType newDevice)
    {
        if (newDevice == CurrentDevice)
            return;

        CurrentDevice = newDevice;


        // Notifie tout le monde
        OnDeviceChanged?.Invoke(CurrentDevice);

        // Mise à jour automatique des icônes
        InputIconDatabase.instance.UpdateGamepadSet();
    }
}
