using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Sistema de IA avanzado para jugadores de fútbol
/// Comportamientos inteligentes y realistas optimizados para móviles
/// </summary>
public class PlayerAI : MonoBehaviour
{
    [Header("🎯 Configuración de IA")]
    [Range(0.1f, 3f)]
    public float reactionTime = 0.5f;
    [Range(1f, 30f)]
    public float visionRange = 20f;
    [Range(0.1f, 2f)]
    public float decisionMakingSpeed = 1f;
    [Range(0.1f, 1f)]
    public float errorRate = 0.1f;
    
    [Header("⚽ Configuración de Comportamiento")]
    [Range(0.1f, 2f)]
    public float aggressiveness = 1f;
    [Range(0.1f, 2f)]
    public float creativity = 1f;
    [Range(0.1f, 2f)]
    public float teamwork = 1f;
    [Range(0.1f, 2f)]
    public float riskTaking = 1f;
    
    [Header("🏃 Configuración de Movimiento")]
    [Range(1f, 20f)]
    public float minDistanceToTarget = 2f;
    [Range(1f, 5f)]
    public float formationTolerance = 3f;
    [Range(0.1f, 2f)]
    public float positioningAccuracy = 1f;
    
    [Header("🎮 Estados de IA")]
    public AIState currentState = AIState.Idle;
    public AIBehaviorType behaviorType = AIBehaviorType.Balanced;
    
    [Header("📱 Optimización")]
    [Range(0.05f, 0.5f)]
    public float aiUpdateRate = 0.1f;
    [Range(0.1f, 1f)]
    public float stateChangeInterval = 0.3f;
    
    // Componentes y referencias
    private PlayerController playerController;
    private NavMeshAgent navAgent;
    private BallController ballController;
    private GameManager gameManager;
    private FormationManager formationManager;
    
    // Estado interno
    private AIState previousState;
    private Vector3 targetPosition;
    private GameObject targetObject;
    private float lastDecisionTime;
    private float lastStateChangeTime;
    private bool isAIActive = true;
    
    // Información del entorno
    private GameObject[] teammates;
    private GameObject[] opponents;
    private GameObject nearestOpponent;
    private GameObject nearestTeammate;
    private Vector3 ballPosition;
    private float distanceToBall;
    
    // Optimización para móviles
    private float nextUpdateTime = 0f;
    private int aiFrame = 0;
    private bool isLowPriorityPlayer = false;
    
    // Memoria de decisiones
    private List<AIDecision> recentDecisions = new List<AIDecision>();
    private const int MAX_DECISION_HISTORY = 10;
    
    void Start()
    {
        InitializeAI();
        CacheComponents();
        SetupOptimizationSettings();
    }
    
    void InitializeAI()
    {
        currentState = AIState.Idle;
        previousState = AIState.Idle;
        targetPosition = transform.position;
        lastDecisionTime = Time.time;
        lastStateChangeTime = Time.time;
        
        // Configurar comportamiento según la posición del jugador
        ConfigureBehaviorForPosition();
    }
    
    void CacheComponents()
    {
        playerController = GetComponent<PlayerController>();
        navAgent = GetComponent<NavMeshAgent>();
        ballController = FindObjectOfType<BallController>();
        gameManager = FindObjectOfType<GameManager>();
        formationManager = FindObjectOfType<FormationManager>();
        
        // Cachear compañeros y oponentes
        UpdateTeamReferences();
    }
    
    void SetupOptimizationSettings()
    {
        // Optimización para dispositivos de gama baja
        if (SystemInfo.systemMemorySize <= 4096) // 4GB RAM o menos
        {
            aiUpdateRate = 0.2f;
            visionRange = 15f;
            stateChangeInterval = 0.5f;
        }
        
        // Determinar prioridad del jugador
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            float distanceToCamera = Vector3.Distance(transform.position, mainCamera.transform.position);
            isLowPriorityPlayer = distanceToCamera > 25f;
        }
    }
    
    void Update()
    {
        if (!isAIActive) return;
        
        if (Time.time >= nextUpdateTime)
        {
            UpdateAI();
            nextUpdateTime = Time.time + (isLowPriorityPlayer ? aiUpdateRate * 2f : aiUpdateRate);
        }
    }
    
    public void UpdateAI()
    {
        aiFrame++;
        
        // Actualizar información del entorno
        UpdateEnvironmentInfo();
        
        // Tomar decisiones
        MakeDecisions();
        
        // Ejecutar comportamiento actual
        ExecuteCurrentBehavior();
        
        // Limpiar memoria antigua
        if (aiFrame % 100 == 0)
        {
            CleanupDecisionHistory();
        }
    }
    
    void UpdateEnvironmentInfo()
    {
        // Actualizar posición del balón
        if (ballController != null)
        {
            ballPosition = ballController.transform.position;
            distanceToBall = Vector3.Distance(transform.position, ballPosition);
        }
        
        // Encontrar oponente más cercano
        UpdateNearestOpponent();
        
        // Encontrar compañero más cercano
        UpdateNearestTeammate();
    }
    
    void UpdateNearestOpponent()
    {
        if (opponents == null || opponents.Length == 0) return;
        
        float nearestDistance = float.MaxValue;
        nearestOpponent = null;
        
        foreach (GameObject opponent in opponents)
        {
            if (opponent == null) continue;
            
            float distance = Vector3.Distance(transform.position, opponent.transform.position);
            if (distance < nearestDistance && distance < visionRange)
            {
                nearestDistance = distance;
                nearestOpponent = opponent;
            }
        }
    }
    
    void UpdateNearestTeammate()
    {
        if (teammates == null || teammates.Length == 0) return;
        
        float nearestDistance = float.MaxValue;
        nearestTeammate = null;
        
        foreach (GameObject teammate in teammates)
        {
            if (teammate == null || teammate == gameObject) continue;
            
            float distance = Vector3.Distance(transform.position, teammate.transform.position);
            if (distance < nearestDistance && distance < visionRange)
            {
                nearestDistance = distance;
                nearestTeammate = teammate;
            }
        }
    }
    
    void MakeDecisions()
    {
        // Evitar cambios de estado muy frecuentes
        if (Time.time - lastStateChangeTime < stateChangeInterval) return;
        
        AIState newState = DetermineOptimalState();
        
        if (newState != currentState)
        {
            ChangeState(newState);
        }
        
        lastDecisionTime = Time.time;
    }
    
    AIState DetermineOptimalState()
    {
        // Prioridad 1: Si tengo el balón
        if (playerController.HasBall())
        {
            return DetermineStateWithBall();
        }
        
        // Prioridad 2: Si mi equipo tiene el balón
        if (IsTeammatePossessing())
        {
            return DetermineStateWhenTeamHasBall();
        }
        
        // Prioridad 3: Si el oponente tiene el balón
        if (IsOpponentPossessing())
        {
            return DetermineStateWhenOpponentHasBall();
        }
        
        // Prioridad 4: Balón libre
        return DetermineStateWhenBallIsFree();
    }
    
    AIState DetermineStateWithBall()
    {
        // Evaluar opciones con el balón
        if (ShouldShoot())
        {
            return AIState.Shooting;
        }
        
        if (ShouldDribble())
        {
            return AIState.Dribbling;
        }
        
        if (ShouldPass())
        {
            return AIState.Passing;
        }
        
        return AIState.WithBall;
    }
    
    AIState DetermineStateWhenTeamHasBall()
    {
        // Evaluar si debo apoyar el ataque
        if (ShouldSupportAttack())
        {
            return AIState.SupportingAttack;
        }
        
        // Mantener posición o moverse a espacio libre
        if (ShouldMoveToSpace())
        {
            return AIState.MovingToSpace;
        }
        
        return AIState.Positioning;
    }
    
    AIState DetermineStateWhenOpponentHasBall()
    {
        // Evaluar si debo presionar
        if (ShouldPress())
        {
            return AIState.Pressing;
        }
        
        // Evaluar si debo marcar
        if (ShouldMark())
        {
            return AIState.Marking;
        }
        
        return AIState.Defending;
    }
    
    AIState DetermineStateWhenBallIsFree()
    {
        // Evaluar si debo perseguir el balón
        if (ShouldChaseBall())
        {
            return AIState.ChasingBall;
        }
        
        return AIState.Positioning;
    }
    
    void ChangeState(AIState newState)
    {
        previousState = currentState;
        currentState = newState;
        lastStateChangeTime = Time.time;
        
        // Registrar decisión
        RecordDecision(newState, GetStateReason(newState));
        
        // Inicializar nuevo estado
        InitializeState(newState);
    }
    
    void InitializeState(AIState state)
    {
        switch (state)
        {
            case AIState.ChasingBall:
                targetPosition = ballPosition;
                break;
                
            case AIState.Pressing:
                if (nearestOpponent != null)
                {
                    targetPosition = nearestOpponent.transform.position;
                }
                break;
                
            case AIState.Marking:
                if (nearestOpponent != null)
                {
                    targetPosition = GetMarkingPosition(nearestOpponent);
                }
                break;
                
            case AIState.Positioning:
                targetPosition = GetFormationPosition();
                break;
                
            case AIState.MovingToSpace:
                targetPosition = FindBestSpace();
                break;
                
            case AIState.SupportingAttack:
                targetPosition = GetSupportPosition();
                break;
        }
    }
    
    void ExecuteCurrentBehavior()
    {
        switch (currentState)
        {
            case AIState.Idle:
                ExecuteIdleBehavior();
                break;
                
            case AIState.ChasingBall:
                ExecuteChasingBehavior();
                break;
                
            case AIState.WithBall:
                ExecuteWithBallBehavior();
                break;
                
            case AIState.Passing:
                ExecutePassingBehavior();
                break;
                
            case AIState.Shooting:
                ExecuteShootingBehavior();
                break;
                
            case AIState.Dribbling:
                ExecuteDribblingBehavior();
                break;
                
            case AIState.Defending:
                ExecuteDefendingBehavior();
                break;
                
            case AIState.Pressing:
                ExecutePressingBehavior();
                break;
                
            case AIState.Marking:
                ExecuteMarkingBehavior();
                break;
                
            case AIState.Positioning:
                ExecutePositioningBehavior();
                break;
                
            case AIState.MovingToSpace:
                ExecuteMovingToSpaceBehavior();
                break;
                
            case AIState.SupportingAttack:
                ExecuteSupportingAttackBehavior();
                break;
        }
    }
    
    void ExecuteIdleBehavior()
    {
        // Mantener posición de formación
        Vector3 formationPos = GetFormationPosition();
        if (Vector3.Distance(transform.position, formationPos) > formationTolerance)
        {
            SetDestination(formationPos);
        }
    }
    
    void ExecuteChasingBehavior()
    {
        // Perseguir el balón
        if (ballController != null)
        {
            Vector3 ballPos = ballController.transform.position;
            Vector3 interceptPos = CalculateInterceptPosition(ballPos, ballController.GetComponent<Rigidbody>().velocity);
            SetDestination(interceptPos);
        }
    }
    
    void ExecuteWithBallBehavior()
    {
        // Mantener posesión mientras evalúo opciones
        if (nearestOpponent != null)
        {
            float distanceToOpponent = Vector3.Distance(transform.position, nearestOpponent.transform.position);
            if (distanceToOpponent < 3f)
            {
                // Alejarme del oponente
                Vector3 escapeDirection = (transform.position - nearestOpponent.transform.position).normalized;
                SetDestination(transform.position + escapeDirection * 2f);
            }
        }
    }
    
    void ExecutePassingBehavior()
    {
        // Encontrar el mejor pase
        GameObject bestPassTarget = FindBestPassTarget();
        if (bestPassTarget != null)
        {
            Vector3 passDirection = (bestPassTarget.transform.position - transform.position).normalized;
            float passForce = CalculatePassForce(bestPassTarget);
            
            // Ejecutar pase con posibilidad de error
            if (Random.value > errorRate)
            {
                playerController.PassBall(passDirection, passForce);
            }
            else
            {
                // Pase con error
                Vector3 errorDirection = passDirection + Random.insideUnitSphere * 0.3f;
                playerController.PassBall(errorDirection, passForce);
            }
        }
    }
    
    void ExecuteShootingBehavior()
    {
        // Disparar a la portería
        Vector3 goalPosition = GetOpponentGoalPosition();
        Vector3 shootDirection = (goalPosition - transform.position).normalized;
        
        // Añadir variación para apuntar a las esquinas
        shootDirection += Random.insideUnitSphere * 0.2f;
        
        float shootForce = CalculateShootForce();
        
        // Ejecutar disparo con posibilidad de error
        if (Random.value > errorRate * 0.5f) // Disparos más precisos
        {
            playerController.ShootBall(shootDirection, shootForce);
        }
        else
        {
            // Disparo con error
            Vector3 errorDirection = shootDirection + Random.insideUnitSphere * 0.5f;
            playerController.ShootBall(errorDirection, shootForce);
        }
    }
    
    void ExecuteDribblingBehavior()
    {
        // Driblar hacia adelante evitando oponentes
        Vector3 dribbleDirection = GetDribbleDirection();
        SetDestination(transform.position + dribbleDirection * 3f);
    }
    
    void ExecuteDefendingBehavior()
    {
        // Mantener posición defensiva
        Vector3 defensivePosition = GetDefensivePosition();
        SetDestination(defensivePosition);
    }
    
    void ExecutePressingBehavior()
    {
        // Presionar al oponente con el balón
        if (nearestOpponent != null)
        {
            Vector3 pressDirection = (nearestOpponent.transform.position - transform.position).normalized;
            SetDestination(nearestOpponent.transform.position - pressDirection * 1.5f);
        }
    }
    
    void ExecuteMarkingBehavior()
    {
        // Marcar al oponente más cercano
        if (nearestOpponent != null)
        {
            Vector3 markingPos = GetMarkingPosition(nearestOpponent);
            SetDestination(markingPos);
        }
    }
    
    void ExecutePositioningBehavior()
    {
        // Mantener posición de formación
        Vector3 formationPos = GetFormationPosition();
        SetDestination(formationPos);
    }
    
    void ExecuteMovingToSpaceBehavior()
    {
        // Moverse a espacio libre
        Vector3 spacePosition = FindBestSpace();
        SetDestination(spacePosition);
    }
    
    void ExecuteSupportingAttackBehavior()
    {
        // Apoyar el ataque
        Vector3 supportPos = GetSupportPosition();
        SetDestination(supportPos);
    }
    
    // Funciones de evaluación
    bool ShouldShoot()
    {
        if (distanceToBall > 1.5f) return false;
        
        Vector3 goalPosition = GetOpponentGoalPosition();
        float distanceToGoal = Vector3.Distance(transform.position, goalPosition);
        
        // Disparar si estoy cerca de la portería y tengo ángulo
        return distanceToGoal < 20f && HasClearShotAngle(goalPosition);
    }
    
    bool ShouldDribble()
    {
        // Driblar si soy creativo y no hay muchos oponentes cerca
        return creativity > 1.2f && CountNearbyOpponents(5f) <= 1;
    }
    
    bool ShouldPass()
    {
        // Pase si hay un compañero bien posicionado
        GameObject passTarget = FindBestPassTarget();
        return passTarget != null && Vector3.Distance(transform.position, passTarget.transform.position) < 15f;
    }
    
    bool ShouldSupportAttack()
    {
        // Apoyar si soy ofensivo y mi equipo tiene el balón
        return playerController.GetPlayerData().attacking > 70 && IsTeammatePossessing();
    }
    
    bool ShouldMoveToSpace()
    {
        // Buscar espacio si no estoy bien posicionado
        return Vector3.Distance(transform.position, FindBestSpace()) > 5f;
    }
    
    bool ShouldPress()
    {
        // Presionar si soy agresivo y el oponente está cerca
        return aggressiveness > 1.0f && nearestOpponent != null && 
               Vector3.Distance(transform.position, nearestOpponent.transform.position) < 8f;
    }
    
    bool ShouldMark()
    {
        // Marcar si hay un oponente peligroso cerca
        return nearestOpponent != null && 
               Vector3.Distance(transform.position, nearestOpponent.transform.position) < 10f;
    }
    
    bool ShouldChaseBall()
    {
        // Perseguir si soy el más cercano al balón
        return distanceToBall < 10f && IsClosestToBall();
    }
    
    // Funciones de utilidad
    Vector3 GetFormationPosition()
    {
        if (formationManager != null)
        {
            return formationManager.GetPlayerPosition(gameObject);
        }
        
        // Posición por defecto basada en la posición del jugador
        PlayerPosition position = playerController.GetPlayerData().preferredPosition;
        return GetDefaultPositionForRole(position);
    }
    
    Vector3 GetDefaultPositionForRole(PlayerPosition position)
    {
        // Posiciones por defecto en el campo
        switch (position)
        {
            case PlayerPosition.Goalkeeper:
                return new Vector3(0, 0, -45);
            case PlayerPosition.CenterBack:
                return new Vector3(0, 0, -30);
            case PlayerPosition.LeftBack:
                return new Vector3(-15, 0, -30);
            case PlayerPosition.RightBack:
                return new Vector3(15, 0, -30);
            case PlayerPosition.DefensiveMidfield:
                return new Vector3(0, 0, -15);
            case PlayerPosition.CentralMidfield:
                return new Vector3(0, 0, 0);
            case PlayerPosition.AttackingMidfield:
                return new Vector3(0, 0, 15);
            case PlayerPosition.LeftWing:
                return new Vector3(-20, 0, 20);
            case PlayerPosition.RightWing:
                return new Vector3(20, 0, 20);
            case PlayerPosition.Striker:
                return new Vector3(0, 0, 35);
            default:
                return transform.position;
        }
    }
    
    Vector3 GetOpponentGoalPosition()
    {
        // Posición de la portería oponente
        return new Vector3(0, 0, 45); // Ajustar según el tamaño del campo
    }
    
    Vector3 CalculateInterceptPosition(Vector3 ballPos, Vector3 ballVelocity)
    {
        // Calcular posición de intercepción
        float timeToIntercept = Vector3.Distance(transform.position, ballPos) / navAgent.speed;
        return ballPos + ballVelocity * timeToIntercept;
    }
    
    Vector3 GetMarkingPosition(GameObject opponent)
    {
        // Posición entre el oponente y mi portería
        Vector3 myGoal = new Vector3(0, 0, -45);
        Vector3 directionToGoal = (myGoal - opponent.transform.position).normalized;
        return opponent.transform.position + directionToGoal * 2f;
    }
    
    Vector3 FindBestSpace()
    {
        // Encontrar el espacio libre más cercano
        Vector3 bestSpace = transform.position;
        float bestScore = 0f;
        
        // Evaluar posiciones en un radio
        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f * Mathf.Deg2Rad;
            Vector3 testPos = transform.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * 5f;
            
            float score = EvaluatePositionScore(testPos);
            if (score > bestScore)
            {
                bestScore = score;
                bestSpace = testPos;
            }
        }
        
        return bestSpace;
    }
    
    float EvaluatePositionScore(Vector3 position)
    {
        float score = 0f;
        
        // Bonus por estar cerca del balón
        if (ballController != null)
        {
            float distanceToBall = Vector3.Distance(position, ballController.transform.position);
            score += Mathf.Max(0, 20f - distanceToBall);
        }
        
        // Penalty por estar cerca de oponentes
        foreach (GameObject opponent in opponents)
        {
            if (opponent != null)
            {
                float distanceToOpponent = Vector3.Distance(position, opponent.transform.position);
                score -= Mathf.Max(0, 10f - distanceToOpponent);
            }
        }
        
        return score;
    }
    
    Vector3 GetSupportPosition()
    {
        // Posición de apoyo ofensivo
        if (ballController != null)
        {
            Vector3 ballPos = ballController.transform.position;
            Vector3 goalPos = GetOpponentGoalPosition();
            
            // Posición lateral al balón, hacia la portería
            Vector3 supportDirection = (goalPos - ballPos).normalized;
            Vector3 lateralDirection = Vector3.Cross(supportDirection, Vector3.up).normalized;
            
            return ballPos + supportDirection * 5f + lateralDirection * 3f;
        }
        
        return transform.position;
    }
    
    Vector3 GetDefensivePosition()
    {
        // Posición defensiva
        Vector3 myGoal = new Vector3(0, 0, -45);
        Vector3 ballPos = ballController != null ? ballController.transform.position : transform.position;
        
        // Posición entre el balón y mi portería
        Vector3 directionToGoal = (myGoal - ballPos).normalized;
        return ballPos + directionToGoal * 10f;
    }
    
    Vector3 GetDribbleDirection()
    {
        // Dirección de dribbling evitando oponentes
        Vector3 goalDirection = (GetOpponentGoalPosition() - transform.position).normalized;
        
        // Evitar oponentes cercanos
        foreach (GameObject opponent in opponents)
        {
            if (opponent != null)
            {
                float distance = Vector3.Distance(transform.position, opponent.transform.position);
                if (distance < 5f)
                {
                    Vector3 avoidDirection = (transform.position - opponent.transform.position).normalized;
                    goalDirection += avoidDirection * (5f - distance) * 0.5f;
                }
            }
        }
        
        return goalDirection.normalized;
    }
    
    GameObject FindBestPassTarget()
    {
        GameObject bestTarget = null;
        float bestScore = 0f;
        
        foreach (GameObject teammate in teammates)
        {
            if (teammate == null || teammate == gameObject) continue;
            
            float score = EvaluatePassTarget(teammate);
            if (score > bestScore)
            {
                bestScore = score;
                bestTarget = teammate;
            }
        }
        
        return bestTarget;
    }
    
    float EvaluatePassTarget(GameObject target)
    {
        float score = 0f;
        
        // Distancia al objetivo
        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance > 25f) return 0f; // Muy lejos
        
        score += Mathf.Max(0, 25f - distance);
        
        // Bonus si está más cerca de la portería
        float distanceToGoal = Vector3.Distance(target.transform.position, GetOpponentGoalPosition());
        score += Mathf.Max(0, 50f - distanceToGoal);
        
        // Penalty si hay oponentes bloqueando el pase
        if (IsPassBlocked(target))
        {
            score *= 0.3f;
        }
        
        return score;
    }
    
    bool IsPassBlocked(GameObject target)
    {
        Vector3 passDirection = (target.transform.position - transform.position).normalized;
        float passDistance = Vector3.Distance(transform.position, target.transform.position);
        
        foreach (GameObject opponent in opponents)
        {
            if (opponent == null) continue;
            
            // Verificar si el oponente está en la línea de pase
            Vector3 toOpponent = opponent.transform.position - transform.position;
            float projectionLength = Vector3.Dot(toOpponent, passDirection);
            
            if (projectionLength > 0 && projectionLength < passDistance)
            {
                Vector3 projection = transform.position + passDirection * projectionLength;
                float distanceToLine = Vector3.Distance(opponent.transform.position, projection);
                
                if (distanceToLine < 2f) // Oponente está bloqueando
                {
                    return true;
                }
            }
        }
        
        return false;
    }
    
    float CalculatePassForce(GameObject target)
    {
        float distance = Vector3.Distance(transform.position, target.transform.position);
        return Mathf.Clamp(distance * 0.5f, 5f, 25f);
    }
    
    float CalculateShootForce()
    {
        float distanceToGoal = Vector3.Distance(transform.position, GetOpponentGoalPosition());
        return Mathf.Clamp(distanceToGoal * 0.7f, 10f, 35f);
    }
    
    bool HasClearShotAngle(Vector3 goalPosition)
    {
        Vector3 shootDirection = (goalPosition - transform.position).normalized;
        float shootDistance = Vector3.Distance(transform.position, goalPosition);
        
        foreach (GameObject opponent in opponents)
        {
            if (opponent == null) continue;
            
            Vector3 toOpponent = opponent.transform.position - transform.position;
            float projectionLength = Vector3.Dot(toOpponent, shootDirection);
            
            if (projectionLength > 0 && projectionLength < shootDistance)
            {
                Vector3 projection = transform.position + shootDirection * projectionLength;
                float distanceToLine = Vector3.Distance(opponent.transform.position, projection);
                
                if (distanceToLine < 3f) // Oponente está bloqueando el disparo
                {
                    return false;
                }
            }
        }
        
        return true;
    }
    
    int CountNearbyOpponents(float radius)
    {
        int count = 0;
        foreach (GameObject opponent in opponents)
        {
            if (opponent != null && Vector3.Distance(transform.position, opponent.transform.position) < radius)
            {
                count++;
            }
        }
        return count;
    }
    
    bool IsTeammatePossessing()
    {
        foreach (GameObject teammate in teammates)
        {
            if (teammate != null)
            {
                PlayerController pc = teammate.GetComponent<PlayerController>();
                if (pc != null && pc.HasBall())
                {
                    return true;
                }
            }
        }
        return false;
    }
    
    bool IsOpponentPossessing()
    {
        foreach (GameObject opponent in opponents)
        {
            if (opponent != null)
            {
                PlayerController pc = opponent.GetComponent<PlayerController>();
                if (pc != null && pc.HasBall())
                {
                    return true;
                }
            }
        }
        return false;
    }
    
    bool IsClosestToBall()
    {
        if (ballController == null) return false;
        
        float myDistance = Vector3.Distance(transform.position, ballController.transform.position);
        
        // Verificar compañeros de equipo
        foreach (GameObject teammate in teammates)
        {
            if (teammate != null)
            {
                float teammateDistance = Vector3.Distance(teammate.transform.position, ballController.transform.position);
                if (teammateDistance < myDistance)
                {
                    return false;
                }
            }
        }
        
        return true;
    }
    
    void SetDestination(Vector3 destination)
    {
        if (navAgent != null && navAgent.enabled)
        {
            navAgent.SetDestination(destination);
        }
    }
    
    void ConfigureBehaviorForPosition()
    {
        PlayerPosition position = playerController.GetPlayerData().preferredPosition;
        
        switch (position)
        {
            case PlayerPosition.Goalkeeper:
                behaviorType = AIBehaviorType.Defensive;
                aggressiveness = 0.3f;
                creativity = 0.4f;
                teamwork = 0.8f;
                riskTaking = 0.2f;
                break;
                
            case PlayerPosition.CenterBack:
                behaviorType = AIBehaviorType.Defensive;
                aggressiveness = 0.7f;
                creativity = 0.5f;
                teamwork = 0.9f;
                riskTaking = 0.3f;
                break;
                
            case PlayerPosition.CentralMidfield:
                behaviorType = AIBehaviorType.Balanced;
                aggressiveness = 0.6f;
                creativity = 0.8f;
                teamwork = 1.0f;
                riskTaking = 0.5f;
                break;
                
            case PlayerPosition.Striker:
                behaviorType = AIBehaviorType.Attacking;
                aggressiveness = 0.8f;
                creativity = 0.9f;
                teamwork = 0.7f;
                riskTaking = 0.8f;
                break;
                
            default:
                behaviorType = AIBehaviorType.Balanced;
                break;
        }
    }
    
    void UpdateTeamReferences()
    {
        teammates = playerController.GetTeammates();
        opponents = playerController.GetOpponents();
    }
    
    void RecordDecision(AIState state, string reason)
    {
        AIDecision decision = new AIDecision
        {
            state = state,
            reason = reason,
            timestamp = Time.time,
            position = transform.position
        };
        
        recentDecisions.Add(decision);
        
        if (recentDecisions.Count > MAX_DECISION_HISTORY)
        {
            recentDecisions.RemoveAt(0);
        }
    }
    
    string GetStateReason(AIState state)
    {
        switch (state)
        {
            case AIState.ChasingBall:
                return "Ball is free and I'm closest";
            case AIState.Shooting:
                return "Clear shot opportunity";
            case AIState.Passing:
                return "Teammate in good position";
            case AIState.Dribbling:
                return "Space available to dribble";
            case AIState.Pressing:
                return "Opponent has ball, applying pressure";
            case AIState.Marking:
                return "Marking nearest opponent";
            case AIState.Positioning:
                return "Maintaining formation";
            default:
                return "Default behavior";
        }
    }
    
    void CleanupDecisionHistory()
    {
        float currentTime = Time.time;
        for (int i = recentDecisions.Count - 1; i >= 0; i--)
        {
            if (currentTime - recentDecisions[i].timestamp > 60f) // Limpiar decisiones de hace más de 1 minuto
            {
                recentDecisions.RemoveAt(i);
            }
        }
    }
    
    // Eventos públicos
    public void OnBallPossessionGained()
    {
        // Cambiar inmediatamente a estado con balón
        ChangeState(AIState.WithBall);
    }
    
    public void OnBallPossessionLost()
    {
        // Cambiar a estado defensivo o de persecución
        ChangeState(AIState.Positioning);
    }
    
    public void SetAIActive(bool active)
    {
        isAIActive = active;
        
        if (!active)
        {
            // Detener movimiento automático
            if (navAgent != null && navAgent.enabled)
            {
                navAgent.ResetPath();
            }
        }
    }
    
    // Métodos públicos para otros sistemas
    public AIState GetCurrentState() => currentState;
    public AIBehaviorType GetBehaviorType() => behaviorType;
    public Vector3 GetTargetPosition() => targetPosition;
    public bool IsAIActive() => isAIActive;
    public List<AIDecision> GetRecentDecisions() => recentDecisions;
    
    public void SetBehaviorType(AIBehaviorType type)
    {
        behaviorType = type;
        
        // Ajustar parámetros según el tipo de comportamiento
        switch (type)
        {
            case AIBehaviorType.Defensive:
                aggressiveness = 0.5f;
                creativity = 0.4f;
                riskTaking = 0.3f;
                break;
                
            case AIBehaviorType.Balanced:
                aggressiveness = 0.7f;
                creativity = 0.7f;
                riskTaking = 0.5f;
                break;
                
            case AIBehaviorType.Attacking:
                aggressiveness = 0.9f;
                creativity = 0.9f;
                riskTaking = 0.8f;
                break;
        }
    }
}

[System.Serializable]
public enum AIState
{
    Idle,
    ChasingBall,
    WithBall,
    Passing,
    Shooting,
    Dribbling,
    Defending,
    Pressing,
    Marking,
    Positioning,
    MovingToSpace,
    SupportingAttack
}

[System.Serializable]
public enum AIBehaviorType
{
    Defensive,
    Balanced,
    Attacking
}

[System.Serializable]
public class AIDecision
{
    public AIState state;
    public string reason;
    public float timestamp;
    public Vector3 position;
}