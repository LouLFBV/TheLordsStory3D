using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private List<GameObject> otherPanels;
    [SerializeField] private PlayerInput playerInput;
    private bool openPanel = false;

    [SerializeField] private UINavigationManager navManager;

    void OnEnable()
    {
        playerInput.actions["Menu"].Enable();
        playerInput.actions["Menu"].performed += OnMenuPerformed;
        playerInput.actions["Menu"].canceled += OnMenuCanceled;
    }
    void OnDisable()
    {
        playerInput.actions["Menu"].performed -= OnMenuPerformed;
        playerInput.actions["Menu"].canceled -= OnMenuCanceled;
        playerInput.actions["Menu"].Disable();
    }

    private void OnMenuPerformed(InputAction.CallbackContext context)
    {
        openPanel = true;
    }

    private void OnMenuCanceled(InputAction.CallbackContext context)
    {
        openPanel = false;
    }

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
