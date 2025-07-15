using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerController : MonoBehaviour
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
    
    private NavMeshAgent agent;
    private Rigidbody rb;
    private BallController ballController;
    private bool hasBall = false;
    private bool isSprinting = false;
    private Vector3 moveDirection;
    private float stamina = 100f;
    private float maxStamina = 100f;
    
    // AI específico
    private PlayerAI playerAI;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        ballController = FindObjectOfType<BallController>();
        playerAI = GetComponent<PlayerAI>();
        
        // Configurar agent
        agent.speed = baseSpeed;
        agent.acceleration = acceleration;
        agent.angularSpeed = rotationSpeed;
        
        // Configurar stats basados en playerData
        if (playerData != null)
        {
            ApplyPlayerStats();
        }
    }
    
    void Update()
    {
        // Actualizar stamina
        UpdateStamina();
        
        // Detectar balón cerca
        CheckBallProximity();
        
        // Actualizar animaciones
        UpdateAnimations();
        
        // Solo procesar input si es controlado por el jugador
        if (isPlayerControlled)
        {
            HandlePlayerInput();
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
    
    void UpdateStamina()
    {
        if (isSprinting)
        {
            stamina = Mathf.Max(0, stamina - Time.deltaTime * 20f);
            if (stamina <= 0)
            {
                isSprinting = false;
            }
        }
        else
        {
            stamina = Mathf.Min(maxStamina, stamina + Time.deltaTime * 10f);
        }
    }
    
    void CheckBallProximity()
    {
        if (ballController != null)
        {
            float distanceToBall = Vector3.Distance(transform.position, ballController.transform.position);
            hasBall = distanceToBall <= ballControlRadius;
        }
    }
    
    void HandlePlayerInput()
    {
        // El TouchControlManager manejará la entrada táctil
        // Este método puede ser usado para controles adicionales
    }
    
    public void MovePlayer(Vector3 direction)
    {
        moveDirection = direction.normalized;
        
        if (moveDirection.magnitude > 0.1f)
        {
            // Actualizar velocidad basada en sprint
            float currentSpeed = isSprinting ? sprintSpeed : baseSpeed;
            
            // Aplicar modificador de stamina
            float staminaModifier = stamina / maxStamina;
            currentSpeed *= staminaModifier;
            
            // Mover usando NavMeshAgent
            Vector3 targetPosition = transform.position + moveDirection * currentSpeed;
            agent.SetDestination(targetPosition);
            
            // Rotar hacia la dirección de movimiento
            Vector3 lookDirection = moveDirection;
            lookDirection.y = 0;
            
            if (lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
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
            // Calcular poder basado en stats del jugador
            float finalPower = power * (playerData.shooting / 100f) * (shootingPower / 25f);
            
            // Añadir variabilidad basada en la técnica
            float accuracy = playerData.technique / 100f;
            Vector3 finalDirection = AddShootingError(direction, 1f - accuracy);
            
            // Determinar tipo de curva basado en la posición del disparo
            CurveType curveType = DetermineCurveType(direction);
            
            ballController.ShootWithCurve(finalDirection, finalPower, curveType);
            
            // Reproducir animación de disparo
            if (animator != null)
            {
                animator.SetTrigger("Shoot");
            }
            
            // Reducir stamina
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
        // Determinar curva basada en la posición del jugador y dirección
        float angle = Vector3.Angle(transform.forward, direction);
        
        if (angle < 30f)
        {
            return CurveType.None; // Disparo directo
        }
        else if (Vector3.Cross(transform.forward, direction).y > 0)
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
            // Encontrar el compañero más cercano
            PlayerController nearestTeammate = FindNearestTeammate();
            
            if (nearestTeammate != null)
            {
                Vector3 passDirection = (nearestTeammate.transform.position - transform.position).normalized;
                float passDistance = Vector3.Distance(transform.position, nearestTeammate.transform.position);
                
                // Ajustar poder basado en la distancia y stats
                float power = Mathf.Clamp(passDistance / 20f, 0.2f, 1f);
                power *= (playerData.passing / 100f);
                
                ballController.KickBall(passDirection, power);
                
                // Animación de pase
                if (animator != null)
                {
                    animator.SetTrigger("Pass");
                }
            }
        }
    }
    
    public void PerformTrick(TrickType trickType)
    {
        if (hasBall && stamina > 10f)
        {
            // Ejecutar truco basado en el tipo
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
            
            // Reducir stamina
            stamina -= 10f;
        }
    }
    
    void ExecuteStepOver(Vector3 direction)
    {
        // Movimiento rápido hacia un lado
        Vector3 stepDirection = transform.TransformDirection(direction);
        rb.AddForce(stepDirection * 300f, ForceMode.Impulse);
        
        // Mover el balón ligeramente
        if (ballController != null)
        {
            ballController.KickBall(stepDirection, 0.3f);
        }
        
        if (animator != null)
        {
            animator.SetTrigger("StepOver");
        }
    }
    
    void ExecuteNutmeg()
    {
        // Pasar el balón entre las piernas del oponente
        if (ballController != null)
        {
            Vector3 nutmegDirection = transform.forward;
            ballController.KickBall(nutmegDirection, 0.8f);
        }
        
        if (animator != null)
        {
            animator.SetTrigger("Nutmeg");
        }
    }
    
    void ExecuteRoulette()
    {
        // Giro de 360 grados con el balón
        StartCoroutine(ExecuteRouletteCoroutine());
    }
    
    System.Collections.IEnumerator ExecuteRouletteCoroutine()
    {
        float rotationTime = 0.5f;
        float elapsedTime = 0f;
        Quaternion startRotation = transform.rotation;
        
        while (elapsedTime < rotationTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / rotationTime;
            
            transform.rotation = Quaternion.Lerp(startRotation, startRotation * Quaternion.Euler(0, 360f, 0), progress);
            
            yield return null;
        }
        
        if (animator != null)
        {
            animator.SetTrigger("Roulette");
        }
    }
    
    void ExecuteElastico()
    {
        // Movimiento de engaño hacia un lado y luego hacia el otro
        StartCoroutine(ExecuteElasticoCoroutine());
    }
    
    System.Collections.IEnumerator ExecuteElasticoCoroutine()
    {
        Vector3 originalPosition = transform.position;
        Vector3 fakeDirection = transform.TransformDirection(Vector3.right);
        Vector3 realDirection = transform.TransformDirection(Vector3.left);
        
        // Movimiento falso
        rb.AddForce(fakeDirection * 200f, ForceMode.Impulse);
        yield return new WaitForSeconds(0.1f);
        
        // Movimiento real
        rb.AddForce(realDirection * 400f, ForceMode.Impulse);
        
        // Mover el balón
        if (ballController != null)
        {
            ballController.KickBall(realDirection, 0.5f);
        }
        
        if (animator != null)
        {
            animator.SetTrigger("Elastico");
        }
    }
    
    void ExecuteRainbowFlick()
    {
        // Levantar el balón por encima del oponente
        if (ballController != null)
        {
            Vector3 rainbowDirection = transform.forward + Vector3.up * 2f;
            ballController.KickBall(rainbowDirection.normalized, 1.2f);
        }
        
        if (animator != null)
        {
            animator.SetTrigger("RainbowFlick");
        }
    }
    
    PlayerController FindNearestTeammate()
    {
        PlayerController[] allPlayers = FindObjectsOfType<PlayerController>();
        PlayerController nearestTeammate = null;
        float nearestDistance = float.MaxValue;
        
        foreach (PlayerController player in allPlayers)
        {
            if (player != this && player.playerData.teamId == playerData.teamId)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestTeammate = player;
                }
            }
        }
        
        return nearestTeammate;
    }
    
    void UpdateAnimations()
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
            animator.SetBool("HasBall", hasBall);
            animator.SetBool("IsSprinting", isSprinting);
        }
    }
    
    // Método para que otros scripts obtengan el nivel de habilidad
    public float GetSkillLevel()
    {
        return playerData != null ? playerData.overall : 50f;
    }
    
    // Método para obtener información del jugador
    public PlayerInfo GetPlayerInfo()
    {
        return new PlayerInfo
        {
            playerData = playerData,
            position = transform.position,
            velocity = agent.velocity,
            stamina = stamina,
            hasBall = hasBall,
            isSprinting = isSprinting
        };
    }
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