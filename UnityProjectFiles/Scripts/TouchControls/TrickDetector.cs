using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Detector avanzado de trucos y gestos para controles táctiles
/// Reconoce patrones complejos de movimiento estilo FIFA móvil
/// </summary>
public class TrickDetector : MonoBehaviour
{
    [Header("🎯 Configuración de Detección")]
    [Range(0.1f, 1f)]
    public float gestureMatchThreshold = 0.7f;
    
    [Range(5, 50)]
    public int minGesturePoints = 8;
    
    [Range(50, 300)]
    public float gestureScaleRange = 150f;
    
    [Range(0.1f, 2f)]
    public float maxGestureTime = 1.5f;
    
    [Header("🎮 Sensibilidad de Trucos")]
    [Range(0.1f, 3f)]
    public float rouletteSensitivity = 1f;
    
    [Range(0.1f, 3f)]
    public float elasticoSensitivity = 1f;
    
    [Range(0.1f, 3f)]
    public float stepOverSensitivity = 1f;
    
    [Range(0.1f, 3f)]
    public float nutmegSensitivity = 1f;
    
    [Header("🌟 Efectos de Trucos")]
    public ParticleSystem trickParticles;
    public AudioSource trickAudioSource;
    public AudioClip[] trickSounds;
    
    [Header("📱 Optimización")]
    [Range(1, 10)]
    public int maxGestureHistory = 5;
    
    [Range(0.01f, 0.1f)]
    public float detectionUpdateRate = 0.05f;
    
    // Patrones de trucos predefinidos
    private Dictionary<TrickType, List<Vector2>> trickPatterns;
    private List<GestureData> gestureHistory;
    private float lastDetectionTime;
    
    // Optimización para móviles
    private bool isDetectionActive = true;
    private int processingFrame = 0;
    
    void Start()
    {
        InitializeTrickPatterns();
        InitializeGestureHistory();
        SetupOptimizationSettings();
    }
    
    void InitializeTrickPatterns()
    {
        trickPatterns = new Dictionary<TrickType, List<Vector2>>();
        
        // Roulette: patrón circular
        trickPatterns[TrickType.Roulette] = GenerateCircularPattern();
        
        // Elastico: patrón en L
        trickPatterns[TrickType.Elastico] = GenerateElasticoPattern();
        
        // Step-over: zigzag
        trickPatterns[TrickType.StepOver] = GenerateStepOverPattern();
        
        // Nutmeg: línea vertical rápida
        trickPatterns[TrickType.Nutmeg] = GenerateNutmegPattern();
        
        // Rainbow flick: arco hacia arriba
        trickPatterns[TrickType.RainbowFlick] = GenerateRainbowFlickPattern();
        
        // Rabona: curva externa
        trickPatterns[TrickType.Rabona] = GenerateRabonaPattern();
        
        // Heel flick: movimiento hacia atrás
        trickPatterns[TrickType.HeelFlick] = GenerateHeelFlickPattern();
        
        // Scorpion: movimiento complejo
        trickPatterns[TrickType.Scorpion] = GenerateScorpionPattern();
    }
    
    void InitializeGestureHistory()
    {
        gestureHistory = new List<GestureData>();
        
        // Configurar audio
        if (trickAudioSource == null)
        {
            trickAudioSource = gameObject.AddComponent<AudioSource>();
        }
        
        trickAudioSource.spatialBlend = 0f; // 2D sound
        trickAudioSource.volume = 0.8f;
    }
    
    void SetupOptimizationSettings()
    {
        // Optimización para dispositivos de gama baja
        if (SystemInfo.systemMemorySize <= 4096) // 4GB RAM o menos
        {
            maxGestureHistory = 3;
            detectionUpdateRate = 0.1f;
            
            // Reducir efectos de partículas
            if (trickParticles != null)
            {
                var main = trickParticles.main;
                main.maxParticles = 15;
            }
        }
    }
    
    public TrickType DetectTrick(List<Vector2> gesturePoints)
    {
        if (!isDetectionActive || gesturePoints.Count < minGesturePoints)
        {
            return TrickType.None;
        }
        
        // Controlar frecuencia de detección
        if (Time.time - lastDetectionTime < detectionUpdateRate)
        {
            return TrickType.None;
        }
        
        lastDetectionTime = Time.time;
        
        // Normalizar el gesto
        List<Vector2> normalizedGesture = NormalizeGesture(gesturePoints);
        
        // Detectar cada tipo de truco
        TrickType detectedTrick = TrickType.None;
        float bestMatch = 0f;
        
        foreach (var trickPattern in trickPatterns)
        {
            float matchScore = CalculateMatchScore(normalizedGesture, trickPattern.Value);
            
            if (matchScore > bestMatch && matchScore > gestureMatchThreshold)
            {
                bestMatch = matchScore;
                detectedTrick = trickPattern.Key;
            }
        }
        
        // Validar con detección especializada
        if (detectedTrick == TrickType.None)
        {
            detectedTrick = DetectSpecializedTricks(normalizedGesture);
        }
        
        // Guardar en historial
        if (detectedTrick != TrickType.None)
        {
            AddToGestureHistory(detectedTrick, normalizedGesture);
            PlayTrickEffect(detectedTrick);
        }
        
        return detectedTrick;
    }
    
    TrickType DetectSpecializedTricks(List<Vector2> gesture)
    {
        // Detección especializada para trucos específicos
        
        // Roulette: detectar movimiento circular
        if (IsCircularMotion(gesture))
        {
            return TrickType.Roulette;
        }
        
        // Elastico: detectar patrón en L
        if (IsLShapeMotion(gesture))
        {
            return TrickType.Elastico;
        }
        
        // Step-over: detectar zigzag
        if (IsZigzagMotion(gesture))
        {
            return TrickType.StepOver;
        }
        
        // Nutmeg: detectar línea vertical rápida
        if (IsVerticalLineMotion(gesture))
        {
            return TrickType.Nutmeg;
        }
        
        // Rainbow flick: detectar arco hacia arriba
        if (IsArcMotion(gesture))
        {
            return TrickType.RainbowFlick;
        }
        
        return TrickType.None;
    }
    
    bool IsCircularMotion(List<Vector2> gesture)
    {
        if (gesture.Count < 8) return false;
        
        Vector2 center = CalculateCenter(gesture);
        float averageRadius = CalculateAverageRadius(gesture, center);
        
        // Verificar si los puntos forman un círculo
        int circularPoints = 0;
        foreach (Vector2 point in gesture)
        {
            float distance = Vector2.Distance(point, center);
            if (Mathf.Abs(distance - averageRadius) < averageRadius * 0.3f)
            {
                circularPoints++;
            }
        }
        
        return (float)circularPoints / gesture.Count > 0.6f;
    }
    
    bool IsLShapeMotion(List<Vector2> gesture)
    {
        if (gesture.Count < 6) return false;
        
        // Dividir el gesto en dos segmentos
        int midPoint = gesture.Count / 2;
        List<Vector2> firstHalf = gesture.GetRange(0, midPoint);
        List<Vector2> secondHalf = gesture.GetRange(midPoint, gesture.Count - midPoint);
        
        // Verificar si el primer segmento es principalmente horizontal
        bool isFirstHorizontal = IsHorizontalMotion(firstHalf);
        
        // Verificar si el segundo segmento es principalmente vertical
        bool isSecondVertical = IsVerticalMotion(secondHalf);
        
        return isFirstHorizontal && isSecondVertical;
    }
    
    bool IsZigzagMotion(List<Vector2> gesture)
    {
        if (gesture.Count < 6) return false;
        
        int directionChanges = 0;
        Vector2 lastDirection = Vector2.zero;
        
        for (int i = 1; i < gesture.Count; i++)
        {
            Vector2 currentDirection = (gesture[i] - gesture[i-1]).normalized;
            
            if (lastDirection != Vector2.zero)
            {
                float angle = Vector2.Angle(lastDirection, currentDirection);
                if (angle > 45f) // Cambio de dirección significativo
                {
                    directionChanges++;
                }
            }
            
            lastDirection = currentDirection;
        }
        
        return directionChanges >= 2;
    }
    
    bool IsVerticalLineMotion(List<Vector2> gesture)
    {
        if (gesture.Count < 3) return false;
        
        Vector2 start = gesture[0];
        Vector2 end = gesture[gesture.Count - 1];
        
        // Verificar si es principalmente vertical
        float horizontalDistance = Mathf.Abs(end.x - start.x);
        float verticalDistance = Mathf.Abs(end.y - start.y);
        
        return verticalDistance > horizontalDistance * 2f;
    }
    
    bool IsArcMotion(List<Vector2> gesture)
    {
        if (gesture.Count < 5) return false;
        
        // Verificar si el gesto forma un arco
        Vector2 start = gesture[0];
        Vector2 end = gesture[gesture.Count - 1];
        Vector2 middle = gesture[gesture.Count / 2];
        
        // El punto medio debe estar por encima de la línea entre inicio y fin
        Vector2 midLine = (start + end) / 2f;
        
        return middle.y > midLine.y + 20f; // Altura mínima del arco
    }
    
    bool IsHorizontalMotion(List<Vector2> gesture)
    {
        if (gesture.Count < 2) return false;
        
        Vector2 start = gesture[0];
        Vector2 end = gesture[gesture.Count - 1];
        
        float horizontalDistance = Mathf.Abs(end.x - start.x);
        float verticalDistance = Mathf.Abs(end.y - start.y);
        
        return horizontalDistance > verticalDistance * 1.5f;
    }
    
    bool IsVerticalMotion(List<Vector2> gesture)
    {
        if (gesture.Count < 2) return false;
        
        Vector2 start = gesture[0];
        Vector2 end = gesture[gesture.Count - 1];
        
        float horizontalDistance = Mathf.Abs(end.x - start.x);
        float verticalDistance = Mathf.Abs(end.y - start.y);
        
        return verticalDistance > horizontalDistance * 1.5f;
    }
    
    List<Vector2> NormalizeGesture(List<Vector2> gesture)
    {
        if (gesture.Count == 0) return gesture;
        
        // Encontrar bounding box
        Vector2 min = gesture[0];
        Vector2 max = gesture[0];
        
        foreach (Vector2 point in gesture)
        {
            min = Vector2.Min(min, point);
            max = Vector2.Max(max, point);
        }
        
        Vector2 size = max - min;
        
        // Normalizar puntos
        List<Vector2> normalized = new List<Vector2>();
        foreach (Vector2 point in gesture)
        {
            Vector2 normalizedPoint = new Vector2(
                (point.x - min.x) / size.x,
                (point.y - min.y) / size.y
            );
            normalized.Add(normalizedPoint);
        }
        
        return normalized;
    }
    
    float CalculateMatchScore(List<Vector2> gesture, List<Vector2> pattern)
    {
        if (gesture.Count == 0 || pattern.Count == 0) return 0f;
        
        // Usar Dynamic Time Warping (DTW) simplificado
        float totalDistance = 0f;
        int matches = 0;
        
        for (int i = 0; i < gesture.Count; i++)
        {
            float minDistance = float.MaxValue;
            
            for (int j = 0; j < pattern.Count; j++)
            {
                float distance = Vector2.Distance(gesture[i], pattern[j]);
                minDistance = Mathf.Min(minDistance, distance);
            }
            
            totalDistance += minDistance;
            matches++;
        }
        
        float averageDistance = totalDistance / matches;
        return 1f - Mathf.Clamp01(averageDistance);
    }
    
    Vector2 CalculateCenter(List<Vector2> points)
    {
        Vector2 sum = Vector2.zero;
        foreach (Vector2 point in points)
        {
            sum += point;
        }
        return sum / points.Count;
    }
    
    float CalculateAverageRadius(List<Vector2> points, Vector2 center)
    {
        float totalRadius = 0f;
        foreach (Vector2 point in points)
        {
            totalRadius += Vector2.Distance(point, center);
        }
        return totalRadius / points.Count;
    }
    
    void AddToGestureHistory(TrickType trickType, List<Vector2> gesture)
    {
        GestureData gestureData = new GestureData
        {
            trickType = trickType,
            gesture = new List<Vector2>(gesture),
            timestamp = Time.time
        };
        
        gestureHistory.Add(gestureData);
        
        // Mantener solo los gestos más recientes
        if (gestureHistory.Count > maxGestureHistory)
        {
            gestureHistory.RemoveAt(0);
        }
    }
    
    void PlayTrickEffect(TrickType trickType)
    {
        // Reproducir sonido específico del truco
        if (trickSounds.Length > 0 && trickAudioSource != null)
        {
            int soundIndex = (int)trickType % trickSounds.Length;
            trickAudioSource.PlayOneShot(trickSounds[soundIndex]);
        }
        
        // Reproducir efectos de partículas
        if (trickParticles != null)
        {
            trickParticles.Play();
        }
        
        // Vibración háptica
        TriggerHapticFeedback(trickType);
    }
    
    void TriggerHapticFeedback(TrickType trickType)
    {
        if (Application.platform == RuntimePlatform.Android || 
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            // Vibración específica para cada truco
            switch (trickType)
            {
                case TrickType.Roulette:
                case TrickType.Elastico:
                    // Vibración media
                    Handheld.Vibrate();
                    break;
                    
                case TrickType.RainbowFlick:
                case TrickType.Scorpion:
                    // Vibración fuerte
                    Handheld.Vibrate();
                    break;
                    
                default:
                    // Vibración suave
                    Handheld.Vibrate();
                    break;
            }
        }
    }
    
    // Generadores de patrones
    List<Vector2> GenerateCircularPattern()
    {
        List<Vector2> pattern = new List<Vector2>();
        int points = 16;
        
        for (int i = 0; i < points; i++)
        {
            float angle = (i * 2 * Mathf.PI) / points;
            Vector2 point = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            pattern.Add(point);
        }
        
        return pattern;
    }
    
    List<Vector2> GenerateElasticoPattern()
    {
        List<Vector2> pattern = new List<Vector2>();
        
        // Segmento horizontal
        for (int i = 0; i < 5; i++)
        {
            pattern.Add(new Vector2(i * 0.25f, 0));
        }
        
        // Segmento vertical
        for (int i = 1; i < 5; i++)
        {
            pattern.Add(new Vector2(1, i * 0.25f));
        }
        
        return pattern;
    }
    
    List<Vector2> GenerateStepOverPattern()
    {
        List<Vector2> pattern = new List<Vector2>();
        
        // Zigzag
        for (int i = 0; i < 8; i++)
        {
            float x = i * 0.125f;
            float y = (i % 2 == 0) ? 0 : 0.5f;
            pattern.Add(new Vector2(x, y));
        }
        
        return pattern;
    }
    
    List<Vector2> GenerateNutmegPattern()
    {
        List<Vector2> pattern = new List<Vector2>();
        
        // Línea vertical
        for (int i = 0; i < 8; i++)
        {
            pattern.Add(new Vector2(0, i * 0.125f));
        }
        
        return pattern;
    }
    
    List<Vector2> GenerateRainbowFlickPattern()
    {
        List<Vector2> pattern = new List<Vector2>();
        
        // Arco hacia arriba
        for (int i = 0; i < 10; i++)
        {
            float t = i / 9f;
            float x = t;
            float y = Mathf.Sin(t * Mathf.PI) * 0.5f;
            pattern.Add(new Vector2(x, y));
        }
        
        return pattern;
    }
    
    List<Vector2> GenerateRabonaPattern()
    {
        List<Vector2> pattern = new List<Vector2>();
        
        // Curva externa
        for (int i = 0; i < 12; i++)
        {
            float t = i / 11f;
            float x = t;
            float y = Mathf.Sin(t * Mathf.PI * 2) * 0.3f;
            pattern.Add(new Vector2(x, y));
        }
        
        return pattern;
    }
    
    List<Vector2> GenerateHeelFlickPattern()
    {
        List<Vector2> pattern = new List<Vector2>();
        
        // Movimiento hacia atrás
        for (int i = 0; i < 6; i++)
        {
            pattern.Add(new Vector2(1 - i * 0.2f, 0));
        }
        
        return pattern;
    }
    
    List<Vector2> GenerateScorpionPattern()
    {
        List<Vector2> pattern = new List<Vector2>();
        
        // Patrón complejo en S
        for (int i = 0; i < 15; i++)
        {
            float t = i / 14f;
            float x = t;
            float y = Mathf.Sin(t * Mathf.PI * 3) * 0.3f;
            pattern.Add(new Vector2(x, y));
        }
        
        return pattern;
    }
    
    // Métodos públicos
    public void SetDetectionActive(bool active)
    {
        isDetectionActive = active;
    }
    
    public List<TrickType> GetRecentTricks(float timeWindow = 5f)
    {
        float currentTime = Time.time;
        return gestureHistory
            .Where(g => currentTime - g.timestamp < timeWindow)
            .Select(g => g.trickType)
            .ToList();
    }
    
    public void ClearGestureHistory()
    {
        gestureHistory.Clear();
    }
    
    public float GetTrickDifficulty(TrickType trickType)
    {
        // Retornar dificultad del truco (0-1)
        switch (trickType)
        {
            case TrickType.StepOver:
                return 0.3f;
            case TrickType.Roulette:
                return 0.5f;
            case TrickType.Elastico:
                return 0.7f;
            case TrickType.RainbowFlick:
                return 0.8f;
            case TrickType.Scorpion:
                return 1f;
            default:
                return 0.5f;
        }
    }
}

[System.Serializable]
public class GestureData
{
    public TrickType trickType;
    public List<Vector2> gesture;
    public float timestamp;
}

[System.Serializable]
public enum TrickType
{
    None,
    StepOver,
    Roulette,
    Elastico,
    Nutmeg,
    RainbowFlick,
    Rabona,
    HeelFlick,
    Scorpion
}