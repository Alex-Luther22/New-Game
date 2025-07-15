using UnityEngine;
using System.Collections;

/// <summary>
/// Controlador avanzado del balón con física realista para móviles
/// Optimizado para dispositivos como Tecno Spark 8C
/// </summary>
public class BallController : MonoBehaviour
{
    [Header("🎯 Configuración de Física")]
    [Range(0.1f, 2f)]
    public float ballMass = 0.43f; // Masa FIFA oficial
    
    [Range(0.01f, 0.1f)]
    public float airResistance = 0.04f; // Resistencia del aire
    
    [Range(0.1f, 0.5f)]
    public float groundFriction = 0.3f; // Fricción del césped
    
    [Range(0.5f, 1f)]
    public float bounciness = 0.7f; // Rebote del balón
    
    [Header("⚽ Curvas y Efectos")]
    [Range(0f, 2f)]
    public float magnusEffect = 1.2f; // Efecto Magnus (curvas)
    
    [Range(0f, 1f)]
    public float knuckleBallChance = 0.15f; // Probabilidad de knuckleball
    
    [Range(10f, 50f)]
    public float maxSpeed = 35f; // Velocidad máxima (m/s)
    
    [Header("🎮 Configuración de Controles")]
    [Range(0.1f, 3f)]
    public float kickForceMultiplier = 1.5f;
    
    [Range(0.1f, 2f)]
    public float spinSensitivity = 1f;
    
    [Header("🌟 Efectos Visuales")]
    public TrailRenderer ballTrail;
    public ParticleSystem grassParticles;
    public ParticleSystem impactParticles;
    
    [Header("🎵 Audio")]
    public AudioSource ballAudioSource;
    public AudioClip[] kickSounds;
    public AudioClip[] bounceGroundSounds;
    public AudioClip[] bouncePostSounds;
    
    // Componentes privados
    private Rigidbody rb;
    private SphereCollider ballCollider;
    private Vector3 lastVelocity;
    private Vector3 spinVector;
    private bool isGrounded;
    private bool isKnuckleBall;
    private float currentSpeed;
    
    // Optimización para móviles
    private float nextFixedUpdate = 0f;
    private const float FIXED_UPDATE_INTERVAL = 0.02f; // 50 FPS physics
    
    void Start()
    {
        InitializeBall();
        SetupOptimizationSettings();
    }
    
    void InitializeBall()
    {
        // Configurar componentes
        rb = GetComponent<Rigidbody>();
        ballCollider = GetComponent<SphereCollider>();
        
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        if (ballCollider == null)
        {
            ballCollider = gameObject.AddComponent<SphereCollider>();
        }
        
        // Configurar física
        rb.mass = ballMass;
        rb.drag = airResistance;
        rb.angularDrag = groundFriction;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        
        // Configurar colisionador
        ballCollider.radius = 0.11f; // Radio FIFA oficial
        ballCollider.material = CreateBallPhysicsMaterial();
        
        // Inicializar audio
        if (ballAudioSource == null)
        {
            ballAudioSource = gameObject.AddComponent<AudioSource>();
        }
        
        ballAudioSource.spatialBlend = 0.8f; // 3D sound
        ballAudioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        ballAudioSource.maxDistance = 50f;
        
        // Configurar trail
        SetupTrailRenderer();
    }
    
    void SetupOptimizationSettings()
    {
        // Optimización para dispositivos como Tecno Spark 8C
        if (SystemInfo.systemMemorySize <= 4096) // 4GB RAM o menos
        {
            // Reducir calidad de efectos
            if (ballTrail != null)
            {
                ballTrail.time = 0.5f;
                ballTrail.widthMultiplier = 0.5f;
            }
            
            if (grassParticles != null)
            {
                var main = grassParticles.main;
                main.maxParticles = 20;
            }
            
            if (impactParticles != null)
            {
                var main = impactParticles.main;
                main.maxParticles = 15;
            }
        }
    }
    
    void FixedUpdate()
    {
        if (Time.fixedTime >= nextFixedUpdate)
        {
            UpdateBallPhysics();
            nextFixedUpdate = Time.fixedTime + FIXED_UPDATE_INTERVAL;
        }
    }
    
    void UpdateBallPhysics()
    {
        lastVelocity = rb.velocity;
        currentSpeed = rb.velocity.magnitude;
        
        // Aplicar efecto Magnus (curvas)
        if (currentSpeed > 1f && spinVector.magnitude > 0.1f)
        {
            ApplyMagnusEffect();
        }
        
        // Controlar velocidad máxima
        if (currentSpeed > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
        
        // Verificar si está en el suelo
        CheckGroundContact();
        
        // Aplicar fricción del césped
        if (isGrounded && currentSpeed > 0.1f)
        {
            ApplyGroundFriction();
        }
        
        // Knuckleball effect
        if (isKnuckleBall && currentSpeed > 5f)
        {
            ApplyKnuckleBallEffect();
        }
    }
    
    void ApplyMagnusEffect()
    {
        // Calcular fuerza Magnus: F = k * (v × ω)
        Vector3 magnusForce = Vector3.Cross(rb.velocity, spinVector) * magnusEffect;
        
        // Aplicar fuerza gradualmente para evitar cambios bruscos
        rb.AddForce(magnusForce * Time.fixedDeltaTime * 100f, ForceMode.Force);
        
        // Reducir spin gradualmente
        spinVector = Vector3.Lerp(spinVector, Vector3.zero, Time.fixedDeltaTime * 2f);
    }
    
    void ApplyGroundFriction()
    {
        Vector3 frictionForce = -rb.velocity.normalized * groundFriction * 10f;
        rb.AddForce(frictionForce, ForceMode.Force);
    }
    
    void ApplyKnuckleBallEffect()
    {
        // Efecto knuckleball: movimiento impredecible
        Vector3 randomForce = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-0.5f, 0.5f),
            Random.Range(-1f, 1f)
        ) * 2f;
        
        rb.AddForce(randomForce, ForceMode.Force);
    }
    
    void CheckGroundContact()
    {
        float groundCheckDistance = ballCollider.radius + 0.1f;
        isGrounded = Physics.SphereCast(
            transform.position, 
            ballCollider.radius, 
            Vector3.down, 
            out RaycastHit hit, 
            groundCheckDistance,
            LayerMask.GetMask("Ground")
        );
    }
    
    public void KickBall(Vector3 direction, float force, Vector3 spin = default)
    {
        // Aplicar fuerza de patada
        Vector3 kickForce = direction.normalized * force * kickForceMultiplier;
        rb.AddForce(kickForce, ForceMode.Impulse);
        
        // Aplicar spin
        if (spin != Vector3.zero)
        {
            spinVector = spin * spinSensitivity;
            rb.AddTorque(spin * spinSensitivity * 10f, ForceMode.Impulse);
        }
        
        // Determinar si es knuckleball
        isKnuckleBall = (spin.magnitude < 0.1f && Random.value < knuckleBallChance);
        
        // Reproducir sonido
        PlayKickSound();
        
        // Activar trail
        if (ballTrail != null && force > 5f)
        {
            ballTrail.enabled = true;
            StartCoroutine(DisableTrailAfterTime(2f));
        }
    }
    
    public void ApplyCurve(CurveType curveType, float intensity)
    {
        Vector3 spin = Vector3.zero;
        
        switch (curveType)
        {
            case CurveType.Left:
                spin = Vector3.up * intensity;
                break;
            case CurveType.Right:
                spin = Vector3.down * intensity;
                break;
            case CurveType.Up:
                spin = Vector3.right * intensity;
                break;
            case CurveType.Down:
                spin = Vector3.left * intensity;
                break;
        }
        
        spinVector = spin;
        rb.AddTorque(spin * 5f, ForceMode.Impulse);
    }
    
    void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision);
    }
    
    void HandleCollision(Collision collision)
    {
        Vector3 impactPoint = collision.contacts[0].point;
        Vector3 impactNormal = collision.contacts[0].normal;
        float impactForce = collision.relativeVelocity.magnitude;
        
        // Reproducir sonido según la superficie
        PlayCollisionSound(collision.gameObject.tag, impactForce);
        
        // Crear efectos de partículas
        CreateImpactEffects(impactPoint, impactNormal, collision.gameObject.tag);
        
        // Aplicar rebote realista
        ApplyRealisticBounce(impactNormal, impactForce);
        
        // Vibración háptica en móviles
        if (impactForce > 3f)
        {
            TriggerHapticFeedback();
        }
    }
    
    void ApplyRealisticBounce(Vector3 normal, float impactForce)
    {
        // Calcular rebote basado en el material
        float bounceMultiplier = bounciness;
        
        // Ajustar según la velocidad de impacto
        bounceMultiplier *= Mathf.Clamp01(impactForce / 10f);
        
        // Aplicar rebote
        Vector3 reflection = Vector3.Reflect(lastVelocity, normal);
        rb.velocity = reflection * bounceMultiplier;
        
        // Agregar spin aleatorio en rebotes
        if (impactForce > 2f)
        {
            Vector3 randomSpin = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f)
            ) * 0.5f;
            
            rb.AddTorque(randomSpin, ForceMode.Impulse);
        }
    }
    
    void PlayKickSound()
    {
        if (kickSounds.Length > 0 && ballAudioSource != null)
        {
            AudioClip randomKick = kickSounds[Random.Range(0, kickSounds.Length)];
            ballAudioSource.PlayOneShot(randomKick);
        }
    }
    
    void PlayCollisionSound(string surfaceTag, float intensity)
    {
        if (ballAudioSource == null) return;
        
        AudioClip[] soundArray = null;
        
        switch (surfaceTag)
        {
            case "Ground":
            case "Grass":
                soundArray = bounceGroundSounds;
                break;
            case "Post":
            case "Crossbar":
                soundArray = bouncePostSounds;
                break;
        }
        
        if (soundArray != null && soundArray.Length > 0)
        {
            AudioClip sound = soundArray[Random.Range(0, soundArray.Length)];
            ballAudioSource.volume = Mathf.Clamp01(intensity / 10f);
            ballAudioSource.PlayOneShot(sound);
        }
    }
    
    void CreateImpactEffects(Vector3 point, Vector3 normal, string surfaceTag)
    {
        // Partículas de césped
        if (surfaceTag == "Grass" && grassParticles != null)
        {
            grassParticles.transform.position = point;
            grassParticles.transform.rotation = Quaternion.LookRotation(normal);
            grassParticles.Play();
        }
        
        // Partículas de impacto general
        if (impactParticles != null)
        {
            impactParticles.transform.position = point;
            impactParticles.transform.rotation = Quaternion.LookRotation(normal);
            impactParticles.Play();
        }
    }
    
    void TriggerHapticFeedback()
    {
        // Vibración para móviles
        if (Application.platform == RuntimePlatform.Android || 
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Handheld.Vibrate();
        }
    }
    
    PhysicMaterial CreateBallPhysicsMaterial()
    {
        PhysicMaterial ballMaterial = new PhysicMaterial("BallMaterial");
        ballMaterial.bounciness = bounciness;
        ballMaterial.friction = 0.4f;
        ballMaterial.frictionCombine = PhysicMaterialCombine.Average;
        ballMaterial.bounceCombine = PhysicMaterialCombine.Average;
        return ballMaterial;
    }
    
    void SetupTrailRenderer()
    {
        if (ballTrail == null)
        {
            ballTrail = gameObject.AddComponent<TrailRenderer>();
        }
        
        ballTrail.time = 1f;
        ballTrail.widthMultiplier = 0.3f;
        ballTrail.enabled = false;
        
        // Configurar colores
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { 
                new GradientColorKey(Color.white, 0.0f), 
                new GradientColorKey(Color.clear, 1.0f) 
            },
            new GradientAlphaKey[] { 
                new GradientAlphaKey(1.0f, 0.0f), 
                new GradientAlphaKey(0.0f, 1.0f) 
            }
        );
        ballTrail.colorGradient = gradient;
    }
    
    IEnumerator DisableTrailAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        if (ballTrail != null)
        {
            ballTrail.enabled = false;
        }
    }
    
    // Métodos públicos para otros sistemas
    public float GetCurrentSpeed() => currentSpeed;
    public bool IsGrounded() => isGrounded;
    public Vector3 GetSpinVector() => spinVector;
    public bool IsKnuckleBall() => isKnuckleBall;
    
    // Método para reiniciar el balón
    public void ResetBall(Vector3 position)
    {
        transform.position = position;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        spinVector = Vector3.zero;
        isKnuckleBall = false;
        
        if (ballTrail != null)
        {
            ballTrail.enabled = false;
        }
    }
}

[System.Serializable]
public enum CurveType
{
    Left,
    Right,
    Up,
    Down
}