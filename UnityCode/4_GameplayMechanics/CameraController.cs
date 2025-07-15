using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform target; // Usualmente el balón
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
    
    private float currentDistance;
    private float currentHeight;
    private float currentAngle;
    private Vector3 currentVelocity;
    private bool isFollowingPlayer = false;
    private PlayerController focusedPlayer;
    private float focusTimer = 0f;
    
    // Para controles táctiles
    private bool isDragging = false;
    private Vector2 lastTouchPosition;
    private float touchStartTime;
    
    void Start()
    {
        // Configurar valores iniciales
        currentDistance = defaultDistance;
        currentHeight = defaultHeight;
        currentAngle = defaultAngle;
        
        // Si no hay target, buscar el balón
        if (target == null)
        {
            BallController ball = FindObjectOfType<BallController>();
            if (ball != null)
            {
                target = ball.transform;
            }
        }
    }
    
    void Update()
    {
        if (target == null) return;
        
        HandleInput();
        
        if (isFollowingPlayer && focusedPlayer != null)
        {
            focusTimer -= Time.deltaTime;
            if (focusTimer <= 0f)
            {
                StopFollowingPlayer();
            }
        }
        
        UpdateCameraPosition();
    }
    
    void HandleInput()
    {
        // Manejo de entrada táctil
        if (Input.touchCount > 0)
        {
            HandleTouchInput();
        }
        // Manejo de entrada de mouse para testing
        else if (Application.isEditor)
        {
            HandleMouseInput();
        }
        
        // Zoom con rueda del mouse o pellizco
        HandleZoom();
    }
    
    void HandleTouchInput()
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
                if (isDragging)
                {
                    Vector2 deltaPosition = touch.position - lastTouchPosition;
                    RotateCamera(deltaPosition);
                    lastTouchPosition = touch.position;
                }
                break;
                
            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                isDragging = false;
                
                // Detectar double tap para reset
                if (Time.time - touchStartTime < 0.3f)
                {
                    ResetCamera();
                }
                break;
        }
        
        // Manejo de zoom con dos dedos
        if (Input.touchCount == 2)
        {
            HandleTwoFingerZoom();
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
            
            // Double click para reset
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
        // Rotar horizontalmente
        float horizontalRotation = deltaPosition.x * rotationSpeed * Time.deltaTime;
        transform.RotateAround(target.position, Vector3.up, horizontalRotation);
        
        // Ajustar ángulo vertical
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
    
    void HandleTwoFingerZoom()
    {
        Touch touch1 = Input.GetTouch(0);
        Touch touch2 = Input.GetTouch(1);
        
        float currentDistance = Vector2.Distance(touch1.position, touch2.position);
        
        if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
        {
            Touch touch1PrevPos = touch1.position - touch1.deltaPosition;
            Touch touch2PrevPos = touch2.position - touch2.deltaPosition;
            
            float prevTouchDeltaMag = Vector2.Distance(touch1PrevPos, touch2PrevPos);
            float touchDeltaMag = Vector2.Distance(touch1.position, touch2.position);
            
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
            
            this.currentDistance = Mathf.Clamp(this.currentDistance + deltaMagnitudeDiff * 0.01f, minDistance, maxDistance);
            this.currentHeight = Mathf.Clamp(this.currentHeight + deltaMagnitudeDiff * 0.005f, minHeight, maxHeight);
        }
    }
    
    void UpdateCameraPosition()
    {
        Vector3 targetPosition;
        
        if (isFollowingPlayer && focusedPlayer != null)
        {
            targetPosition = focusedPlayer.transform.position;
        }
        else
        {
            targetPosition = target.position;
        }
        
        // Restringir la posición objetivo dentro de los límites del campo
        targetPosition.x = Mathf.Clamp(targetPosition.x, fieldMin.x + boundaryBuffer, fieldMax.x - boundaryBuffer);
        targetPosition.z = Mathf.Clamp(targetPosition.z, fieldMin.z + boundaryBuffer, fieldMax.z - boundaryBuffer);
        
        // Calcular la posición de la cámara
        Vector3 direction = -transform.forward;
        Vector3 desiredPosition = targetPosition + direction * currentDistance;
        desiredPosition.y = targetPosition.y + currentHeight;
        
        // Suavizar el movimiento
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, 1f / followSpeed);
        
        // Mirar al objetivo
        Vector3 lookDirection = (targetPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    
    public void FocusOnPlayer(PlayerController player, float duration = 3f)
    {
        focusedPlayer = player;
        isFollowingPlayer = true;
        focusTimer = duration;
        
        // Acercar un poco más al jugador
        currentDistance = Mathf.Max(currentDistance * 0.7f, minDistance);
        currentHeight = Mathf.Max(currentHeight * 0.8f, minHeight);
    }
    
    public void StopFollowingPlayer()
    {
        isFollowingPlayer = false;
        focusedPlayer = null;
        focusTimer = 0f;
        
        // Volver a la distancia normal
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
        switch (mode)
        {
            case CameraMode.Follow:
                followSpeed = 5f;
                currentDistance = defaultDistance;
                break;
                
            case CameraMode.Cinematic:
                followSpeed = 2f;
                currentDistance = defaultDistance * 1.5f;
                break;
                
            case CameraMode.Action:
                followSpeed = 8f;
                currentDistance = defaultDistance * 0.8f;
                break;
                
            case CameraMode.Tactical:
                followSpeed = 3f;
                currentDistance = maxDistance;
                currentHeight = maxHeight;
                currentAngle = maxAngle;
                break;
        }
    }
    
    // Método para obtener información de la cámara
    public CameraInfo GetCameraInfo()
    {
        return new CameraInfo
        {
            position = transform.position,
            rotation = transform.rotation,
            target = target,
            distance = currentDistance,
            height = currentHeight,
            angle = currentAngle,
            isFollowingPlayer = isFollowingPlayer
        };
    }
    
    void OnDrawGizmos()
    {
        // Visualizar los límites del campo
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((fieldMin + fieldMax) / 2, fieldMax - fieldMin);
        
        // Visualizar el objetivo
        if (target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(target.position, 1f);
        }
    }
}

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