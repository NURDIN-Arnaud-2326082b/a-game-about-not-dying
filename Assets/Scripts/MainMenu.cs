using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour
{

    [SerializeField]
    private GameObject optionsMenuPanel;

    [SerializeField]
    private GameObject mainMenuPanel;

    [SerializeField]
    public Button loadGameButton;

    [SerializeField]
    private Dropdown resolutionDropdown;

    [SerializeField]
    private AudioMixer audioMixer;

    [SerializeField]
    private Slider volumeSlider;

    [SerializeField]
    private Dropdown qualityDropdown;

    [SerializeField]
    public Button deleteSaveFileButton;

    [SerializeField]
    private GameObject deathPanel;

    public static bool loadSavedGame = false;

    void Start()
    {
        //initialiser les panneaux
        mainMenuPanel.SetActive(true);
        optionsMenuPanel.SetActive(false);

        //vérifier si un fichier de sauvegarde existe pour activer le bouton de chargement
        loadGameButton.interactable = System.IO.File.Exists(Application.persistentDataPath + "/savefile.json");

        //initialiser les résolutions disponibles
        Resolution[] resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        foreach (Resolution resolution in resolutions)
        {
            string option = resolution.width + " x " + resolution.height + " (" + resolution.refreshRateRatio + "Hz)";
            options.Add(option);
            if (resolution.width == Screen.currentResolution.width &&
                resolution.height == Screen.currentResolution.height)
            {
                currentResolutionIndex = options.Count - 1;
            }
        }
        //ranger du plus haut au plus bas
        options.Reverse();
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        //initialiser le volume
        audioMixer.GetFloat("Volume", out float soundValue);
        volumeSlider.value = soundValue;

        //initialiser la qualité graphique
        string[] qualities = QualitySettings.names;
        int currentQualityIndex = 0;
        qualityDropdown.ClearOptions();
        List<string> qualityOptions = new List<string>();
        foreach (string quality in qualities)
        {
            qualityOptions.Add(quality);
            if (quality == QualitySettings.names[currentQualityIndex])
            {
                currentQualityIndex = qualityOptions.Count - 1;
            }
        }

        qualityOptions.Reverse();
        qualityDropdown.AddOptions(qualityOptions);
        qualityDropdown.value = currentQualityIndex;
        qualityDropdown.RefreshShownValue();
        SetQuality(currentQualityIndex);

    }

    public void NewGameButton()
    {
        deathPanel.SetActive(false);
        loadSavedGame = false;
        deleteSaveFileButton.interactable = true;
        if (System.IO.File.Exists(Application.persistentDataPath + "/savefile.json"))
        {
            System.IO.File.Delete(Application.persistentDataPath + "/savefile.json");
        }
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGameButton()
    {
        Application.Quit();
    }

    public void LoadGameButton()
    {
        //si le panneau de mort existe dans la sène
        if (deathPanel.activeSelf)
        {
            deathPanel.SetActive(false);
        }
        loadSavedGame = true;
        SceneManager.LoadScene("GameScene");
    }

    public void OpenOptionsMenu()
    {
        mainMenuPanel.SetActive(false);
        optionsMenuPanel.SetActive(true);
    }

    public void CloseOptionsMenu()
    {
        mainMenuPanel.SetActive(true);
        optionsMenuPanel.SetActive(false);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution[] resolutions = Screen.resolutions.Reverse().ToArray();
        Resolution screenResolution = resolutions[resolutionIndex];
        Screen.SetResolution(screenResolution.width, screenResolution.height, Screen.fullScreen);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Volume", volume);
    }

    public void DeleteSaveFile()
    {
        System.IO.File.Delete(Application.persistentDataPath + "/savefile.json");
        loadGameButton.interactable = false;
        deleteSaveFileButton.interactable = false;
        
    }

    public void SetQuality(int qualityIndex)
    {
        string[] qualities = QualitySettings.names.Reverse().ToArray();
        string qualityName = qualities[qualityIndex];
        int index = System.Array.IndexOf(QualitySettings.names, qualityName);
        QualitySettings.SetQualityLevel(index);
    }

    public void GoBackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}