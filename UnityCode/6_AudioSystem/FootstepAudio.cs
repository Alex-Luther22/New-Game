using UnityEngine;

public class FootstepAudio : MonoBehaviour
{
    [Header("Footstep Settings")]
    public AudioClip[] grassFootsteps;
    public AudioClip[] dirtFootsteps;
    public AudioClip[] concreteFootsteps;
    
    [Header("Audio Settings")]
    public float stepInterval = 0.5f;
    public float volumeVariation = 0.1f;
    public float pitchVariation = 0.2f;
    
    private AudioSource audioSource;
    private PlayerController playerController;
    private UnityEngine.AI.NavMeshAgent agent;
    private float lastStepTime;
    private bool isMoving = false;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        playerController = GetComponent<PlayerController>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        
        // Configurar audio source
        audioSource.spatialBlend = 1f; // 3D sound
        audioSource.volume = 0.5f;
        audioSource.pitch = 1f;
    }
    
    void Update()
    {
        CheckMovement();
        
        if (isMoving && Time.time - lastStepTime >= stepInterval)
        {
            PlayFootstep();
            lastStepTime = Time.time;
        }
    }
    
    void CheckMovement()
    {
        if (agent != null)
        {
            isMoving = agent.velocity.magnitude > 0.1f;
            
            // Ajustar intervalo basado en velocidad
            float speedMultiplier = agent.velocity.magnitude / agent.speed;
            stepInterval = Mathf.Lerp(0.6f, 0.3f, speedMultiplier);
        }
    }
    
    void PlayFootstep()
    {
        AudioClip[] footstepsToUse = GetFootstepsForSurface();
        
        if (footstepsToUse != null && footstepsToUse.Length > 0)
        {
            AudioClip stepClip = footstepsToUse[Random.Range(0, footstepsToUse.Length)];
            
            // Variación de volumen y pitch
            float volume = audioSource.volume + Random.Range(-volumeVariation, volumeVariation);
            float pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
            
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(stepClip, volume);
        }
    }
    
    AudioClip[] GetFootstepsForSurface()
    {
        // Detectar superficie con raycast
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out hit, 1f))
        {
            string surfaceTag = hit.collider.tag;
            
            switch (surfaceTag)
            {
                case "Grass":
                    return grassFootsteps;
                case "Dirt":
                    return dirtFootsteps;
                case "Concrete":
                    return concreteFootsteps;
                default:
                    return grassFootsteps; // Por defecto césped
            }
        }
        
        return grassFootsteps;
    }
}