using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private List<GameObject> otherPanels;
    private PlayerControls controls;

    [SerializeField] private UINavigationManager navManager;

    void Awake()
    {
        controls = new PlayerControls();
    }
    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    private bool IsAnyPanelActive()
    {
        return otherPanels.Any(panel => panel != null && panel.activeSelf);
    }

    void Update()
    {
        if (controls.UI.Menu.triggered && !IsAnyPanelActive())
        {
            bool isActive = !pauseMenuUI.activeSelf;

            if(isActive)
            {
                OpenMenu();
            }
            else
            {
                CloseMenu();
            }
        }
    }

    private void OpenMenu()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        if (navManager != null)
        {
            navManager.onCancel = CloseMenu;
        }
    }
    private void CloseMenu()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        if (navManager != null)
        {
            navManager.onCancel = null;
        }
    }
}
