using UnityEngine;
using UnityEngine.UI;

public class GameSetup : MonoBehaviour
{
    [Header("Setup Options")]
    public bool autoSetupOnStart = true;
    public bool createUI = true;
    public bool setupAudio = true;
    public bool setupLighting = true;
    public bool setupCamera = true;
    
    [Header("UI Canvas")]
    public Canvas gameCanvas;
    
    void Start()
    {
        if (autoSetupOnStart)
        {
            SetupCompleteGame();
        }
    }
    
    [ContextMenu("Setup Complete Game")]
    public void SetupCompleteGame()
    {
        Debug.Log("Setting up complete Football Master game...");
        
        // Setup in order
        SetupLayers();
        SetupTags();
        SetupLighting();
        SetupCamera();
        SetupAudio();
        SetupUI();
        SetupManagers();
        
        Debug.Log("Game setup complete!");
    }
    
    void SetupLayers()
    {
        // Note: Layers must be set up manually in Unity
        // This just logs the required layers
        Debug.Log("Please set up these layers in Unity:");
        Debug.Log("Layer 8: Ground");
        Debug.Log("Layer 9: Player");
        Debug.Log("Layer 10: Ball");
        Debug.Log("Layer 11: HomeTeam");
        Debug.Log("Layer 12: AwayTeam");
        Debug.Log("Layer 13: UI");
        Debug.Log("Layer 14: Minimap");
    }
    
    void SetupTags()
    {
        // Note: Tags must be set up manually in Unity
        // This just logs the required tags
        Debug.Log("Please set up these tags in Unity:");
        Debug.Log("- Ball");
        Debug.Log("- Player");
        Debug.Log("- Goal");
        Debug.Log("- Ground");
        Debug.Log("- Post");
        Debug.Log("- Crossbar");
        Debug.Log("- Grass");
        Debug.Log("- Dirt");
        Debug.Log("- Concrete");
        Debug.Log("- Boundary");
    }
    
    void SetupLighting()
    {
        if (!setupLighting) return;
        
        // Setup main light
        GameObject mainLight = GameObject.Find("Directional Light");
        if (mainLight == null)
        {
            mainLight = new GameObject("Directional Light");
            Light light = mainLight.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.2f;
            light.shadows = LightShadows.Soft;
            light.color = new Color(1f, 0.95f, 0.8f);
            mainLight.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }
        
        // Setup ambient lighting
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = new Color(0.5f, 0.7f, 1f);
        RenderSettings.ambientEquatorColor = new Color(0.4f, 0.4f, 0.4f);
        RenderSettings.ambientGroundColor = new Color(0.2f, 0.3f, 0.2f);
        
        // Setup fog
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.5f, 0.6f, 0.7f);
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogDensity = 0.01f;
        
        Debug.Log("Lighting setup complete");
    }
    
    void SetupCamera()
    {
        if (!setupCamera) return;
        
        // Find or create main camera
        GameObject mainCamera = GameObject.Find("Main Camera");
        if (mainCamera == null)
        {
            mainCamera = new GameObject("Main Camera");
            mainCamera.AddComponent<Camera>();
            mainCamera.AddComponent<AudioListener>();
            mainCamera.tag = "MainCamera";
        }
        
        // Setup camera
        Camera cam = mainCamera.GetComponent<Camera>();
        cam.fieldOfView = 60f;
        cam.nearClipPlane = 0.1f;
        cam.farClipPlane = 200f;
        cam.backgroundColor = new Color(0.5f, 0.8f, 1f);
        
        // Add camera controller
        CameraController_120fps cameraController = mainCamera.GetComponent<CameraController_120fps>();
        if (cameraController == null)
        {
            cameraController = mainCamera.AddComponent<CameraController_120fps>();
        }
        
        // Position camera
        mainCamera.transform.position = new Vector3(0, 10, -15);
        mainCamera.transform.rotation = Quaternion.Euler(20, 0, 0);
        
        Debug.Log("Camera setup complete");
    }
    
    void SetupAudio()
    {
        if (!setupAudio) return;
        
        // Create audio manager
        GameObject audioManager = GameObject.Find("Audio Manager");
        if (audioManager == null)
        {
            audioManager = new GameObject("Audio Manager");
            AudioManager audioComponent = audioManager.AddComponent<AudioManager>();
            
            // Add audio sources
            AudioSource musicSource = audioManager.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.volume = 0.5f;
            
            AudioSource sfxSource = audioManager.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.volume = 0.7f;
            
            AudioSource crowdSource = audioManager.AddComponent<AudioSource>();
            crowdSource.loop = true;
            crowdSource.volume = 0.3f;
            
            AudioSource commentarySource = audioManager.AddComponent<AudioSource>();
            commentarySource.loop = false;
            commentarySource.volume = 0.8f;
        }
        
        Debug.Log("Audio setup complete");
    }
    
    void SetupUI()
    {
        if (!createUI) return;
        
        // Create UI Canvas
        if (gameCanvas == null)
        {
            GameObject canvasObj = new GameObject("Game Canvas");
            gameCanvas = canvasObj.AddComponent<Canvas>();
            gameCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            gameCanvas.sortingOrder = 0;
            
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        
        // Create EventSystem
        GameObject eventSystem = GameObject.Find("EventSystem");
        if (eventSystem == null)
        {
            eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
        
        // Create gameplay UI
        CreateGameplayUI();
        
        Debug.Log("UI setup complete");
    }
    
    void CreateGameplayUI()
    {
        // Score UI
        GameObject scorePanel = new GameObject("Score Panel");
        scorePanel.transform.SetParent(gameCanvas.transform, false);
        
        RectTransform scoreRect = scorePanel.AddComponent<RectTransform>();
        scoreRect.anchorMin = new Vector2(0.5f, 1f);
        scoreRect.anchorMax = new Vector2(0.5f, 1f);
        scoreRect.pivot = new Vector2(0.5f, 1f);
        scoreRect.sizeDelta = new Vector2(300, 80);
        scoreRect.anchoredPosition = new Vector2(0, -20);
        
        Image scoreBg = scorePanel.AddComponent<Image>();
        scoreBg.color = new Color(0, 0, 0, 0.5f);
        
        // Home score
        GameObject homeScore = new GameObject("Home Score");
        homeScore.transform.SetParent(scorePanel.transform, false);
        RectTransform homeRect = homeScore.AddComponent<RectTransform>();
        homeRect.anchorMin = new Vector2(0.2f, 0.5f);
        homeRect.anchorMax = new Vector2(0.2f, 0.5f);
        homeRect.pivot = new Vector2(0.5f, 0.5f);
        homeRect.sizeDelta = new Vector2(60, 60);
        
        Text homeText = homeScore.AddComponent<Text>();
        homeText.text = "0";
        homeText.fontSize = 36;
        homeText.color = Color.white;
        homeText.alignment = TextAnchor.MiddleCenter;
        homeText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        
        // Away score
        GameObject awayScore = new GameObject("Away Score");
        awayScore.transform.SetParent(scorePanel.transform, false);
        RectTransform awayRect = awayScore.AddComponent<RectTransform>();
        awayRect.anchorMin = new Vector2(0.8f, 0.5f);
        awayRect.anchorMax = new Vector2(0.8f, 0.5f);
        awayRect.pivot = new Vector2(0.5f, 0.5f);
        awayRect.sizeDelta = new Vector2(60, 60);
        
        Text awayText = awayScore.AddComponent<Text>();
        awayText.text = "0";
        awayText.fontSize = 36;
        awayText.color = Color.white;
        awayText.alignment = TextAnchor.MiddleCenter;
        awayText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        
        // Time display
        GameObject timeDisplay = new GameObject("Time Display");
        timeDisplay.transform.SetParent(scorePanel.transform, false);
        RectTransform timeRect = timeDisplay.AddComponent<RectTransform>();
        timeRect.anchorMin = new Vector2(0.5f, 0.2f);
        timeRect.anchorMax = new Vector2(0.5f, 0.2f);
        timeRect.pivot = new Vector2(0.5f, 0.5f);
        timeRect.sizeDelta = new Vector2(100, 30);
        
        Text timeText = timeDisplay.AddComponent<Text>();
        timeText.text = "00:00";
        timeText.fontSize = 20;
        timeText.color = Color.white;
        timeText.alignment = TextAnchor.MiddleCenter;
        timeText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        
        // Game state text
        GameObject gameStateText = new GameObject("Game State Text");
        gameStateText.transform.SetParent(gameCanvas.transform, false);
        RectTransform stateRect = gameStateText.AddComponent<RectTransform>();
        stateRect.anchorMin = new Vector2(0.5f, 0.7f);
        stateRect.anchorMax = new Vector2(0.5f, 0.7f);
        stateRect.pivot = new Vector2(0.5f, 0.5f);
        stateRect.sizeDelta = new Vector2(400, 50);
        
        Text stateText = gameStateText.AddComponent<Text>();
        stateText.text = "";
        stateText.fontSize = 24;
        stateText.color = Color.yellow;
        stateText.alignment = TextAnchor.MiddleCenter;
        stateText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        
        // Touch controls UI
        CreateTouchControlsUI();
    }
    
    void CreateTouchControlsUI()
    {
        // Touch indicator
        GameObject touchIndicator = new GameObject("Touch Indicator");
        touchIndicator.transform.SetParent(gameCanvas.transform, false);
        RectTransform touchRect = touchIndicator.AddComponent<RectTransform>();
        touchRect.sizeDelta = new Vector2(60, 60);
        
        Image touchImage = touchIndicator.AddComponent<Image>();
        touchImage.color = new Color(1, 1, 1, 0.5f);
        touchImage.sprite = CreateCircleSprite();
        touchIndicator.SetActive(false);
        
        // Power indicator
        GameObject powerIndicator = new GameObject("Power Indicator");
        powerIndicator.transform.SetParent(gameCanvas.transform, false);
        RectTransform powerRect = powerIndicator.AddComponent<RectTransform>();
        powerRect.sizeDelta = new Vector2(100, 100);
        
        Image powerImage = powerIndicator.AddComponent<Image>();
        powerImage.color = new Color(1, 0, 0, 0.7f);
        powerImage.sprite = CreateCircleSprite();
        powerIndicator.SetActive(false);
    }
    
    Sprite CreateCircleSprite()
    {
        Texture2D texture = new Texture2D(64, 64);
        Color[] colors = new Color[64 * 64];
        
        for (int x = 0; x < 64; x++)
        {
            for (int y = 0; y < 64; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(32, 32));
                if (distance < 30)
                {
                    colors[x + y * 64] = Color.white;
                }
                else
                {
                    colors[x + y * 64] = Color.clear;
                }
            }
        }
        
        texture.SetPixels(colors);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
    }
    
    void SetupManagers()
    {
        // Create Game Manager
        GameObject gameManager = GameObject.Find("Game Manager");
        if (gameManager == null)
        {
            gameManager = new GameObject("Game Manager");
            GameManager_120fps gameManagerComponent = gameManager.AddComponent<GameManager_120fps>();
            
            // Setup references
            gameManagerComponent.cameraController = FindObjectOfType<CameraController_120fps>();
            gameManagerComponent.stadiumAudio = FindObjectOfType<AudioSource>();
            
            // Setup UI references
            gameManagerComponent.homeScoreText = GameObject.Find("Home Score")?.GetComponent<Text>();
            gameManagerComponent.awayScoreText = GameObject.Find("Away Score")?.GetComponent<Text>();
            gameManagerComponent.timeText = GameObject.Find("Time Display")?.GetComponent<Text>();
            gameManagerComponent.gameStateText = GameObject.Find("Game State Text")?.GetComponent<Text>();
        }
        
        // Create Touch Control Manager
        GameObject touchManager = GameObject.Find("Touch Control Manager");
        if (touchManager == null)
        {
            touchManager = new GameObject("Touch Control Manager");
            TouchControlManager_120fps touchComponent = touchManager.AddComponent<TouchControlManager_120fps>();
            
            // Setup references
            touchComponent.ballController = FindObjectOfType<BallController_120fps>();
            touchComponent.cameraController = FindObjectOfType<CameraController_120fps>();
            touchComponent.touchIndicator = GameObject.Find("Touch Indicator");
            touchComponent.powerIndicator = GameObject.Find("Power Indicator");
        }
        
        // Create Effects Manager
        GameObject effectsManager = GameObject.Find("Effects Manager");
        if (effectsManager == null)
        {
            effectsManager = new GameObject("Effects Manager");
            effectsManager.AddComponent<EffectsManager>();
        }
        
        // Create Achievement Manager
        GameObject achievementManager = GameObject.Find("Achievement Manager");
        if (achievementManager == null)
        {
            achievementManager = new GameObject("Achievement Manager");
            achievementManager.AddComponent<AchievementManager>();
        }
        
        Debug.Log("Managers setup complete");
    }
    
    [ContextMenu("Setup Quality Settings")]
    public void SetupQualitySettings()
    {
        // Set quality settings for 120fps
        QualitySettings.vSyncCount = 0;
        QualitySettings.antiAliasing = 2;
        QualitySettings.shadows = ShadowQuality.HardOnly;
        QualitySettings.shadowResolution = ShadowResolution.Low;
        QualitySettings.shadowDistance = 30f;
        QualitySettings.shadowCascades = 1;
        QualitySettings.lodBias = 0.7f;
        QualitySettings.maximumLODLevel = 1;
        QualitySettings.particleRaycastBudget = 64;
        QualitySettings.softVegetation = false;
        QualitySettings.realtimeReflectionProbes = false;
        QualitySettings.billboardsFaceCameraPosition = false;
        
        // Set target framerate
        Application.targetFrameRate = 120;
        
        Debug.Log("Quality settings optimized for 120fps");
    }
}