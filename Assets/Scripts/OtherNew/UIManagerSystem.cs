using UnityEngine;
using System.Collections.Generic;

public class UIManagerSystem : MonoBehaviour
{
    public static UIManagerSystem Instance;

    [SerializeField] private Menu menu;
    // On garde les valeurs en mémoire pour les rétablir plus tard
    private float _defaultRotationSpeed;
    private float _defaultVerticalSpeed;

    [SerializeField] private GameObject crosshair;

    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject questsPanel;
    [SerializeField] private GameObject equipmentPanel;
    [SerializeField] private GameObject mapPanel;
    [SerializeField] private GameObject tooltipPanel;

    [SerializeField] private GameObject pauseMenuPanel;

    [Header("HUD Elements")]
    public List<GameObject> hudElements;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // On récupère les vitesses de base via l'Instance de la caméra
        if (ThirdPersonCameraController.Instance != null)
        {
            _defaultRotationSpeed = ThirdPersonCameraController.Instance.RotationSpeed;
            _defaultVerticalSpeed = ThirdPersonCameraController.Instance.VerticalSpeed;
        }
        ToggleCursor(false); // On commence sans le curseur
    }

    public void ToggleCursor(bool isVisible)
    {
        Cursor.visible = isVisible;
        Cursor.lockState = isVisible ? CursorLockMode.None : CursorLockMode.Locked;

        // Si l'UI est visible, vitesse = 0. Sinon, on remet les vitesses par défaut.
        if (ThirdPersonCameraController.Instance != null)
        {
            ThirdPersonCameraController.Instance.RotationSpeed = isVisible ? 0f : _defaultRotationSpeed;
            ThirdPersonCameraController.Instance.VerticalSpeed = isVisible ? 0f : _defaultVerticalSpeed;
        }
    }

    public void OpenPanel(UIPanelType type)
    {
        // Ferme tout d'abord
        CloseAll();

        switch (type)
        {
            case UIPanelType.Inventory:
                inventoryPanel.SetActive(true);
                break;
            case UIPanelType.PauseMenu:
                pauseMenuPanel.SetActive(true);
                break;
        }
    }

    public void CloseAll()
    {
        inventoryPanel.SetActive(false);
        pauseMenuPanel.SetActive(false);
        questsPanel.SetActive(false);
        equipmentPanel.SetActive(false);
        mapPanel.SetActive(false);
        tooltipPanel.SetActive(false);
        menu.CloseAllSettingsPanel();
        NewQuestLog.instance.DesactivePanel();
        ActiveDesactiveHUD(true);
    }


    public void ShowCrosshair(bool show)
    {
        if (crosshair != null)
        {
            crosshair.SetActive(show);
        }
    }

    #region --- Méthodes d'ouverture spécifiques pour les boutons de l'UI ---
    public void OpenInventoryAndCloseOthers()
    {
        CloseAll();
        inventoryPanel.SetActive(true);
    }
    public void OpenQuestsAndCloseOthers()
    {
        CloseAll();
        NewQuestLog.instance.OnAffichageQuestPanel(NewQuestManager.instance.activeQuests);
        questsPanel.SetActive(true);
    }
    public void OpenEquipmentAndCloseOthers()
    {
        CloseAll();
        ActiveDesactiveHUD(false);
        equipmentPanel.SetActive(true);
    }
    public void OpenMapAndCloseOthers()
    {
        CloseAll();
        mapPanel.SetActive(true);
    }

    private void ActiveDesactiveHUD(bool actived)
    {
        foreach (GameObject element in hudElements)
        {
            if (element != null)
                element.SetActive(actived);
        }
    }
    #endregion 
}