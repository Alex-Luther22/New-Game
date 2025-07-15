using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Sistema avanzado de controles móviles personalizable con soporte para mandos
/// Optimizado para dispositivos de 2GB RAM como Tecno Spark 8C
/// </summary>
public class AdvancedMobileControlsUI : MonoBehaviour
{
    [Header("🎮 Joystick Virtual")]
    public GameObject joystickArea;
    public Image joystickBackground;
    public Image joystickHandle;
    public float joystickRange = 100f;
    public float deadZone = 0.1f;
    public bool isDraggable = true;
    
    [Header("🔘 Botones de Acción")]
    public Button passButton;
    public Button shootButton;
    public Button sprintButton;
    public Button tackleButton;
    public Button throughPassButton;
    public Button crossButton;
    public Button slideTackleButton;
    public Button callForBallButton;
    
    [Header("🌟 Área de Trucos Expandida")]
    public GameObject trickArea;
    public Image trickAreaBackground;
    public float trickAreaWidth = 400f;
    public float trickAreaHeight = 300f;
    public RectTransform trickAreaRect;
    
    [Header("⚽ Controles de Juego")]
    public Button changePlayerButton;
    public Button ballCameraButton;
    public Button substitutionButton;
    public Button pauseButton;
    public Toggle ballFollowToggle;
    public Toggle playerNameToggle;
    public Slider gameSpeedSlider;
    
    [Header("🎯 Sistema de Modos")]
    public GameObject practiceMode;
    public GameObject gameMode;
    public GameObject tutorialMode;
    public Dropdown controlSchemeDropdown;
    
    [Header("🎮 Soporte para Mandos")]
    public bool gamepadSupported = true;
    public string gamepadScheme = "Xbox";
    public KeyCode[] gamepadButtons = new KeyCode[16];
    
    [Header("📱 Optimización 2GB RAM")]
    public bool lowMemoryMode = false;
    public int maxParticles = 10;
    public float uiUpdateRate = 0.05f;
    public int maxTrickTrailPoints = 20;
    
    [Header("🎨 Personalización")]
    public Color primaryColor = Color.cyan;
    public Color secondaryColor = Color.white;
    public float controlOpacity = 0.8f;
    public float buttonSize = 70f;
    public Vector2 joystickPosition = new Vector2(100, 100);
    public Vector2 trickAreaPosition = new Vector2(-200, -150);
    
    [Header("🎵 Audio")]
    public AudioSource uiAudioSource;
    public AudioClip buttonClickSound;
    public AudioClip trickSuccessSound;
    public AudioClip whistleSound;
    public AudioClip crowdCheerSound;
    
    // Estado del sistema
    private bool isJoystickActive = false;
    private Vector2 joystickInput = Vector2.zero;
    private int joystickTouchId = -1;
    private Dictionary<string, bool> buttonStates = new Dictionary<string, bool>();
    
    // Sistema de trucos expandido
    private List<TrickData> availableTricks = new List<TrickData>();
    private bool isTrickGestureActive = false;
    private List<Vector2> trickGesturePoints = new List<Vector2>();
    private TrickDetector trickDetector;
    private float lastTrickTime = 0f;
    private string lastDetectedTrick = "";
    
    // Referencias
    private PlayerController currentPlayer;
    private BallController ballController;
    private GameManager gameManager;
    private CameraController cameraController;
    private Canvas mobileUICanvas;
    
    // Personalización
    private Dictionary<string, Vector2> buttonPositions = new Dictionary<string, Vector2>();
    private bool isCustomizationMode = false;
    private GameObject selectedControl;
    
    // Optimización
    private float lastUpdateTime = 0f;
    private int frameSkipCounter = 0;
    private bool isLowPowerMode = false;
    
    // Sistema de seguimiento
    private bool ballFollowEnabled = true;
    private bool autoPlayerSwitch = true;
    private float lastPlayerSwitchTime = 0f;
    
    void Start()
    {
        InitializeAdvancedControls();
        SetupTrickSystem();
        ConfigureForLowMemoryDevice();
        LoadControlSettings();
        SetupGamepadSupport();
        InitializeTrickData();
    }
    
    void InitializeAdvancedControls()
    {
        // Obtener referencias
        mobileUICanvas = GetComponentInParent<Canvas>();
        trickDetector = GetComponent<TrickDetector>();
        ballController = FindObjectOfType<BallController>();
        gameManager = FindObjectOfType<GameManager>();
        cameraController = FindObjectOfType<CameraController>();
        
        // Configurar Canvas para dispositivos de 2GB
        if (mobileUICanvas != null)
        {
            mobileUICanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            mobileUICanvas.scaleFactor = Mathf.Min(Screen.width / 1920f, 1.2f);
            mobileUICanvas.pixelPerfect = false; // Mejor rendimiento
        }
        
        // Configurar audio
        if (uiAudioSource == null)
        {
            uiAudioSource = gameObject.AddComponent<AudioSource>();
        }
        uiAudioSource.playOnAwake = false;
        uiAudioSource.volume = 0.7f;
        
        // Configurar posiciones iniciales
        SetupInitialPositions();
        
        // Configurar eventos
        SetupButtonEvents();
        SetupTrickArea();
        SetupGameControls();
    }
    
    void SetupInitialPositions()
    {
        // Posicionar joystick (personalizable)
        if (joystickArea != null)
        {
            RectTransform joystickRect = joystickArea.GetComponent<RectTransform>();
            joystickRect.anchorMin = new Vector2(0, 0);
            joystickRect.anchorMax = new Vector2(0, 0);
            joystickRect.anchoredPosition = joystickPosition;
            joystickRect.sizeDelta = new Vector2(150, 150);
        }
        
        // Posicionar área de trucos (superior derecha)
        if (trickArea != null)
        {
            trickAreaRect = trickArea.GetComponent<RectTransform>();
            trickAreaRect.anchorMin = new Vector2(1, 1);
            trickAreaRect.anchorMax = new Vector2(1, 1);
            trickAreaRect.anchoredPosition = trickAreaPosition;
            trickAreaRect.sizeDelta = new Vector2(trickAreaWidth, trickAreaHeight);
        }
        
        // Posicionar botones de acción
        SetupActionButtonPositions();
        
        // Posicionar controles de juego
        SetupGameControlPositions();
    }
    
    void SetupActionButtonPositions()
    {
        // Configuración optimizada para una mano
        Dictionary<Button, Vector2> buttonLayout = new Dictionary<Button, Vector2>
        {
            { passButton, new Vector2(-100, 120) },
            { shootButton, new Vector2(-100, 200) },
            { throughPassButton, new Vector2(-180, 160) },
            { crossButton, new Vector2(-20, 160) },
            { sprintButton, new Vector2(-60, 80) },
            { tackleButton, new Vector2(-140, 80) },
            { slideTackleButton, new Vector2(-60, 40) },
            { callForBallButton, new Vector2(-140, 40) }
        };
        
        foreach (var pair in buttonLayout)
        {
            if (pair.Key != null)
            {
                SetupButton(pair.Key, pair.Value, buttonSize);
            }
        }
    }
    
    void SetupGameControlPositions()
    {
        // Controles de juego en la parte superior
        Dictionary<Component, Vector2> gameControlLayout = new Dictionary<Component, Vector2>
        {
            { changePlayerButton, new Vector2(-300, -50) },
            { ballCameraButton, new Vector2(-200, -50) },
            { substitutionButton, new Vector2(-100, -50) },
            { pauseButton, new Vector2(50, -50) },
            { ballFollowToggle, new Vector2(-300, -100) },
            { playerNameToggle, new Vector2(-200, -100) },
            { gameSpeedSlider, new Vector2(0, -100) }
        };
        
        foreach (var pair in gameControlLayout)
        {
            if (pair.Key != null)
            {
                SetupGameControl(pair.Key, pair.Value);
            }
        }
    }
    
    void SetupButton(Button button, Vector2 position, float size)
    {
        RectTransform buttonRect = button.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(1, 0);
        buttonRect.anchorMax = new Vector2(1, 0);
        buttonRect.anchoredPosition = position;
        buttonRect.sizeDelta = new Vector2(size, size);
        
        // Configurar apariencia
        Image buttonImage = button.GetComponent<Image>();
        buttonImage.color = Color.Lerp(secondaryColor, primaryColor, controlOpacity);
        
        // Hacer draggable si está en modo personalización
        if (isDraggable)
        {
            SetupDraggableButton(button);
        }
    }
    
    void SetupGameControl(Component control, Vector2 position)
    {
        RectTransform controlRect = control.GetComponent<RectTransform>();
        controlRect.anchorMin = new Vector2(0.5f, 1);
        controlRect.anchorMax = new Vector2(0.5f, 1);
        controlRect.anchoredPosition = position;
        
        if (control is Button)
        {
            controlRect.sizeDelta = new Vector2(80, 40);
        }
        else if (control is Toggle)
        {
            controlRect.sizeDelta = new Vector2(100, 30);
        }
        else if (control is Slider)
        {
            controlRect.sizeDelta = new Vector2(150, 20);
        }
    }
    
    void SetupDraggableButton(Button button)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }
        
        // Drag event
        EventTrigger.Entry dragEntry = new EventTrigger.Entry();
        dragEntry.eventID = EventTriggerType.Drag;
        dragEntry.callback.AddListener((data) => {
            if (isCustomizationMode)
            {
                PointerEventData pointerData = (PointerEventData)data;
                button.transform.position = pointerData.position;
            }
        });
        trigger.triggers.Add(dragEntry);
    }
    
    void SetupTrickArea()
    {
        if (trickArea == null) return;
        
        // Configurar apariencia
        if (trickAreaBackground != null)
        {
            trickAreaBackground.color = new Color(primaryColor.r, primaryColor.g, primaryColor.b, 0.15f);
        }
        
        // Configurar eventos táctiles
        EventTrigger trickTrigger = trickArea.GetComponent<EventTrigger>();
        if (trickTrigger == null)
        {
            trickTrigger = trickArea.AddComponent<EventTrigger>();
        }
        
        // Inicio del gesto
        EventTrigger.Entry trickStartEntry = new EventTrigger.Entry();
        trickStartEntry.eventID = EventTriggerType.PointerDown;
        trickStartEntry.callback.AddListener(OnTrickGestureStart);
        trickTrigger.triggers.Add(trickStartEntry);
        
        // Movimiento del gesto
        EventTrigger.Entry trickMoveEntry = new EventTrigger.Entry();
        trickMoveEntry.eventID = EventTriggerType.Drag;
        trickMoveEntry.callback.AddListener(OnTrickGestureMove);
        trickTrigger.triggers.Add(trickMoveEntry);
        
        // Fin del gesto
        EventTrigger.Entry trickEndEntry = new EventTrigger.Entry();
        trickEndEntry.eventID = EventTriggerType.PointerUp;
        trickEndEntry.callback.AddListener(OnTrickGestureEnd);
        trickTrigger.triggers.Add(trickEndEntry);
        
        // Agregar texto explicativo
        AddTrickAreaText();
    }
    
    void AddTrickAreaText()
    {
        GameObject textObj = new GameObject("TrickAreaText");
        textObj.transform.SetParent(trickArea.transform);
        
        Text trickText = textObj.AddComponent<Text>();
        trickText.text = "🌟 ÁREA DE TRUCOS 🌟\nDibuja gestos aquí\n" + availableTricks.Count + " trucos disponibles";
        trickText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        trickText.fontSize = 14;
        trickText.color = Color.white;
        trickText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
    }
    
    void SetupButtonEvents()
    {
        // Configurar eventos para cada botón
        if (passButton != null)
            SetupButtonEvent(passButton, "pass", OnPassPressed, OnPassReleased);
        if (shootButton != null)
            SetupButtonEvent(shootButton, "shoot", OnShootPressed, OnShootReleased);
        if (throughPassButton != null)
            SetupButtonEvent(throughPassButton, "throughPass", OnThroughPassPressed, OnThroughPassReleased);
        if (crossButton != null)
            SetupButtonEvent(crossButton, "cross", OnCrossPressed, OnCrossReleased);
        if (sprintButton != null)
            SetupButtonEvent(sprintButton, "sprint", OnSprintPressed, OnSprintReleased);
        if (tackleButton != null)
            SetupButtonEvent(tackleButton, "tackle", OnTacklePressed, OnTackleReleased);
        if (slideTackleButton != null)
            SetupButtonEvent(slideTackleButton, "slideTackle", OnSlideTacklePressed, OnSlideTackleReleased);
        if (callForBallButton != null)
            SetupButtonEvent(callForBallButton, "callForBall", OnCallForBallPressed, OnCallForBallReleased);
    }
    
    void SetupButtonEvent(Button button, string buttonName, System.Action onPressed, System.Action onReleased)
    {
        buttonStates[buttonName] = false;
        
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }
        
        // Pressed
        EventTrigger.Entry pressedEntry = new EventTrigger.Entry();
        pressedEntry.eventID = EventTriggerType.PointerDown;
        pressedEntry.callback.AddListener((data) => {
            buttonStates[buttonName] = true;
            button.GetComponent<Image>().color = primaryColor;
            PlayButtonSound();
            onPressed?.Invoke();
        });
        trigger.triggers.Add(pressedEntry);
        
        // Released
        EventTrigger.Entry releasedEntry = new EventTrigger.Entry();
        releasedEntry.eventID = EventTriggerType.PointerUp;
        releasedEntry.callback.AddListener((data) => {
            buttonStates[buttonName] = false;
            button.GetComponent<Image>().color = Color.Lerp(secondaryColor, primaryColor, controlOpacity);
            onReleased?.Invoke();
        });
        trigger.triggers.Add(releasedEntry);
    }
    
    void SetupGameControls()
    {
        // Configurar controles de juego
        if (changePlayerButton != null)
        {
            changePlayerButton.onClick.AddListener(() => {
                ChangeToNearestPlayer();
                PlayButtonSound();
            });
        }
        
        if (ballCameraButton != null)
        {
            ballCameraButton.onClick.AddListener(() => {
                ToggleBallCamera();
                PlayButtonSound();
            });
        }
        
        if (substitutionButton != null)
        {
            substitutionButton.onClick.AddListener(() => {
                OpenSubstitutionPanel();
                PlayButtonSound();
            });
        }
        
        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(() => {
                TogglePause();
                PlayButtonSound();
            });
        }
        
        if (ballFollowToggle != null)
        {
            ballFollowToggle.onValueChanged.AddListener((value) => {
                ballFollowEnabled = value;
                if (cameraController != null)
                {
                    cameraController.SetBallFollow(value);
                }
            });
        }
        
        if (playerNameToggle != null)
        {
            playerNameToggle.onValueChanged.AddListener((value) => {
                ShowPlayerNames(value);
            });
        }
        
        if (gameSpeedSlider != null)
        {
            gameSpeedSlider.onValueChanged.AddListener((value) => {
                Time.timeScale = value;
            });
        }
    }
    
    void ConfigureForLowMemoryDevice()
    {
        // Detectar dispositivos de 2GB RAM
        if (SystemInfo.systemMemorySize <= 2048)
        {
            lowMemoryMode = true;
            isLowPowerMode = true;
            
            // Reducir calidad de UI
            uiUpdateRate = 0.1f;
            maxTrickTrailPoints = 10;
            maxParticles = 5;
            
            // Simplificar efectos visuales
            if (trickAreaBackground != null)
            {
                trickAreaBackground.material = null;
            }
            
            // Reducir frame rate de UI
            Application.targetFrameRate = 30;
        }
        else
        {
            lowMemoryMode = false;
            uiUpdateRate = 0.033f;
            maxTrickTrailPoints = 20;
            maxParticles = 15;
        }
    }
    
    void SetupGamepadSupport()
    {
        if (!gamepadSupported) return;
        
        // Configurar mapeo de botones para Xbox/PlayStation
        gamepadButtons[0] = KeyCode.JoystickButton0; // A/X - Pass
        gamepadButtons[1] = KeyCode.JoystickButton1; // B/Circle - Shoot
        gamepadButtons[2] = KeyCode.JoystickButton2; // X/Square - Through Pass
        gamepadButtons[3] = KeyCode.JoystickButton3; // Y/Triangle - Cross
        gamepadButtons[4] = KeyCode.JoystickButton4; // LB/L1 - Sprint
        gamepadButtons[5] = KeyCode.JoystickButton5; // RB/R1 - Tackle
        gamepadButtons[6] = KeyCode.JoystickButton6; // LT/L2 - Slide Tackle
        gamepadButtons[7] = KeyCode.JoystickButton7; // RT/R2 - Call for Ball
        gamepadButtons[8] = KeyCode.JoystickButton8; // Select/Share - Change Player
        gamepadButtons[9] = KeyCode.JoystickButton9; // Start/Options - Pause
        gamepadButtons[10] = KeyCode.JoystickButton10; // L3 - Ball Camera
        gamepadButtons[11] = KeyCode.JoystickButton11; // R3 - Substitution
    }
    
    void InitializeTrickData()
    {
        availableTricks = new List<TrickData>
        {
            new TrickData { name = "Step-over", difficulty = 1, description = "Dibuja zigzag", pattern = TrickPattern.Zigzag, unlocked = true },
            new TrickData { name = "Roulette", difficulty = 2, description = "Dibuja círculo completo", pattern = TrickPattern.Circle, unlocked = true },
            new TrickData { name = "Elastico", difficulty = 3, description = "Dibuja L (derecha-abajo)", pattern = TrickPattern.LShape, unlocked = true },
            new TrickData { name = "Nutmeg", difficulty = 2, description = "Dibuja línea vertical", pattern = TrickPattern.VerticalLine, unlocked = true },
            new TrickData { name = "Rainbow Flick", difficulty = 4, description = "Dibuja arco hacia arriba", pattern = TrickPattern.Arc, unlocked = false },
            new TrickData { name = "Rabona", difficulty = 4, description = "Dibuja curva externa", pattern = TrickPattern.Curve, unlocked = false },
            new TrickData { name = "Heel Flick", difficulty = 3, description = "Dibuja hacia atrás", pattern = TrickPattern.Backward, unlocked = false },
            new TrickData { name = "Scorpion", difficulty = 5, description = "Dibuja S compleja", pattern = TrickPattern.SShape, unlocked = false },
            new TrickData { name = "Maradona Turn", difficulty = 4, description = "Dibuja medio círculo", pattern = TrickPattern.HalfCircle, unlocked = false },
            new TrickData { name = "Cruyff Turn", difficulty = 3, description = "Dibuja V invertida", pattern = TrickPattern.VShape, unlocked = false },
            new TrickData { name = "Bicycle Kick", difficulty = 5, description = "Dibuja X", pattern = TrickPattern.XShape, unlocked = false },
            new TrickData { name = "Fake Shot", difficulty = 2, description = "Dibuja línea corta", pattern = TrickPattern.ShortLine, unlocked = true }
        };
    }
    
    void Update()
    {
        if (Time.time - lastUpdateTime < uiUpdateRate) return;
        lastUpdateTime = Time.time;
        
        // Actualizar controles
        UpdateJoystick();
        UpdateGamepadInput();
        UpdateGameFeatures();
        
        // Optimización para dispositivos de 2GB
        if (lowMemoryMode)
        {
            frameSkipCounter++;
            if (frameSkipCounter % 2 == 0) // Saltar frames alternos
            {
                UpdateVisualEffects();
            }
        }
        else
        {
            UpdateVisualEffects();
        }
    }
    
    void UpdateJoystick()
    {
        // Procesar input del joystick
        if (Application.platform == RuntimePlatform.Android || 
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            ProcessMobileJoystick();
        }
        else
        {
            ProcessMouseJoystick();
        }
        
        // Aplicar movimiento del jugador
        if (joystickInput.magnitude > deadZone && currentPlayer != null)
        {
            Vector3 movement = new Vector3(joystickInput.x, 0, joystickInput.y);
            currentPlayer.MovePlayer(movement);
        }
        else if (currentPlayer != null)
        {
            currentPlayer.StopMovement();
        }
    }
    
    void UpdateGamepadInput()
    {
        if (!gamepadSupported) return;
        
        // Analógico izquierdo para movimiento
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        if (Mathf.Abs(horizontal) > deadZone || Mathf.Abs(vertical) > deadZone)
        {
            joystickInput = new Vector2(horizontal, vertical);
            if (currentPlayer != null)
            {
                Vector3 movement = new Vector3(horizontal, 0, vertical);
                currentPlayer.MovePlayer(movement);
            }
        }
        
        // Botones del gamepad
        for (int i = 0; i < gamepadButtons.Length; i++)
        {
            if (Input.GetKeyDown(gamepadButtons[i]))
            {
                HandleGamepadButton(i);
            }
        }
    }
    
    void UpdateGameFeatures()
    {
        // Seguimiento automático del balón
        if (ballFollowEnabled && ballController != null && cameraController != null)
        {
            if (ballController.GetCurrentSpeed() > 5f)
            {
                cameraController.FollowBall();
            }
        }
        
        // Cambio automático de jugador
        if (autoPlayerSwitch && Time.time - lastPlayerSwitchTime > 0.5f)
        {
            CheckAutoPlayerSwitch();
        }
        
        // Validar fuera de juego
        if (gameManager != null)
        {
            gameManager.CheckOffside();
        }
    }
    
    void UpdateVisualEffects()
    {
        // Actualizar efectos del joystick
        if (joystickHandle != null)
        {
            Vector2 handlePos = joystickInput * (joystickRange * 0.3f);
            joystickHandle.transform.localPosition = handlePos;
            
            Color joystickColor = isJoystickActive ? primaryColor : secondaryColor;
            joystickBackground.color = Color.Lerp(joystickBackground.color, joystickColor, Time.deltaTime * 5f);
        }
        
        // Actualizar área de trucos
        if (isTrickGestureActive && trickAreaBackground != null)
        {
            trickAreaBackground.color = new Color(primaryColor.r, primaryColor.g, primaryColor.b, 0.4f);
        }
        else if (trickAreaBackground != null)
        {
            trickAreaBackground.color = new Color(primaryColor.r, primaryColor.g, primaryColor.b, 0.15f);
        }
    }
    
    void ProcessMobileJoystick()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            
            if (IsPositionInJoystickArea(touch.position))
            {
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        if (joystickTouchId == -1)
                        {
                            joystickTouchId = touch.fingerId;
                            isJoystickActive = true;
                            UpdateJoystickInput(touch.position);
                        }
                        break;
                        
                    case TouchPhase.Moved:
                        if (joystickTouchId == touch.fingerId)
                        {
                            UpdateJoystickInput(touch.position);
                        }
                        break;
                        
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        if (joystickTouchId == touch.fingerId)
                        {
                            DeactivateJoystick();
                        }
                        break;
                }
            }
        }
    }
    
    void ProcessMouseJoystick()
    {
        Vector2 mousePos = Input.mousePosition;
        
        if (Input.GetMouseButtonDown(0) && IsPositionInJoystickArea(mousePos))
        {
            isJoystickActive = true;
            UpdateJoystickInput(mousePos);
        }
        else if (Input.GetMouseButton(0) && isJoystickActive)
        {
            UpdateJoystickInput(mousePos);
        }
        else if (Input.GetMouseButtonUp(0) && isJoystickActive)
        {
            DeactivateJoystick();
        }
    }
    
    bool IsPositionInJoystickArea(Vector2 screenPosition)
    {
        if (joystickArea == null) return false;
        
        RectTransform joystickRect = joystickArea.GetComponent<RectTransform>();
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickRect, screenPosition, mobileUICanvas.worldCamera, out localPos);
        
        return joystickRect.rect.Contains(localPos);
    }
    
    void UpdateJoystickInput(Vector2 screenPosition)
    {
        if (joystickArea == null) return;
        
        RectTransform joystickRect = joystickArea.GetComponent<RectTransform>();
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickRect, screenPosition, mobileUICanvas.worldCamera, out localPos);
        
        // Calcular input normalizado
        Vector2 direction = localPos;
        float distance = direction.magnitude;
        
        if (distance > joystickRange)
        {
            direction = direction.normalized * joystickRange;
        }
        
        joystickInput = direction / joystickRange;
        
        // Aplicar zona muerta
        if (joystickInput.magnitude < deadZone)
        {
            joystickInput = Vector2.zero;
        }
    }
    
    void DeactivateJoystick()
    {
        isJoystickActive = false;
        joystickTouchId = -1;
        joystickInput = Vector2.zero;
    }
    
    void HandleGamepadButton(int buttonIndex)
    {
        switch (buttonIndex)
        {
            case 0: OnPassPressed(); break;
            case 1: OnShootPressed(); break;
            case 2: OnThroughPassPressed(); break;
            case 3: OnCrossPressed(); break;
            case 4: OnSprintPressed(); break;
            case 5: OnTacklePressed(); break;
            case 6: OnSlideTacklePressed(); break;
            case 7: OnCallForBallPressed(); break;
            case 8: ChangeToNearestPlayer(); break;
            case 9: TogglePause(); break;
            case 10: ToggleBallCamera(); break;
            case 11: OpenSubstitutionPanel(); break;
        }
    }
    
    // Eventos de botones de acción
    void OnPassPressed()
    {
        if (currentPlayer != null && currentPlayer.HasBall())
        {
            Vector3 passDirection = CalculatePassDirection();
            currentPlayer.PassBall(passDirection, 15f);
        }
    }
    
    void OnPassReleased() { }
    
    void OnShootPressed()
    {
        if (currentPlayer != null && currentPlayer.HasBall())
        {
            Vector3 shootDirection = CalculateShootDirection();
            currentPlayer.ShootBall(shootDirection, 25f);
        }
    }
    
    void OnShootReleased() { }
    
    void OnThroughPassPressed()
    {
        if (currentPlayer != null && currentPlayer.HasBall())
        {
            Vector3 passDirection = CalculatePassDirection();
            currentPlayer.PassBall(passDirection, 20f); // Más fuerte que pase normal
        }
    }
    
    void OnThroughPassReleased() { }
    
    void OnCrossPressed()
    {
        if (currentPlayer != null && currentPlayer.HasBall())
        {
            Vector3 crossDirection = CalculateCrossDirection();
            currentPlayer.CrossBall(crossDirection, 18f);
        }
    }
    
    void OnCrossReleased() { }
    
    void OnSprintPressed()
    {
        if (currentPlayer != null)
        {
            currentPlayer.SetSprintActive(true);
        }
    }
    
    void OnSprintReleased()
    {
        if (currentPlayer != null)
        {
            currentPlayer.SetSprintActive(false);
        }
    }
    
    void OnTacklePressed()
    {
        if (currentPlayer != null)
        {
            currentPlayer.ExecuteTackle();
        }
    }
    
    void OnTackleReleased() { }
    
    void OnSlideTacklePressed()
    {
        if (currentPlayer != null)
        {
            currentPlayer.ExecuteSlideTackle();
        }
    }
    
    void OnSlideTackleReleased() { }
    
    void OnCallForBallPressed()
    {
        if (currentPlayer != null)
        {
            currentPlayer.CallForBall();
        }
    }
    
    void OnCallForBallReleased() { }
    
    // Eventos de trucos
    void OnTrickGestureStart(BaseEventData eventData)
    {
        PointerEventData pointerData = (PointerEventData)eventData;
        isTrickGestureActive = true;
        trickGesturePoints.Clear();
        trickGesturePoints.Add(pointerData.position);
    }
    
    void OnTrickGestureMove(BaseEventData eventData)
    {
        if (!isTrickGestureActive) return;
        
        PointerEventData pointerData = (PointerEventData)eventData;
        trickGesturePoints.Add(pointerData.position);
        
        // Limitar puntos para dispositivos de 2GB
        if (trickGesturePoints.Count > maxTrickTrailPoints)
        {
            trickGesturePoints.RemoveAt(0);
        }
    }
    
    void OnTrickGestureEnd(BaseEventData eventData)
    {
        if (!isTrickGestureActive) return;
        
        isTrickGestureActive = false;
        
        // Detectar truco solo si han pasado 0.5 segundos desde el último
        if (Time.time - lastTrickTime < 0.5f) return;
        
        if (trickGesturePoints.Count > 5)
        {
            TrickType detectedTrick = DetectTrickFromGesture(trickGesturePoints);
            if (detectedTrick != TrickType.None)
            {
                ExecuteTrick(detectedTrick);
                lastTrickTime = Time.time;
            }
        }
        
        trickGesturePoints.Clear();
    }
    
    TrickType DetectTrickFromGesture(List<Vector2> gesturePoints)
    {
        // Usar el sistema de detección avanzado
        if (trickDetector != null)
        {
            return trickDetector.DetectTrick(gesturePoints);
        }
        
        return TrickType.None;
    }
    
    void ExecuteTrick(TrickType trickType)
    {
        if (currentPlayer == null || !currentPlayer.HasBall()) return;
        
        // Verificar si el truco está desbloqueado
        TrickData trickData = availableTricks.FirstOrDefault(t => t.name == trickType.ToString());
        if (trickData != null && !trickData.unlocked) return;
        
        // Ejecutar truco
        currentPlayer.PlayTrickAnimation(trickType.ToString());
        
        // Efectos de sonido
        PlayTrickSuccessSound();
        
        // Actualizar estadísticas
        if (gameManager != null)
        {
            gameManager.OnTrickExecuted(trickType);
        }
        
        lastDetectedTrick = trickType.ToString();
    }
    
    // Funciones de cálculo de dirección
    Vector3 CalculatePassDirection()
    {
        if (joystickInput.magnitude > 0.1f)
        {
            return new Vector3(joystickInput.x, 0, joystickInput.y).normalized;
        }
        
        // Dirección por defecto hacia adelante
        return Vector3.forward;
    }
    
    Vector3 CalculateShootDirection()
    {
        if (joystickInput.magnitude > 0.1f)
        {
            return new Vector3(joystickInput.x, 0, joystickInput.y).normalized;
        }
        
        // Dirección hacia la portería
        if (gameManager != null)
        {
            return gameManager.GetOpponentGoalDirection(currentPlayer.transform.position);
        }
        
        return Vector3.forward;
    }
    
    Vector3 CalculateCrossDirection()
    {
        // Dirección hacia el área rival
        if (gameManager != null)
        {
            return gameManager.GetCrossDirection(currentPlayer.transform.position);
        }
        
        return Vector3.forward;
    }
    
    // Funciones de juego
    void ChangeToNearestPlayer()
    {
        if (gameManager != null)
        {
            GameObject nearestPlayer = gameManager.GetNearestPlayerToBall();
            if (nearestPlayer != null)
            {
                SetCurrentPlayer(nearestPlayer.GetComponent<PlayerController>());
                lastPlayerSwitchTime = Time.time;
            }
        }
    }
    
    void ToggleBallCamera()
    {
        if (cameraController != null)
        {
            cameraController.ToggleBallCamera();
        }
    }
    
    void OpenSubstitutionPanel()
    {
        if (gameManager != null)
        {
            gameManager.OpenSubstitutionPanel();
        }
    }
    
    void TogglePause()
    {
        if (gameManager != null)
        {
            gameManager.TogglePause();
        }
    }
    
    void ShowPlayerNames(bool show)
    {
        if (gameManager != null)
        {
            gameManager.ShowPlayerNames(show);
        }
    }
    
    void CheckAutoPlayerSwitch()
    {
        if (!autoPlayerSwitch || ballController == null) return;
        
        // Cambiar al jugador más cercano al balón automáticamente
        if (ballController.GetCurrentSpeed() > 2f)
        {
            ChangeToNearestPlayer();
        }
    }
    
    // Funciones de audio
    void PlayButtonSound()
    {
        if (uiAudioSource != null && buttonClickSound != null)
        {
            uiAudioSource.PlayOneShot(buttonClickSound);
        }
    }
    
    void PlayTrickSuccessSound()
    {
        if (uiAudioSource != null && trickSuccessSound != null)
        {
            uiAudioSource.PlayOneShot(trickSuccessSound);
        }
    }
    
    void PlayWhistleSound()
    {
        if (uiAudioSource != null && whistleSound != null)
        {
            uiAudioSource.PlayOneShot(whistleSound);
        }
    }
    
    // Sistema de personalización
    public void EnterCustomizationMode()
    {
        isCustomizationMode = true;
        // Mostrar UI de personalización
    }
    
    public void ExitCustomizationMode()
    {
        isCustomizationMode = false;
        SaveControlSettings();
    }
    
    void SaveControlSettings()
    {
        // Guardar posiciones de controles
        PlayerPrefs.SetFloat("JoystickPosX", joystickPosition.x);
        PlayerPrefs.SetFloat("JoystickPosY", joystickPosition.y);
        PlayerPrefs.SetFloat("TrickAreaPosX", trickAreaPosition.x);
        PlayerPrefs.SetFloat("TrickAreaPosY", trickAreaPosition.y);
        PlayerPrefs.SetFloat("ButtonSize", buttonSize);
        PlayerPrefs.SetFloat("ControlOpacity", controlOpacity);
        PlayerPrefs.Save();
    }
    
    void LoadControlSettings()
    {
        // Cargar posiciones guardadas
        joystickPosition.x = PlayerPrefs.GetFloat("JoystickPosX", 100f);
        joystickPosition.y = PlayerPrefs.GetFloat("JoystickPosY", 100f);
        trickAreaPosition.x = PlayerPrefs.GetFloat("TrickAreaPosX", -200f);
        trickAreaPosition.y = PlayerPrefs.GetFloat("TrickAreaPosY", -150f);
        buttonSize = PlayerPrefs.GetFloat("ButtonSize", 70f);
        controlOpacity = PlayerPrefs.GetFloat("ControlOpacity", 0.8f);
    }
    
    // Métodos públicos
    public void SetCurrentPlayer(PlayerController player)
    {
        currentPlayer = player;
    }
    
    public Vector2 GetJoystickInput()
    {
        return joystickInput;
    }
    
    public bool IsButtonPressed(string buttonName)
    {
        return buttonStates.ContainsKey(buttonName) && buttonStates[buttonName];
    }
    
    public List<TrickData> GetAvailableTricks()
    {
        return availableTricks;
    }
    
    public void UnlockTrick(string trickName)
    {
        TrickData trick = availableTricks.FirstOrDefault(t => t.name == trickName);
        if (trick != null)
        {
            trick.unlocked = true;
        }
    }
    
    public string GetLastDetectedTrick()
    {
        return lastDetectedTrick;
    }
    
    public void SetLowMemoryMode(bool enabled)
    {
        lowMemoryMode = enabled;
        ConfigureForLowMemoryDevice();
    }
    
    public void SetGamepadSupport(bool enabled)
    {
        gamepadSupported = enabled;
        if (enabled)
        {
            SetupGamepadSupport();
        }
    }
}

[System.Serializable]
public class TrickData
{
    public string name;
    public int difficulty;
    public string description;
    public TrickPattern pattern;
    public bool unlocked;
}

[System.Serializable]
public enum TrickPattern
{
    Zigzag,
    Circle,
    LShape,
    VerticalLine,
    Arc,
    Curve,
    Backward,
    SShape,
    HalfCircle,
    VShape,
    XShape,
    ShortLine
}