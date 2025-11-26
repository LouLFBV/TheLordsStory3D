using UnityEngine;

public class GamepadIconDatabase : MonoBehaviour
{
    public static GamepadIconDatabase instance;

    public ButtonIconSet xboxSet;
    public ButtonIconSet playstationSet;
    public ButtonIconSet switchSet;

    private ButtonIconSet activeSet;

    private void Awake()
    {
        instance = this;
        UpdateActiveSet();
    }

    public void UpdateActiveSet()
    {
        var type = GamepadDetector.DetectCurrentGamepad();

        switch (type)
        {
            case GamepadType.PlayStation:
                activeSet = playstationSet;
                break;
            case GamepadType.Switch:
                activeSet = switchSet;
                break;
            default:
                activeSet = xboxSet;
                break;
        }
    }

    public Sprite GetIcon(string controlPath)
    {
        if (activeSet == null)
            UpdateActiveSet();

        return activeSet.GetIcon(controlPath);
    }
}
