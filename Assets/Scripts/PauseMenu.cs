using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private List<GameObject> otherPanels;
    private bool IsAnyPanelActive()
    {
        return otherPanels.Any(panel => panel != null && panel.activeSelf);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !IsAnyPanelActive())
        {
            bool isActive = !pauseMenuUI.activeSelf;

            pauseMenuUI.SetActive(isActive);
            optionsPanel.SetActive(false);

            Time.timeScale = isActive ? 0f : 1f;
        }
    }
}
