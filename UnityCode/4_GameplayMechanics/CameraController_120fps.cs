using UnityEngine;
using System.Collections;

public class CameraController_120fps : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform target;
    public float followSpeed = 5f;
    public float rotationSpeed = 2f;
    public float zoomSpeed = 2f;
    
    [Header("Distance Settings")]
    public float minDistance = 5f;
    public float maxDistance = 20f;
    public float defaultDistance = 10f;
    
    [Header("Height Settings")]
    public float minHeight = 2f;
    public float maxHeight = 15f;
    public float defaultHeight = 8f;
    
    [Header("Angle Settings")]
    public float minAngle = 10f;
    public float maxAngle = 80f;
    public float defaultAngle = 45f;
    
    [Header("Boundary Settings")]
    public Vector3 fieldMin = new Vector3(-30, 0, -50);
    public Vector3 fieldMax = new Vector3(30, 0, 50);
    public float boundaryBuffer = 5f;
    
    [Header("120fps Optimizations")]
    public bool enableSmoothing = true;
    public bool enablePrediction = true;
    public bool enableLOD = true;
    public float cullingDistance = 100f;
    
    // Camera state
    private float currentDistance;
    private float currentHeight;
    private float currentAngle;
    private Vector3 currentVelocity;
    private Vector3 targetVelocity;
    
    // Focus system
    private bool isFollowingPlayer = false;
    private PlayerController_120fps focusedPlayer;
    private float focusTimer = 0f;
    
    // Input handling
    private bool isDragging = false;
    private Vector2 lastTouchPosition;
    private float touchStartTime;
    private bool isZooming = false;
    
    // Performance optimizations
    private Transform cameraTransform;
    private Camera cameraComponent;
    private float lastCullingUpdate;
    private float cullingUpdateInterval = 0.1f;
    
    // Prediction system
    private Vector3 predictedTargetPosition;
    private float predictionStrength = 0.5f;
    
    // Smooth movement
    private Vector3 lastTargetPosition;
    private float smoothingFactor = 0.1f;
    
    // Cinematic modes
    private CameraMode currentMode = CameraMode.Follow;
    private bool isCinematicMode = false;
    
    void Start()
    {
        InitializeCamera();
        SetupOptimizations();
    }
    
    void InitializeCamera()
    {
        cameraTransform = transform;
        cameraComponent = GetComponent<Camera>();
        
        // Set initial values
        currentDistance = defaultDistance;
        currentHeight = defaultHeight;
        currentAngle = defaultAngle;
        
        // Find ball if no target assigned
        if (target == null)
        {
            BallController_120fps ball = FindObjectOfType<BallController_120fps>();
            if (ball != null)
            {
                target = ball.transform;
            }
        }
        
        lastTargetPosition = target != null ? target.position : Vector3.zero;
    }
    
    void SetupOptimizations()
    {
        // Configure camera for 120fps
        if (enableLOD)
        {
            SetupLODSystem();
        }
        
        // Optimize camera settings
        cameraComponent.useOcclusionCulling = true;
        cameraComponent.allowDynamicResolution = true;
        
        // Set rendering path for performance
        cameraComponent.renderingPath = RenderingPath.Forward;
    }
    
    void SetupLODSystem()
    {
        // Configure LOD bias for performance
        QualitySettings.lodBias = 1.0f;
        QualitySettings.maximumLODLevel = 2;
    }
    
    void Update()
    {
        if (target == null) return;
        
        float deltaTime = Time.deltaTime;
        
        // Handle input at 120fps for responsiveness
        HandleInput();
        
        // Update focus system
        UpdateFocusSystem(deltaTime);
        
        // Update camera position with prediction
        UpdateCameraPosition(deltaTime);
        
        // Performance culling (lower frequency)
        if (Time.time - lastCullingUpdate > cullingUpdateInterval)
        {
            UpdateCulling();
            lastCullingUpdate = Time.time;
        }
    }
    
    void HandleInput()
    {
        if (Input.touchCount > 0)
        {
            HandleTouchInput();
        }
        else if (Application.isEditor)
        {
            HandleMouseInput();
        }
        
        HandleZoom();
    }
    
    void HandleTouchInput()
    {
        if (Input.touchCount == 1)
        {
            HandleSingleTouchInput();
        }
        else if (Input.touchCount == 2)
        {
            HandleTwoFingerInput();
        }
    }
    
    void HandleSingleTouchInput()
    {
        Touch touch = Input.GetTouch(0);
        
        switch (touch.phase)
        {
            case TouchPhase.Began:
                isDragging = true;
                lastTouchPosition = touch.position;
                touchStartTime = Time.time;
                break;
                
            case TouchPhase.Moved:
                if (isDragging && !isZooming)
                {
                    Vector2 deltaPosition = touch.position - lastTouchPosition;
                    RotateCamera(deltaPosition);
                    lastTouchPosition = touch.position;
                }
                break;
                
            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                isDragging = false;
                
                // Double tap to reset
                if (Time.time - touchStartTime < 0.3f)
                {
                    ResetCamera();
                }
                break;
        }
    }
    
    void HandleTwoFingerInput()
    {
        isZooming = true;
        HandlePinchZoom();
    }
    
    void HandlePinchZoom()
    {
        Touch touch1 = Input.GetTouch(0);
        Touch touch2 = Input.GetTouch(1);
        
        if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
        {
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
            Vector2 touch2PrevPos = touch2.position - touch2.deltaPosition;
            
            float prevTouchDeltaMag = Vector2.Distance(touch1PrevPos, touch2PrevPos);
            float touchDeltaMag = Vector2.Distance(touch1.position, touch2.position);
            
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
            
            currentDistance = Mathf.Clamp(currentDistance + deltaMagnitudeDiff * 0.01f, minDistance, maxDistance);
            currentHeight = Mathf.Clamp(currentHeight + deltaMagnitudeDiff * 0.005f, minHeight, maxHeight);
        }
        
        if (touch1.phase == TouchPhase.Ended || touch2.phase == TouchPhase.Ended)
        {
            isZooming = false;
        }
    }
    
    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastTouchPosition = Input.mousePosition;
            touchStartTime = Time.time;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            
            if (Time.time - touchStartTime < 0.3f)
            {
                ResetCamera();
            }
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Vector2 deltaPosition = (Vector2)Input.mousePosition - lastTouchPosition;
            RotateCamera(deltaPosition);
            lastTouchPosition = Input.mousePosition;
        }
    }
    
    void RotateCamera(Vector2 deltaPosition)
    {
        // Horizontal rotation
        float horizontalRotation = deltaPosition.x * rotationSpeed * Time.deltaTime;
        cameraTransform.RotateAround(target.position, Vector3.up, horizontalRotation);
        
        // Vertical angle adjustment
        float verticalRotation = -deltaPosition.y * rotationSpeed * Time.deltaTime;
        currentAngle = Mathf.Clamp(currentAngle + verticalRotation, minAngle, maxAngle);
    }
    
    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        
        if (scroll != 0f)
        {
            currentDistance = Mathf.Clamp(currentDistance - scroll * zoomSpeed, minDistance, maxDistance);
            currentHeight = Mathf.Clamp(currentHeight - scroll * zoomSpeed * 0.5f, minHeight, maxHeight);
        }
    }
    
    void UpdateFocusSystem(float deltaTime)
    {
        if (isFollowingPlayer && focusedPlayer != null)
        {
            focusTimer -= deltaTime;
            if (focusTimer <= 0f)
            {
                StopFollowingPlayer();
            }
        }
    }
    
    void UpdateCameraPosition(float deltaTime)
    {
        Vector3 targetPosition = GetTargetPosition();
        
        // Apply prediction
        if (enablePrediction)
        {
            targetPosition = ApplyPrediction(targetPosition);
        }
        
        // Clamp to field boundaries
        targetPosition = ClampToFieldBoundaries(targetPosition);
        
        // Calculate desired camera position
        Vector3 desiredPosition = CalculateDesiredPosition(targetPosition);
        
        // Smooth movement
        if (enableSmoothing)
        {
            cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position, desiredPosition, ref currentVelocity, 1f / followSpeed);
        }
        else
        {
            cameraTransform.position = desiredPosition;
        }
        
        // Update camera rotation
        UpdateCameraRotation(targetPosition);
        
        lastTargetPosition = targetPosition;
    }
    
    Vector3 GetTargetPosition()
    {
        if (isFollowingPlayer && focusedPlayer != null)
        {
            return focusedPlayer.transform.position;
        }
        else
        {
            return target.position;
        }
    }
    
    Vector3 ApplyPrediction(Vector3 targetPosition)
    {
        Vector3 velocity = (targetPosition - lastTargetPosition) / Time.deltaTime;
        predictedTargetPosition = targetPosition + velocity * predictionStrength;
        
        return Vector3.Lerp(targetPosition, predictedTargetPosition, predictionStrength);
    }
    
    Vector3 ClampToFieldBoundaries(Vector3 position)
    {
        position.x = Mathf.Clamp(position.x, fieldMin.x + boundaryBuffer, fieldMax.x - boundaryBuffer);
        position.z = Mathf.Clamp(position.z, fieldMin.z + boundaryBuffer, fieldMax.z - boundaryBuffer);
        return position;
    }
    
    Vector3 CalculateDesiredPosition(Vector3 targetPosition)
    {
        Vector3 direction = -cameraTransform.forward;
        Vector3 desiredPosition = targetPosition + direction * currentDistance;
        desiredPosition.y = targetPosition.y + currentHeight;
        
        return desiredPosition;
    }
    
    void UpdateCameraRotation(Vector3 targetPosition)
    {
        Vector3 lookDirection = (targetPosition - cameraTransform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        
        cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    
    void UpdateCulling()
    {
        // Frustum culling optimization
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cameraComponent);
        
        // Update LOD based on distance
        float distanceToTarget = Vector3.Distance(cameraTransform.position, target.position);
        
        if (distanceToTarget > cullingDistance)
        {
            // Reduce camera update frequency when far away
            enabled = false;
            StartCoroutine(ReEnableAfterDelay(0.1f));
        }
    }
    
    IEnumerator ReEnableAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        enabled = true;
    }
    
    // Public methods
    public void FocusOnPlayer(PlayerController_120fps player, float duration = 3f)
    {
        focusedPlayer = player;
        isFollowingPlayer = true;
        focusTimer = duration;
        
        // Closer view for player focus
        currentDistance = Mathf.Max(currentDistance * 0.7f, minDistance);
        currentHeight = Mathf.Max(currentHeight * 0.8f, minHeight);
    }
    
    public void StopFollowingPlayer()
    {
        isFollowingPlayer = false;
        focusedPlayer = null;
        focusTimer = 0f;
        
        // Return to normal distance
        currentDistance = defaultDistance;
        currentHeight = defaultHeight;
    }
    
    public void ResetCamera()
    {
        currentDistance = defaultDistance;
        currentHeight = defaultHeight;
        currentAngle = defaultAngle;
        
        StopFollowingPlayer();
    }
    
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        StopFollowingPlayer();
    }
    
    public void SetCameraMode(CameraMode mode)
    {
        currentMode = mode;
        
        switch (mode)
        {
            case CameraMode.Follow:
                followSpeed = 5f;
                currentDistance = defaultDistance;
                enableSmoothing = true;
                break;
                
            case CameraMode.Cinematic:
                followSpeed = 2f;
                currentDistance = defaultDistance * 1.5f;
                enableSmoothing = true;
                isCinematicMode = true;
                StartCoroutine(CinematicModeSequence());
                break;
                
            case CameraMode.Action:
                followSpeed = 8f;
                currentDistance = defaultDistance * 0.8f;
                enableSmoothing = false;
                break;
                
            case CameraMode.Tactical:
                followSpeed = 3f;
                currentDistance = maxDistance;
                currentHeight = maxHeight;
                currentAngle = maxAngle;
                enableSmoothing = true;
                break;
        }
    }
    
    IEnumerator CinematicModeSequence()
    {
        float originalFOV = cameraComponent.fieldOfView;
        
        while (isCinematicMode)
        {
            // Slowly change FOV for cinematic effect
            cameraComponent.fieldOfView = Mathf.Sin(Time.time * 0.5f) * 5f + originalFOV;
            yield return null;
        }
        
        cameraComponent.fieldOfView = originalFOV;
    }
    
    public void EnableCinematicMode(bool enable)
    {
        isCinematicMode = enable;
        
        if (enable)
        {
            SetCameraMode(CameraMode.Cinematic);
        }
        else
        {
            SetCameraMode(CameraMode.Follow);
        }
    }
    
    public CameraInfo GetCameraInfo()
    {
        return new CameraInfo
        {
            position = cameraTransform.position,
            rotation = cameraTransform.rotation,
            target = target,
            distance = currentDistance,
            height = currentHeight,
            angle = currentAngle,
            isFollowingPlayer = isFollowingPlayer
        };
    }
    
    // Debug visualization
    void OnDrawGizmos()
    {
        // Draw field boundaries
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((fieldMin + fieldMax) / 2, fieldMax - fieldMin);
        
        // Draw target
        if (target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(target.position, 1f);
        }
        
        // Draw predicted position
        if (enablePrediction)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(predictedTargetPosition, 0.5f);
        }
        
        // Draw camera frustum
        if (cameraComponent != null)
        {
            Gizmos.color = Color.green;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            Gizmos.DrawFrustum(Vector3.zero, cameraComponent.fieldOfView, cameraComponent.farClipPlane, cameraComponent.nearClipPlane, cameraComponent.aspect);
        }
    }
}

// Supporting enums and classes
public enum CameraMode
{
    Follow,
    Cinematic,
    Action,
    Tactical
}

[System.Serializable]
public class CameraInfo
{
    public Vector3 position;
    public Quaternion rotation;
    public Transform target;
    public float distance;
    public float height;
    public float angle;
    public bool isFollowingPlayer;
}