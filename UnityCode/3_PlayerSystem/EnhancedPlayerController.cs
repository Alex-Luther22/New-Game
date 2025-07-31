using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using FootballMaster.PlayerSystem;

[RequireComponent(typeof(NavMeshAgent))]
public class EnhancedPlayerController : MonoBehaviour
{
    [Header("Player Info")]
    public PlayerData playerData;
    public StarPlayerTemplate starPlayerTemplate;
    public PlayerPosition position;
    public bool isPlayerControlled = false;
    public bool isStarPlayer = false;
    
    [Header("Movement & Physics")]
    public float baseSpeed = 5f;
    public float sprintSpeed = 8f;
    public float acceleration = 10f;
    public float rotationSpeed = 720f;
    public float jumpForce = 300f;
    
    [Header("Ball Control")]
    public Transform ballControlPoint;
    public float ballControlRadius = 1.5f;
    public float passingPower = 15f;
    public float shootingPower = 25f;
    public float ballTouchSensitivity = 0.8f;
    
    [Header("Special Abilities")]
    public float specialAbilityCooldown = 0f;
    public bool canUseSpecialAbility = true;
    public ParticleSystem abilityEffect;
    public AudioSource abilityAudio;
    
    [Header("Animations & Effects")]
    public Animator animator;
    public Transform effectsParent;
    public GameObject[] skillEffects;
    
    [Header("AI & Behavior")]
    public float visionRadius = 15f;
    public LayerMask enemyLayerMask;
    public LayerMask teammateLayerMask;
    
    // Private variables
    private NavMeshAgent agent;
    private Rigidbody rb;
    private BallController ballController;
    private bool hasBall = false;
    private bool isSprinting = false;
    private bool isJumping = false;
    private Vector3 moveDirection;
    private float stamina = 100f;
    private float maxStamina = 100f;
    private float energy = 100f;
    private float maxEnergy = 100f;
    
    // Special ability tracking
    private Dictionary<string, float> abilityCooldowns = new Dictionary<string, float>();
    private List<SpecialAbility> activeAbilities = new List<SpecialAbility>();
    
    // Performance tracking
    private int goalsScored = 0;
    private int assistsMade = 0;
    private int successfulTricks = 0;
    private int passesCompleted = 0;
    private float matchRating = 6.0f;
    
    // AI específico
    private PlayerAI playerAI;
    
    void Start()
    {
        InitializeComponents();
        ConfigureAgent();
        ApplyPlayerStats();
        InitializeSpecialAbilities();
    }
    
    void Update()
    {
        UpdateStamina();
        UpdateEnergy();
        UpdateAbilityCooldowns();
        CheckBallProximity();
        UpdateAnimations();
        UpdateMatchRating();
        
        if (isPlayerControlled)
        {
            HandlePlayerInput();
        }
    }
    
    void InitializeComponents()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        ballController = FindObjectOfType<BallController>();
        playerAI = GetComponent<PlayerAI>();
        
        if (abilityAudio == null)
            abilityAudio = GetComponent<AudioSource>();
    }
    
    void ConfigureAgent()
    {
        agent.speed = baseSpeed;
        agent.acceleration = acceleration;
        agent.angularSpeed = rotationSpeed;
        agent.stoppingDistance = 0.5f;
        agent.radius = 0.5f;
    }
    
    void ApplyPlayerStats()
    {
        if (playerData != null)
        {
            baseSpeed = playerData.speed * 0.12f;
            sprintSpeed = playerData.speed * 0.18f;
            passingPower = playerData.passing * 0.35f;
            shootingPower = playerData.shooting * 0.45f;
            maxStamina = playerData.stamina;
            stamina = maxStamina;
            
            // Enhanced stats for star players
            if (isStarPlayer && starPlayerTemplate != null)
            {
                ApplyStarPlayerBonuses();
            }
        }
    }
    
    void ApplyStarPlayerBonuses()
    {
        // Star players get stat bonuses
        baseSpeed *= 1.15f;
        sprintSpeed *= 1.2f;
        shootingPower *= 1.25f;
        passingPower *= 1.2f;
        maxEnergy = 150f; // More energy for special abilities
        energy = maxEnergy;
        
        // Apply playstyle modifiers
        switch (starPlayerTemplate.preferredPlayStyle)
        {
            case PlayStyle.SpeedMerchant:
                baseSpeed *= 1.3f;
                sprintSpeed *= 1.4f;
                break;
            case PlayStyle.TechnicalDribbler:
                ballControlRadius *= 1.2f;
                ballTouchSensitivity *= 1.3f;
                break;
            case PlayStyle.PlayMaker:
                passingPower *= 1.3f;
                visionRadius *= 1.4f;
                break;
            case PlayStyle.TargetMan:
                jumpForce *= 1.3f;
                shootingPower *= 1.2f;
                break;
        }
    }
    
    void InitializeSpecialAbilities()
    {
        if (isStarPlayer && starPlayerTemplate != null)
        {
            activeAbilities = new List<SpecialAbility>(starPlayerTemplate.specialAbilities);
            
            // Initialize cooldown tracking
            foreach (var ability in activeAbilities)
            {
                abilityCooldowns[ability.abilityId] = 0f;
            }
        }
    }
    
    void UpdateStamina()
    {
        if (isSprinting)
        {
            float staminaDrain = 25f;
            if (isStarPlayer) staminaDrain *= 0.8f; // Star players have better stamina management
            
            stamina = Mathf.Max(0, stamina - Time.deltaTime * staminaDrain);
            if (stamina <= 10f)
            {
                isSprinting = false;
            }
        }
        else
        {
            float staminaRecover = 12f;
            if (isStarPlayer) staminaRecover *= 1.2f;
            
            stamina = Mathf.Min(maxStamina, stamina + Time.deltaTime * staminaRecover);
        }
    }
    
    void UpdateEnergy()
    {
        if (energy < maxEnergy)
        {
            float energyRecover = 8f;
            if (isStarPlayer) energyRecover *= 1.5f;
            
            energy = Mathf.Min(maxEnergy, energy + Time.deltaTime * energyRecover);
        }
    }
    
    void UpdateAbilityCooldowns()
    {
        List<string> keys = new List<string>(abilityCooldowns.Keys);
        foreach (string abilityId in keys)
        {
            if (abilityCooldowns[abilityId] > 0)
            {
                abilityCooldowns[abilityId] -= Time.deltaTime;
                if (abilityCooldowns[abilityId] <= 0)
                {
                    abilityCooldowns[abilityId] = 0;
                }
            }
        }
    }
    
    void CheckBallProximity()
    {
        if (ballController != null)
        {
            float distanceToBall = Vector3.Distance(transform.position, ballController.transform.position);
            bool previousHasBall = hasBall;
            hasBall = distanceToBall <= ballControlRadius;
            
            // Ball possession gained
            if (hasBall && !previousHasBall)
            {
                OnBallPossessionGained();
            }
        }
    }
    
    void OnBallPossessionGained()
    {
        // Update match rating
        matchRating += 0.1f;
        
        // Star player effects
        if (isStarPlayer && abilityEffect != null)
        {
            abilityEffect.Play();
        }
    }
    
    void HandlePlayerInput()
    {
        // Enhanced input handling for special abilities
        if (Input.GetKeyDown(KeyCode.Q) && isStarPlayer)
        {
            UseRandomSpecialAbility();
        }
        
        if (Input.GetKeyDown(KeyCode.E) && hasBall)
        {
            PerformSignatureTrick();
        }
    }
    
    public void UseRandomSpecialAbility()
    {
        if (!isStarPlayer || activeAbilities.Count == 0) return;
        
        // Find available abilities
        List<SpecialAbility> availableAbilities = activeAbilities.FindAll(ability => 
            abilityCooldowns[ability.abilityId] <= 0 && energy >= ability.energyCost);
        
        if (availableAbilities.Count > 0)
        {
            SpecialAbility selectedAbility = availableAbilities[Random.Range(0, availableAbilities.Count)];
            UseSpecialAbility(selectedAbility);
        }
    }
    
    public void UseSpecialAbility(SpecialAbility ability)
    {
        if (!CanUseAbility(ability)) return;
        
        // Consume energy and set cooldown
        energy -= ability.energyCost;
        abilityCooldowns[ability.abilityId] = ability.cooldownTime;
        
        // Execute ability based on type
        switch (ability.type)
        {
            case AbilityType.Dribbling:
                ExecuteDribblingAbility(ability);
                break;
            case AbilityType.Shooting:
                ExecuteShootingAbility(ability);
                break;
            case AbilityType.Passing:
                ExecutePassingAbility(ability);
                break;
            case AbilityType.Physical:
                ExecutePhysicalAbility(ability);
                break;
            case AbilityType.SetPiece:
                ExecuteSetPieceAbility(ability);
                break;
            case AbilityType.Leadership:
                ExecuteLeadershipAbility(ability);
                break;
        }
        
        // Play effects
        PlayAbilityEffects(ability);
        
        // Update match rating
        matchRating += 0.5f;
    }
    
    bool CanUseAbility(SpecialAbility ability)
    {
        return isStarPlayer && 
               energy >= ability.energyCost && 
               abilityCooldowns[ability.abilityId] <= 0;
    }
    
    void ExecuteDribblingAbility(SpecialAbility ability)
    {
        if (!hasBall) return;
        
        switch (ability.abilityId)
        {
            case "elastico_master":
                StartCoroutine(ExecuteEnhancedElastico(ability.effectiveness));
                break;
            case "rainbow_legend":
                ExecuteEnhancedRainbow(ability.effectiveness);
                break;
            case "stepover_king":
                ExecuteEnhancedStepover(ability.effectiveness);
                break;
        }
        
        successfulTricks++;
    }
    
    void ExecuteShootingAbility(SpecialAbility ability)
    {
        if (!hasBall) return;
        
        Vector3 goalDirection = FindGoalDirection();
        
        switch (ability.abilityId)
        {
            case "rocket_shot":
                ExecuteRocketShot(goalDirection, ability.effectiveness);
                break;
            case "curve_master":
                ExecuteCurvedShot(goalDirection, ability.effectiveness);
                break;
            case "chip_specialist":
                ExecuteChipShot(goalDirection, ability.effectiveness);
                break;
        }
    }
    
    void ExecutePassingAbility(SpecialAbility ability)
    {
        if (!hasBall) return;
        
        PlayerController targetPlayer = FindBestPassTarget();
        if (targetPlayer == null) return;
        
        switch (ability.abilityId)
        {
            case "vision_master":
                ExecuteThroughBall(targetPlayer, ability.effectiveness);
                break;
            case "long_pass_king":
                ExecuteLongPass(targetPlayer, ability.effectiveness);
                break;
        }
        
        passesCompleted++;
    }
    
    void ExecutePhysicalAbility(SpecialAbility ability)
    {
        switch (ability.abilityId)
        {
            case "lightning_pace":
                StartCoroutine(ExecuteSpeedBurst(ability.effectiveness));
                break;
            case "aerial_dominance":
                ExecuteEnhancedJump(ability.effectiveness);
                break;
        }
    }
    
    void ExecuteSetPieceAbility(SpecialAbility ability)
    {
        // These would be triggered during free kicks/penalties
        Debug.Log($"Set piece ability {ability.abilityName} ready for execution");
    }
    
    void ExecuteLeadershipAbility(SpecialAbility ability)
    {
        // Boost nearby teammates
        Collider[] nearbyTeammates = Physics.OverlapSphere(transform.position, 10f, teammateLayerMask);
        
        foreach (Collider teammate in nearbyTeammates)
        {
            EnhancedPlayerController teammateController = teammate.GetComponent<EnhancedPlayerController>();
            if (teammateController != null && teammateController != this)
            {
                teammateController.ApplyLeadershipBoost(ability.effectiveness);
            }
        }
    }
    
    // Enhanced trick implementations
    IEnumerator ExecuteEnhancedElastico(float effectiveness)
    {
        Vector3 originalPosition = transform.position;
        Vector3 fakeDirection = transform.TransformDirection(Vector3.right);
        Vector3 realDirection = transform.TransformDirection(Vector3.left);
        
        // Enhanced fake movement
        rb.AddForce(fakeDirection * 250f * effectiveness, ForceMode.Impulse);
        yield return new WaitForSeconds(0.08f);
        
        // Real movement with increased power
        rb.AddForce(realDirection * 500f * effectiveness, ForceMode.Impulse);
        
        // Enhanced ball control
        if (ballController != null)
        {
            ballController.KickBall(realDirection, 0.6f * effectiveness);
        }
        
        if (animator != null)
        {
            animator.SetTrigger("EnhancedElastico");
        }
    }
    
    void ExecuteEnhancedRainbow(float effectiveness)
    {
        if (ballController != null)
        {
            Vector3 rainbowDirection = (transform.forward + Vector3.up * 2.5f).normalized;
            float power = 1.4f * effectiveness;
            ballController.KickBall(rainbowDirection, power);
            
            // Add spectacular effects
            CreateSkillEffect(0);
        }
        
        if (animator != null)
        {
            animator.SetTrigger("EnhancedRainbow");
        }
    }
    
    void ExecuteEnhancedStepover(float effectiveness)
    {
        Vector3 stepDirection = transform.TransformDirection(Random.value > 0.5f ? Vector3.right : Vector3.left);
        rb.AddForce(stepDirection * 400f * effectiveness, ForceMode.Impulse);
        
        if (ballController != null)
        {
            ballController.KickBall(stepDirection, 0.4f * effectiveness);
        }
        
        if (animator != null)
        {
            animator.SetTrigger("EnhancedStepover");
        }
    }
    
    void ExecuteRocketShot(Vector3 direction, float effectiveness)
    {
        if (ballController != null)
        {
            float power = shootingPower * 1.5f * effectiveness;
            ballController.ShootWithCurve(direction, power, CurveType.None);
            CreateSkillEffect(1);
        }
        
        if (animator != null)
        {
            animator.SetTrigger("RocketShot");
        }
    }
    
    void ExecuteCurvedShot(Vector3 direction, float effectiveness)
    {
        if (ballController != null)
        {
            float power = shootingPower * 1.2f * effectiveness;
            CurveType curve = Random.value > 0.5f ? CurveType.Left : CurveType.Right;
            ballController.ShootWithCurve(direction, power, curve);
            CreateSkillEffect(2);
        }
        
        if (animator != null)
        {
            animator.SetTrigger("CurveShot");
        }
    }
    
    void ExecuteChipShot(Vector3 direction, float effectiveness)
    {
        if (ballController != null)
        {
            Vector3 chipDirection = (direction + Vector3.up * 0.8f).normalized;
            float power = shootingPower * 0.8f * effectiveness;
            ballController.ShootWithCurve(chipDirection, power, CurveType.None);
        }
        
        if (animator != null)
        {
            animator.SetTrigger("ChipShot");
        }
    }
    
    void ExecuteThroughBall(PlayerController targetPlayer, float effectiveness)
    {
        Vector3 passDirection = (targetPlayer.transform.position - transform.position).normalized;
        float power = passingPower * 1.3f * effectiveness;
        
        if (ballController != null)
        {
            ballController.KickBall(passDirection, power);
        }
    }
    
    void ExecuteLongPass(PlayerController targetPlayer, float effectiveness)
    {
        Vector3 passDirection = (targetPlayer.transform.position - transform.position).normalized;
        Vector3 lobDirection = (passDirection + Vector3.up * 0.5f).normalized;
        float power = passingPower * 1.5f * effectiveness;
        
        if (ballController != null)
        {
            ballController.KickBall(lobDirection, power);
        }
    }
    
    IEnumerator ExecuteSpeedBurst(float effectiveness)
    {
        float originalSpeed = baseSpeed;
        float originalSprintSpeed = sprintSpeed;
        
        // Temporary speed boost
        baseSpeed *= (1.5f * effectiveness);
        sprintSpeed *= (1.7f * effectiveness);
        agent.speed = sprintSpeed;
        
        yield return new WaitForSeconds(3f);
        
        // Restore original speeds
        baseSpeed = originalSpeed;
        sprintSpeed = originalSprintSpeed;
        agent.speed = baseSpeed;
    }
    
    void ExecuteEnhancedJump(float effectiveness)
    {
        if (!isJumping)
        {
            isJumping = true;
            rb.AddForce(Vector3.up * jumpForce * effectiveness, ForceMode.Impulse);
            
            if (animator != null)
            {
                animator.SetTrigger("PowerJump");
            }
            
            StartCoroutine(ResetJumpFlag());
        }
    }
    
    IEnumerator ResetJumpFlag()
    {
        yield return new WaitForSeconds(1f);
        isJumping = false;
    }
    
    void PerformSignatureTrick()
    {
        if (!isStarPlayer || !hasBall || starPlayerTemplate.signatureTricks.Count == 0) return;
        
        TrickType randomTrick = starPlayerTemplate.signatureTricks[Random.Range(0, starPlayerTemplate.signatureTricks.Count)];
        PerformTrick(randomTrick);
    }
    
    public void PerformTrick(TrickType trickType)
    {
        if (hasBall && stamina > 10f)
        {
            float effectiveness = isStarPlayer ? 1.2f : 1.0f;
            
            switch (trickType)
            {
                case TrickType.StepOverRight:
                    ExecuteStepOver(Vector3.right, effectiveness);
                    break;
                case TrickType.StepOverLeft:
                    ExecuteStepOver(Vector3.left, effectiveness);
                    break;
                case TrickType.Nutmeg:
                    ExecuteNutmeg(effectiveness);
                    break;
                case TrickType.Roulette:
                    ExecuteRoulette(effectiveness);
                    break;
                case TrickType.Elastico:
                    StartCoroutine(ExecuteElastico(effectiveness));
                    break;
                case TrickType.RainbowFlick:
                    ExecuteRainbowFlick(effectiveness);
                    break;
            }
            
            stamina -= 10f;
            successfulTricks++;
        }
    }
    
    // Enhanced basic tricks with effectiveness parameter
    void ExecuteStepOver(Vector3 direction, float effectiveness)
    {
        Vector3 stepDirection = transform.TransformDirection(direction);
        rb.AddForce(stepDirection * 300f * effectiveness, ForceMode.Impulse);
        
        if (ballController != null)
        {
            ballController.KickBall(stepDirection, 0.3f * effectiveness);
        }
        
        if (animator != null)
        {
            animator.SetTrigger("StepOver");
        }
    }
    
    void ExecuteNutmeg(float effectiveness)
    {
        if (ballController != null)
        {
            Vector3 nutmegDirection = transform.forward;
            ballController.KickBall(nutmegDirection, 0.8f * effectiveness);
        }
        
        if (animator != null)
        {
            animator.SetTrigger("Nutmeg");
        }
    }
    
    void ExecuteRoulette(float effectiveness)
    {
        StartCoroutine(ExecuteRouletteCoroutine(effectiveness));
    }
    
    IEnumerator ExecuteRouletteCoroutine(float effectiveness)
    {
        float rotationTime = 0.4f / effectiveness; // Faster for star players
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
    
    IEnumerator ExecuteElastico(float effectiveness)
    {
        Vector3 originalPosition = transform.position;
        Vector3 fakeDirection = transform.TransformDirection(Vector3.right);
        Vector3 realDirection = transform.TransformDirection(Vector3.left);
        
        rb.AddForce(fakeDirection * 200f * effectiveness, ForceMode.Impulse);
        yield return new WaitForSeconds(0.1f);
        
        rb.AddForce(realDirection * 400f * effectiveness, ForceMode.Impulse);
        
        if (ballController != null)
        {
            ballController.KickBall(realDirection, 0.5f * effectiveness);
        }
        
        if (animator != null)
        {
            animator.SetTrigger("Elastico");
        }
    }
    
    void ExecuteRainbowFlick(float effectiveness)
    {
        if (ballController != null)
        {
            Vector3 rainbowDirection = transform.forward + Vector3.up * 2f;
            ballController.KickBall(rainbowDirection.normalized, 1.2f * effectiveness);
        }
        
        if (animator != null)
        {
            animator.SetTrigger("RainbowFlick");
        }
    }
    
    // Utility methods
    Vector3 FindGoalDirection()
    {
        // Find the nearest goal
        GameObject[] goals = GameObject.FindGameObjectsWithTag("Goal");
        if (goals.Length > 0)
        {
            GameObject nearestGoal = goals[0];
            float nearestDistance = Vector3.Distance(transform.position, nearestGoal.transform.position);
            
            foreach (GameObject goal in goals)
            {
                float distance = Vector3.Distance(transform.position, goal.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestGoal = goal;
                }
            }
            
            return (nearestGoal.transform.position - transform.position).normalized;
        }
        
        return transform.forward;
    }
    
    PlayerController FindBestPassTarget()
    {
        PlayerController[] allPlayers = FindObjectsOfType<PlayerController>();
        PlayerController bestTarget = null;
        float bestScore = 0f;
        
        foreach (PlayerController player in allPlayers)
        {
            if (player != this && player.playerData.teamId == playerData.teamId)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                float score = 1f / (distance + 1f); // Closer players get higher scores
                
                // Add bonus for players in good positions
                if (IsPlayerInGoodPosition(player))
                {
                    score *= 2f;
                }
                
                if (score > bestScore)
                {
                    bestScore = score;
                    bestTarget = player;
                }
            }
        }
        
        return bestTarget;
    }
    
    bool IsPlayerInGoodPosition(PlayerController player)
    {
        // Simple logic to determine if a player is in a good position
        // This could be enhanced with more sophisticated analysis
        return Vector3.Distance(player.transform.position, FindGoalDirection() * 50f) < 20f;
    }
    
    void ApplyLeadershipBoost(float effectiveness)
    {
        // Temporary boost from captain's influence
        StartCoroutine(ApplyTemporaryBoost(effectiveness));
    }
    
    IEnumerator ApplyTemporaryBoost(float effectiveness)
    {
        float boostMultiplier = 1f + (0.2f * effectiveness);
        float originalSpeed = baseSpeed;
        float originalPassing = passingPower;
        float originalShooting = shootingPower;
        
        baseSpeed *= boostMultiplier;
        passingPower *= boostMultiplier;
        shootingPower *= boostMultiplier;
        
        yield return new WaitForSeconds(30f); // Boost lasts 30 seconds
        
        baseSpeed = originalSpeed;
        passingPower = originalPassing;
        shootingPower = originalShooting;
    }
    
    void CreateSkillEffect(int effectIndex)
    {
        if (skillEffects != null && effectIndex < skillEffects.Length && skillEffects[effectIndex] != null)
        {
            GameObject effect = Instantiate(skillEffects[effectIndex], transform.position, transform.rotation);
            Destroy(effect, 2f);
        }
    }
    
    void PlayAbilityEffects(SpecialAbility ability)
    {
        if (abilityEffect != null)
        {
            abilityEffect.Play();
        }
        
        if (abilityAudio != null && ability.soundEffect != null)
        {
            abilityAudio.PlayOneShot(ability.soundEffect);
        }
        
        if (animator != null && !string.IsNullOrEmpty(ability.animationTrigger))
        {
            animator.SetTrigger(ability.animationTrigger);
        }
    }
    
    void UpdateAnimations()
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
            animator.SetBool("HasBall", hasBall);
            animator.SetBool("IsSprinting", isSprinting);
            animator.SetBool("IsStarPlayer", isStarPlayer);
            animator.SetFloat("Stamina", stamina / maxStamina);
            animator.SetFloat("Energy", energy / maxEnergy);
        }
    }
    
    void UpdateMatchRating()
    {
        // Dynamic match rating based on performance
        float baseRating = 6.0f;
        baseRating += (goalsScored * 1.0f);
        baseRating += (assistsMade * 0.5f);
        baseRating += (successfulTricks * 0.1f);
        baseRating += (passesCompleted * 0.02f);
        
        if (isStarPlayer)
        {
            baseRating += 0.5f; // Star player bonus
        }
        
        matchRating = Mathf.Clamp(baseRating, 1.0f, 10.0f);
    }
    
    // Public getters for UI and other systems
    public float GetStamina() => stamina / maxStamina;
    public float GetEnergy() => energy / maxEnergy;
    public float GetMatchRating() => matchRating;
    public bool HasBall() => hasBall;
    public bool IsStarPlayer() => isStarPlayer;
    public List<SpecialAbility> GetAvailableAbilities()
    {
        return activeAbilities.FindAll(ability => 
            abilityCooldowns[ability.abilityId] <= 0 && energy >= ability.energyCost);
    }
    
    // Event handlers
    public void OnGoalScored()
    {
        goalsScored++;
        matchRating += 1.0f;
        energy = Mathf.Min(maxEnergy, energy + 20f); // Boost energy on goal
    }
    
    public void OnAssistMade()
    {
        assistsMade++;
        matchRating += 0.5f;
        energy = Mathf.Min(maxEnergy, energy + 15f);
    }
    
    public void OnMatchStart()
    {
        stamina = maxStamina;
        energy = maxEnergy;
        matchRating = 6.0f;
        goalsScored = 0;
        assistsMade = 0;
        successfulTricks = 0;
        passesCompleted = 0;
        
        // Reset all ability cooldowns
        foreach (string abilityId in abilityCooldowns.Keys.ToArray())
        {
            abilityCooldowns[abilityId] = 0f;
        }
    }
}