using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public List<Button> newGameButtons;
    public List<Button> loadGameButtons;


    public List<Button> clearSavedDataButtons;

    [SerializeField]
    private Dropdown resolutionDropdown;

    [SerializeField]
    private Dropdown qualitiesDropdown;

    [SerializeField]
    private AudioMixer audioMixer;

    [SerializeField]
    private Slider volumeSlider;

    [SerializeField]
    private Toggle fullScreenToggle;

    [SerializeField]
    private GameObject optionsPanel;

    [SerializeField] private bool isMainMenu = false;  

    private void Start()
    {
        if (isMainMenu)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        // Initialisation du slider de volume
        audioMixer.GetFloat("Volume", out float soundValueForSlider);
        volumeSlider.value = soundValueForSlider;


        // Désactivation des boutons de chargement et de suppression si pas de sauvegarde
        foreach (Button loadButton in loadGameButtons)
        {
            int slotIndex = loadGameButtons.IndexOf(loadButton) + 1;
            Debug.Log("Vérification de la sauvegarde pour le slot " + slotIndex);
            if (!SaveManager.Instance.HasSave(slotIndex))
            {
                Debug.Log("Désactivation du bouton de chargement pour le slot " + slotIndex);
                loadButton.interactable = false;
            }
        }
        foreach (Button clearButton in clearSavedDataButtons)
        {
            int slotIndex = clearSavedDataButtons.IndexOf(clearButton) + 1;
            if (!SaveManager.Instance.HasSave(slotIndex))
            {
                clearButton.interactable = false;
            }
        }
        foreach (Button newGameButton in newGameButtons)
        {
            int slotIndex = clearSavedDataButtons.IndexOf(newGameButton) + 1;
            if (SaveManager.Instance.HasSave(slotIndex))
            {
                newGameButton.interactable = false;
            }
        }

        // Initialisation des qualités graphiques
        QualitySettings.SetQualityLevel(QualitySettings.names.Length - 1, true);

        string[] qualities = QualitySettings.names;
        qualitiesDropdown.ClearOptions();

        List<string> qualityOptions = new List<string>();
        for (int i = 0; i < qualities.Length; i++)
        {
            qualityOptions.Add(qualities[i]);
        }

        qualitiesDropdown.AddOptions(qualityOptions);

        // Synchronisation du dropdown avec la qualité réelle
        qualitiesDropdown.value = QualitySettings.GetQualityLevel();
        qualitiesDropdown.RefreshShownValue();


        // Initialisation des différentes résolutions
        Resolution[] resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> resolutionsOptions = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + " (" + resolutions[i].refreshRateRatio + " Hz) ";
            resolutionsOptions.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(resolutionsOptions);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // Initialisation du mode plein écran
        fullScreenToggle.isOn = Screen.fullScreen;

        Time.timeScale = 1f; // Assurez-vous que le temps est normalisé au démarrage du menu
    }
    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
    public void Quit()
    {
        Application.Quit();
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolutions = Screen.resolutions[resolutionIndex];
        Screen.SetResolution(resolutions.width, resolutions.height, Screen.fullScreen);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Volume", volume);
    }


    public void EnableDisableOptionsPanel()
    {
        optionsPanel.SetActive(!optionsPanel.activeSelf);
    }

    #region Save System

    public void NewGame(int slot)
    {
        SaveManager.Instance.SetCurrentSlot(slot);
        SceneManager.LoadScene("Donjon");
    }

    public void LoadGame(int slot)
    {
        SaveManager.Instance.LoadGame(slot);
    }

    private void DisableSlotUI(int slot)
    {
        int index = slot - 1;

        if (index < 0) return;

        if (index < loadGameButtons.Count)
            loadGameButtons[index].interactable = false;

        if (index < clearSavedDataButtons.Count)
            clearSavedDataButtons[index].interactable = false;
    }

    public void OnDeleteSlot(int slot)
    {
        SaveManager.Instance.DeleteSave(slot);
        DisableSlotUI(slot);
    }


    public void ConfirmDeleteSlot(int slot)
    {
        // ouvrir un popup "Êtes-vous sûr ?"
    }


    #endregion
}
