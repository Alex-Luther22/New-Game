using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class BallController_120fps : MonoBehaviour
{
    [Header("Ball Physics")]
    public float bounceForce = 8f;
    public float airResistance = 0.98f;
    public float groundFriction = 0.92f;
    public float spinDecay = 0.95f;
    public float maxBounceAngle = 60f;
    
    [Header("Curve Settings")]
    public float curveMultiplier = 1.5f;
    public float magnusEffectStrength = 2f;
    public AnimationCurve curveFalloff = AnimationCurve.Linear(0, 1, 1, 0);
    
    [Header("120fps Optimizations")]
    public bool usePhysicsOptimization = true;
    public bool enableTrailOptimization = true;
    public int maxTrailPoints = 20;
    public float cullDistance = 100f;
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip kickSound;
    public AudioClip bounceSound;
    public AudioClip grassSound;
    
    // Cached components
    private Rigidbody rb;
    private Transform ballTransform;
    private TrailRenderer trailRenderer;
    private SphereCollider sphereCollider;
    
    // Physics state
    private Vector3 spinVector;
    private float initialSpeed;
    private float timeSinceKick;
    private bool isGrounded;
    private Vector3 lastVelocity;
    private Vector3 lastPosition;
    
    // Magnus effect optimization
    private Vector3 magnusForce;
    private float magnusTimer;
    
    // Performance optimizations
    private Camera mainCamera;
    private bool isVisible = true;
    private float lastVisibilityCheck;
    private float visibilityCheckInterval = 0.1f;
    
    // Object pooling for effects
    private static Queue<ParticleSystem> particlePool = new Queue<ParticleSystem>();
    private static Dictionary<SurfaceType, PhysicMaterial> surfaceMaterials = new Dictionary<SurfaceType, PhysicMaterial>();
    
    // Collision optimization
    private ContactPoint[] contactPoints = new ContactPoint[10];
    private int contactCount;
    
    // Prediction system
    private Vector3[] trajectoryPoints;
    private int trajectoryLength = 50;
    private bool trajectoryNeedsUpdate = true;
    
    // Audio optimization
    private float lastSoundTime;
    private float soundCooldown = 0.1f;
    
    void Start()
    {
        InitializeComponents();
        SetupPhysics();
        SetupOptimizations();
        InitializeTrajectoryArray();
    }
    
    void InitializeComponents()
    {
        rb = GetComponent<Rigidbody>();
        ballTransform = transform;
        trailRenderer = GetComponent<TrailRenderer>();
        sphereCollider = GetComponent<SphereCollider>();
        mainCamera = Camera.main;
        
        lastPosition = ballTransform.position;
        lastVelocity = rb.velocity;
    }
    
    void SetupPhysics()
    {
        // Configure physics for 120fps
        rb.material.bounciness = 0.6f;
        rb.material.dynamicFriction = 0.4f;
        rb.material.staticFriction = 0.4f;
        rb.drag = 0.05f;
        rb.angularDrag = 0.3f;
        
        // Set collision detection for high speed
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        
        // Initialize surface materials
        if (surfaceMaterials.Count == 0)
        {
            InitializeSurfaceMaterials();
        }
    }
    
    void SetupOptimizations()
    {
        // Initialize particle pool
        if (particlePool.Count == 0)
        {
            InitializeParticlePool();
        }
        
        // Optimize trail renderer
        if (enableTrailOptimization && trailRenderer != null)
        {
            trailRenderer.autodestruct = false;
            trailRenderer.time = 0.5f;
            trailRenderer.minVertexDistance = 0.1f;
        }
    }
    
    void InitializeSurfaceMaterials()
    {
        surfaceMaterials[SurfaceType.Grass] = Resources.Load<PhysicMaterial>("GrassPhysics");
        surfaceMaterials[SurfaceType.Dirt] = Resources.Load<PhysicMaterial>("DirtPhysics");
        surfaceMaterials[SurfaceType.Concrete] = Resources.Load<PhysicMaterial>("ConcretePhysics");
    }
    
    void InitializeParticlePool()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject particleObj = new GameObject("ParticleEffect");
            ParticleSystem particles = particleObj.AddComponent<ParticleSystem>();
            particles.Stop();
            particleObj.SetActive(false);
            particlePool.Enqueue(particles);
        }
    }
    
    void InitializeTrajectoryArray()
    {
        trajectoryPoints = new Vector3[trajectoryLength];
    }
    
    void Update()
    {
        float deltaTime = Time.deltaTime;
        
        // Check visibility for culling
        if (Time.time - lastVisibilityCheck > visibilityCheckInterval)
        {
            UpdateVisibility();
            lastVisibilityCheck = Time.time;
        }
        
        // Skip heavy calculations if not visible and far away
        if (!isVisible && Vector3.Distance(ballTransform.position, mainCamera.transform.position) > cullDistance)
        {
            return;
        }
        
        // High-frequency physics updates
        timeSinceKick += deltaTime;
        
        // Apply air resistance
        ApplyAirResistance(deltaTime);
        
        // Apply Magnus effect (curves)
        ApplyMagnusEffect(deltaTime);
        
        // Ground detection
        CheckGrounded();
        
        // Ground friction
        if (isGrounded)
        {
            ApplyGroundFriction(deltaTime);
        }
        
        // Reduce spin
        spinVector *= Mathf.Pow(spinDecay, deltaTime * 60f); // Frame-rate independent
        
        // Update trajectory prediction
        if (trajectoryNeedsUpdate && rb.velocity.magnitude > 1f)
        {
            UpdateTrajectoryPrediction();
            trajectoryNeedsUpdate = false;
        }
        
        // Update trail renderer
        if (enableTrailOptimization && trailRenderer != null)
        {
            UpdateTrailRenderer();
        }
        
        lastVelocity = rb.velocity;
        lastPosition = ballTransform.position;
    }
    
    void UpdateVisibility()
    {
        if (mainCamera != null)
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
            isVisible = GeometryUtility.TestPlanesAABB(planes, sphereCollider.bounds);
        }
    }
    
    void ApplyAirResistance(float deltaTime)
    {
        if (!isGrounded)
        {
            float resistanceMultiplier = Mathf.Pow(airResistance, deltaTime * 60f);
            rb.velocity *= resistanceMultiplier;
        }
    }
    
    void ApplyMagnusEffect(float deltaTime)
    {
        if (spinVector.magnitude > 0.1f && rb.velocity.magnitude > 1f)
        {
            // Calculate Magnus force
            magnusForce = Vector3.Cross(spinVector, rb.velocity.normalized) * magnusEffectStrength;
            
            // Apply curve falloff
            float curveStrength = curveFalloff.Evaluate(timeSinceKick / 3f);
            magnusForce *= curveStrength;
            
            rb.AddForce(magnusForce * deltaTime * 60f, ForceMode.Force);
        }
    }
    
    void CheckGrounded()
    {
        // Optimized ground check using sphere cast
        isGrounded = Physics.SphereCast(ballTransform.position, sphereCollider.radius * 0.9f, Vector3.down, out RaycastHit hit, 0.1f);
        
        if (isGrounded)
        {
            // Adjust surface material based on ground type
            UpdateSurfaceMaterial(hit.collider.tag);
        }
    }
    
    void UpdateSurfaceMaterial(string surfaceTag)
    {
        SurfaceType surfaceType = GetSurfaceType(surfaceTag);
        if (surfaceMaterials.ContainsKey(surfaceType))
        {
            rb.material = surfaceMaterials[surfaceType];
        }
    }
    
    SurfaceType GetSurfaceType(string tag)
    {
        switch (tag)
        {
            case "Grass": return SurfaceType.Grass;
            case "Dirt": return SurfaceType.Dirt;
            case "Concrete": return SurfaceType.Concrete;
            default: return SurfaceType.Grass;
        }
    }
    
    void ApplyGroundFriction(float deltaTime)
    {
        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        float frictionMultiplier = Mathf.Pow(groundFriction, deltaTime * 60f);
        horizontalVelocity *= frictionMultiplier;
        
        rb.velocity = new Vector3(horizontalVelocity.x, rb.velocity.y, horizontalVelocity.z);
    }
    
    void UpdateTrailRenderer()
    {
        if (rb.velocity.magnitude > 5f)
        {
            trailRenderer.enabled = true;
            trailRenderer.time = Mathf.Clamp(rb.velocity.magnitude * 0.1f, 0.2f, 1f);
        }
        else
        {
            trailRenderer.enabled = false;
        }
    }
    
    void UpdateTrajectoryPrediction()
    {
        Vector3 currentPos = ballTransform.position;
        Vector3 currentVel = rb.velocity;
        
        for (int i = 0; i < trajectoryLength; i++)
        {
            trajectoryPoints[i] = currentPos;
            
            // Simulate physics
            currentVel += Physics.gravity * Time.fixedDeltaTime;
            currentVel *= airResistance;
            currentPos += currentVel * Time.fixedDeltaTime;
            
            // Stop if hits ground
            if (currentPos.y <= 0)
            {
                trajectoryPoints[i] = new Vector3(currentPos.x, 0, currentPos.z);
                break;
            }
        }
    }
    
    public void KickBall(Vector3 direction, float power, Vector3 spin = default)
    {
        timeSinceKick = 0f;
        trajectoryNeedsUpdate = true;
        
        // Apply force
        float finalPower = Mathf.Clamp(power, 0.1f, 2f);
        Vector3 kickForce = direction * finalPower * 1000f;
        
        rb.AddForce(kickForce, ForceMode.Impulse);
        
        // Apply spin
        if (spin != Vector3.zero)
        {
            spinVector = spin * curveMultiplier;
            rb.AddTorque(spin * 200f, ForceMode.Impulse);
        }
        
        initialSpeed = rb.velocity.magnitude;
        
        // Play sound with cooldown
        if (Time.time - lastSoundTime > soundCooldown)
        {
            PlaySound(kickSound);
            lastSoundTime = Time.time;
        }
        
        // Particle effect
        PlayParticleEffect(ballTransform.position, "KickEffect");
    }
    
    public void ShootWithCurve(Vector3 direction, float power, CurveType curveType)
    {
        Vector3 spin = Vector3.zero;
        
        switch (curveType)
        {
            case CurveType.Right:
                spin = new Vector3(0, 0, -1);
                break;
            case CurveType.Left:
                spin = new Vector3(0, 0, 1);
                break;
            case CurveType.Up:
                spin = new Vector3(-1, 0, 0);
                break;
            case CurveType.Down:
                spin = new Vector3(1, 0, 0);
                break;
            case CurveType.Knuckleball:
                StartCoroutine(ApplyKnuckleballEffect());
                break;
        }
        
        KickBall(direction, power, spin);
    }
    
    IEnumerator ApplyKnuckleballEffect()
    {
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(0.1f);
            
            Vector3 randomForce = new Vector3(
                Random.Range(-50f, 50f),
                Random.Range(-20f, 20f),
                Random.Range(-50f, 50f)
            );
            
            rb.AddForce(randomForce, ForceMode.Force);
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        // Optimized collision handling
        contactCount = collision.GetContacts(contactPoints);
        
        if (contactCount > 0)
        {
            ContactPoint contact = contactPoints[0];
            
            if (collision.gameObject.CompareTag("Ground"))
            {
                HandleGroundBounce(contact);
            }
            else if (collision.gameObject.CompareTag("Post") || collision.gameObject.CompareTag("Crossbar"))
            {
                HandlePostBounce(contact);
            }
            else if (collision.gameObject.CompareTag("Player"))
            {
                HandlePlayerCollision(collision);
            }
            
            // Play bounce sound with cooldown
            if (Time.time - lastSoundTime > soundCooldown && rb.velocity.magnitude > 2f)
            {
                PlaySound(bounceSound);
                lastSoundTime = Time.time;
            }
            
            // Particle effect
            PlayParticleEffect(contact.point, "BounceEffect");
        }
        
        trajectoryNeedsUpdate = true;
    }
    
    void HandleGroundBounce(ContactPoint contact)
    {
        Vector3 incomingVector = lastVelocity;
        Vector3 reflectedVector = Vector3.Reflect(incomingVector, contact.normal);
        
        float bounceReduction = 0.7f;
        rb.velocity = reflectedVector * bounceReduction;
        
        PlaySound(grassSound);
    }
    
    void HandlePostBounce(ContactPoint contact)
    {
        Vector3 incomingVector = lastVelocity;
        Vector3 reflectedVector = Vector3.Reflect(incomingVector, contact.normal);
        
        float bounceReduction = 0.8f;
        rb.velocity = reflectedVector * bounceReduction;
        
        Vector3 randomSpin = new Vector3(
            Random.Range(-2f, 2f),
            Random.Range(-2f, 2f),
            Random.Range(-2f, 2f)
        );
        
        rb.AddTorque(randomSpin, ForceMode.Impulse);
    }
    
    void HandlePlayerCollision(Collision collision)
    {
        PlayerController_120fps player = collision.gameObject.GetComponent<PlayerController_120fps>();
        
        if (player != null)
        {
            BodyPart bodyPart = GetBodyPartFromCollision(collision);
            ApplyBodyPartEffect(bodyPart, player);
        }
    }
    
    BodyPart GetBodyPartFromCollision(Collision collision)
    {
        float collisionHeight = contactPoints[0].point.y;
        float playerHeight = collision.transform.position.y;
        float relativeHeight = collisionHeight - playerHeight;
        
        if (relativeHeight > 1.5f)
            return BodyPart.Head;
        else if (relativeHeight > 0.8f)
            return BodyPart.Chest;
        else
            return BodyPart.Foot;
    }
    
    void ApplyBodyPartEffect(BodyPart bodyPart, PlayerController_120fps player)
    {
        switch (bodyPart)
        {
            case BodyPart.Head:
                rb.AddForce(Vector3.down * 300f, ForceMode.Impulse);
                break;
            case BodyPart.Chest:
                rb.velocity *= 0.5f;
                break;
            case BodyPart.Foot:
                float skillMultiplier = player.GetSkillLevel() * 0.01f;
                rb.velocity *= (1f + skillMultiplier);
                break;
        }
    }
    
    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    
    void PlayParticleEffect(Vector3 position, string effectType)
    {
        if (particlePool.Count > 0)
        {
            ParticleSystem particle = particlePool.Dequeue();
            particle.transform.position = position;
            particle.gameObject.SetActive(true);
            particle.Play();
            
            StartCoroutine(ReturnParticleToPool(particle));
        }
    }
    
    IEnumerator ReturnParticleToPool(ParticleSystem particle)
    {
        yield return new WaitForSeconds(2f);
        particle.Stop();
        particle.gameObject.SetActive(false);
        particlePool.Enqueue(particle);
    }
    
    // Public methods
    public BallInfo GetBallInfo()
    {
        return new BallInfo
        {
            position = ballTransform.position,
            velocity = rb.velocity,
            spin = spinVector,
            isGrounded = isGrounded,
            speed = rb.velocity.magnitude
        };
    }
    
    public Vector3[] GetTrajectoryPoints()
    {
        return trajectoryPoints;
    }
    
    public bool IsMoving()
    {
        return rb.velocity.magnitude > 0.1f;
    }
    
    // Cleanup
    void OnDestroy()
    {
        StopAllCoroutines();
    }
}

// Supporting enums and classes
public enum CurveType
{
    None,
    Right,
    Left,
    Up,
    Down,
    Knuckleball
}

public enum BodyPart
{
    Head,
    Chest,
    Foot
}

public enum SurfaceType
{
    Grass,
    Dirt,
    Concrete
}

[System.Serializable]
public class BallInfo
{
    public Vector3 position;
    public Vector3 velocity;
    public Vector3 spin;
    public bool isGrounded;
    public float speed;
}