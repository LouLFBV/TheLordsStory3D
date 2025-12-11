using UnityEngine;
public class UIPanelWatcher : MonoBehaviour
{
    private void OnEnable()
    {
        var uiManager = UIManager.instance;
        if (uiManager != null)
        {
            uiManager.HandlePanelOpened();
        }
    }

    private void OnDisable()
    {
        var uiManager = UIManager.instance;
        if (uiManager != null)
        {
            uiManager.HandlePanelClosed();
        }
    }
}
