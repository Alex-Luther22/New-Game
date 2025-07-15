using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AdvancedSettings : MonoBehaviour
{
    public static AdvancedSettings Instance;
    
    [Header("Graphics Settings")]
    public Dropdown qualityDropdown;
    public Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public Slider frameRateSlider;
    public Text frameRateText;
    public Toggle vSyncToggle;
    public Toggle antiAliasingToggle;
    
    [Header("Audio Settings")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider crowdVolumeSlider;
    public Slider commentaryVolumeSlider;
    public Toggle muteToggle;
    
    [Header("Gameplay Settings")]
    public Slider difficultySlider;
    public Text difficultyText;
    public Slider gameSpeedSlider;
    public Text gameSpeedText;
    public Toggle autoSaveToggle;
    public Slider autoSaveIntervalSlider;
    public Text autoSaveIntervalText;
    
    [Header("Control Settings")]
    public Slider touchSensitivitySlider;
    public Text touchSensitivityText;
    public Toggle vibrateToggle;
    public Toggle showTutorialToggle;
    public Dropdown controlSchemeDropdown;
    
    [Header("Camera Settings")]
    public Slider cameraSpeedSlider;
    public Text cameraSpeedText;
    public Slider cameraZoomSlider;
    public Text cameraZoomText;
    public Toggle cameraShakeToggle;
    public Dropdown cameraViewDropdown;
    
    [Header("Network Settings")]
    public InputField playerNameInput;
    public Dropdown regionDropdown;
    public Toggle autoConnectToggle;
    public Slider pingThresholdSlider;
    public Text pingThresholdText;
    
    [Header("Advanced Graphics")]
    public Toggle shadowsToggle;
    public Slider shadowQualitySlider;
    public Text shadowQualityText;
    public Toggle particleEffectsToggle;
    public Slider particleQualitySlider;
    public Text particleQualityText;
    public Toggle bloomToggle;
    public Toggle motionBlurToggle;
    
    [Header("Language Settings")]
    public Dropdown languageDropdown;
    public Toggle subtitlesToggle;
    
    [Header("Debug Settings")]
    public Toggle showFPSToggle;
    public Toggle showPingToggle;
    public Toggle debugModeToggle;
    
    private GameSettings gameSettings;
    private AudioManager audioManager;
    private EffectsManager effectsManager;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        audioManager = AudioManager.Instance;
        effectsManager = EffectsManager.Instance;
        
        LoadSettings();
        SetupUI();
        SetupEventListeners();
    }
    
    void LoadSettings()
    {
        gameSettings = GameData.Instance?.gameSettings ?? new GameSettings();
    }
    
    void SetupUI()
    {
        // Configurar dropdowns
        SetupQualityDropdown();
        SetupResolutionDropdown();
        SetupLanguageDropdown();
        SetupControlSchemeDropdown();
        SetupCameraViewDropdown();
        SetupRegionDropdown();
        
        // Cargar valores actuales
        LoadCurrentValues();
    }
    
    void SetupQualityDropdown()
    {
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new List<string> { "Bajo", "Medio", "Alto", "Ultra" });
        qualityDropdown.value = gameSettings.qualityLevel;
    }
    
    void SetupResolutionDropdown()
    {
        resolutionDropdown.ClearOptions();
        
        List<string> resolutions = new List<string>();
        Resolution[] availableResolutions = Screen.resolutions;
        
        foreach (Resolution res in availableResolutions)
        {
            resolutions.Add($"{res.width} x {res.height}");
        }
        
        resolutionDropdown.AddOptions(resolutions);
    }
    
    void SetupLanguageDropdown()
    {
        languageDropdown.ClearOptions();
        languageDropdown.AddOptions(new List<string> { "Español", "English", "Français", "Deutsch", "Italiano", "Português" });
        
        string currentLanguage = gameSettings.language;
        int languageIndex = GetLanguageIndex(currentLanguage);
        languageDropdown.value = languageIndex;
    }
    
    void SetupControlSchemeDropdown()
    {
        controlSchemeDropdown.ClearOptions();
        controlSchemeDropdown.AddOptions(new List<string> { "Clásico", "Moderno", "Personalizado" });
    }
    
    void SetupCameraViewDropdown()
    {
        cameraViewDropdown.ClearOptions();
        cameraViewDropdown.AddOptions(new List<string> { "Seguimiento", "Táctico", "Cinematográfico", "Acción" });
    }
    
    void SetupRegionDropdown()
    {
        regionDropdown.ClearOptions();
        regionDropdown.AddOptions(new List<string> { "América", "Europa", "Asia", "Oceanía" });
    }
    
    void LoadCurrentValues()
    {
        // Graphics
        qualityDropdown.value = gameSettings.qualityLevel;
        fullscreenToggle.isOn = gameSettings.fullscreen;
        frameRateSlider.value = Application.targetFrameRate;
        vSyncToggle.isOn = QualitySettings.vSyncCount > 0;
        antiAliasingToggle.isOn = QualitySettings.antiAliasing > 0;
        
        // Audio
        masterVolumeSlider.value = gameSettings.masterVolume;
        musicVolumeSlider.value = gameSettings.musicVolume;
        sfxVolumeSlider.value = gameSettings.sfxVolume;
        crowdVolumeSlider.value = gameSettings.crowdVolume;
        commentaryVolumeSlider.value = gameSettings.commentaryVolume;
        
        // Gameplay
        difficultySlider.value = gameSettings.difficulty;
        gameSpeedSlider.value = gameSettings.gameplaySpeed;
        autoSaveToggle.isOn = gameSettings.autoSave;
        
        // Controls
        touchSensitivitySlider.value = 1f; // Default value
        vibrateToggle.isOn = gameSettings.vibrateOnMobile;
        showTutorialToggle.isOn = gameSettings.showTutorial;
        
        // Language
        subtitlesToggle.isOn = gameSettings.subtitlesEnabled;
        
        UpdateAllTexts();
    }
    
    void SetupEventListeners()
    {
        // Graphics
        qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
        frameRateSlider.onValueChanged.AddListener(OnFrameRateChanged);
        vSyncToggle.onValueChanged.AddListener(OnVSyncChanged);
        antiAliasingToggle.onValueChanged.AddListener(OnAntiAliasingChanged);
        
        // Audio
        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        crowdVolumeSlider.onValueChanged.AddListener(OnCrowdVolumeChanged);
        commentaryVolumeSlider.onValueChanged.AddListener(OnCommentaryVolumeChanged);
        muteToggle.onValueChanged.AddListener(OnMuteChanged);
        
        // Gameplay
        difficultySlider.onValueChanged.AddListener(OnDifficultyChanged);
        gameSpeedSlider.onValueChanged.AddListener(OnGameSpeedChanged);
        autoSaveToggle.onValueChanged.AddListener(OnAutoSaveChanged);
        autoSaveIntervalSlider.onValueChanged.AddListener(OnAutoSaveIntervalChanged);
        
        // Controls
        touchSensitivitySlider.onValueChanged.AddListener(OnTouchSensitivityChanged);
        vibrateToggle.onValueChanged.AddListener(OnVibrateChanged);
        showTutorialToggle.onValueChanged.AddListener(OnShowTutorialChanged);
        controlSchemeDropdown.onValueChanged.AddListener(OnControlSchemeChanged);
        
        // Camera
        cameraSpeedSlider.onValueChanged.AddListener(OnCameraSpeedChanged);
        cameraZoomSlider.onValueChanged.AddListener(OnCameraZoomChanged);
        cameraShakeToggle.onValueChanged.AddListener(OnCameraShakeChanged);
        cameraViewDropdown.onValueChanged.AddListener(OnCameraViewChanged);
        
        // Network
        playerNameInput.onEndEdit.AddListener(OnPlayerNameChanged);
        regionDropdown.onValueChanged.AddListener(OnRegionChanged);
        autoConnectToggle.onValueChanged.AddListener(OnAutoConnectChanged);
        pingThresholdSlider.onValueChanged.AddListener(OnPingThresholdChanged);
        
        // Advanced Graphics
        shadowsToggle.onValueChanged.AddListener(OnShadowsChanged);
        shadowQualitySlider.onValueChanged.AddListener(OnShadowQualityChanged);
        particleEffectsToggle.onValueChanged.AddListener(OnParticleEffectsChanged);
        particleQualitySlider.onValueChanged.AddListener(OnParticleQualityChanged);
        bloomToggle.onValueChanged.AddListener(OnBloomChanged);
        motionBlurToggle.onValueChanged.AddListener(OnMotionBlurChanged);
        
        // Language
        languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
        subtitlesToggle.onValueChanged.AddListener(OnSubtitlesChanged);
        
        // Debug
        showFPSToggle.onValueChanged.AddListener(OnShowFPSChanged);
        showPingToggle.onValueChanged.AddListener(OnShowPingChanged);
        debugModeToggle.onValueChanged.AddListener(OnDebugModeChanged);
    }
    
    // Graphics Event Handlers
    void OnQualityChanged(int value)
    {
        gameSettings.qualityLevel = value;
        QualitySettings.SetQualityLevel(value);
        
        if (effectsManager != null)
        {
            effectsManager.SetEffectsQuality(value);
        }
        
        SaveSettings();
    }
    
    void OnResolutionChanged(int value)
    {
        Resolution resolution = Screen.resolutions[value];
        gameSettings.screenWidth = resolution.width;
        gameSettings.screenHeight = resolution.height;
        
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        SaveSettings();
    }
    
    void OnFullscreenChanged(bool value)
    {
        gameSettings.fullscreen = value;
        Screen.fullScreen = value;
        SaveSettings();
    }
    
    void OnFrameRateChanged(float value)
    {
        Application.targetFrameRate = Mathf.RoundToInt(value);
        frameRateText.text = $"{Application.targetFrameRate} FPS";
    }
    
    void OnVSyncChanged(bool value)
    {
        QualitySettings.vSyncCount = value ? 1 : 0;
    }
    
    void OnAntiAliasingChanged(bool value)
    {
        QualitySettings.antiAliasing = value ? 4 : 0;
    }
    
    // Audio Event Handlers
    void OnMasterVolumeChanged(float value)
    {
        gameSettings.masterVolume = value;
        audioManager?.SetMasterVolume(value);
        SaveSettings();
    }
    
    void OnMusicVolumeChanged(float value)
    {
        gameSettings.musicVolume = value;
        audioManager?.SetMusicVolume(value);
        SaveSettings();
    }
    
    void OnSFXVolumeChanged(float value)
    {
        gameSettings.sfxVolume = value;
        audioManager?.SetSFXVolume(value);
        SaveSettings();
    }
    
    void OnCrowdVolumeChanged(float value)
    {
        gameSettings.crowdVolume = value;
        audioManager?.SetCrowdVolume(value);
        SaveSettings();
    }
    
    void OnCommentaryVolumeChanged(float value)
    {
        gameSettings.commentaryVolume = value;
        audioManager?.SetCommentaryVolume(value);
        SaveSettings();
    }
    
    void OnMuteChanged(bool value)
    {
        audioManager?.MuteAll(value);
    }
    
    // Gameplay Event Handlers
    void OnDifficultyChanged(float value)
    {
        gameSettings.difficulty = Mathf.RoundToInt(value);
        difficultyText.text = GetDifficultyText(gameSettings.difficulty);
        SaveSettings();
    }
    
    void OnGameSpeedChanged(float value)
    {
        gameSettings.gameplaySpeed = value;
        gameSpeedText.text = $"{value:F1}x";
        SaveSettings();
    }
    
    void OnAutoSaveChanged(bool value)
    {
        gameSettings.autoSave = value;
        autoSaveIntervalSlider.interactable = value;
        SaveSettings();
    }
    
    void OnAutoSaveIntervalChanged(float value)
    {
        autoSaveIntervalText.text = $"{value:F0} min";
    }
    
    // Control Event Handlers
    void OnTouchSensitivityChanged(float value)
    {
        touchSensitivityText.text = $"{value:F1}";
        
        // Aplicar sensibilidad a TouchControlManager
        TouchControlManager touchManager = FindObjectOfType<TouchControlManager>();
        if (touchManager != null)
        {
            touchManager.touchSensitivity = value;
        }
    }
    
    void OnVibrateChanged(bool value)
    {
        gameSettings.vibrateOnMobile = value;
        SaveSettings();
    }
    
    void OnShowTutorialChanged(bool value)
    {
        gameSettings.showTutorial = value;
        SaveSettings();
    }
    
    void OnControlSchemeChanged(int value)
    {
        // Implementar esquemas de control
        Debug.Log($"Control scheme changed to: {value}");
    }
    
    // Camera Event Handlers
    void OnCameraSpeedChanged(float value)
    {
        cameraSpeedText.text = $"{value:F1}";
        
        CameraController cameraController = FindObjectOfType<CameraController>();
        if (cameraController != null)
        {
            cameraController.followSpeed = value;
        }
    }
    
    void OnCameraZoomChanged(float value)
    {
        cameraZoomText.text = $"{value:F1}";
    }
    
    void OnCameraShakeChanged(bool value)
    {
        // Implementar configuración de shake
        Debug.Log($"Camera shake: {value}");
    }
    
    void OnCameraViewChanged(int value)
    {
        CameraController cameraController = FindObjectOfType<CameraController>();
        if (cameraController != null)
        {
            cameraController.SetCameraMode((CameraMode)value);
        }
    }
    
    // Network Event Handlers
    void OnPlayerNameChanged(string value)
    {
        NetworkManager networkManager = NetworkManager.Instance;
        if (networkManager != null)
        {
            networkManager.playerName = value;
        }
    }
    
    void OnRegionChanged(int value)
    {
        Debug.Log($"Region changed to: {value}");
    }
    
    void OnAutoConnectChanged(bool value)
    {
        NetworkManager networkManager = NetworkManager.Instance;
        if (networkManager != null)
        {
            networkManager.autoConnect = value;
        }
    }
    
    void OnPingThresholdChanged(float value)
    {
        pingThresholdText.text = $"{value:F0} ms";
    }
    
    // Advanced Graphics Event Handlers
    void OnShadowsChanged(bool value)
    {
        QualitySettings.shadows = value ? ShadowQuality.All : ShadowQuality.Disable;
    }
    
    void OnShadowQualityChanged(float value)
    {
        shadowQualityText.text = GetShadowQualityText(Mathf.RoundToInt(value));
        QualitySettings.shadowDistance = value * 50f;
    }
    
    void OnParticleEffectsChanged(bool value)
    {
        // Activar/desactivar efectos de partículas
        if (effectsManager != null)
        {
            effectsManager.gameObject.SetActive(value);
        }
    }
    
    void OnParticleQualityChanged(float value)
    {
        particleQualityText.text = GetParticleQualityText(Mathf.RoundToInt(value));
        
        if (effectsManager != null)
        {
            effectsManager.SetEffectsQuality(Mathf.RoundToInt(value));
        }
    }
    
    void OnBloomChanged(bool value)
    {
        // Implementar bloom effect
        Debug.Log($"Bloom: {value}");
    }
    
    void OnMotionBlurChanged(bool value)
    {
        // Implementar motion blur
        Debug.Log($"Motion blur: {value}");
    }
    
    // Language Event Handlers
    void OnLanguageChanged(int value)
    {
        gameSettings.language = GetLanguageString(value);
        SaveSettings();
        
        // Aplicar cambio de idioma
        ApplyLanguageChange();
    }
    
    void OnSubtitlesChanged(bool value)
    {
        gameSettings.subtitlesEnabled = value;
        SaveSettings();
    }
    
    // Debug Event Handlers
    void OnShowFPSChanged(bool value)
    {
        // Mostrar/ocultar FPS
        Debug.Log($"Show FPS: {value}");
    }
    
    void OnShowPingChanged(bool value)
    {
        // Mostrar/ocultar ping
        Debug.Log($"Show ping: {value}");
    }
    
    void OnDebugModeChanged(bool value)
    {
        Debug.unityLogger.logEnabled = value;
    }
    
    // Helper Methods
    void UpdateAllTexts()
    {
        frameRateText.text = $"{Application.targetFrameRate} FPS";
        difficultyText.text = GetDifficultyText(gameSettings.difficulty);
        gameSpeedText.text = $"{gameSettings.gameplaySpeed:F1}x";
        touchSensitivityText.text = $"{touchSensitivitySlider.value:F1}";
        cameraSpeedText.text = $"{cameraSpeedSlider.value:F1}";
        cameraZoomText.text = $"{cameraZoomSlider.value:F1}";
        pingThresholdText.text = $"{pingThresholdSlider.value:F0} ms";
        shadowQualityText.text = GetShadowQualityText(Mathf.RoundToInt(shadowQualitySlider.value));
        particleQualityText.text = GetParticleQualityText(Mathf.RoundToInt(particleQualitySlider.value));
        autoSaveIntervalText.text = $"{autoSaveIntervalSlider.value:F0} min";
    }
    
    string GetDifficultyText(int difficulty)
    {
        return difficulty switch
        {
            0 => "Fácil",
            1 => "Normal",
            2 => "Difícil",
            3 => "Experto",
            _ => "Normal"
        };
    }
    
    string GetShadowQualityText(int quality)
    {
        return quality switch
        {
            0 => "Desactivado",
            1 => "Bajo",
            2 => "Medio",
            3 => "Alto",
            _ => "Medio"
        };
    }
    
    string GetParticleQualityText(int quality)
    {
        return quality switch
        {
            0 => "Bajo",
            1 => "Medio",
            2 => "Alto",
            3 => "Ultra",
            _ => "Medio"
        };
    }
    
    int GetLanguageIndex(string language)
    {
        return language switch
        {
            "Spanish" => 0,
            "English" => 1,
            "French" => 2,
            "German" => 3,
            "Italian" => 4,
            "Portuguese" => 5,
            _ => 0
        };
    }
    
    string GetLanguageString(int index)
    {
        return index switch
        {
            0 => "Spanish",
            1 => "English",
            2 => "French",
            3 => "German",
            4 => "Italian",
            5 => "Portuguese",
            _ => "Spanish"
        };
    }
    
    void ApplyLanguageChange()
    {
        // Implementar cambio de idioma
        Debug.Log($"Language changed to: {gameSettings.language}");
    }
    
    void SaveSettings()
    {
        if (GameData.Instance != null)
        {
            GameData.Instance.SaveGameData();
        }
    }
    
    public void ResetToDefaults()
    {
        gameSettings = new GameSettings();
        LoadCurrentValues();
        SaveSettings();
        
        Debug.Log("Settings reset to defaults!");
    }
    
    public void ApplySettings()
    {
        SaveSettings();
        Debug.Log("Settings applied!");
    }
}