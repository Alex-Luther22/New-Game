using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Match Statistics")]
    public int goals = 0;
    public int assists = 0;
    public int shots = 0;
    public int shotsOnTarget = 0;
    public int passes = 0;
    public int passesCompleted = 0;
    public int tackles = 0;
    public int tacklesWon = 0;
    public int interceptions = 0;
    public int fouls = 0;
    public int yellowCards = 0;
    public int redCards = 0;
    
    [Header("Advanced Statistics")]
    public float distanceRun = 0f;
    public float topSpeed = 0f;
    public int touches = 0;
    public int dribbles = 0;
    public int dribblesSuccessful = 0;
    public int crosses = 0;
    public int crossesSuccessful = 0;
    public int headers = 0;
    public int blocks = 0;
    public int saves = 0; // Para porteros
    
    [Header("Performance Metrics")]
    public float averagePosition = 0f;
    public float heatMapIntensity = 0f;
    public int duelsWon = 0;
    public int duelsLost = 0;
    public float possessionTime = 0f;
    
    private PlayerController playerController;
    private Vector3 lastPosition;
    private float lastUpdateTime;
    private bool hadBallLastFrame = false;
    private float ballPossessionStartTime;
    
    // Para tracking de posición
    private System.Collections.Generic.List<Vector3> positionHistory = new System.Collections.Generic.List<Vector3>();
    private float positionUpdateInterval = 1f;
    private float lastPositionUpdate;
    
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        lastPosition = transform.position;
        lastUpdateTime = Time.time;
    }
    
    void Update()
    {
        UpdateBasicStats();
        UpdatePositionTracking();
        UpdatePossessionTracking();
    }
    
    void UpdateBasicStats()
    {
        // Actualizar distancia recorrida
        float distanceThisFrame = Vector3.Distance(transform.position, lastPosition);
        distanceRun += distanceThisFrame;
        lastPosition = transform.position;
        
        // Actualizar velocidad máxima
        if (playerController != null)
        {
            float currentSpeed = playerController.GetComponent<UnityEngine.AI.NavMeshAgent>().velocity.magnitude;
            if (currentSpeed > topSpeed)
            {
                topSpeed = currentSpeed;
            }
        }
    }
    
    void UpdatePositionTracking()
    {
        if (Time.time - lastPositionUpdate >= positionUpdateInterval)
        {
            positionHistory.Add(transform.position);
            
            // Mantener solo los últimos 100 puntos
            if (positionHistory.Count > 100)
            {
                positionHistory.RemoveAt(0);
            }
            
            // Calcular posición promedio
            Vector3 averagePos = Vector3.zero;
            foreach (Vector3 pos in positionHistory)
            {
                averagePos += pos;
            }
            averagePos /= positionHistory.Count;
            averagePosition = averagePos.z; // Posición promedio en el campo
            
            lastPositionUpdate = Time.time;
        }
    }
    
    void UpdatePossessionTracking()
    {
        if (playerController != null)
        {
            bool hasBall = playerController.GetPlayerInfo().hasBall;
            
            if (hasBall && !hadBallLastFrame)
            {
                // Comenzó a tener el balón
                ballPossessionStartTime = Time.time;
                touches++;
            }
            else if (!hasBall && hadBallLastFrame)
            {
                // Perdió el balón
                possessionTime += Time.time - ballPossessionStartTime;
            }
            
            hadBallLastFrame = hasBall;
        }
    }
    
    public void RecordShot(bool onTarget)
    {
        shots++;
        if (onTarget)
        {
            shotsOnTarget++;
        }
    }
    
    public void RecordGoal()
    {
        goals++;
    }
    
    public void RecordAssist()
    {
        assists++;
    }
    
    public void RecordPass(bool successful)
    {
        passes++;
        if (successful)
        {
            passesCompleted++;
        }
    }
    
    public void RecordTackle(bool successful)
    {
        tackles++;
        if (successful)
        {
            tacklesWon++;
        }
    }
    
    public void RecordInterception()
    {
        interceptions++;
    }
    
    public void RecordFoul()
    {
        fouls++;
    }
    
    public void RecordCard(CardType cardType)
    {
        switch (cardType)
        {
            case CardType.Yellow:
                yellowCards++;
                break;
            case CardType.Red:
                redCards++;
                break;
        }
    }
    
    public void RecordDribble(bool successful)
    {
        dribbles++;
        if (successful)
        {
            dribblesSuccessful++;
        }
    }
    
    public void RecordCross(bool successful)
    {
        crosses++;
        if (successful)
        {
            crossesSuccessful++;
        }
    }
    
    public void RecordHeader()
    {
        headers++;
    }
    
    public void RecordBlock()
    {
        blocks++;
    }
    
    public void RecordSave()
    {
        saves++;
    }
    
    public void RecordDuel(bool won)
    {
        if (won)
        {
            duelsWon++;
        }
        else
        {
            duelsLost++;
        }
    }
    
    // Métodos para obtener porcentajes y estadísticas calculadas
    public float GetShotAccuracy()
    {
        return shots > 0 ? (float)shotsOnTarget / shots * 100f : 0f;
    }
    
    public float GetPassAccuracy()
    {
        return passes > 0 ? (float)passesCompleted / passes * 100f : 0f;
    }
    
    public float GetTackleSuccessRate()
    {
        return tackles > 0 ? (float)tacklesWon / tackles * 100f : 0f;
    }
    
    public float GetDribbleSuccessRate()
    {
        return dribbles > 0 ? (float)dribblesSuccessful / dribbles * 100f : 0f;
    }
    
    public float GetCrossAccuracy()
    {
        return crosses > 0 ? (float)crossesSuccessful / crosses * 100f : 0f;
    }
    
    public float GetDuelSuccessRate()
    {
        int totalDuels = duelsWon + duelsLost;
        return totalDuels > 0 ? (float)duelsWon / totalDuels * 100f : 0f;
    }
    
    public float GetOverallRating()
    {
        // Calcular rating basado en rendimiento
        float rating = 6.0f; // Rating base
        
        // Bonus por goles y asistencias
        rating += goals * 0.3f;
        rating += assists * 0.2f;
        
        // Bonus por precisión de pases
        rating += GetPassAccuracy() * 0.01f;
        
        // Bonus por tackles exitosos
        rating += tacklesWon * 0.1f;
        
        // Bonus por intercepciones
        rating += interceptions * 0.05f;
        
        // Penalización por faltas y tarjetas
        rating -= fouls * 0.1f;
        rating -= yellowCards * 0.2f;
        rating -= redCards * 2f;
        
        return Mathf.Clamp(rating, 1f, 10f);
    }
    
    public void UpdateStats()
    {
        // Método para actualizar estadísticas complejas
        // Calcular intensidad del mapa de calor
        CalculateHeatMapIntensity();
    }
    
    void CalculateHeatMapIntensity()
    {
        // Calcular intensidad basada en tiempo en diferentes áreas
        float intensity = 0f;
        
        // Más intensidad por toques y tiempo de posesión
        intensity += touches * 0.1f;
        intensity += possessionTime * 0.05f;
        
        // Más intensidad por distancia recorrida
        intensity += distanceRun * 0.01f;
        
        heatMapIntensity = intensity;
    }
    
    // Método para obtener estadísticas completas
    public PlayerMatchStats GetCompleteStats()
    {
        return new PlayerMatchStats
        {
            playerName = playerController.playerData.playerName,
            goals = goals,
            assists = assists,
            shots = shots,
            shotsOnTarget = shotsOnTarget,
            shotAccuracy = GetShotAccuracy(),
            passes = passes,
            passesCompleted = passesCompleted,
            passAccuracy = GetPassAccuracy(),
            tackles = tackles,
            tacklesWon = tacklesWon,
            tackleSuccessRate = GetTackleSuccessRate(),
            interceptions = interceptions,
            fouls = fouls,
            yellowCards = yellowCards,
            redCards = redCards,
            distanceRun = distanceRun,
            topSpeed = topSpeed,
            touches = touches,
            dribbles = dribbles,
            dribblesSuccessful = dribblesSuccessful,
            dribbleSuccessRate = GetDribbleSuccessRate(),
            overallRating = GetOverallRating(),
            averagePosition = averagePosition,
            possessionTime = possessionTime,
            duelsWon = duelsWon,
            duelsLost = duelsLost,
            duelSuccessRate = GetDuelSuccessRate()
        };
    }
    
    // Método para resetear estadísticas
    public void ResetStats()
    {
        goals = 0;
        assists = 0;
        shots = 0;
        shotsOnTarget = 0;
        passes = 0;
        passesCompleted = 0;
        tackles = 0;
        tacklesWon = 0;
        interceptions = 0;
        fouls = 0;
        yellowCards = 0;
        redCards = 0;
        distanceRun = 0f;
        topSpeed = 0f;
        touches = 0;
        dribbles = 0;
        dribblesSuccessful = 0;
        crosses = 0;
        crossesSuccessful = 0;
        headers = 0;
        blocks = 0;
        saves = 0;
        duelsWon = 0;
        duelsLost = 0;
        possessionTime = 0f;
        positionHistory.Clear();
    }
}

public enum CardType
{
    Yellow,
    Red
}

[System.Serializable]
public class PlayerMatchStats
{
    public string playerName;
    public int goals;
    public int assists;
    public int shots;
    public int shotsOnTarget;
    public float shotAccuracy;
    public int passes;
    public int passesCompleted;
    public float passAccuracy;
    public int tackles;
    public int tacklesWon;
    public float tackleSuccessRate;
    public int interceptions;
    public int fouls;
    public int yellowCards;
    public int redCards;
    public float distanceRun;
    public float topSpeed;
    public int touches;
    public int dribbles;
    public int dribblesSuccessful;
    public float dribbleSuccessRate;
    public float overallRating;
    public float averagePosition;
    public float possessionTime;
    public int duelsWon;
    public int duelsLost;
    public float duelSuccessRate;
}