using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerController_120fps : MonoBehaviour
{
    [Header("Player Info")]
    public PlayerData playerData;
    public PlayerPosition position;
    public bool isPlayerControlled = false;
    
    [Header("Movement")]
    public float baseSpeed = 5f;
    public float sprintSpeed = 8f;
    public float acceleration = 10f;
    public float rotationSpeed = 720f;
    
    [Header("Ball Control")]
    public Transform ballControlPoint;
    public float ballControlRadius = 1.5f;
    public float passingPower = 15f;
    public float shootingPower = 25f;
    
    [Header("Animations")]
    public Animator animator;
    
    [Header("120fps Optimizations")]
    public bool useObjectPooling = true;
    public bool enableLODSystem = true;
    public float cullingDistance = 50f;
    
    // Cache components for performance
    private NavMeshAgent agent;
    private Rigidbody rb;
    private Transform playerTransform;
    private BallController_120fps ballController;
    
    // Movement state
    private bool hasBall = false;
    private bool isSprinting = false;
    private Vector3 moveDirection;
    private float stamina = 100f;
    private float maxStamina = 100f;
    
    // AI and performance optimizations
    private PlayerAI playerAI;
    private float lastUpdateTime;
    private float updateInterval = 0.016f; // 60fps for AI, 120fps for movement
    private bool isVisible = true;
    private Camera playerCamera;
    
    // Pool system for effects
    private static Queue<GameObject> effectPool = new Queue<GameObject>();
    private static Dictionary<string, AudioClip> audioCache = new Dictionary<string, AudioClip>();
    
    // Animation optimization
    private int speedHash;
    private int hasBallHash;
    private int isSprintingHash;
    private int shootHash;
    private int passHash;
    
    // Movement prediction for 120fps
    private Vector3 predictedPosition;
    private Vector3 lastPosition;
    private float moveSmoothing = 0.1f;
    
    void Start()
    {
        InitializeComponents();
        SetupAnimationHashes();
        SetupPerformanceOptimizations();
    }
    
    void InitializeComponents()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        playerTransform = transform;
        ballController = FindObjectOfType<BallController_120fps>();
        playerAI = GetComponent<PlayerAI>();
        playerCamera = Camera.main;
        
        // Configure agent for 120fps
        agent.speed = baseSpeed;
        agent.acceleration = acceleration;
        agent.angularSpeed = rotationSpeed;
        agent.updateRotation = false; // We'll handle rotation manually for better performance
        
        // Apply player stats
        if (playerData != null)
        {
            ApplyPlayerStats();
        }
        
        lastPosition = playerTransform.position;
    }
    
    void SetupAnimationHashes()
    {
        speedHash = Animator.StringToHash("Speed");
        hasBallHash = Animator.StringToHash("HasBall");
        isSprintingHash = Animator.StringToHash("IsSprinting");
        shootHash = Animator.StringToHash("Shoot");
        passHash = Animator.StringToHash("Pass");
    }
    
    void SetupPerformanceOptimizations()
    {
        // Setup LOD system
        if (enableLODSystem)
        {
            LODGroup lodGroup = gameObject.AddComponent<LODGroup>();
            SetupLODLevels(lodGroup);
        }
        
        // Initialize effect pool
        if (useObjectPooling && effectPool.Count == 0)
        {
            InitializeEffectPool();
        }
    }
    
    void SetupLODLevels(LODGroup lodGroup)
    {
        LOD[] lods = new LOD[3];
        
        // High quality LOD (close)
        lods[0] = new LOD(0.6f, GetComponentsInChildren<Renderer>());
        
        // Medium quality LOD (medium distance)
        lods[1] = new LOD(0.3f, GetComponentsInChildren<Renderer>());
        
        // Low quality LOD (far)
        lods[2] = new LOD(0.1f, new Renderer[0]); // Culled at distance
        
        lodGroup.SetLODs(lods);
    }
    
    void InitializeEffectPool()
    {
        // Pre-instantiate effect objects for pooling
        for (int i = 0; i < 20; i++)
        {
            GameObject effect = new GameObject("Effect");
            effect.SetActive(false);
            effectPool.Enqueue(effect);
        }
    }
    
    void Update()
    {
        // 120fps optimized update
        float deltaTime = Time.deltaTime;
        
        // Check visibility for culling
        UpdateVisibility();
        
        if (!isVisible && Vector3.Distance(playerTransform.position, playerCamera.transform.position) > cullingDistance)
        {
            return; // Skip update if not visible and far away
        }
        
        // High frequency updates (120fps)
        UpdateMovement(deltaTime);
        UpdateStamina(deltaTime);
        CheckBallProximity();
        
        // Lower frequency updates (60fps)
        if (Time.time - lastUpdateTime >= updateInterval)
        {
            UpdateAI();
            UpdateAnimations();
            lastUpdateTime = Time.time;
        }
        
        // Player input (120fps for responsiveness)
        if (isPlayerControlled)
        {
            HandlePlayerInput();
        }
    }
    
    void UpdateVisibility()
    {
        if (playerCamera != null)
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(playerCamera);
            isVisible = GeometryUtility.TestPlanesAABB(planes, GetComponent<Collider>().bounds);
        }
    }
    
    void UpdateMovement(float deltaTime)
    {
        // Predict next position for smoother movement
        predictedPosition = Vector3.Lerp(lastPosition, playerTransform.position, moveSmoothing);
        
        // Update rotation manually for better performance
        if (moveDirection.magnitude > 0.1f)
        {
            Vector3 lookDirection = moveDirection.normalized;
            lookDirection.y = 0;
            
            if (lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, targetRotation, rotationSpeed * deltaTime);
            }
        }
        
        lastPosition = playerTransform.position;
    }
    
    void UpdateStamina(float deltaTime)
    {
        if (isSprinting)
        {
            stamina = Mathf.Max(0, stamina - deltaTime * 20f);
            if (stamina <= 0)
            {
                isSprinting = false;
            }
        }
        else
        {
            stamina = Mathf.Min(maxStamina, stamina + deltaTime * 10f);
        }
    }
    
    void CheckBallProximity()
    {
        if (ballController != null)
        {
            float sqrDistance = Vector3.SqrMagnitude(playerTransform.position - ballController.transform.position);
            hasBall = sqrDistance <= ballControlRadius * ballControlRadius; // Use squared distance for performance
        }
    }
    
    void UpdateAI()
    {
        if (playerAI != null && !isPlayerControlled)
        {
            playerAI.UpdateAI();
        }
    }
    
    void UpdateAnimations()
    {
        if (animator != null)
        {
            animator.SetFloat(speedHash, agent.velocity.magnitude);
            animator.SetBool(hasBallHash, hasBall);
            animator.SetBool(isSprintingHash, isSprinting);
        }
    }
    
    void ApplyPlayerStats()
    {
        baseSpeed = playerData.speed * 0.1f;
        sprintSpeed = playerData.speed * 0.15f;
        passingPower = playerData.passing * 0.3f;
        shootingPower = playerData.shooting * 0.4f;
        maxStamina = playerData.stamina;
        stamina = maxStamina;
    }
    
    void HandlePlayerInput()
    {
        // High-frequency input handling for 120fps responsiveness
        // This will be called by TouchControlManager
    }
    
    public void MovePlayer(Vector3 direction)
    {
        moveDirection = direction.normalized;
        
        if (moveDirection.magnitude > 0.1f)
        {
            // Optimized movement calculation
            float currentSpeed = isSprinting ? sprintSpeed : baseSpeed;
            float staminaModifier = stamina / maxStamina;
            currentSpeed *= staminaModifier;
            
            // Use predicted position for smoother movement
            Vector3 targetPosition = predictedPosition + moveDirection * currentSpeed;
            agent.SetDestination(targetPosition);
        }
    }
    
    public void Sprint(bool sprinting)
    {
        if (stamina > 0)
        {
            isSprinting = sprinting;
        }
    }
    
    public void Shoot(Vector3 direction, float power)
    {
        if (hasBall && ballController != null)
        {
            // Optimized shooting calculation
            float finalPower = power * (playerData.shooting * 0.01f) * (shootingPower * 0.04f);
            
            // Add accuracy variation
            float accuracy = playerData.technique * 0.01f;
            Vector3 finalDirection = AddShootingError(direction, 1f - accuracy);
            
            // Determine curve type
            CurveType curveType = DetermineCurveType(direction);
            
            ballController.ShootWithCurve(finalDirection, finalPower, curveType);
            
            // Optimized animation trigger
            if (animator != null)
            {
                animator.SetTrigger(shootHash);
            }
            
            // Play effect from pool
            PlayEffectFromPool("ShootEffect", playerTransform.position);
            
            stamina -= 5f;
        }
    }
    
    Vector3 AddShootingError(Vector3 direction, float errorAmount)
    {
        Vector3 error = new Vector3(
            Random.Range(-errorAmount, errorAmount),
            Random.Range(-errorAmount * 0.5f, errorAmount * 0.5f),
            Random.Range(-errorAmount, errorAmount)
        );
        
        return (direction + error).normalized;
    }
    
    CurveType DetermineCurveType(Vector3 direction)
    {
        float angle = Vector3.Angle(playerTransform.forward, direction);
        
        if (angle < 30f)
        {
            return CurveType.None;
        }
        else if (Vector3.Cross(playerTransform.forward, direction).y > 0)
        {
            return CurveType.Right;
        }
        else
        {
            return CurveType.Left;
        }
    }
    
    public void PerformShortPass()
    {
        if (hasBall && ballController != null)
        {
            PlayerController_120fps nearestTeammate = FindNearestTeammate();
            
            if (nearestTeammate != null)
            {
                Vector3 passDirection = (nearestTeammate.playerTransform.position - playerTransform.position).normalized;
                float passDistance = Vector3.Distance(playerTransform.position, nearestTeammate.playerTransform.position);
                
                float power = Mathf.Clamp(passDistance * 0.05f, 0.2f, 1f);
                power *= (playerData.passing * 0.01f);
                
                ballController.KickBall(passDirection, power);
                
                if (animator != null)
                {
                    animator.SetTrigger(passHash);
                }
                
                PlayEffectFromPool("PassEffect", playerTransform.position);
            }
        }
    }
    
    public void PerformTrick(TrickType trickType)
    {
        if (hasBall && stamina > 10f)
        {
            switch (trickType)
            {
                case TrickType.StepOverRight:
                    ExecuteStepOver(Vector3.right);
                    break;
                case TrickType.StepOverLeft:
                    ExecuteStepOver(Vector3.left);
                    break;
                case TrickType.Nutmeg:
                    ExecuteNutmeg();
                    break;
                case TrickType.Roulette:
                    ExecuteRoulette();
                    break;
                case TrickType.Elastico:
                    ExecuteElastico();
                    break;
                case TrickType.RainbowFlick:
                    ExecuteRainbowFlick();
                    break;
            }
            
            stamina -= 10f;
        }
    }
    
    void ExecuteStepOver(Vector3 direction)
    {
        Vector3 stepDirection = playerTransform.TransformDirection(direction);
        rb.AddForce(stepDirection * 300f, ForceMode.Impulse);
        
        if (ballController != null)
        {
            ballController.KickBall(stepDirection, 0.3f);
        }
        
        PlayEffectFromPool("StepOverEffect", playerTransform.position);
    }
    
    void ExecuteNutmeg()
    {
        if (ballController != null)
        {
            Vector3 nutmegDirection = playerTransform.forward;
            ballController.KickBall(nutmegDirection, 0.8f);
        }
        
        PlayEffectFromPool("NutmegEffect", playerTransform.position);
    }
    
    void ExecuteRoulette()
    {
        StartCoroutine(ExecuteRouletteCoroutine());
    }
    
    IEnumerator ExecuteRouletteCoroutine()
    {
        float rotationTime = 0.5f;
        float elapsedTime = 0f;
        Quaternion startRotation = playerTransform.rotation;
        
        while (elapsedTime < rotationTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / rotationTime;
            
            playerTransform.rotation = Quaternion.Lerp(startRotation, startRotation * Quaternion.Euler(0, 360f, 0), progress);
            
            yield return null;
        }
        
        PlayEffectFromPool("RouletteEffect", playerTransform.position);
    }
    
    void ExecuteElastico()
    {
        StartCoroutine(ExecuteElasticoCoroutine());
    }
    
    IEnumerator ExecuteElasticoCoroutine()
    {
        Vector3 fakeDirection = playerTransform.TransformDirection(Vector3.right);
        Vector3 realDirection = playerTransform.TransformDirection(Vector3.left);
        
        // Fake movement
        rb.AddForce(fakeDirection * 200f, ForceMode.Impulse);
        yield return new WaitForSeconds(0.1f);
        
        // Real movement
        rb.AddForce(realDirection * 400f, ForceMode.Impulse);
        
        if (ballController != null)
        {
            ballController.KickBall(realDirection, 0.5f);
        }
        
        PlayEffectFromPool("ElasticoEffect", playerTransform.position);
    }
    
    void ExecuteRainbowFlick()
    {
        if (ballController != null)
        {
            Vector3 rainbowDirection = playerTransform.forward + Vector3.up * 2f;
            ballController.KickBall(rainbowDirection.normalized, 1.2f);
        }
        
        PlayEffectFromPool("RainbowEffect", playerTransform.position);
    }
    
    PlayerController_120fps FindNearestTeammate()
    {
        PlayerController_120fps[] allPlayers = FindObjectsOfType<PlayerController_120fps>();
        PlayerController_120fps nearestTeammate = null;
        float nearestSqrDistance = float.MaxValue;
        
        foreach (PlayerController_120fps player in allPlayers)
        {
            if (player != this && player.playerData.teamId == playerData.teamId)
            {
                float sqrDistance = Vector3.SqrMagnitude(playerTransform.position - player.playerTransform.position);
                if (sqrDistance < nearestSqrDistance)
                {
                    nearestSqrDistance = sqrDistance;
                    nearestTeammate = player;
                }
            }
        }
        
        return nearestTeammate;
    }
    
    void PlayEffectFromPool(string effectName, Vector3 position)
    {
        if (useObjectPooling && effectPool.Count > 0)
        {
            GameObject effect = effectPool.Dequeue();
            effect.transform.position = position;
            effect.SetActive(true);
            
            // Return to pool after delay
            StartCoroutine(ReturnToPool(effect, 1f));
        }
    }
    
    IEnumerator ReturnToPool(GameObject effect, float delay)
    {
        yield return new WaitForSeconds(delay);
        effect.SetActive(false);
        effectPool.Enqueue(effect);
    }
    
    // Public methods for getting player info
    public float GetSkillLevel()
    {
        return playerData != null ? playerData.overall : 50f;
    }
    
    public PlayerInfo GetPlayerInfo()
    {
        return new PlayerInfo
        {
            playerData = playerData,
            position = playerTransform.position,
            velocity = agent.velocity,
            stamina = stamina,
            hasBall = hasBall,
            isSprinting = isSprinting
        };
    }
    
    // Cleanup
    void OnDestroy()
    {
        StopAllCoroutines();
    }
}

// Supporting classes and enums
public enum TrickType
{
    StepOverRight,
    StepOverLeft,
    Nutmeg,
    Roulette,
    Elastico,
    RainbowFlick
}

public enum PlayerPosition
{
    Goalkeeper,
    CenterBack,
    LeftBack,
    RightBack,
    DefensiveMidfield,
    CentralMidfield,
    AttackingMidfield,
    LeftWing,
    RightWing,
    Striker
}

[System.Serializable]
public class PlayerInfo
{
    public PlayerData playerData;
    public Vector3 position;
    public Vector3 velocity;
    public float stamina;
    public bool hasBall;
    public bool isSprinting;
}