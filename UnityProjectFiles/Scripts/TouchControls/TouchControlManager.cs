using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

/// <summary>
/// Controlador avanzado de controles táctiles estilo FIFA móvil
/// Optimizado para dispositivos de gama baja como Tecno Spark 8C
/// </summary>
public class TouchControlManager : MonoBehaviour
{
    [Header("🎯 Referencias")]
    public BallController ballController;
    public GameObject currentPlayer;
    public Camera mainCamera;
    
    [Header("⚽ Configuración de Patada")]
    [Range(5f, 50f)]
    public float minKickForce = 8f;
    [Range(10f, 100f)]
    public float maxKickForce = 30f;
    [Range(0.1f, 2f)]
    public float kickChargeTime = 1f;
    
    [Header("🎮 Configuración de Gestos")]
    [Range(10f, 100f)]
    public float minSwipeDistance = 20f;
    [Range(0.1f, 1f)]
    public float swipeTimeLimit = 0.5f;
    [Range(0.5f, 3f)]
    public float doubleTapTime = 0.3f;
    
    [Header("🌟 Sensibilidad")]
    [Range(0.1f, 3f)]
    public float movementSensitivity = 1f;
    [Range(0.1f, 3f)]
    public float spinSensitivity = 1f;
    [Range(0.1f, 2f)]
    public float curveSensitivity = 1f;
    
    [Header("🎨 Efectos Visuales")]
    public LineRenderer trajectoryLine;
    public GameObject kickIndicator;
    public GameObject directionArrow;
    
    [Header("🎵 Audio")]
    public AudioSource uiAudioSource;
    public AudioClip tapSound;
    public AudioClip swipeSound;
    public AudioClip trickSound;
    
    [Header("📱 Optimización Móvil")]
    [Range(1, 10)]
    public int maxTouchPoints = 5;
    [Range(0.01f, 0.1f)]
    public float touchUpdateInterval = 0.033f; // 30 FPS
    
    // Variables privadas
    private List<TouchData> activeTouches = new List<TouchData>();
    private Vector2 lastTouchPosition;
    private float lastTapTime;
    private float kickStartTime;
    private bool isChargingKick;
    private bool isDrawingTrajectory;
    private TrickDetector trickDetector;
    
    // Sistema de gestos
    private Vector2 swipeStartPos;
    private float swipeStartTime;
    private List<Vector2> gesturePoints = new List<Vector2>();
    
    // Optimización
    private float nextUpdateTime = 0f;
    private int frameCount = 0;
    
    void Start()
    {
        InitializeTouchControls();
        SetupOptimizationSettings();
    }
    
    void InitializeTouchControls()
    {
        // Inicializar componentes
        trickDetector = GetComponent<TrickDetector>();
        if (trickDetector == null)
        {
            trickDetector = gameObject.AddComponent<TrickDetector>();
        }
        
        // Configurar cámara si no está asignada
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        // Configurar audio
        if (uiAudioSource == null)
        {
            uiAudioSource = gameObject.AddComponent<AudioSource>();
        }
        
        uiAudioSource.spatialBlend = 0f; // 2D sound
        uiAudioSource.volume = 0.7f;
        
        // Configurar línea de trayectoria
        SetupTrajectoryLine();
        
        // Configurar indicadores visuales
        SetupVisualIndicators();
    }
    
    void SetupOptimizationSettings()
    {
        // Optimización para dispositivos de gama baja
        if (SystemInfo.systemMemorySize <= 4096) // 4GB RAM o menos
        {
            maxTouchPoints = 3;
            touchUpdateInterval = 0.05f; // 20 FPS
            
            // Reducir calidad de efectos visuales
            if (trajectoryLine != null)
            {
                trajectoryLine.positionCount = 20; // Reducir puntos
            }
        }
        
        // Configurar frame rate target
        Application.targetFrameRate = 30;
    }
    
    void Update()
    {
        if (Time.time >= nextUpdateTime)
        {
            HandleTouchInput();
            UpdateVisualFeedback();
            nextUpdateTime = Time.time + touchUpdateInterval;
        }
    }
    
    void HandleTouchInput()
    {
        // Limpiar touches antiguos
        CleanupTouches();
        
        // Procesar input según la plataforma
        if (Application.platform == RuntimePlatform.Android || 
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            HandleMobileInput();
        }
        else
        {
            HandleMouseInput(); // Para testing en editor
        }
    }
    
    void HandleMobileInput()
    {
        for (int i = 0; i < Input.touchCount && i < maxTouchPoints; i++)
        {
            Touch touch = Input.GetTouch(i);
            ProcessTouch(touch.fingerId, touch.position, touch.phase);
        }
    }
    
    void HandleMouseInput()
    {
        Vector2 mousePos = Input.mousePosition;
        
        if (Input.GetMouseButtonDown(0))
        {
            ProcessTouch(0, mousePos, TouchPhase.Began);
        }
        else if (Input.GetMouseButton(0))
        {
            ProcessTouch(0, mousePos, TouchPhase.Moved);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            ProcessTouch(0, mousePos, TouchPhase.Ended);
        }
    }
    
    void ProcessTouch(int fingerId, Vector2 position, TouchPhase phase)
    {
        // Ignorar touches sobre UI
        if (EventSystem.current.IsPointerOverGameObject(fingerId))
        {
            return;
        }
        
        TouchData touchData = GetOrCreateTouchData(fingerId);
        
        switch (phase)
        {
            case TouchPhase.Began:
                HandleTouchBegan(touchData, position);
                break;
                
            case TouchPhase.Moved:
                HandleTouchMoved(touchData, position);
                break;
                
            case TouchPhase.Ended:
                HandleTouchEnded(touchData, position);
                break;
                
            case TouchPhase.Canceled:
                HandleTouchCanceled(touchData);
                break;
        }
    }
    
    void HandleTouchBegan(TouchData touchData, Vector2 position)
    {
        touchData.startPosition = position;
        touchData.currentPosition = position;
        touchData.startTime = Time.time;
        touchData.isActive = true;
        
        // Detectar doble tap
        if (Time.time - lastTapTime < doubleTapTime)
        {
            HandleDoubleTap(position);
        }
        
        lastTapTime = Time.time;
        lastTouchPosition = position;
        
        // Iniciar carga de patada
        StartKickCharge(position);
        
        // Limpiar puntos de gesto
        gesturePoints.Clear();
        gesturePoints.Add(position);
        
        // Reproducir sonido
        PlayUISound(tapSound);
    }
    
    void HandleTouchMoved(TouchData touchData, Vector2 position)
    {
        if (!touchData.isActive) return;
        
        Vector2 deltaPos = position - touchData.currentPosition;
        touchData.currentPosition = position;
        
        // Agregar punto al gesto
        gesturePoints.Add(position);
        
        // Detectar swipe
        float swipeDistance = Vector2.Distance(touchData.startPosition, position);
        if (swipeDistance > minSwipeDistance)
        {
            HandleSwipe(touchData, position);
        }
        
        // Actualizar dirección de patada
        if (isChargingKick)
        {
            UpdateKickDirection(position);
        }
        
        // Mover jugador si no está cargando patada
        if (!isChargingKick && ballController != null)
        {
            MovePlayer(deltaPos);
        }
    }
    
    void HandleTouchEnded(TouchData touchData, Vector2 position)
    {
        if (!touchData.isActive) return;
        
        float touchDuration = Time.time - touchData.startTime;
        Vector2 swipeVector = position - touchData.startPosition;
        float swipeDistance = swipeVector.magnitude;
        
        // Determinar tipo de acción
        if (swipeDistance < minSwipeDistance)
        {
            // Tap simple
            HandleTap(position);
        }
        else if (touchDuration < swipeTimeLimit)
        {
            // Swipe rápido (disparo)
            HandleShot(swipeVector, touchDuration);
        }
        else
        {
            // Swipe lento (pase)
            HandlePass(swipeVector, touchDuration);
        }
        
        // Detectar trucos
        if (gesturePoints.Count > 5)
        {
            TrickType detectedTrick = trickDetector.DetectTrick(gesturePoints);
            if (detectedTrick != TrickType.None)
            {
                ExecuteTrick(detectedTrick);
            }
        }
        
        // Finalizar carga de patada
        if (isChargingKick)
        {
            ExecuteKick();
        }
        
        // Limpiar datos del touch
        touchData.isActive = false;
        HideVisualIndicators();
    }
    
    void HandleTouchCanceled(TouchData touchData)
    {
        touchData.isActive = false;
        isChargingKick = false;
        HideVisualIndicators();
    }
    
    void HandleTap(Vector2 position)
    {
        // Tap simple - seleccionar jugador o pase suave
        Vector3 worldPos = ScreenToWorldPoint(position);
        
        // Buscar jugador más cercano
        GameObject nearestPlayer = FindNearestPlayer(worldPos);
        if (nearestPlayer != null)
        {
            SelectPlayer(nearestPlayer);
        }
        else
        {
            // Pase suave hacia la posición
            PassToPosition(worldPos, minKickForce);
        }
    }
    
    void HandleDoubleTap(Vector2 position)
    {
        // Doble tap - disparo rápido
        Vector3 worldPos = ScreenToWorldPoint(position);
        Vector3 direction = (worldPos - ballController.transform.position).normalized;
        
        float shootForce = maxKickForce * 0.7f;
        ballController.KickBall(direction, shootForce);
        
        // Efectos visuales
        ShowKickEffect(worldPos);
        
        // Reproducir sonido
        PlayUISound(swipeSound);
    }
    
    void HandleSwipe(TouchData touchData, Vector2 position)
    {
        swipeStartPos = touchData.startPosition;
        swipeStartTime = touchData.startTime;
        
        // Mostrar indicador de dirección
        ShowDirectionIndicator(touchData.startPosition, position);
    }
    
    void HandleShot(Vector2 swipeVector, float duration)
    {
        // Swipe rápido = disparo potente
        Vector3 direction = ScreenToWorldDirection(swipeVector);
        float force = Mathf.Lerp(minKickForce, maxKickForce, swipeVector.magnitude / 200f);
        
        // Aplicar curva basada en la dirección del swipe
        Vector3 spin = CalculateSpinFromSwipe(swipeVector);
        
        ballController.KickBall(direction, force, spin);
        
        // Efectos
        ShowKickEffect(lastTouchPosition);
        PlayUISound(swipeSound);
        
        // Vibración
        TriggerHapticFeedback(HapticFeedbackType.Medium);
    }
    
    void HandlePass(Vector2 swipeVector, float duration)
    {
        // Swipe lento = pase controlado
        Vector3 direction = ScreenToWorldDirection(swipeVector);
        float force = Mathf.Lerp(minKickForce, maxKickForce * 0.6f, swipeVector.magnitude / 300f);
        
        ballController.KickBall(direction, force);
        
        // Efectos suaves
        ShowPassEffect(lastTouchPosition);
        PlayUISound(tapSound);
        
        // Vibración suave
        TriggerHapticFeedback(HapticFeedbackType.Light);
    }
    
    void StartKickCharge(Vector2 position)
    {
        kickStartTime = Time.time;
        isChargingKick = true;
        isDrawingTrajectory = true;
        
        // Mostrar indicador de carga
        ShowKickIndicator(position);
    }
    
    void UpdateKickDirection(Vector2 position)
    {
        if (!isChargingKick) return;
        
        Vector3 direction = ScreenToWorldDirection(position - lastTouchPosition);
        float chargeLevel = Mathf.Clamp01((Time.time - kickStartTime) / kickChargeTime);
        
        // Actualizar línea de trayectoria
        UpdateTrajectoryLine(direction, chargeLevel);
        
        // Actualizar indicador de fuerza
        UpdateKickIndicator(chargeLevel);
    }
    
    void ExecuteKick()
    {
        if (!isChargingKick) return;
        
        float chargeLevel = Mathf.Clamp01((Time.time - kickStartTime) / kickChargeTime);
        Vector3 direction = ScreenToWorldDirection(lastTouchPosition - swipeStartPos);
        float force = Mathf.Lerp(minKickForce, maxKickForce, chargeLevel);
        
        ballController.KickBall(direction, force);
        
        isChargingKick = false;
        isDrawingTrajectory = false;
        
        // Efectos
        ShowKickEffect(lastTouchPosition);
        PlayUISound(swipeSound);
        
        // Vibración proporcional a la fuerza
        HapticFeedbackType feedbackType = chargeLevel > 0.7f ? HapticFeedbackType.Heavy : HapticFeedbackType.Medium;
        TriggerHapticFeedback(feedbackType);
    }
    
    void ExecuteTrick(TrickType trickType)
    {
        // Ejecutar truco específico
        switch (trickType)
        {
            case TrickType.Roulette:
                ExecuteRoulette();
                break;
            case TrickType.Elastico:
                ExecuteElastico();
                break;
            case TrickType.StepOver:
                ExecuteStepOver();
                break;
            case TrickType.Nutmeg:
                ExecuteNutmeg();
                break;
            case TrickType.RainbowFlick:
                ExecuteRainbowFlick();
                break;
        }
        
        // Efectos generales
        PlayUISound(trickSound);
        TriggerHapticFeedback(HapticFeedbackType.Heavy);
    }
    
    void ExecuteRoulette()
    {
        // Roulette: girar el balón en círculo
        Vector3 spin = Vector3.up * 3f;
        ballController.KickBall(Vector3.zero, 2f, spin);
        
        // Animar jugador (si tienes animaciones)
        if (currentPlayer != null)
        {
            // Activar animación de roulette
            PlayerController playerController = currentPlayer.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.PlayTrickAnimation("Roulette");
            }
        }
    }
    
    void ExecuteElastico()
    {
        // Elastico: movimiento en forma de L
        Vector3 direction = Vector3.forward;
        Vector3 spin = Vector3.right * 2f;
        ballController.KickBall(direction, 5f, spin);
        
        if (currentPlayer != null)
        {
            PlayerController playerController = currentPlayer.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.PlayTrickAnimation("Elastico");
            }
        }
    }
    
    void ExecuteStepOver()
    {
        // Step-over: amago hacia un lado
        Vector3 direction = Vector3.right;
        ballController.KickBall(direction, 3f);
        
        if (currentPlayer != null)
        {
            PlayerController playerController = currentPlayer.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.PlayTrickAnimation("StepOver");
            }
        }
    }
    
    void ExecuteNutmeg()
    {
        // Nutmeg: túnel
        Vector3 direction = Vector3.forward;
        ballController.KickBall(direction, 4f);
        
        if (currentPlayer != null)
        {
            PlayerController playerController = currentPlayer.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.PlayTrickAnimation("Nutmeg");
            }
        }
    }
    
    void ExecuteRainbowFlick()
    {
        // Rainbow flick: arco hacia arriba
        Vector3 direction = Vector3.up + Vector3.forward;
        Vector3 spin = Vector3.back * 2f;
        ballController.KickBall(direction, 8f, spin);
        
        if (currentPlayer != null)
        {
            PlayerController playerController = currentPlayer.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.PlayTrickAnimation("RainbowFlick");
            }
        }
    }
    
    void MovePlayer(Vector2 deltaPos)
    {
        if (currentPlayer == null) return;
        
        Vector3 movement = ScreenToWorldDirection(deltaPos) * movementSensitivity;
        movement.y = 0; // Mantener en el suelo
        
        PlayerController playerController = currentPlayer.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.MovePlayer(movement);
        }
    }
    
    void PassToPosition(Vector3 worldPosition, float force)
    {
        if (ballController == null) return;
        
        Vector3 direction = (worldPosition - ballController.transform.position).normalized;
        ballController.KickBall(direction, force);
    }
    
    GameObject FindNearestPlayer(Vector3 worldPosition)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject nearest = null;
        float minDistance = float.MaxValue;
        
        foreach (GameObject player in players)
        {
            float distance = Vector3.Distance(player.transform.position, worldPosition);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = player;
            }
        }
        
        return nearest;
    }
    
    void SelectPlayer(GameObject player)
    {
        currentPlayer = player;
        
        // Mostrar indicador de selección
        ShowPlayerSelection(player);
        
        // Reproducir sonido
        PlayUISound(tapSound);
    }
    
    Vector3 ScreenToWorldPoint(Vector2 screenPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        
        if (groundPlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }
        
        return Vector3.zero;
    }
    
    Vector3 ScreenToWorldDirection(Vector2 screenDirection)
    {
        Vector3 worldDirection = mainCamera.ScreenToWorldPoint(new Vector3(screenDirection.x, screenDirection.y, 10f));
        worldDirection = worldDirection.normalized;
        worldDirection.y = 0; // Mantener en el plano horizontal
        return worldDirection;
    }
    
    Vector3 CalculateSpinFromSwipe(Vector2 swipeVector)
    {
        Vector3 spin = Vector3.zero;
        
        // Convertir dirección 2D a spin 3D
        float horizontalSpin = swipeVector.x * spinSensitivity;
        float verticalSpin = swipeVector.y * spinSensitivity;
        
        spin = new Vector3(verticalSpin, horizontalSpin, 0);
        
        return spin;
    }
    
    TouchData GetOrCreateTouchData(int fingerId)
    {
        TouchData touchData = activeTouches.Find(t => t.fingerId == fingerId);
        
        if (touchData == null)
        {
            touchData = new TouchData { fingerId = fingerId };
            activeTouches.Add(touchData);
        }
        
        return touchData;
    }
    
    void CleanupTouches()
    {
        for (int i = activeTouches.Count - 1; i >= 0; i--)
        {
            if (!activeTouches[i].isActive)
            {
                activeTouches.RemoveAt(i);
            }
        }
    }
    
    void UpdateVisualFeedback()
    {
        frameCount++;
        
        // Actualizar solo cada N frames para optimización
        if (frameCount % 3 == 0)
        {
            if (isDrawingTrajectory)
            {
                UpdateTrajectoryVisualization();
            }
        }
    }
    
    void UpdateTrajectoryVisualization()
    {
        if (trajectoryLine == null || !isDrawingTrajectory) return;
        
        // Calcular trayectoria predicha
        Vector3 ballPosition = ballController.transform.position;
        Vector3 direction = ScreenToWorldDirection(lastTouchPosition - swipeStartPos);
        float force = Mathf.Clamp01((Time.time - kickStartTime) / kickChargeTime);
        
        // Generar puntos de trayectoria
        List<Vector3> trajectoryPoints = CalculateTrajectory(ballPosition, direction, force);
        
        trajectoryLine.positionCount = trajectoryPoints.Count;
        trajectoryLine.SetPositions(trajectoryPoints.ToArray());
    }
    
    List<Vector3> CalculateTrajectory(Vector3 startPos, Vector3 direction, float force)
    {
        List<Vector3> points = new List<Vector3>();
        
        Vector3 velocity = direction * force * kickForceMultiplier;
        Vector3 position = startPos;
        
        float timeStep = 0.1f;
        float gravity = Physics.gravity.y;
        
        for (int i = 0; i < 30; i++)
        {
            points.Add(position);
            
            // Aplicar gravedad
            velocity.y += gravity * timeStep;
            
            // Aplicar resistencia del aire
            velocity *= 0.98f;
            
            // Actualizar posición
            position += velocity * timeStep;
            
            // Parar si toca el suelo
            if (position.y <= 0)
            {
                position.y = 0;
                points.Add(position);
                break;
            }
        }
        
        return points;
    }
    
    void SetupTrajectoryLine()
    {
        if (trajectoryLine == null)
        {
            GameObject trajectoryObject = new GameObject("TrajectoryLine");
            trajectoryObject.transform.SetParent(transform);
            trajectoryLine = trajectoryObject.AddComponent<LineRenderer>();
        }
        
        trajectoryLine.material = new Material(Shader.Find("Sprites/Default"));
        trajectoryLine.color = Color.yellow;
        trajectoryLine.startWidth = 0.1f;
        trajectoryLine.endWidth = 0.05f;
        trajectoryLine.enabled = false;
    }
    
    void SetupVisualIndicators()
    {
        // Configurar indicadores visuales aquí
        // Esto se puede expandir con prefabs específicos
    }
    
    void ShowKickIndicator(Vector2 position)
    {
        if (kickIndicator != null)
        {
            kickIndicator.SetActive(true);
            kickIndicator.transform.position = ScreenToWorldPoint(position);
        }
    }
    
    void UpdateKickIndicator(float chargeLevel)
    {
        if (kickIndicator != null)
        {
            // Actualizar escala o color basado en la carga
            float scale = Mathf.Lerp(0.5f, 1.5f, chargeLevel);
            kickIndicator.transform.localScale = Vector3.one * scale;
        }
    }
    
    void ShowDirectionIndicator(Vector2 start, Vector2 end)
    {
        if (directionArrow != null)
        {
            directionArrow.SetActive(true);
            Vector3 direction = ScreenToWorldDirection(end - start);
            directionArrow.transform.rotation = Quaternion.LookRotation(direction);
        }
    }
    
    void ShowKickEffect(Vector2 position)
    {
        // Mostrar efecto de patada
        Vector3 worldPos = ScreenToWorldPoint(position);
        // Aquí puedes instanciar partículas o efectos
    }
    
    void ShowPassEffect(Vector2 position)
    {
        // Mostrar efecto de pase
        Vector3 worldPos = ScreenToWorldPoint(position);
        // Aquí puedes instanciar partículas o efectos suaves
    }
    
    void ShowPlayerSelection(GameObject player)
    {
        // Mostrar indicador de selección del jugador
        // Aquí puedes instanciar un círculo o highlight
    }
    
    void HideVisualIndicators()
    {
        if (kickIndicator != null)
        {
            kickIndicator.SetActive(false);
        }
        
        if (directionArrow != null)
        {
            directionArrow.SetActive(false);
        }
        
        if (trajectoryLine != null)
        {
            trajectoryLine.enabled = false;
        }
    }
    
    void PlayUISound(AudioClip clip)
    {
        if (uiAudioSource != null && clip != null)
        {
            uiAudioSource.PlayOneShot(clip);
        }
    }
    
    void TriggerHapticFeedback(HapticFeedbackType type)
    {
        if (Application.platform == RuntimePlatform.Android || 
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            switch (type)
            {
                case HapticFeedbackType.Light:
                    // Vibración ligera
                    Handheld.Vibrate();
                    break;
                case HapticFeedbackType.Medium:
                    // Vibración media
                    Handheld.Vibrate();
                    break;
                case HapticFeedbackType.Heavy:
                    // Vibración fuerte
                    Handheld.Vibrate();
                    break;
            }
        }
    }
    
    // Métodos públicos para otros sistemas
    public void SetCurrentPlayer(GameObject player)
    {
        currentPlayer = player;
    }
    
    public bool IsChargingKick()
    {
        return isChargingKick;
    }
    
    public float GetKickChargeLevel()
    {
        if (!isChargingKick) return 0f;
        return Mathf.Clamp01((Time.time - kickStartTime) / kickChargeTime);
    }
}

[System.Serializable]
public class TouchData
{
    public int fingerId;
    public Vector2 startPosition;
    public Vector2 currentPosition;
    public float startTime;
    public bool isActive;
}

[System.Serializable]
public enum HapticFeedbackType
{
    Light,
    Medium,
    Heavy
}