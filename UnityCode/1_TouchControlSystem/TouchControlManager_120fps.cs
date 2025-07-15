using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TouchControlManager_120fps : MonoBehaviour
{
    [Header("Control Settings")]
    public float touchSensitivity = 1f;
    public float swipeThreshold = 50f;
    public float tapTimeThreshold = 0.3f;
    public float doubleTapTimeThreshold = 0.5f;
    
    [Header("Player References")]
    public PlayerController_120fps controlledPlayer;
    public BallController_120fps ballController;
    public CameraController_120fps cameraController;
    
    [Header("UI References")]
    public GameObject touchIndicator;
    public LineRenderer trajectoryLine;
    public GameObject powerIndicator;
    
    [Header("120fps Optimizations")]
    public bool enableTouchPrediction = true;
    public bool enableHapticFeedback = true;
    public float inputBufferTime = 0.1f;
    
    // Touch tracking
    private List<TouchInfo> activeTouches = new List<TouchInfo>();
    private Dictionary<int, TouchInfo> touchHistory = new Dictionary<int, TouchInfo>();
    private Queue<InputCommand> inputBuffer = new Queue<InputCommand>();
    
    // Gesture detection
    private TrickDetector_120fps trickDetector;
    private Vector2 lastTouchPosition;
    private float lastTouchTime;
    private bool isDragging = false;
    
    // Power system
    private bool isChargingPower = false;
    private float powerChargeTime = 0f;
    private float maxPowerTime = 2f;
    
    // Input prediction
    private Vector2 predictedTouchPosition;
    private float predictionStrength = 0.3f;
    
    // Performance optimization
    private Camera mainCamera;
    private bool isInputEnabled = true;
    private float lastInputTime;
    
    void Start()
    {
        InitializeComponents();
        SetupTouchControl();
    }
    
    void InitializeComponents()
    {
        mainCamera = Camera.main;
        trickDetector = GetComponent<TrickDetector_120fps>();
        
        if (trickDetector == null)
        {
            trickDetector = gameObject.AddComponent<TrickDetector_120fps>();
        }
        
        // Find player if not assigned
        if (controlledPlayer == null)
        {
            controlledPlayer = FindObjectOfType<PlayerController_120fps>();
        }
        
        if (ballController == null)
        {
            ballController = FindObjectOfType<BallController_120fps>();
        }
        
        if (cameraController == null)
        {
            cameraController = FindObjectOfType<CameraController_120fps>();
        }
    }
    
    void SetupTouchControl()
    {
        // Configure input settings for 120fps
        Input.multiTouchEnabled = true;
        Input.touchPressureSupported = true;
        
        // Setup trajectory line
        if (trajectoryLine == null)
        {
            GameObject lineObj = new GameObject("TrajectoryLine");
            trajectoryLine = lineObj.AddComponent<LineRenderer>();
            trajectoryLine.material = new Material(Shader.Find("Sprites/Default"));
            trajectoryLine.color = Color.white;
            trajectoryLine.width = 0.1f;
            trajectoryLine.enabled = false;
        }
    }
    
    void Update()
    {
        if (!isInputEnabled) return;
        
        // Process touch input at 120fps
        ProcessTouchInput();
        
        // Handle input buffer
        ProcessInputBuffer();
        
        // Update power charging
        UpdatePowerCharging();
        
        // Update visual feedback
        UpdateVisualFeedback();
    }
    
    void ProcessTouchInput()
    {
        // Clear previous frame touches
        activeTouches.Clear();
        
        // Process all active touches
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            ProcessIndividualTouch(touch);
        }
        
        // Handle mouse input for testing
        if (Application.isEditor)
        {
            ProcessMouseInput();
        }
        
        // Clean up old touch history
        CleanupTouchHistory();
    }
    
    void ProcessIndividualTouch(Touch touch)
    {
        TouchInfo touchInfo = new TouchInfo
        {
            fingerId = touch.fingerId,
            position = touch.position,
            deltaPosition = touch.deltaPosition,
            phase = touch.phase,
            pressure = touch.pressure,
            timestamp = Time.time
        };
        
        activeTouches.Add(touchInfo);
        
        // Update touch history
        if (!touchHistory.ContainsKey(touch.fingerId))
        {
            touchHistory[touch.fingerId] = touchInfo;
        }
        else
        {
            touchHistory[touch.fingerId] = touchInfo;
        }
        
        // Process touch based on phase
        switch (touch.phase)
        {
            case TouchPhase.Began:
                HandleTouchBegan(touchInfo);
                break;
            case TouchPhase.Moved:
                HandleTouchMoved(touchInfo);
                break;
            case TouchPhase.Ended:
                HandleTouchEnded(touchInfo);
                break;
            case TouchPhase.Canceled:
                HandleTouchCanceled(touchInfo);
                break;
        }
    }
    
    void ProcessMouseInput()
    {
        // Mouse input for testing in editor
        if (Input.GetMouseButtonDown(0))
        {
            TouchInfo mouseTouch = new TouchInfo
            {
                fingerId = 0,
                position = Input.mousePosition,
                phase = TouchPhase.Began,
                timestamp = Time.time
            };
            HandleTouchBegan(mouseTouch);
        }
        else if (Input.GetMouseButton(0))
        {
            TouchInfo mouseTouch = new TouchInfo
            {
                fingerId = 0,
                position = Input.mousePosition,
                deltaPosition = (Vector2)Input.mousePosition - lastTouchPosition,
                phase = TouchPhase.Moved,
                timestamp = Time.time
            };
            HandleTouchMoved(mouseTouch);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            TouchInfo mouseTouch = new TouchInfo
            {
                fingerId = 0,
                position = Input.mousePosition,
                phase = TouchPhase.Ended,
                timestamp = Time.time
            };
            HandleTouchEnded(mouseTouch);
        }
    }
    
    void HandleTouchBegan(TouchInfo touch)
    {
        lastTouchPosition = touch.position;
        lastTouchTime = touch.timestamp;
        isDragging = false;
        
        // Show touch indicator
        if (touchIndicator != null)
        {
            Vector3 worldPos = ScreenToWorldPoint(touch.position);
            touchIndicator.transform.position = worldPos;
            touchIndicator.SetActive(true);
        }
        
        // Start power charging for potential shot
        if (IsPlayerControllingBall())
        {
            StartPowerCharging();
        }
        
        // Haptic feedback
        if (enableHapticFeedback)
        {
            Handheld.Vibrate();
        }
    }
    
    void HandleTouchMoved(TouchInfo touch)
    {
        Vector2 deltaMove = touch.position - lastTouchPosition;
        
        if (deltaMove.magnitude > swipeThreshold)
        {
            isDragging = true;
            
            // Handle player movement
            if (controlledPlayer != null)
            {
                Vector3 moveDirection = ScreenToWorldDirection(deltaMove);
                controlledPlayer.MovePlayer(moveDirection);
            }
            
            // Update trajectory preview
            if (IsPlayerControllingBall())
            {
                UpdateTrajectoryPreview(touch.position);
            }
        }
        
        lastTouchPosition = touch.position;
    }
    
    void HandleTouchEnded(TouchInfo touch)
    {
        float touchDuration = touch.timestamp - lastTouchTime;
        Vector2 totalDelta = touch.position - GetTouchStartPosition(touch.fingerId);
        
        // Hide touch indicator
        if (touchIndicator != null)
        {
            touchIndicator.SetActive(false);
        }
        
        // Hide trajectory line
        if (trajectoryLine != null)
        {
            trajectoryLine.enabled = false;
        }
        
        if (touchDuration < tapTimeThreshold && !isDragging)
        {
            // Handle tap
            HandleTap(touch);
        }
        else if (isDragging)
        {
            // Handle swipe
            HandleSwipe(touch, totalDelta);
        }
        
        // Stop power charging
        StopPowerCharging();
        
        // Check for tricks
        if (trickDetector != null)
        {
            TrickType detectedTrick = trickDetector.DetectTrick(touchHistory[touch.fingerId]);
            if (detectedTrick != TrickType.StepOverRight) // Default value check
            {
                ExecuteTrick(detectedTrick);
            }
        }
        
        isDragging = false;
    }
    
    void HandleTouchCanceled(TouchInfo touch)
    {
        // Clean up canceled touch
        if (touchIndicator != null)
        {
            touchIndicator.SetActive(false);
        }
        
        if (trajectoryLine != null)
        {
            trajectoryLine.enabled = false;
        }
        
        StopPowerCharging();
        isDragging = false;
    }
    
    void HandleTap(TouchInfo touch)
    {
        // Simple tap - short pass or player switch
        if (IsPlayerControllingBall())
        {
            controlledPlayer.PerformShortPass();
        }
        else
        {
            // Switch to nearest player
            SwitchToNearestPlayer(touch.position);
        }
    }
    
    void HandleSwipe(TouchInfo touch, Vector2 swipeDirection)
    {
        float swipeStrength = swipeDirection.magnitude / Screen.height;
        Vector3 worldDirection = ScreenToWorldDirection(swipeDirection);
        
        if (IsPlayerControllingBall())
        {
            if (swipeStrength > 0.3f)
            {
                // Strong swipe - shoot
                float power = Mathf.Clamp01(swipeStrength * 2f);
                controlledPlayer.Shoot(worldDirection, power);
            }
            else
            {
                // Weak swipe - pass
                controlledPlayer.PerformShortPass();
            }
        }
        else
        {
            // Move player
            controlledPlayer.MovePlayer(worldDirection);
        }
    }
    
    void ExecuteTrick(TrickType trickType)
    {
        if (controlledPlayer != null && IsPlayerControllingBall())
        {
            controlledPlayer.PerformTrick(trickType);
        }
    }
    
    void StartPowerCharging()
    {
        isChargingPower = true;
        powerChargeTime = 0f;
        
        if (powerIndicator != null)
        {
            powerIndicator.SetActive(true);
        }
    }
    
    void StopPowerCharging()
    {
        if (isChargingPower)
        {
            float power = Mathf.Clamp01(powerChargeTime / maxPowerTime);
            
            // Use charged power for shot
            if (IsPlayerControllingBall() && power > 0.1f)
            {
                Vector3 shootDirection = ScreenToWorldDirection(lastTouchPosition - GetTouchStartPosition(0));
                controlledPlayer.Shoot(shootDirection, power);
            }
        }
        
        isChargingPower = false;
        powerChargeTime = 0f;
        
        if (powerIndicator != null)
        {
            powerIndicator.SetActive(false);
        }
    }
    
    void UpdatePowerCharging()
    {
        if (isChargingPower)
        {
            powerChargeTime += Time.deltaTime;
            
            // Update power indicator
            if (powerIndicator != null)
            {
                float powerPercent = Mathf.Clamp01(powerChargeTime / maxPowerTime);
                powerIndicator.transform.localScale = Vector3.one * (0.5f + powerPercent * 0.5f);
            }
        }
    }
    
    void UpdateTrajectoryPreview(Vector2 screenPos)
    {
        if (trajectoryLine != null && IsPlayerControllingBall())
        {
            Vector3 shootDirection = ScreenToWorldDirection(screenPos - GetTouchStartPosition(0));
            Vector3[] trajectory = ballController.PredictTrajectory(30);
            
            trajectoryLine.positionCount = trajectory.Length;
            trajectoryLine.SetPositions(trajectory);
            trajectoryLine.enabled = true;
        }
    }
    
    void UpdateVisualFeedback()
    {
        // Update touch indicator position
        if (touchIndicator != null && touchIndicator.activeInHierarchy)
        {
            if (activeTouches.Count > 0)
            {
                Vector3 worldPos = ScreenToWorldPoint(activeTouches[0].position);
                touchIndicator.transform.position = worldPos;
            }
        }
    }
    
    void ProcessInputBuffer()
    {
        // Process buffered inputs for smoother controls
        while (inputBuffer.Count > 0)
        {
            InputCommand command = inputBuffer.Dequeue();
            
            if (Time.time - command.timestamp < inputBufferTime)
            {
                ExecuteInputCommand(command);
            }
        }
    }
    
    void ExecuteInputCommand(InputCommand command)
    {
        switch (command.type)
        {
            case InputType.Move:
                if (controlledPlayer != null)
                {
                    controlledPlayer.MovePlayer(command.direction);
                }
                break;
            case InputType.Shoot:
                if (controlledPlayer != null)
                {
                    controlledPlayer.Shoot(command.direction, command.power);
                }
                break;
            case InputType.Pass:
                if (controlledPlayer != null)
                {
                    controlledPlayer.PerformShortPass();
                }
                break;
        }
    }
    
    // Helper methods
    Vector3 ScreenToWorldPoint(Vector2 screenPos)
    {
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, mainCamera.nearClipPlane + 10f));
        worldPos.y = 0f; // Keep on ground level
        return worldPos;
    }
    
    Vector3 ScreenToWorldDirection(Vector2 screenDelta)
    {
        Vector3 worldDelta = mainCamera.ScreenToWorldPoint(new Vector3(screenDelta.x, screenDelta.y, mainCamera.nearClipPlane + 10f)) - 
                            mainCamera.ScreenToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane + 10f));
        worldDelta.y = 0f;
        return worldDelta.normalized;
    }
    
    Vector2 GetTouchStartPosition(int fingerId)
    {
        if (touchHistory.ContainsKey(fingerId))
        {
            return touchHistory[fingerId].position;
        }
        return Vector2.zero;
    }
    
    bool IsPlayerControllingBall()
    {
        return controlledPlayer != null && controlledPlayer.GetPlayerInfo().hasBall;
    }
    
    void SwitchToNearestPlayer(Vector2 screenPos)
    {
        Vector3 worldPos = ScreenToWorldPoint(screenPos);
        PlayerController_120fps[] allPlayers = FindObjectsOfType<PlayerController_120fps>();
        
        PlayerController_120fps nearestPlayer = null;
        float nearestDistance = float.MaxValue;
        
        foreach (PlayerController_120fps player in allPlayers)
        {
            float distance = Vector3.Distance(worldPos, player.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestPlayer = player;
            }
        }
        
        if (nearestPlayer != null)
        {
            controlledPlayer = nearestPlayer;
        }
    }
    
    void CleanupTouchHistory()
    {
        List<int> keysToRemove = new List<int>();
        
        foreach (var kvp in touchHistory)
        {
            if (Time.time - kvp.Value.timestamp > 1f) // Remove old touches
            {
                keysToRemove.Add(kvp.Key);
            }
        }
        
        foreach (int key in keysToRemove)
        {
            touchHistory.Remove(key);
        }
    }
    
    // Public methods
    public void SetControlledPlayer(PlayerController_120fps player)
    {
        controlledPlayer = player;
    }
    
    public void EnableInput(bool enable)
    {
        isInputEnabled = enable;
    }
    
    public float GetCurrentPower()
    {
        return isChargingPower ? Mathf.Clamp01(powerChargeTime / maxPowerTime) : 0f;
    }
}

// Supporting classes
[System.Serializable]
public class TouchInfo
{
    public int fingerId;
    public Vector2 position;
    public Vector2 deltaPosition;
    public TouchPhase phase;
    public float pressure;
    public float timestamp;
}

[System.Serializable]
public class InputCommand
{
    public InputType type;
    public Vector3 direction;
    public float power;
    public float timestamp;
}

public enum InputType
{
    Move,
    Shoot,
    Pass,
    Trick
}