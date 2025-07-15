using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject mainMenuPanel;
    public GameObject playMenuPanel;
    public GameObject settingsPanel;
    public GameObject creditsPanel;
    public GameObject teamSelectionPanel;
    
    [Header("Main Menu Buttons")]
    public Button playButton;
    public Button settingsButton;
    public Button creditsButton;
    public Button exitButton;
    
    [Header("Play Menu Buttons")]
    public Button quickMatchButton;
    public Button tournamentButton;
    public Button careerButton;
    public Button multiplayerButton;
    public Button backFromPlayButton;
    
    [Header("Settings Components")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Dropdown qualityDropdown;
    public Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public Button backFromSettingsButton;
    
    [Header("Team Selection")]
    public GameObject teamCardPrefab;
    public Transform teamGrid;
    public Button confirmTeamButton;
    public Button backFromTeamButton;
    
    [Header("Animation")]
    public Animator menuAnimator;
    public float fadeSpeed = 1f;
    
    private TeamData selectedTeam;
    private AudioManager audioManager;
    
    void Start()
    {
        audioManager = AudioManager.Instance;
        
        // Configurar botones
        SetupButtons();
        
        // Configurar settings
        SetupSettings();
        
        // Mostrar panel principal
        ShowMainMenu();
        
        // Generar equipos para selección
        GenerateTeamCards();
    }
    
    void SetupButtons()
    {
        // Botones del menú principal
        playButton.onClick.AddListener(() => {
            audioManager.PlayButtonSound();
            ShowPlayMenu();
        });
        
        settingsButton.onClick.AddListener(() => {
            audioManager.PlayButtonSound();
            ShowSettings();
        });
        
        creditsButton.onClick.AddListener(() => {
            audioManager.PlayButtonSound();
            ShowCredits();
        });
        
        exitButton.onClick.AddListener(() => {
            audioManager.PlayButtonSound();
            ExitGame();
        });
        
        // Botones del menú de juego
        quickMatchButton.onClick.AddListener(() => {
            audioManager.PlayButtonSound();
            ShowTeamSelection();
        });
        
        tournamentButton.onClick.AddListener(() => {
            audioManager.PlayButtonSound();
            StartTournament();
        });
        
        careerButton.onClick.AddListener(() => {
            audioManager.PlayButtonSound();
            StartCareer();
        });
        
        multiplayerButton.onClick.AddListener(() => {
            audioManager.PlayButtonSound();
            ShowMultiplayer();
        });
        
        backFromPlayButton.onClick.AddListener(() => {
            audioManager.PlayButtonSound();
            ShowMainMenu();
        });
        
        // Botones de configuración
        backFromSettingsButton.onClick.AddListener(() => {
            audioManager.PlayButtonSound();
            ShowMainMenu();
        });
        
        // Botones de selección de equipo
        confirmTeamButton.onClick.AddListener(() => {
            audioManager.PlayButtonSound();
            StartQuickMatch();
        });
        
        backFromTeamButton.onClick.AddListener(() => {
            audioManager.PlayButtonSound();
            ShowPlayMenu();
        });
    }
    
    void SetupSettings()
    {
        // Configurar sliders de volumen
        masterVolumeSlider.value = audioManager.masterVolume;
        musicVolumeSlider.value = audioManager.musicVolume;
        sfxVolumeSlider.value = audioManager.sfxVolume;
        
        // Eventos de sliders
        masterVolumeSlider.onValueChanged.AddListener(audioManager.SetMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(audioManager.SetMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(audioManager.SetSFXVolume);
        
        // Configurar calidad
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new System.Collections.Generic.List<string> { "Bajo", "Medio", "Alto", "Ultra" });
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.onValueChanged.AddListener(SetQuality);
        
        // Configurar resolución
        SetupResolutionDropdown();
        
        // Configurar pantalla completa
        fullscreenToggle.isOn = Screen.fullScreen;
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
    }
    
    void SetupResolutionDropdown()
    {
        resolutionDropdown.ClearOptions();
        
        System.Collections.Generic.List<string> resolutionOptions = new System.Collections.Generic.List<string>();
        Resolution[] resolutions = Screen.resolutions;
        
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            resolutionOptions.Add(option);
        }
        
        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.value = GetCurrentResolutionIndex();
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }
    
    int GetCurrentResolutionIndex()
    {
        Resolution[] resolutions = Screen.resolutions;
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                return i;
            }
        }
        return 0;
    }
    
    void GenerateTeamCards()
    {
        // Generar algunos equipos de ejemplo
        string[] teamNames = { "Real Madrid", "Barcelona", "Manchester United", "Bayern Munich", "Juventus", "PSG", "Liverpool", "Chelsea" };
        
        for (int i = 0; i < teamNames.Length; i++)
        {
            TeamData team = TeamData.GenerateRandomTeam(teamNames[i], i + 1, "España");
            CreateTeamCard(team);
        }
    }
    
    void CreateTeamCard(TeamData team)
    {
        GameObject card = Instantiate(teamCardPrefab, teamGrid);
        TeamCard cardComponent = card.GetComponent<TeamCard>();
        
        if (cardComponent != null)
        {
            cardComponent.SetupTeam(team);
            cardComponent.OnTeamSelected += SelectTeam;
        }
    }
    
    void SelectTeam(TeamData team)
    {
        selectedTeam = team;
        audioManager.PlayButtonSound();
        
        // Actualizar UI para mostrar equipo seleccionado
        confirmTeamButton.interactable = true;
    }
    
    void ShowMainMenu()
    {
        HideAllPanels();
        mainMenuPanel.SetActive(true);
        
        if (menuAnimator != null)
        {
            menuAnimator.SetTrigger("ShowMain");
        }
    }
    
    void ShowPlayMenu()
    {
        HideAllPanels();
        playMenuPanel.SetActive(true);
        
        if (menuAnimator != null)
        {
            menuAnimator.SetTrigger("ShowPlay");
        }
    }
    
    void ShowSettings()
    {
        HideAllPanels();
        settingsPanel.SetActive(true);
        
        if (menuAnimator != null)
        {
            menuAnimator.SetTrigger("ShowSettings");
        }
    }
    
    void ShowCredits()
    {
        HideAllPanels();
        creditsPanel.SetActive(true);
        
        if (menuAnimator != null)
        {
            menuAnimator.SetTrigger("ShowCredits");
        }
    }
    
    void ShowTeamSelection()
    {
        HideAllPanels();
        teamSelectionPanel.SetActive(true);
        confirmTeamButton.interactable = false;
        
        if (menuAnimator != null)
        {
            menuAnimator.SetTrigger("ShowTeamSelection");
        }
    }
    
    void HideAllPanels()
    {
        mainMenuPanel.SetActive(false);
        playMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        teamSelectionPanel.SetActive(false);
    }
    
    void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        audioManager.PlayButtonSound();
    }
    
    void SetResolution(int resolutionIndex)
    {
        Resolution resolution = Screen.resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        audioManager.PlayButtonSound();
    }
    
    void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        audioManager.PlayButtonSound();
    }
    
    void StartQuickMatch()
    {
        if (selectedTeam != null)
        {
            // Guardar equipo seleccionado
            GameData.Instance.selectedTeam = selectedTeam;
            
            // Cargar escena del juego
            SceneManager.LoadScene("GameplayScene");
        }
    }
    
    void StartTournament()
    {
        // Cargar escena de torneo
        SceneManager.LoadScene("TournamentScene");
    }
    
    void StartCareer()
    {
        // Cargar modo carrera
        SceneManager.LoadScene("CareerScene");
    }
    
    void ShowMultiplayer()
    {
        // Mostrar opciones multijugador
        SceneManager.LoadScene("MultiplayerScene");
    }
    
    void ExitGame()
    {
        Application.Quit();
    }
    
    // Método para hover effects
    public void OnButtonHover()
    {
        audioManager.PlayHoverSound();
    }
}