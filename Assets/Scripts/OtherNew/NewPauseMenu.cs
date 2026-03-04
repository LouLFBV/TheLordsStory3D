using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class NewPauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private List<GameObject> otherPanels;
    private bool openPanel = false;

    [SerializeField] private UINavigationManager navManager;

    private bool IsAnyPanelActive()
    {
        return otherPanels.Any(panel => panel != null && panel.activeSelf);
    }

    void Update()
    {
        if (openPanel && !IsAnyPanelActive())
        {
            if (!pauseMenuUI.activeSelf)
            {
                OpenMenu();
            }
            else
            {
                CloseMenu();
            }
            openPanel = false;
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
        if (IsAnyPanelActive()) return;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        if (navManager != null)
        {
            navManager.onCancel = null;
        }
    }
}
