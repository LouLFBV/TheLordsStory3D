using UnityEngine;

public class NavigationController : MonoBehaviour
{
    public enum ActivePanel { Inventory, Tooltip, Menu, QuestJournal }

    public ActivePanel currentPanel;

    public UINavigationManager inventoryManager;
    public UINavigationManager tooltipManager;
    public UINavigationManager menuManager;
    public UINavigationManager journalManager;

    private PlayerControls controls;

    private void Awake()
    {
        controls = new PlayerControls();
    }
    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();
    private void Start()
    {
        SetActivePanel(ActivePanel.Inventory);
    }
    private void Update()
    {
        if (controls.UI.Cancel.triggered)
        {
            SetActivePanel((ActivePanel)(((int)currentPanel + 1) % 4));
        }

    }
    public void SetActivePanel(ActivePanel panel)
    {
        currentPanel = panel;
        inventoryManager.isActive = panel == ActivePanel.Inventory;
        tooltipManager.isActive = panel == ActivePanel.Tooltip;
        menuManager.isActive = panel == ActivePanel.Menu;
        journalManager.isActive = panel == ActivePanel.QuestJournal;
    }

}
