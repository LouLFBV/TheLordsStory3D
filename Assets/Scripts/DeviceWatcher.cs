using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;

public class DeviceWatcher : MonoBehaviour
{
    public static DeviceWatcher Instance;

    public DeviceType CurrentDevice { get; private set; }

    public event Action<DeviceType> OnDeviceChanged;

    [SerializeField] private PlayerInput playerInput;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        CurrentDevice = Gamepad.current != null ? DeviceType.Gamepad : DeviceType.Keyboard;
    }

    private void OnEnable()
    {
        playerInput.onActionTriggered += OnActionTriggered;
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    private void OnDisable()
    {
        playerInput.onActionTriggered -= OnActionTriggered;
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void OnActionTriggered(InputAction.CallbackContext ctx)
    {
        var dev = ctx.control.device;

        if (dev is Gamepad)
            SwitchTo(DeviceType.Gamepad);

        else if (dev is Keyboard || dev is Mouse)
            SwitchTo(DeviceType.Keyboard);
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (device is Gamepad &&
           (change == InputDeviceChange.Added || change == InputDeviceChange.Reconnected))
        {
            SwitchTo(DeviceType.Gamepad);
        }

        if (device is Gamepad && change == InputDeviceChange.Disconnected)
            SwitchTo(DeviceType.Keyboard);
    }

    private void SwitchTo(DeviceType newDevice)
    {
        if (newDevice == CurrentDevice)
            return;

        CurrentDevice = newDevice;

        OnDeviceChanged?.Invoke(CurrentDevice);

        StartCoroutine(RefreshIcons());
    }

    private IEnumerator RefreshIcons()
    {
        yield return null;      // attendre 1 frame
        yield return null;      // attendre une deuxième frame
        InputIconDatabase.instance.UpdateGamepadSet();
    }
}
