using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public Button loadGameButton;

    public static bool loadSaveData;

    public Button clearSavedDataButton;

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

        // Initialisation du bouton de chargement de données
        //bool saveFileExist = (System.IO.File.Exists(Application.persistentDataPath + "/SaveData.json"));
        //if (loadGameButton != null)
        //{
        //    loadGameButton.interactable = saveFileExist;
        //}
        //clearSavedDataButton.interactable = saveFileExist;

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
    public void LoadScene(string sceneName)
    {
        loadSaveData = false;
        SceneManager.LoadScene(sceneName);
    }

    public void LoadDataScene(string sceneName)
    {
        loadSaveData = true;
        SceneManager.LoadScene(sceneName);
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

    public void ClearSavedData()
    {
        string filePath = Application.persistentDataPath + "/SaveData.json";
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
            Debug.Log("Saved data cleared successfully.");
            if (loadGameButton != null)
            {
                loadGameButton.interactable = false;
            }
            clearSavedDataButton.interactable = false;
        }
        else
        {
            Debug.LogWarning("No saved data found to clear.");
        }
    }

    public void EnableDisableOptionsPanel()
    {
        optionsPanel.SetActive(!optionsPanel.activeSelf);
    }

    public void NewGame(int slot)
    {
        SaveManager.Instance.SetCurrentSlot(slot);
        SceneManager.LoadScene("Donjon");
    }

    public void LoadGame(int slot)
    {
        SaveManager.Instance.LoadGame(slot);
    }

}
