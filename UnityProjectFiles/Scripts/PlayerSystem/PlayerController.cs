using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Controlador avanzado de jugadores con IA realista
/// Optimizado para dispositivos móviles como Tecno Spark 8C
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("🎯 Configuración del Jugador")]
    public PlayerData playerData;
    public PlayerPosition position;
    public PlayerTeam team;
    
    [Header("⚽ Movimiento")]
    [Range(1f, 15f)]
    public float walkSpeed = 3f;
    [Range(3f, 25f)]
    public float runSpeed = 8f;
    [Range(5f, 30f)]
    public float sprintSpeed = 12f;
    [Range(0.1f, 2f)]
    public float acceleration = 1f;
    [Range(0.1f, 2f)]
    public float deceleration = 1.5f;
    
    [Header("🏃 Stamina")]
    [Range(50f, 100f)]
    public float maxStamina = 100f;
    [Range(0.1f, 5f)]
    public float staminaDecayRate = 1f;
    [Range(0.1f, 5f)]
    public float staminaRecoveryRate = 2f;
    
    [Header("⚽ Habilidades de Balón")]
    [Range(0.1f, 2f)]
    public float ballControlRadius = 1.2f;
    [Range(1f, 10f)]
    public float passingAccuracy = 5f;
    [Range(1f, 10f)]
    public float shootingPower = 6f;
    [Range(0.1f, 2f)]
    public float dribbleSpeed = 1.5f;
    
    [Header("🎮 Configuración de IA")]
    [Range(0.1f, 3f)]
    public float reactionTime = 0.3f;
    [Range(1f, 20f)]
    public float visionRange = 15f;
    [Range(0.1f, 2f)]
    public float decisionMakingSpeed = 1f;
    
    [Header("🎨 Efectos Visuales")]
    public ParticleSystem runDustParticles;
    public ParticleSystem sweatParticles;
    public GameObject selectionIndicator;
    public GameObject ballControlIndicator;
    
    [Header("🎵 Audio")]
    public AudioSource playerAudioSource;
    public AudioClip[] runSounds;
    public AudioClip[] breathingSounds;
    public AudioClip[] effortSounds;
    
    [Header("📱 Optimización")]
    [Range(0.01f, 0.1f)]
    public float aiUpdateInterval = 0.05f;
    [Range(0.01f, 0.1f)]
    public float animationUpdateInterval = 0.033f;
    
    // Componentes privados
    private NavMeshAgent navAgent;
    private Animator animator;
    private Rigidbody rb;
    private CapsuleCollider playerCollider;
    private PlayerAI playerAI;
    private PlayerStats playerStats;
    
    // Estado del jugador
    private PlayerState currentState;
    private MovementType currentMovement;
    private float currentStamina;
    private bool hasBall;
    private bool isSelected;
    private bool isControlledByUser;
    
    // Control de movimiento
    private Vector3 targetPosition;
    private Vector3 moveDirection;
    private float currentSpeed;
    private bool isMoving;
    
    // Optimización para móviles
    private float nextAIUpdate = 0f;
    private float nextAnimationUpdate = 0f;
    private int updateFrame = 0;
    
    // Referencias
    private BallController ballController;
    private GameManager gameManager;
    private GameObject[] teammates;
    private GameObject[] opponents;
    
    void Start()
    {
        InitializePlayer();
        SetupComponents();
        SetupOptimizationSettings();
        CacheReferences();
    }
    
    void InitializePlayer()
    {
        // Inicializar datos del jugador si no existen
        if (playerData == null)
        {
            playerData = CreateDefaultPlayerData();
        }
        
        // Configurar estado inicial
        currentState = PlayerState.Idle;
        currentMovement = MovementType.Walk;
        currentStamina = maxStamina;
        hasBall = false;
        isSelected = false;
        isControlledByUser = false;
        
        // Configurar posición inicial
        targetPosition = transform.position;
        moveDirection = Vector3.zero;
        currentSpeed = 0f;
    }
    
    void SetupComponents()
    {
        // NavMeshAgent
        navAgent = GetComponent<NavMeshAgent>();
        if (navAgent == null)
        {
            navAgent = gameObject.AddComponent<NavMeshAgent>();
        }
        
        navAgent.speed = walkSpeed;
        navAgent.acceleration = acceleration * 10f;
        navAgent.angularSpeed = 360f;
        navAgent.stoppingDistance = 0.5f;
        
        // Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        rb.mass = 75f; // Peso promedio de un jugador
        rb.drag = 2f;
        rb.angularDrag = 10f;
        rb.freezeRotation = true;
        
        // Collider
        playerCollider = GetComponent<CapsuleCollider>();
        if (playerCollider == null)
        {
            playerCollider = gameObject.AddComponent<CapsuleCollider>();
        }
        
        playerCollider.height = 1.8f;
        playerCollider.radius = 0.3f;
        playerCollider.center = new Vector3(0, 0.9f, 0);
        
        // Animator
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning($"No Animator found on {gameObject.name}. Animations will not work.");
        }
        
        // IA
        playerAI = GetComponent<PlayerAI>();
        if (playerAI == null)
        {
            playerAI = gameObject.AddComponent<PlayerAI>();
        }
        
        // Estadísticas
        playerStats = GetComponent<PlayerStats>();
        if (playerStats == null)
        {
            playerStats = gameObject.AddComponent<PlayerStats>();
        }
        
        // Audio
        if (playerAudioSource == null)
        {
            playerAudioSource = gameObject.AddComponent<AudioSource>();
        }
        
        playerAudioSource.spatialBlend = 1f; // 3D sound
        playerAudioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        playerAudioSource.maxDistance = 20f;
        playerAudioSource.volume = 0.6f;
    }
    
    void SetupOptimizationSettings()
    {
        // Optimización para dispositivos de gama baja
        if (SystemInfo.systemMemorySize <= 4096) // 4GB RAM o menos
        {
            aiUpdateInterval = 0.1f;
            animationUpdateInterval = 0.05f;
            visionRange = 10f;
            
            // Reducir efectos de partículas
            if (runDustParticles != null)
            {
                var main = runDustParticles.main;
                main.maxParticles = 10;
            }
            
            if (sweatParticles != null)
            {
                var main = sweatParticles.main;
                main.maxParticles = 5;
            }
        }
        
        // Configurar LOD según la distancia a la cámara
        StartCoroutine(UpdateLOD());
    }
    
    void CacheReferences()
    {
        // Encontrar referencias importantes
        ballController = FindObjectOfType<BallController>();
        gameManager = FindObjectOfType<GameManager>();
        
        // Encontrar compañeros de equipo y oponentes
        CacheTeammates();
        CacheOpponents();
    }
    
    void Update()
    {
        updateFrame++;
        
        // Actualizar IA
        if (Time.time >= nextAIUpdate && !isControlledByUser)
        {
            UpdateAI();
            nextAIUpdate = Time.time + aiUpdateInterval;
        }
        
        // Actualizar animaciones
        if (Time.time >= nextAnimationUpdate)
        {
            UpdateAnimations();
            nextAnimationUpdate = Time.time + animationUpdateInterval;
        }
        
        // Actualizar movimiento
        UpdateMovement();
        
        // Actualizar stamina
        UpdateStamina();
        
        // Actualizar efectos visuales
        UpdateVisualEffects();
        
        // Verificar posesión del balón
        CheckBallPossession();
    }
    
    void UpdateAI()
    {
        if (playerAI != null && !isControlledByUser)
        {
            playerAI.UpdateAI();
        }
    }
    
    void UpdateMovement()
    {
        if (navAgent.enabled && !isControlledByUser)
        {
            // Movimiento controlado por IA
            isMoving = navAgent.velocity.magnitude > 0.1f;
            currentSpeed = navAgent.velocity.magnitude;
            
            // Actualizar tipo de movimiento basado en la velocidad
            UpdateMovementType();
        }
        else if (isControlledByUser)
        {
            // Movimiento controlado por el usuario
            UpdateUserControlledMovement();
        }
        
        // Actualizar dirección de movimiento
        if (isMoving)
        {
            moveDirection = navAgent.velocity.normalized;
            
            // Rotar hacia la dirección de movimiento
            if (moveDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(moveDirection),
                    Time.deltaTime * 10f
                );
            }
        }
    }
    
    void UpdateMovementType()
    {
        float speedRatio = currentSpeed / sprintSpeed;
        
        if (speedRatio < 0.3f)
        {
            currentMovement = MovementType.Walk;
            navAgent.speed = walkSpeed;
        }
        else if (speedRatio < 0.7f)
        {
            currentMovement = MovementType.Run;
            navAgent.speed = runSpeed;
        }
        else
        {
            currentMovement = MovementType.Sprint;
            navAgent.speed = sprintSpeed;
        }
        
        // Consumir stamina según el tipo de movimiento
        ConsumeStamina();
    }
    
    void UpdateUserControlledMovement()
    {
        // Mover el jugador según el input del usuario
        if (moveDirection != Vector3.zero)
        {
            Vector3 movement = moveDirection * currentSpeed * Time.deltaTime;
            transform.position += movement;
            
            isMoving = true;
            
            // Rotar hacia la dirección de movimiento
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(moveDirection),
                Time.deltaTime * 10f
            );
        }
        else
        {
            isMoving = false;
        }
    }
    
    void UpdateStamina()
    {
        if (isMoving)
        {
            ConsumeStamina();
        }
        else
        {
            RecoverStamina();
        }
        
        // Limitar stamina
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        
        // Efectos de stamina baja
        if (currentStamina < maxStamina * 0.3f)
        {
            HandleLowStamina();
        }
    }
    
    void ConsumeStamina()
    {
        float staminaCost = 0f;
        
        switch (currentMovement)
        {
            case MovementType.Walk:
                staminaCost = staminaDecayRate * 0.2f;
                break;
            case MovementType.Run:
                staminaCost = staminaDecayRate * 0.5f;
                break;
            case MovementType.Sprint:
                staminaCost = staminaDecayRate * 1f;
                break;
        }
        
        currentStamina -= staminaCost * Time.deltaTime;
        
        // Reproducir sonidos de esfuerzo
        if (currentMovement == MovementType.Sprint && Random.value < 0.01f)
        {
            PlayEffortSound();
        }
    }
    
    void RecoverStamina()
    {
        currentStamina += staminaRecoveryRate * Time.deltaTime;
    }
    
    void HandleLowStamina()
    {
        // Reducir velocidad cuando la stamina es baja
        float staminaRatio = currentStamina / maxStamina;
        navAgent.speed *= staminaRatio;
        
        // Activar efectos de cansancio
        if (sweatParticles != null && !sweatParticles.isPlaying)
        {
            sweatParticles.Play();
        }
        
        // Reproducir sonidos de respiración agitada
        if (Random.value < 0.05f)
        {
            PlayBreathingSound();
        }
    }
    
    void UpdateAnimations()
    {
        if (animator == null) return;
        
        // Actualizar parámetros de animación
        animator.SetBool("IsMoving", isMoving);
        animator.SetFloat("Speed", currentSpeed);
        animator.SetFloat("MovementType", (float)currentMovement);
        animator.SetBool("HasBall", hasBall);
        animator.SetFloat("Stamina", currentStamina / maxStamina);
        animator.SetBool("IsSelected", isSelected);
        
        // Parámetros específicos de estado
        animator.SetInteger("PlayerState", (int)currentState);
    }
    
    void UpdateVisualEffects()
    {
        // Partículas de polvo al correr
        if (runDustParticles != null)
        {
            if (isMoving && currentMovement >= MovementType.Run)
            {
                if (!runDustParticles.isPlaying)
                {
                    runDustParticles.Play();
                }
            }
            else
            {
                if (runDustParticles.isPlaying)
                {
                    runDustParticles.Stop();
                }
            }
        }
        
        // Indicador de selección
        if (selectionIndicator != null)
        {
            selectionIndicator.SetActive(isSelected);
        }
        
        // Indicador de control del balón
        if (ballControlIndicator != null)
        {
            ballControlIndicator.SetActive(hasBall);
        }
    }
    
    void CheckBallPossession()
    {
        if (ballController == null) return;
        
        float distanceToBall = Vector3.Distance(transform.position, ballController.transform.position);
        bool wasHasBall = hasBall;
        
        hasBall = distanceToBall <= ballControlRadius && ballController.GetCurrentSpeed() < 2f;
        
        // Eventos de posesión del balón
        if (hasBall && !wasHasBall)
        {
            OnBallPossessionGained();
        }
        else if (!hasBall && wasHasBall)
        {
            OnBallPossessionLost();
        }
    }
    
    void OnBallPossessionGained()
    {
        // Actualizar estado
        currentState = PlayerState.WithBall;
        
        // Notificar al sistema de IA
        if (playerAI != null)
        {
            playerAI.OnBallPossessionGained();
        }
        
        // Actualizar estadísticas
        if (playerStats != null)
        {
            playerStats.OnBallTouched();
        }
        
        // Efectos visuales
        ShowBallControlEffect();
    }
    
    void OnBallPossessionLost()
    {
        // Actualizar estado
        currentState = PlayerState.Idle;
        
        // Notificar al sistema de IA
        if (playerAI != null)
        {
            playerAI.OnBallPossessionLost();
        }
    }
    
    public void MovePlayer(Vector3 direction)
    {
        if (!isControlledByUser) return;
        
        moveDirection = direction.normalized;
        currentSpeed = GetSpeedBasedOnStamina();
        
        // Actualizar tipo de movimiento
        UpdateMovementType();
        
        // Desactivar NavMeshAgent temporalmente
        if (navAgent.enabled)
        {
            navAgent.enabled = false;
        }
    }
    
    public void StopMovement()
    {
        moveDirection = Vector3.zero;
        currentSpeed = 0f;
        
        // Reactivar NavMeshAgent
        if (!navAgent.enabled)
        {
            navAgent.enabled = true;
        }
    }
    
    public void SetDestination(Vector3 destination)
    {
        if (navAgent.enabled)
        {
            navAgent.SetDestination(destination);
            targetPosition = destination;
        }
    }
    
    public void SetUserControl(bool userControlled)
    {
        isControlledByUser = userControlled;
        
        if (userControlled)
        {
            // Desactivar IA
            if (playerAI != null)
            {
                playerAI.SetAIActive(false);
            }
            
            // Configurar para control manual
            if (navAgent.enabled)
            {
                navAgent.enabled = false;
            }
        }
        else
        {
            // Activar IA
            if (playerAI != null)
            {
                playerAI.SetAIActive(true);
            }
            
            // Reactivar NavMeshAgent
            if (!navAgent.enabled)
            {
                navAgent.enabled = true;
            }
        }
    }
    
    public void SelectPlayer()
    {
        isSelected = true;
        SetUserControl(true);
        
        // Efectos visuales de selección
        ShowSelectionEffect();
    }
    
    public void DeselectPlayer()
    {
        isSelected = false;
        SetUserControl(false);
        
        // Ocultar efectos de selección
        HideSelectionEffect();
    }
    
    public void PassBall(Vector3 direction, float force)
    {
        if (!hasBall || ballController == null) return;
        
        // Ejecutar pase
        ballController.KickBall(direction, force);
        
        // Actualizar estadísticas
        if (playerStats != null)
        {
            playerStats.OnPassAttempted();
        }
        
        // Animación de pase
        PlayPassAnimation();
        
        // Efectos de sonido
        PlayPassSound();
    }
    
    public void ShootBall(Vector3 direction, float force)
    {
        if (!hasBall || ballController == null) return;
        
        // Ejecutar disparo
        ballController.KickBall(direction, force * shootingPower);
        
        // Actualizar estadísticas
        if (playerStats != null)
        {
            playerStats.OnShotAttempted();
        }
        
        // Animación de disparo
        PlayShootAnimation();
        
        // Efectos de sonido
        PlayShootSound();
    }
    
    public void PlayTrickAnimation(string trickName)
    {
        if (animator == null) return;
        
        // Reproducir animación de truco
        animator.SetTrigger($"Play{trickName}");
        
        // Efectos especiales
        ShowTrickEffect(trickName);
    }
    
    float GetSpeedBasedOnStamina()
    {
        float staminaRatio = currentStamina / maxStamina;
        float baseSpeed = 0f;
        
        switch (currentMovement)
        {
            case MovementType.Walk:
                baseSpeed = walkSpeed;
                break;
            case MovementType.Run:
                baseSpeed = runSpeed;
                break;
            case MovementType.Sprint:
                baseSpeed = sprintSpeed;
                break;
        }
        
        return baseSpeed * staminaRatio;
    }
    
    void CacheTeammates()
    {
        List<GameObject> teammatesList = new List<GameObject>();
        PlayerController[] allPlayers = FindObjectsOfType<PlayerController>();
        
        foreach (PlayerController player in allPlayers)
        {
            if (player != this && player.team == this.team)
            {
                teammatesList.Add(player.gameObject);
            }
        }
        
        teammates = teammatesList.ToArray();
    }
    
    void CacheOpponents()
    {
        List<GameObject> opponentsList = new List<GameObject>();
        PlayerController[] allPlayers = FindObjectsOfType<PlayerController>();
        
        foreach (PlayerController player in allPlayers)
        {
            if (player != this && player.team != this.team)
            {
                opponentsList.Add(player.gameObject);
            }
        }
        
        opponents = opponentsList.ToArray();
    }
    
    IEnumerator UpdateLOD()
    {
        while (true)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                float distanceToCamera = Vector3.Distance(transform.position, mainCamera.transform.position);
                
                // Ajustar calidad según la distancia
                if (distanceToCamera > 30f)
                {
                    // LOD bajo
                    aiUpdateInterval = 0.2f;
                    animationUpdateInterval = 0.1f;
                }
                else if (distanceToCamera > 15f)
                {
                    // LOD medio
                    aiUpdateInterval = 0.1f;
                    animationUpdateInterval = 0.05f;
                }
                else
                {
                    // LOD alto
                    aiUpdateInterval = 0.05f;
                    animationUpdateInterval = 0.033f;
                }
            }
            
            yield return new WaitForSeconds(1f);
        }
    }
    
    void ShowSelectionEffect()
    {
        // Efectos visuales de selección
        if (selectionIndicator != null)
        {
            selectionIndicator.SetActive(true);
        }
    }
    
    void HideSelectionEffect()
    {
        if (selectionIndicator != null)
        {
            selectionIndicator.SetActive(false);
        }
    }
    
    void ShowBallControlEffect()
    {
        // Efectos visuales cuando se controla el balón
        if (ballControlIndicator != null)
        {
            ballControlIndicator.SetActive(true);
        }
    }
    
    void ShowTrickEffect(string trickName)
    {
        // Efectos específicos para cada truco
        // Aquí se pueden agregar efectos de partículas específicos
    }
    
    void PlayPassAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Pass");
        }
    }
    
    void PlayShootAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Shoot");
        }
    }
    
    void PlayPassSound()
    {
        // Reproducir sonido de pase
        if (playerAudioSource != null && runSounds.Length > 0)
        {
            playerAudioSource.PlayOneShot(runSounds[0]);
        }
    }
    
    void PlayShootSound()
    {
        // Reproducir sonido de disparo
        if (playerAudioSource != null && effortSounds.Length > 0)
        {
            playerAudioSource.PlayOneShot(effortSounds[0]);
        }
    }
    
    void PlayEffortSound()
    {
        if (effortSounds.Length > 0 && playerAudioSource != null)
        {
            AudioClip randomEffort = effortSounds[Random.Range(0, effortSounds.Length)];
            playerAudioSource.PlayOneShot(randomEffort);
        }
    }
    
    void PlayBreathingSound()
    {
        if (breathingSounds.Length > 0 && playerAudioSource != null)
        {
            AudioClip randomBreathing = breathingSounds[Random.Range(0, breathingSounds.Length)];
            playerAudioSource.PlayOneShot(randomBreathing);
        }
    }
    
    PlayerData CreateDefaultPlayerData()
    {
        return new PlayerData
        {
            playerName = "Player " + Random.Range(1, 1000),
            overall = Random.Range(60, 85),
            pace = Random.Range(50, 90),
            shooting = Random.Range(40, 85),
            passing = Random.Range(45, 90),
            dribbling = Random.Range(40, 85),
            defending = Random.Range(30, 85),
            physical = Random.Range(45, 85)
        };
    }
    
    // Métodos públicos para otros sistemas
    public bool HasBall() => hasBall;
    public bool IsSelected() => isSelected;
    public bool IsControlledByUser() => isControlledByUser;
    public float GetCurrentStamina() => currentStamina;
    public float GetStaminaPercentage() => currentStamina / maxStamina;
    public PlayerState GetCurrentState() => currentState;
    public MovementType GetMovementType() => currentMovement;
    public Vector3 GetMoveDirection() => moveDirection;
    public float GetCurrentSpeed() => currentSpeed;
    public GameObject[] GetTeammates() => teammates;
    public GameObject[] GetOpponents() => opponents;
    public PlayerData GetPlayerData() => playerData;
    public Vector3 GetTargetPosition() => targetPosition;
    
    public void SetPlayerData(PlayerData data)
    {
        playerData = data;
        
        // Actualizar estadísticas basadas en los datos
        UpdateStatsFromData();
    }
    
    void UpdateStatsFromData()
    {
        if (playerData == null) return;
        
        // Actualizar velocidades basadas en pace
        float paceModifier = playerData.pace / 100f;
        walkSpeed *= paceModifier;
        runSpeed *= paceModifier;
        sprintSpeed *= paceModifier;
        
        // Actualizar otras habilidades
        passingAccuracy = playerData.passing / 10f;
        shootingPower = playerData.shooting / 10f;
        dribbleSpeed = playerData.dribbling / 50f;
        
        // Actualizar stamina basada en physical
        maxStamina = 50f + (playerData.physical / 2f);
        currentStamina = maxStamina;
    }
    
    public void ResetPlayer()
    {
        currentState = PlayerState.Idle;
        currentMovement = MovementType.Walk;
        currentStamina = maxStamina;
        hasBall = false;
        isSelected = false;
        isControlledByUser = false;
        moveDirection = Vector3.zero;
        currentSpeed = 0f;
        
        if (navAgent.enabled)
        {
            navAgent.ResetPath();
        }
        
        HideSelectionEffect();
    }
}

[System.Serializable]
public enum PlayerState
{
    Idle,
    Moving,
    WithBall,
    Passing,
    Shooting,
    Defending,
    Celebrating,
    Tired
}

[System.Serializable]
public enum MovementType
{
    Walk,
    Run,
    Sprint
}

[System.Serializable]
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
public enum PlayerTeam
{
    Home,
    Away
}