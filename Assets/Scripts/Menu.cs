using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Menu : MonoBehaviour
{
    public Button newGameButton;
    public Button loadGameButton;
    public Button loadLastGameButton;
    public Button clearSavedDataButton;
    [SerializeField] private Animator animatorPanelChargerPartie;

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

    [Header("Settings Panel")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject controllerInputPanel;
    [SerializeField] private GameObject keyboardInpuPanel;

    [SerializeField] private bool isMainMenu = false;
    [SerializeField] private GameObject partiesPanel;

    [Header("Input Settings UI")]
    [SerializeField] private Slider mouseSensitivitySlider;
    [SerializeField] private Slider gamepadSensitivitySlider;
    [SerializeField] private Slider deadzoneSlider;


    private int pendingSlot;
    private bool isNewGame;
    private bool isTransitioning = false;


    public static Menu Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (isMainMenu)
        {
            Time.timeScale = 1f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            loadLastGameButton.interactable = SaveManager.Instance.HasSave(pendingSlot);
            DisableSlotUI(false);

            ActiveNewGame();

            OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
            return;
        }
        // Initialisation du slider de volume
        audioMixer.GetFloat("Volume", out float soundValueForSlider);
        volumeSlider.value = soundValueForSlider;



        


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

        if (PlayerController.Instance != null && PlayerController.Instance.Input != null)
        {
            var input = PlayerController.Instance.Input;
            mouseSensitivitySlider.value = input.mouseSensitivity;
            gamepadSensitivitySlider.value = input.gamepadSensitivity;
            deadzoneSlider.value = input.stickDeadzone;
        }

        resolutionDropdown.AddOptions(resolutionsOptions);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // Initialisation du mode plein écran
        fullScreenToggle.isOn = Screen.fullScreen;

        Time.timeScale = 1f; // Assurez-vous que le temps est normalisé au démarrage du menu

        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Bootstrap")
        {
            Debug.Log("Menu Start: LoadScene Bootstrap");
            if (TransitionPanel.Instance != null)
                TransitionPanel.Instance.PlayTransitionIn();
        }
    }
    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
    public void LoadMenu()
    {
        if (TransitionPanel.Instance != null)
            TransitionPanel.Instance.PlayTransitionOut();
        else SceneManager.LoadScene("MainMenu");
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



    #region Save System


    public void NewGame()
    {
        int slot = -1;
        SaveManager saveManager = SaveManager.Instance;
        for (int i = 1; i <= 3; i++)
        {
            if (!saveManager.HasSave(i))
            {
                slot = i;
                break;
            }
        }
        if (isTransitioning) return;
        isTransitioning = true;

        pendingSlot = slot;
        isNewGame = true;
        TransitionPanel.Instance.PlayTransitionOut();
    }
    public void Continue()
    {
        if (isTransitioning) return;
        isTransitioning = true;
        isNewGame = false;
        TransitionPanel.Instance.PlayTransitionOut();
    }
    public void SaveGame() { SaveManager.Instance.SaveGame(); }
    public void LoadGame(int slot)
    {
        pendingSlot = slot;
        isNewGame = false;
        TransitionPanel.Instance.PlayTransitionOut();
    }
    public void OpenPanelParties()
    {
        partiesPanel.SetActive(!partiesPanel.activeSelf);
        DisableSlotUI(false);
    }

    //  Appelé par l'Animation Event
    public void OnOpenAnimationFinished()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            SceneManager.LoadScene("MainMenu");
        }
        else if (isNewGame)
        {
            SaveManager.Instance.SetCurrentSlot(pendingSlot);
            SceneManager.LoadScene("Donjon");
        }
        else
        {
            SaveManager.Instance.LoadGame(pendingSlot);
        }
    }

    private void ActiveNewGame()
    {
        SaveManager saveManager = SaveManager.Instance;
        newGameButton.interactable = !(saveManager.HasSave(1) && saveManager.HasSave(2) && saveManager.HasSave(3));
    }


    public void SelectSlot(int slot)
    {
        pendingSlot = slot;
        DisableSlotUI(true);
    }

    private void DisableSlotUI(bool actived)
    {
        clearSavedDataButton.interactable = actived;
        loadGameButton.interactable = actived;
    }

    public void OnDeleteSlot()
    {
        SaveManager.Instance.DeleteSave(pendingSlot);

        DisableSlotUI(false);
    }


    public void ConfirmDeleteSlot(int slot)
    {
        // ouvrir un popup "Êtes-vous sûr ?"
    }


    #endregion

    #region Settings Panel

    public void OpenSettingsPanel()
    {
        settingsPanel.SetActive(true);
        optionsPanel.SetActive(true);
        controllerInputPanel.SetActive(false);
        keyboardInpuPanel.SetActive(false);
    }

    public void OpenControllerInputPanel()
    {
        controllerInputPanel.SetActive(true);
        keyboardInpuPanel.SetActive(false);
        optionsPanel.SetActive(false);
    }

    public void OpenKeyboardInputPanel()
    {
        controllerInputPanel.SetActive(false);
        keyboardInpuPanel.SetActive(true);
        optionsPanel.SetActive(false);
    }

    public void OpenOptionsPanel()
    {
        optionsPanel.SetActive(true);
        controllerInputPanel.SetActive(false);
        keyboardInpuPanel.SetActive(false);
    }
    public void CloseAllSettingsPanel()
    {
        settingsPanel.SetActive(false);
        controllerInputPanel.SetActive(false);
        keyboardInpuPanel.SetActive(false);
        optionsPanel.SetActive(false);
        PlayerPrefs.Save();
    }

    public void OnMouseSensitivityChanged(float value)
    {
        if (PlayerController.Instance != null && PlayerController.Instance.Input != null)
        {
            PlayerController.Instance.Input.mouseSensitivity = value;
            // Optionnel : Sauvegarder immédiatement
            PlayerPrefs.SetFloat("MouseSensi", value);
        }
    }

    public void OnGamepadSensitivityChanged(float value)
    {
        if (PlayerController.Instance != null && PlayerController.Instance.Input != null)
        {
            PlayerController.Instance.Input.gamepadSensitivity = value;
            PlayerPrefs.SetFloat("GamepadSensi", value);
        }
    }

    public void OnDeadzoneChanged(float value)
    {
        if (PlayerController.Instance != null && PlayerController.Instance.Input != null)
        {
            PlayerController.Instance.Input.stickDeadzone = value;
            PlayerPrefs.SetFloat("Deadzone", value);
        }
    }

    #endregion
}
