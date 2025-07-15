using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// Sistema de controles móviles híbridos con joystick virtual y botones
/// Gestos táctiles solo para trucos en área específica
/// </summary>
public class MobileControlsUI : MonoBehaviour
{
    [Header("🎮 Joystick Virtual")]
    public GameObject joystickArea;
    public Image joystickBackground;
    public Image joystickHandle;
    public float joystickRange = 100f;
    public float deadZone = 0.1f;
    
    [Header("🔘 Botones de Acción")]
    public Button passButton;
    public Button shootButton;
    public Button sprintButton;
    public Button tackleButton;
    
    [Header("🌟 Área de Trucos")]
    public GameObject trickArea;
    public Image trickAreaBackground;
    public float trickAreaSensitivity = 1f;
    
    [Header("📱 Configuración de UI")]
    public Canvas mobileUICanvas;
    public float buttonSize = 80f;
    public float joystickSize = 150f;
    
    [Header("🎯 Referencias")]
    public TouchControlManager touchControlManager;
    public PlayerController currentPlayer;
    public BallController ballController;
    
    [Header("🎨 Efectos Visuales")]
    public Color joystickActiveColor = Color.cyan;
    public Color joystickInactiveColor = Color.white;
    public Color buttonPressedColor = Color.green;
    public Color buttonNormalColor = Color.white;
    
    // Estado del joystick
    private bool isJoystickActive = false;
    private Vector2 joystickInput = Vector2.zero;
    private Vector2 joystickStartPosition;
    private int joystickTouchId = -1;
    
    // Estado de botones
    private bool isPassPressed = false;
    private bool isShootPressed = false;
    private bool isSprintPressed = false;
    private bool isTacklePressed = false;
    
    // Sistema de trucos
    private bool isTrickGestureActive = false;
    private Vector2 trickGestureStart;
    private System.Collections.Generic.List<Vector2> trickGesturePoints = new System.Collections.Generic.List<Vector2>();
    private TrickDetector trickDetector;
    
    // Optimización
    private float lastUpdateTime = 0f;
    private const float UPDATE_INTERVAL = 0.016f; // 60 FPS
    
    void Start()
    {
        InitializeMobileControls();
        SetupUIElements();
        ConfigureForDevice();
    }
    
    void InitializeMobileControls()
    {
        // Obtener referencias
        if (touchControlManager == null)
            touchControlManager = FindObjectOfType<TouchControlManager>();
        
        if (ballController == null)
            ballController = FindObjectOfType<BallController>();
        
        if (trickDetector == null)
            trickDetector = GetComponent<TrickDetector>();
        
        // Configurar posiciones iniciales
        joystickStartPosition = joystickBackground.transform.position;
        
        // Configurar eventos de botones
        SetupButtonEvents();
        
        // Configurar área de trucos
        SetupTrickArea();
    }
    
    void SetupUIElements()
    {
        // Configurar Canvas para móviles
        if (mobileUICanvas == null)
            mobileUICanvas = GetComponentInParent<Canvas>();
        
        mobileUICanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        mobileUICanvas.scaleFactor = Screen.width / 1920f; // Escalar según resolución
        
        // Posicionar joystick (esquina inferior izquierda)
        RectTransform joystickRect = joystickArea.GetComponent<RectTransform>();
        joystickRect.anchorMin = new Vector2(0, 0);
        joystickRect.anchorMax = new Vector2(0, 0);
        joystickRect.anchoredPosition = new Vector2(100, 100);
        joystickRect.sizeDelta = new Vector2(joystickSize, joystickSize);
        
        // Posicionar botones (esquina inferior derecha)
        PositionActionButtons();
        
        // Configurar área de trucos (parte superior central)
        SetupTrickAreaPosition();
    }
    
    void PositionActionButtons()
    {
        float spacing = buttonSize + 20f;
        Vector2 basePosition = new Vector2(Screen.width - 100, 100);
        
        // Botón de pase
        SetButtonPosition(passButton, basePosition);
        
        // Botón de disparo
        SetButtonPosition(shootButton, basePosition + new Vector2(0, spacing));
        
        // Botón de sprint
        SetButtonPosition(sprintButton, basePosition + new Vector2(-spacing, spacing * 0.5f));
        
        // Botón de tackle
        SetButtonPosition(tackleButton, basePosition + new Vector2(spacing, spacing * 0.5f));
    }
    
    void SetButtonPosition(Button button, Vector2 position)
    {
        RectTransform buttonRect = button.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0, 0);
        buttonRect.anchorMax = new Vector2(0, 0);
        buttonRect.anchoredPosition = position;
        buttonRect.sizeDelta = new Vector2(buttonSize, buttonSize);
    }
    
    void SetupTrickAreaPosition()
    {
        RectTransform trickRect = trickArea.GetComponent<RectTransform>();
        trickRect.anchorMin = new Vector2(0.5f, 0.7f);
        trickRect.anchorMax = new Vector2(0.5f, 0.7f);
        trickRect.anchoredPosition = Vector2.zero;
        trickRect.sizeDelta = new Vector2(300, 200);
        
        // Configurar apariencia
        trickAreaBackground.color = new Color(1f, 1f, 1f, 0.1f);
        
        // Agregar texto explicativo
        Text trickText = trickArea.GetComponentInChildren<Text>();
        if (trickText != null)
        {
            trickText.text = "Área de Trucos\nDibuja aquí los gestos";
            trickText.color = new Color(1f, 1f, 1f, 0.7f);
            trickText.fontSize = 16;
            trickText.alignment = TextAnchor.MiddleCenter;
        }
    }
    
    void SetupButtonEvents()
    {
        // Configurar eventos de botones
        SetupButton(passButton, "PASE", OnPassPressed, OnPassReleased);
        SetupButton(shootButton, "DISPARO", OnShootPressed, OnShootReleased);
        SetupButton(sprintButton, "SPRINT", OnSprintPressed, OnSprintReleased);
        SetupButton(tackleButton, "TACKLE", OnTacklePressed, OnTackleReleased);
    }
    
    void SetupButton(Button button, string label, System.Action onPressed, System.Action onReleased)
    {
        // Configurar apariencia
        Image buttonImage = button.GetComponent<Image>();
        buttonImage.color = buttonNormalColor;
        
        // Agregar texto
        Text buttonText = button.GetComponentInChildren<Text>();
        if (buttonText == null)
        {
            GameObject textObj = new GameObject("ButtonText");
            textObj.transform.SetParent(button.transform);
            buttonText = textObj.AddComponent<Text>();
            buttonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }
        
        buttonText.text = label;
        buttonText.color = Color.black;
        buttonText.fontSize = 14;
        buttonText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform textRect = buttonText.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        // Configurar eventos táctiles
        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();
        
        // Pressed
        EventTrigger.Entry pressedEntry = new EventTrigger.Entry();
        pressedEntry.eventID = EventTriggerType.PointerDown;
        pressedEntry.callback.AddListener((data) => {
            buttonImage.color = buttonPressedColor;
            onPressed?.Invoke();
        });
        trigger.triggers.Add(pressedEntry);
        
        // Released
        EventTrigger.Entry releasedEntry = new EventTrigger.Entry();
        releasedEntry.eventID = EventTriggerType.PointerUp;
        releasedEntry.callback.AddListener((data) => {
            buttonImage.color = buttonNormalColor;
            onReleased?.Invoke();
        });
        trigger.triggers.Add(releasedEntry);
    }
    
    void SetupTrickArea()
    {
        // Configurar área de trucos con eventos táctiles
        EventTrigger trickTrigger = trickArea.AddComponent<EventTrigger>();
        
        // Inicio del gesto
        EventTrigger.Entry trickStartEntry = new EventTrigger.Entry();
        trickStartEntry.eventID = EventTriggerType.PointerDown;
        trickStartEntry.callback.AddListener((data) => {
            PointerEventData pointerData = (PointerEventData)data;
            OnTrickGestureStart(pointerData.position);
        });
        trickTrigger.triggers.Add(trickStartEntry);
        
        // Movimiento del gesto
        EventTrigger.Entry trickMoveEntry = new EventTrigger.Entry();
        trickMoveEntry.eventID = EventTriggerType.Drag;
        trickMoveEntry.callback.AddListener((data) => {
            PointerEventData pointerData = (PointerEventData)data;
            OnTrickGestureMove(pointerData.position);
        });
        trickTrigger.triggers.Add(trickMoveEntry);
        
        // Fin del gesto
        EventTrigger.Entry trickEndEntry = new EventTrigger.Entry();
        trickEndEntry.eventID = EventTriggerType.PointerUp;
        trickEndEntry.callback.AddListener((data) => {
            PointerEventData pointerData = (PointerEventData)data;
            OnTrickGestureEnd(pointerData.position);
        });
        trickTrigger.triggers.Add(trickEndEntry);
    }
    
    void ConfigureForDevice()
    {
        // Configurar según el dispositivo
        if (SystemInfo.systemMemorySize <= 4096) // Dispositivos de gama baja
        {
            // Reducir tamaño de elementos para mejor rendimiento
            buttonSize = 70f;
            joystickSize = 130f;
            trickAreaSensitivity = 0.8f;
        }
        
        // Configurar para diferentes resoluciones
        float screenRatio = (float)Screen.width / Screen.height;
        if (screenRatio > 2f) // Pantallas muy anchas
        {
            // Ajustar posiciones para pantallas anchas
            AdjustForWideScreen();
        }
    }
    
    void AdjustForWideScreen()
    {
        // Mover joystick más hacia el centro
        RectTransform joystickRect = joystickArea.GetComponent<RectTransform>();
        joystickRect.anchoredPosition = new Vector2(150, 100);
        
        // Ajustar posición de botones
        PositionActionButtons();
    }
    
    void Update()
    {
        if (Time.time - lastUpdateTime < UPDATE_INTERVAL) return;
        lastUpdateTime = Time.time;
        
        // Actualizar joystick
        UpdateJoystick();
        
        // Procesar input del joystick
        ProcessJoystickInput();
        
        // Actualizar efectos visuales
        UpdateVisualEffects();
    }
    
    void UpdateJoystick()
    {
        // Procesar touches para el joystick
        if (Application.platform == RuntimePlatform.Android || 
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            ProcessMobileJoystick();
        }
        else
        {
            ProcessMouseJoystick(); // Para testing en editor
        }
    }
    
    void ProcessMobileJoystick()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            Vector2 touchPos = touch.position;
            
            // Verificar si el touch está en el área del joystick
            if (IsPositionInJoystickArea(touchPos))
            {
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        if (joystickTouchId == -1)
                        {
                            joystickTouchId = touch.fingerId;
                            ActivateJoystick(touchPos);
                        }
                        break;
                        
                    case TouchPhase.Moved:
                        if (joystickTouchId == touch.fingerId)
                        {
                            UpdateJoystickPosition(touchPos);
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
            ActivateJoystick(mousePos);
        }
        else if (Input.GetMouseButton(0) && isJoystickActive)
        {
            UpdateJoystickPosition(mousePos);
        }
        else if (Input.GetMouseButtonUp(0) && isJoystickActive)
        {
            DeactivateJoystick();
        }
    }
    
    bool IsPositionInJoystickArea(Vector2 position)
    {
        RectTransform joystickRect = joystickArea.GetComponent<RectTransform>();
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickRect, position, mobileUICanvas.worldCamera, out localPos);
        
        return joystickRect.rect.Contains(localPos);
    }
    
    void ActivateJoystick(Vector2 position)
    {
        isJoystickActive = true;
        joystickBackground.color = joystickActiveColor;
        
        // Mover el joystick a la posición del touch
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickArea.GetComponent<RectTransform>(), position, mobileUICanvas.worldCamera, out localPos);
        
        joystickBackground.transform.localPosition = localPos;
        joystickHandle.transform.localPosition = Vector2.zero;
        
        joystickStartPosition = position;
    }
    
    void UpdateJoystickPosition(Vector2 position)
    {
        Vector2 direction = position - joystickStartPosition;
        float distance = direction.magnitude;
        
        // Limitar la distancia al rango del joystick
        if (distance > joystickRange)
        {
            direction = direction.normalized * joystickRange;
            distance = joystickRange;
        }
        
        // Actualizar posición del handle
        joystickHandle.transform.localPosition = direction;
        
        // Calcular input normalizado
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
        
        // Resetear posición visual
        joystickBackground.color = joystickInactiveColor;
        joystickHandle.transform.localPosition = Vector2.zero;
    }
    
    void ProcessJoystickInput()
    {
        if (joystickInput.magnitude > 0.1f && currentPlayer != null)
        {
            // Convertir input 2D a movimiento 3D
            Vector3 movement = new Vector3(joystickInput.x, 0, joystickInput.y);
            
            // Aplicar movimiento al jugador
            currentPlayer.MovePlayer(movement);
        }
        else if (currentPlayer != null)
        {
            // Detener movimiento
            currentPlayer.StopMovement();
        }
    }
    
    void UpdateVisualEffects()
    {
        // Actualizar efectos visuales del joystick
        if (isJoystickActive)
        {
            float intensity = joystickInput.magnitude;
            Color activeColor = Color.Lerp(joystickInactiveColor, joystickActiveColor, intensity);
            joystickBackground.color = activeColor;
        }
        
        // Actualizar área de trucos
        if (isTrickGestureActive)
        {
            trickAreaBackground.color = new Color(1f, 1f, 0f, 0.3f);
        }
        else
        {
            trickAreaBackground.color = new Color(1f, 1f, 1f, 0.1f);
        }
    }
    
    // Eventos de botones
    void OnPassPressed()
    {
        isPassPressed = true;
        ExecutePass();
    }
    
    void OnPassReleased()
    {
        isPassPressed = false;
    }
    
    void OnShootPressed()
    {
        isShootPressed = true;
        ExecuteShoot();
    }
    
    void OnShootReleased()
    {
        isShootPressed = false;
    }
    
    void OnSprintPressed()
    {
        isSprintPressed = true;
        if (currentPlayer != null)
        {
            // Activar sprint
            currentPlayer.SetSprintActive(true);
        }
    }
    
    void OnSprintReleased()
    {
        isSprintPressed = false;
        if (currentPlayer != null)
        {
            // Desactivar sprint
            currentPlayer.SetSprintActive(false);
        }
    }
    
    void OnTacklePressed()
    {
        isTacklePressed = true;
        ExecuteTackle();
    }
    
    void OnTackleReleased()
    {
        isTacklePressed = false;
    }
    
    // Eventos de trucos
    void OnTrickGestureStart(Vector2 position)
    {
        isTrickGestureActive = true;
        trickGestureStart = position;
        trickGesturePoints.Clear();
        trickGesturePoints.Add(position);
    }
    
    void OnTrickGestureMove(Vector2 position)
    {
        if (isTrickGestureActive)
        {
            trickGesturePoints.Add(position);
        }
    }
    
    void OnTrickGestureEnd(Vector2 position)
    {
        if (isTrickGestureActive)
        {
            trickGesturePoints.Add(position);
            
            // Detectar truco
            if (trickDetector != null)
            {
                TrickType detectedTrick = trickDetector.DetectTrick(trickGesturePoints);
                if (detectedTrick != TrickType.None)
                {
                    ExecuteTrick(detectedTrick);
                }
            }
            
            isTrickGestureActive = false;
            trickGesturePoints.Clear();
        }
    }
    
    // Ejecutar acciones
    void ExecutePass()
    {
        if (currentPlayer != null && currentPlayer.HasBall())
        {
            // Calcular dirección del pase basado en el joystick
            Vector3 passDirection = Vector3.forward; // Dirección por defecto
            
            if (joystickInput.magnitude > 0.1f)
            {
                passDirection = new Vector3(joystickInput.x, 0, joystickInput.y).normalized;
            }
            
            float passForce = 15f; // Fuerza estándar de pase
            currentPlayer.PassBall(passDirection, passForce);
        }
    }
    
    void ExecuteShoot()
    {
        if (currentPlayer != null && currentPlayer.HasBall())
        {
            // Calcular dirección del disparo
            Vector3 shootDirection = Vector3.forward; // Hacia la portería por defecto
            
            if (joystickInput.magnitude > 0.1f)
            {
                shootDirection = new Vector3(joystickInput.x, 0, joystickInput.y).normalized;
            }
            
            float shootForce = 25f; // Fuerza estándar de disparo
            currentPlayer.ShootBall(shootDirection, shootForce);
        }
    }
    
    void ExecuteTackle()
    {
        if (currentPlayer != null)
        {
            // Ejecutar tackle/entrada
            PlayerController playerController = currentPlayer.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.ExecuteTackle();
            }
        }
    }
    
    void ExecuteTrick(TrickType trickType)
    {
        if (currentPlayer != null && currentPlayer.HasBall())
        {
            // Ejecutar truco específico
            switch (trickType)
            {
                case TrickType.Roulette:
                    currentPlayer.PlayTrickAnimation("Roulette");
                    break;
                case TrickType.Elastico:
                    currentPlayer.PlayTrickAnimation("Elastico");
                    break;
                case TrickType.StepOver:
                    currentPlayer.PlayTrickAnimation("StepOver");
                    break;
                case TrickType.Nutmeg:
                    currentPlayer.PlayTrickAnimation("Nutmeg");
                    break;
                case TrickType.RainbowFlick:
                    currentPlayer.PlayTrickAnimation("RainbowFlick");
                    break;
            }
        }
    }
    
    // Métodos públicos para otros sistemas
    public Vector2 GetJoystickInput()
    {
        return joystickInput;
    }
    
    public bool IsJoystickActive()
    {
        return isJoystickActive;
    }
    
    public bool IsSprintPressed()
    {
        return isSprintPressed;
    }
    
    public void SetCurrentPlayer(PlayerController player)
    {
        currentPlayer = player;
    }
    
    public void ShowUI()
    {
        gameObject.SetActive(true);
    }
    
    public void HideUI()
    {
        gameObject.SetActive(false);
    }
    
    public void SetUIVisible(bool visible)
    {
        mobileUICanvas.enabled = visible;
    }
    
    // Configuración dinámica
    public void SetJoystickSensitivity(float sensitivity)
    {
        joystickRange = 100f * sensitivity;
    }
    
    public void SetButtonSize(float size)
    {
        buttonSize = size;
        PositionActionButtons();
    }
    
    public void SetTrickAreaSensitivity(float sensitivity)
    {
        trickAreaSensitivity = sensitivity;
    }
}