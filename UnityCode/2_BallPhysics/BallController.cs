using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallController : MonoBehaviour
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
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip kickSound;
    public AudioClip bounceSound;
    public AudioClip grassSound;
    
    private Rigidbody rb;
    private Vector3 spinVector;
    private float initialSpeed;
    private float timeSinceKick;
    private bool isGrounded;
    private Vector3 lastVelocity;
    
    // Magnus effect (efecto Magnus para curvas)
    private Vector3 magnusForce;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.material.bounciness = 0.6f;
        rb.material.dynamicFriction = 0.4f;
        rb.material.staticFriction = 0.4f;
        
        // Configurar física realista
        rb.drag = 0.05f;
        rb.angularDrag = 0.3f;
        
        lastVelocity = rb.velocity;
    }
    
    void Update()
    {
        timeSinceKick += Time.deltaTime;
        
        // Aplicar resistencia del aire
        ApplyAirResistance();
        
        // Aplicar efecto Magnus (curvas)
        ApplyMagnusEffect();
        
        // Detectar si está en el suelo
        CheckGrounded();
        
        // Aplicar fricción del suelo
        if (isGrounded)
        {
            ApplyGroundFriction();
        }
        
        // Reducir el spin gradualmente
        spinVector *= spinDecay;
        
        lastVelocity = rb.velocity;
    }
    
    void ApplyAirResistance()
    {
        if (!isGrounded)
        {
            rb.velocity = rb.velocity * airResistance;
        }
    }
    
    void ApplyMagnusEffect()
    {
        if (spinVector.magnitude > 0.1f && rb.velocity.magnitude > 1f)
        {
            // Calcular el efecto Magnus
            magnusForce = Vector3.Cross(spinVector, rb.velocity.normalized) * magnusEffectStrength;
            
            // Aplicar la curva con falloff basado en el tiempo
            float curveStrength = curveFalloff.Evaluate(timeSinceKick / 3f);
            magnusForce *= curveStrength;
            
            rb.AddForce(magnusForce, ForceMode.Force);
        }
    }
    
    void CheckGrounded()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.6f);
    }
    
    void ApplyGroundFriction()
    {
        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        horizontalVelocity *= groundFriction;
        
        rb.velocity = new Vector3(horizontalVelocity.x, rb.velocity.y, horizontalVelocity.z);
    }
    
    public void KickBall(Vector3 direction, float power, Vector3 spin = default)
    {
        // Resetear el tiempo desde el último golpe
        timeSinceKick = 0f;
        
        // Aplicar fuerza basada en la dirección y potencia
        float finalPower = Mathf.Clamp(power, 0.1f, 2f);
        Vector3 kickForce = direction * finalPower * 1000f;
        
        rb.AddForce(kickForce, ForceMode.Impulse);
        
        // Aplicar spin para curvas
        if (spin != Vector3.zero)
        {
            spinVector = spin * curveMultiplier;
            rb.AddTorque(spin * 200f, ForceMode.Impulse);
        }
        
        // Guardar velocidad inicial para cálculos
        initialSpeed = rb.velocity.magnitude;
        
        // Reproducir sonido de golpe
        if (audioSource && kickSound)
        {
            audioSource.PlayOneShot(kickSound);
        }
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
                // Sin spin, pero con turbulencia
                StartCoroutine(ApplyKnuckleballEffect());
                break;
        }
        
        KickBall(direction, power, spin);
    }
    
    System.Collections.IEnumerator ApplyKnuckleballEffect()
    {
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(0.1f);
            
            // Aplicar pequeñas fuerzas aleatorias
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
        // Manejar rebotes
        if (collision.gameObject.CompareTag("Ground"))
        {
            HandleGroundBounce(collision);
        }
        else if (collision.gameObject.CompareTag("Post") || collision.gameObject.CompareTag("Crossbar"))
        {
            HandlePostBounce(collision);
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            HandlePlayerCollision(collision);
        }
        
        // Reproducir sonido de rebote
        if (audioSource && bounceSound && rb.velocity.magnitude > 2f)
        {
            audioSource.PlayOneShot(bounceSound, rb.velocity.magnitude * 0.1f);
        }
    }
    
    void HandleGroundBounce(Collision collision)
    {
        Vector3 incomingVector = lastVelocity;
        Vector3 reflectedVector = Vector3.Reflect(incomingVector, collision.contacts[0].normal);
        
        // Reducir la velocidad del rebote
        float bounceReduction = 0.7f;
        rb.velocity = reflectedVector * bounceReduction;
        
        // Reproducir sonido de césped
        if (audioSource && grassSound)
        {
            audioSource.PlayOneShot(grassSound, 0.3f);
        }
    }
    
    void HandlePostBounce(Collision collision)
    {
        Vector3 incomingVector = lastVelocity;
        Vector3 reflectedVector = Vector3.Reflect(incomingVector, collision.contacts[0].normal);
        
        // Rebote más fuerte en los postes
        float bounceReduction = 0.8f;
        rb.velocity = reflectedVector * bounceReduction;
        
        // Añadir un poco de spin aleatorio
        Vector3 randomSpin = new Vector3(
            Random.Range(-2f, 2f),
            Random.Range(-2f, 2f),
            Random.Range(-2f, 2f)
        );
        
        rb.AddTorque(randomSpin, ForceMode.Impulse);
    }
    
    void HandlePlayerCollision(Collision collision)
    {
        // Obtener el componente del jugador
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        
        if (player != null)
        {
            // Aplicar el efecto basado en la parte del cuerpo que tocó
            BodyPart bodyPart = GetBodyPartFromCollision(collision);
            ApplyBodyPartEffect(bodyPart, player);
        }
    }
    
    BodyPart GetBodyPartFromCollision(Collision collision)
    {
        // Determinar qué parte del cuerpo tocó el balón basado en la altura
        float collisionHeight = collision.contacts[0].point.y;
        float playerHeight = collision.transform.position.y;
        
        float relativeHeight = collisionHeight - playerHeight;
        
        if (relativeHeight > 1.5f)
            return BodyPart.Head;
        else if (relativeHeight > 0.8f)
            return BodyPart.Chest;
        else if (relativeHeight > 0.3f)
            return BodyPart.Foot;
        else
            return BodyPart.Foot;
    }
    
    void ApplyBodyPartEffect(BodyPart bodyPart, PlayerController player)
    {
        switch (bodyPart)
        {
            case BodyPart.Head:
                // Cabeceo - más potencia hacia abajo
                rb.AddForce(Vector3.down * 300f, ForceMode.Impulse);
                break;
                
            case BodyPart.Chest:
                // Pecho - control suave
                rb.velocity *= 0.5f;
                break;
                
            case BodyPart.Foot:
                // Pie - según la habilidad del jugador
                float skillMultiplier = player.GetSkillLevel() * 0.1f;
                rb.velocity *= (1f + skillMultiplier);
                break;
        }
    }
    
    // Método para obtener información del balón
    public BallInfo GetBallInfo()
    {
        return new BallInfo
        {
            position = transform.position,
            velocity = rb.velocity,
            spin = spinVector,
            isGrounded = isGrounded,
            speed = rb.velocity.magnitude
        };
    }
    
    // Método para predecir la trayectoria
    public Vector3[] PredictTrajectory(int steps = 50)
    {
        Vector3[] trajectory = new Vector3[steps];
        
        Vector3 currentPos = transform.position;
        Vector3 currentVel = rb.velocity;
        
        for (int i = 0; i < steps; i++)
        {
            trajectory[i] = currentPos;
            
            // Simular física básica
            currentVel += Physics.gravity * Time.fixedDeltaTime;
            currentVel *= airResistance;
            currentPos += currentVel * Time.fixedDeltaTime;
            
            // Parar si toca el suelo
            if (currentPos.y <= 0)
            {
                trajectory[i] = new Vector3(currentPos.x, 0, currentPos.z);
                break;
            }
        }
        
        return trajectory;
    }
}

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

[System.Serializable]
public class BallInfo
{
    public Vector3 position;
    public Vector3 velocity;
    public Vector3 spin;
    public bool isGrounded;
    public float speed;
}