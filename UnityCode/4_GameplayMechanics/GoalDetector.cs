using UnityEngine;

public class GoalDetector : MonoBehaviour
{
    [Header("Goal Settings")]
    public int goalForTeam; // ID del equipo que anota al entrar en esta portería
    public bool isHomeGoal = false;
    
    [Header("Effects")]
    public ParticleSystem goalEffect;
    public AudioSource goalAudio;
    public AudioClip goalSound;
    
    [Header("Visual Feedback")]
    public GameObject goalText;
    public Light goalLight;
    public Color goalColor = Color.green;
    
    private GameManager gameManager;
    private bool goalScored = false;
    
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        
        // Configurar el trigger
        GetComponent<Collider>().isTrigger = true;
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Verificar si es el balón
        if (other.CompareTag("Ball") && !goalScored)
        {
            BallController ballController = other.GetComponent<BallController>();
            if (ballController != null)
            {
                RegisterGoal(ballController);
            }
        }
    }
    
    void RegisterGoal(BallController ballController)
    {
        goalScored = true;
        
        // Encontrar quién pateó el balón por última vez
        PlayerController lastKicker = FindLastKicker();
        
        if (lastKicker != null)
        {
            // Registrar el gol
            gameManager.ScoreGoal(goalForTeam, lastKicker);
            
            // Efectos visuales y sonoros
            PlayGoalEffects();
            
            // Actualizar estadísticas del jugador
            UpdatePlayerStats(lastKicker);
        }
        
        // Resetear después de un tiempo
        Invoke("ResetGoalDetector", 2f);
    }
    
    PlayerController FindLastKicker()
    {
        // Encontrar el jugador más cercano al balón (simplificado)
        // En un sistema más complejo, trackearíamos el último jugador que tocó el balón
        PlayerController[] allPlayers = FindObjectsOfType<PlayerController>();
        PlayerController closest = null;
        float closestDistance = float.MaxValue;
        
        foreach (PlayerController player in allPlayers)
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = player;
            }
        }
        
        return closest;
    }
    
    void PlayGoalEffects()
    {
        // Efecto de partículas
        if (goalEffect != null)
        {
            goalEffect.Play();
        }
        
        // Sonido de gol
        if (goalAudio != null && goalSound != null)
        {
            goalAudio.PlayOneShot(goalSound);
        }
        
        // Efecto de luz
        if (goalLight != null)
        {
            StartCoroutine(FlashGoalLight());
        }
        
        // Texto de gol
        if (goalText != null)
        {
            goalText.SetActive(true);
            Invoke("HideGoalText", 3f);
        }
    }
    
    System.Collections.IEnumerator FlashGoalLight()
    {
        Color originalColor = goalLight.color;
        
        for (int i = 0; i < 6; i++)
        {
            goalLight.color = goalColor;
            yield return new WaitForSeconds(0.2f);
            goalLight.color = originalColor;
            yield return new WaitForSeconds(0.2f);
        }
    }
    
    void HideGoalText()
    {
        if (goalText != null)
        {
            goalText.SetActive(false);
        }
    }
    
    void UpdatePlayerStats(PlayerController player)
    {
        // Actualizar estadísticas del jugador (goles, etc.)
        PlayerStats stats = player.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.goals++;
            stats.UpdateStats();
        }
    }
    
    void ResetGoalDetector()
    {
        goalScored = false;
    }
    
    void OnDrawGizmos()
    {
        // Visualizar el área de gol en el editor
        Gizmos.color = goalColor;
        Gizmos.DrawWireCube(transform.position, GetComponent<Collider>().bounds.size);
    }
}