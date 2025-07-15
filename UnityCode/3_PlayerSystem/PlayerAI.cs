using UnityEngine;
using UnityEngine.AI;

public class PlayerAI : MonoBehaviour
{
    [Header("AI Settings")]
    public float decisionInterval = 0.2f;
    public float reactionTime = 0.3f;
    public float awarenessRadius = 15f;
    public LayerMask playerLayer;
    public LayerMask ballLayer;
    
    private PlayerController playerController;
    private NavMeshAgent agent;
    private BallController ballController;
    private float lastDecisionTime;
    private AIState currentState;
    private Vector3 assignedPosition;
    private PlayerController targetPlayer;
    
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        agent = GetComponent<NavMeshAgent>();
        ballController = FindObjectOfType<BallController>();
        
        currentState = AIState.Positioning;
        assignedPosition = transform.position;
        
        // Ajustar posición asignada basada en la posición del jugador
        SetAssignedPosition();
    }
    
    void Update()
    {
        if (Time.time - lastDecisionTime >= decisionInterval)
        {
            MakeDecision();
            lastDecisionTime = Time.time;
        }
        
        ExecuteCurrentState();
    }
    
    void MakeDecision()
    {
        if (ballController == null) return;
        
        float distanceToBall = Vector3.Distance(transform.position, ballController.transform.position);
        bool hasBall = playerController.GetPlayerInfo().hasBall;
        
        if (hasBall)
        {
            DecideWithBall();
        }
        else
        {
            DecideWithoutBall(distanceToBall);
        }
    }
    
    void DecideWithBall()
    {
        // Buscar opciones de pase
        PlayerController bestPassOption = FindBestPassOption();
        
        if (bestPassOption != null)
        {
            float passQuality = EvaluatePassQuality(bestPassOption);
            
            if (passQuality > 0.7f)
            {
                currentState = AIState.Passing;
                targetPlayer = bestPassOption;
                return;
            }
        }
        
        // Evaluar oportunidad de disparo
        if (IsInShootingPosition())
        {
            currentState = AIState.Shooting;
            return;
        }
        
        // Evaluar regates
        if (ShouldDribble())
        {
            currentState = AIState.Dribbling;
            return;
        }
        
        // Por defecto, mantener el balón
        currentState = AIState.HoldingBall;
    }
    
    void DecideWithoutBall(float distanceToBall)
    {
        // Si el balón está cerca, intentar recuperarlo
        if (distanceToBall < 5f && !IsBallControlledByTeammate())
        {
            currentState = AIState.ChasingBall;
            return;
        }
        
        // Evaluar si debe presionar
        if (ShouldPress())
        {
            currentState = AIState.Pressing;
            return;
        }
        
        // Evaluar si debe marcar
        if (ShouldMark())
        {
            currentState = AIState.Marking;
            return;
        }
        
        // Evaluar si debe dar apoyo
        if (ShouldSupportTeammate())
        {
            currentState = AIState.Supporting;
            return;
        }
        
        // Por defecto, mantener posición
        currentState = AIState.Positioning;
    }
    
    void ExecuteCurrentState()
    {
        switch (currentState)
        {
            case AIState.Positioning:
                MoveToAssignedPosition();
                break;
            case AIState.ChasingBall:
                ChaseBall();
                break;
            case AIState.Pressing:
                PressOpponent();
                break;
            case AIState.Marking:
                MarkOpponent();
                break;
            case AIState.Supporting:
                SupportTeammate();
                break;
            case AIState.Passing:
                ExecutePass();
                break;
            case AIState.Shooting:
                ExecuteShoot();
                break;
            case AIState.Dribbling:
                ExecuteDribble();
                break;
            case AIState.HoldingBall:
                HoldBall();
                break;
        }
    }
    
    void MoveToAssignedPosition()
    {
        agent.SetDestination(assignedPosition);
    }
    
    void ChaseBall()
    {
        if (ballController != null)
        {
            agent.SetDestination(ballController.transform.position);
        }
    }
    
    void PressOpponent()
    {
        PlayerController nearestOpponent = FindNearestOpponent();
        if (nearestOpponent != null)
        {
            agent.SetDestination(nearestOpponent.transform.position);
        }
    }
    
    void MarkOpponent()
    {
        PlayerController opponentToMark = FindOpponentToMark();
        if (opponentToMark != null)
        {
            Vector3 markingPosition = CalculateMarkingPosition(opponentToMark);
            agent.SetDestination(markingPosition);
        }
    }
    
    void SupportTeammate()
    {
        Vector3 supportPosition = CalculateSupportPosition();
        agent.SetDestination(supportPosition);
    }
    
    void ExecutePass()
    {
        if (targetPlayer != null)
        {
            Vector3 passDirection = (targetPlayer.transform.position - transform.position).normalized;
            
            // Añadir predicción de movimiento
            Vector3 predictedPosition = PredictPlayerPosition(targetPlayer);
            passDirection = (predictedPosition - transform.position).normalized;
            
            float passDistance = Vector3.Distance(transform.position, predictedPosition);
            float power = Mathf.Clamp(passDistance / 20f, 0.2f, 1f);
            
            ballController.KickBall(passDirection, power);
            
            currentState = AIState.Positioning;
        }
    }
    
    void ExecuteShoot()
    {
        Vector3 goalPosition = FindGoalPosition();
        Vector3 shootDirection = (goalPosition - transform.position).normalized;
        
        // Añadir variación basada en la habilidad
        float accuracy = playerController.playerData.technique / 100f;
        shootDirection = AddShootingVariation(shootDirection, 1f - accuracy);
        
        float power = CalculateShootingPower();
        
        playerController.Shoot(shootDirection, power);
        
        currentState = AIState.Positioning;
    }
    
    void ExecuteDribble()
    {
        // Ejecutar regate basado en la situación
        TrickType trickType = ChooseTrickType();
        playerController.PerformTrick(trickType);
        
        // Después del regate, continuar con el balón
        currentState = AIState.HoldingBall;
    }
    
    void HoldBall()
    {
        // Mantener el balón y buscar opciones
        Vector3 safeDirection = FindSafeDirection();
        playerController.MovePlayer(safeDirection);
    }
    
    PlayerController FindBestPassOption()
    {
        PlayerController[] teammates = FindTeammates();
        PlayerController bestOption = null;
        float bestScore = 0f;
        
        foreach (PlayerController teammate in teammates)
        {
            float score = EvaluatePassQuality(teammate);
            if (score > bestScore)
            {
                bestScore = score;
                bestOption = teammate;
            }
        }
        
        return bestOption;
    }
    
    float EvaluatePassQuality(PlayerController teammate)
    {
        float distance = Vector3.Distance(transform.position, teammate.transform.position);
        float distanceScore = Mathf.Clamp(1f - (distance / 30f), 0f, 1f);
        
        // Evaluar si hay oponentes en el camino
        Vector3 passDirection = (teammate.transform.position - transform.position).normalized;
        bool clearPath = !Physics.Raycast(transform.position, passDirection, distance, playerLayer);
        float pathScore = clearPath ? 1f : 0.3f;
        
        // Evaluar posición del compañero
        float positionScore = EvaluatePlayerPosition(teammate);
        
        return (distanceScore * 0.3f + pathScore * 0.4f + positionScore * 0.3f);
    }
    
    bool IsInShootingPosition()
    {
        Vector3 goalPosition = FindGoalPosition();
        float distanceToGoal = Vector3.Distance(transform.position, goalPosition);
        
        // Evaluar si está en rango de disparo
        if (distanceToGoal > 25f) return false;
        
        // Evaluar ángulo de disparo
        Vector3 toGoal = (goalPosition - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, toGoal);
        
        return angle < 45f;
    }
    
    bool ShouldDribble()
    {
        // Evaluar si hay oponentes cerca
        PlayerController nearestOpponent = FindNearestOpponent();
        if (nearestOpponent == null) return false;
        
        float distanceToOpponent = Vector3.Distance(transform.position, nearestOpponent.transform.position);
        
        // Solo regatear si el oponente está cerca pero no demasiado
        return distanceToOpponent > 1f && distanceToOpponent < 3f;
    }
    
    bool IsBallControlledByTeammate()
    {
        PlayerController[] teammates = FindTeammates();
        
        foreach (PlayerController teammate in teammates)
        {
            if (teammate.GetPlayerInfo().hasBall)
            {
                return true;
            }
        }
        
        return false;
    }
    
    bool ShouldPress()
    {
        PlayerController ballCarrier = FindBallCarrier();
        if (ballCarrier == null) return false;
        
        // Solo presionar si el portador es oponente
        return ballCarrier.playerData.teamId != playerController.playerData.teamId;
    }
    
    bool ShouldMark()
    {
        PlayerController nearestOpponent = FindNearestOpponent();
        if (nearestOpponent == null) return false;
        
        float distance = Vector3.Distance(transform.position, nearestOpponent.transform.position);
        return distance < 10f;
    }
    
    bool ShouldSupportTeammate()
    {
        PlayerController ballCarrier = FindBallCarrier();
        if (ballCarrier == null) return false;
        
        // Solo dar apoyo si el portador es compañero
        return ballCarrier.playerData.teamId == playerController.playerData.teamId;
    }
    
    PlayerController[] FindTeammates()
    {
        PlayerController[] allPlayers = FindObjectsOfType<PlayerController>();
        System.Collections.Generic.List<PlayerController> teammates = new System.Collections.Generic.List<PlayerController>();
        
        foreach (PlayerController player in allPlayers)
        {
            if (player != this && player.playerData.teamId == playerController.playerData.teamId)
            {
                teammates.Add(player);
            }
        }
        
        return teammates.ToArray();
    }
    
    PlayerController FindNearestOpponent()
    {
        PlayerController[] allPlayers = FindObjectsOfType<PlayerController>();
        PlayerController nearest = null;
        float nearestDistance = float.MaxValue;
        
        foreach (PlayerController player in allPlayers)
        {
            if (player.playerData.teamId != playerController.playerData.teamId)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = player;
                }
            }
        }
        
        return nearest;
    }
    
    PlayerController FindBallCarrier()
    {
        PlayerController[] allPlayers = FindObjectsOfType<PlayerController>();
        
        foreach (PlayerController player in allPlayers)
        {
            if (player.GetPlayerInfo().hasBall)
            {
                return player;
            }
        }
        
        return null;
    }
    
    Vector3 FindGoalPosition()
    {
        // Buscar la portería oponente
        // Esto debería estar conectado con un sistema de porterías
        return new Vector3(0, 0, 50); // Placeholder
    }
    
    Vector3 PredictPlayerPosition(PlayerController player)
    {
        Vector3 currentPos = player.transform.position;
        Vector3 velocity = player.GetComponent<NavMeshAgent>().velocity;
        
        // Predecir posición basada en velocidad actual
        return currentPos + velocity * 0.5f;
    }
    
    Vector3 AddShootingVariation(Vector3 direction, float errorAmount)
    {
        Vector3 error = new Vector3(
            Random.Range(-errorAmount, errorAmount),
            Random.Range(-errorAmount * 0.5f, errorAmount * 0.5f),
            Random.Range(-errorAmount, errorAmount)
        );
        
        return (direction + error).normalized;
    }
    
    float CalculateShootingPower()
    {
        Vector3 goalPosition = FindGoalPosition();
        float distance = Vector3.Distance(transform.position, goalPosition);
        
        return Mathf.Clamp(distance / 25f, 0.5f, 1f);
    }
    
    TrickType ChooseTrickType()
    {
        // Elegir truco basado en la situación y habilidad del jugador
        int skillLevel = playerController.playerData.skillMoves;
        
        if (skillLevel >= 4)
        {
            return (TrickType)Random.Range(0, System.Enum.GetValues(typeof(TrickType)).Length);
        }
        else if (skillLevel >= 3)
        {
            return (TrickType)Random.Range(0, 4); // Trucos básicos
        }
        else
        {
            return (TrickType)Random.Range(0, 2); // Solo step overs
        }
    }
    
    Vector3 FindSafeDirection()
    {
        // Encontrar dirección segura para mantener el balón
        Vector3 safeDirection = Vector3.zero;
        
        PlayerController nearestOpponent = FindNearestOpponent();
        if (nearestOpponent != null)
        {
            Vector3 awayFromOpponent = (transform.position - nearestOpponent.transform.position).normalized;
            safeDirection = awayFromOpponent;
        }
        
        return safeDirection;
    }
    
    PlayerController FindOpponentToMark()
    {
        return FindNearestOpponent();
    }
    
    Vector3 CalculateMarkingPosition(PlayerController opponent)
    {
        Vector3 ballPosition = ballController.transform.position;
        Vector3 opponentPosition = opponent.transform.position;
        
        // Posicionarse entre el oponente y el balón
        Vector3 direction = (opponentPosition - ballPosition).normalized;
        return ballPosition + direction * 2f;
    }
    
    Vector3 CalculateSupportPosition()
    {
        PlayerController ballCarrier = FindBallCarrier();
        if (ballCarrier == null) return assignedPosition;
        
        // Posicionarse para recibir un pase
        Vector3 supportPosition = ballCarrier.transform.position;
        supportPosition += ballCarrier.transform.forward * 5f;
        supportPosition += ballCarrier.transform.right * (Random.Range(0, 2) == 0 ? 3f : -3f);
        
        return supportPosition;
    }
    
    float EvaluatePlayerPosition(PlayerController player)
    {
        // Evaluar qué tan buena es la posición del jugador para recibir un pase
        Vector3 goalPosition = FindGoalPosition();
        float distanceToGoal = Vector3.Distance(player.transform.position, goalPosition);
        
        // Mejor posición = más cerca del gol
        return Mathf.Clamp(1f - (distanceToGoal / 50f), 0f, 1f);
    }
    
    void SetAssignedPosition()
    {
        // Establecer posición asignada basada en el role del jugador
        switch (playerController.playerData.preferredPosition)
        {
            case PlayerPosition.Goalkeeper:
                assignedPosition = new Vector3(0, 0, -45);
                break;
            case PlayerPosition.CenterBack:
                assignedPosition = new Vector3(0, 0, -30);
                break;
            case PlayerPosition.LeftBack:
                assignedPosition = new Vector3(-20, 0, -30);
                break;
            case PlayerPosition.RightBack:
                assignedPosition = new Vector3(20, 0, -30);
                break;
            case PlayerPosition.DefensiveMidfield:
                assignedPosition = new Vector3(0, 0, -15);
                break;
            case PlayerPosition.CentralMidfield:
                assignedPosition = new Vector3(0, 0, 0);
                break;
            case PlayerPosition.AttackingMidfield:
                assignedPosition = new Vector3(0, 0, 15);
                break;
            case PlayerPosition.LeftWing:
                assignedPosition = new Vector3(-25, 0, 20);
                break;
            case PlayerPosition.RightWing:
                assignedPosition = new Vector3(25, 0, 20);
                break;
            case PlayerPosition.Striker:
                assignedPosition = new Vector3(0, 0, 35);
                break;
        }
    }
}

public enum AIState
{
    Positioning,
    ChasingBall,
    Pressing,
    Marking,
    Supporting,
    Passing,
    Shooting,
    Dribbling,
    HoldingBall
}