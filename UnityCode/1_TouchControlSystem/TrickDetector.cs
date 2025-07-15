using UnityEngine;
using System.Collections.Generic;

public enum TrickType
{
    StepOverRight,
    StepOverLeft,
    Nutmeg,
    Roulette,
    Rabona,
    Elastico,
    CutBack,
    HeelFlick,
    RainbowFlick,
    Scorpion
}

public class TrickDetector : MonoBehaviour
{
    [Header("Trick Settings")]
    public float gestureTimeWindow = 1.0f;
    public float minGestureDistance = 30f;
    public float maxGestureDistance = 200f;
    
    private List<Vector2> gesturePoints = new List<Vector2>();
    private List<float> gestureTimestamps = new List<float>();
    
    public System.Action<TrickType> OnTrickDetected;
    
    void Update()
    {
        // Limpiar gestos antiguos
        CleanOldGestures();
    }
    
    public void AddGesturePoint(Vector2 point)
    {
        gesturePoints.Add(point);
        gestureTimestamps.Add(Time.time);
        
        // Analizar patrón si tenemos suficientes puntos
        if (gesturePoints.Count >= 3)
        {
            AnalyzeGesture();
        }
    }
    
    void CleanOldGestures()
    {
        float currentTime = Time.time;
        
        for (int i = gestureTimestamps.Count - 1; i >= 0; i--)
        {
            if (currentTime - gestureTimestamps[i] > gestureTimeWindow)
            {
                gesturePoints.RemoveAt(i);
                gestureTimestamps.RemoveAt(i);
            }
        }
    }
    
    void AnalyzeGesture()
    {
        if (gesturePoints.Count < 3) return;
        
        // Obtener los últimos 3 puntos
        Vector2 point1 = gesturePoints[gesturePoints.Count - 3];
        Vector2 point2 = gesturePoints[gesturePoints.Count - 2];
        Vector2 point3 = gesturePoints[gesturePoints.Count - 1];
        
        // Calcular direcciones
        Vector2 dir1 = (point2 - point1).normalized;
        Vector2 dir2 = (point3 - point2).normalized;
        
        // Calcular distancias
        float dist1 = Vector2.Distance(point1, point2);
        float dist2 = Vector2.Distance(point2, point3);
        
        // Verificar si las distancias están en el rango válido
        if (dist1 < minGestureDistance || dist2 < minGestureDistance ||
            dist1 > maxGestureDistance || dist2 > maxGestureDistance)
            return;
        
        // Detectar patrones específicos
        TrickType detectedTrick = DetectTrickPattern(dir1, dir2, dist1, dist2);
        
        if (detectedTrick != TrickType.StepOverRight) // Usar como valor por defecto
        {
            OnTrickDetected?.Invoke(detectedTrick);
            ClearGesture();
        }
    }
    
    TrickType DetectTrickPattern(Vector2 dir1, Vector2 dir2, float dist1, float dist2)
    {
        float angle = Vector2.Angle(dir1, dir2);
        
        // Patrón circular (Roulette)
        if (IsCircularPattern())
        {
            return TrickType.Roulette;
        }
        
        // Patrón en L (Elastico)
        if (angle > 60f && angle < 120f)
        {
            if (dir1.x > 0 && dir2.y > 0)
                return TrickType.Elastico;
        }
        
        // Patrón en zigzag (Step Over)
        if (angle > 120f && angle < 180f)
        {
            if (dir1.x > 0)
                return TrickType.StepOverRight;
            else
                return TrickType.StepOverLeft;
        }
        
        // Patrón vertical rápido (Nutmeg)
        if (Mathf.Abs(dir1.y) > 0.8f && Mathf.Abs(dir2.y) > 0.8f)
        {
            return TrickType.Nutmeg;
        }
        
        // Patrón hacia atrás (CutBack)
        if (dir1.y > 0 && dir2.y < -0.5f)
        {
            return TrickType.CutBack;
        }
        
        // Patrón curvo hacia arriba (Rainbow Flick)
        if (IsRainbowPattern())
        {
            return TrickType.RainbowFlick;
        }
        
        return TrickType.StepOverRight; // Por defecto
    }
    
    bool IsCircularPattern()
    {
        if (gesturePoints.Count < 4) return false;
        
        float totalAngle = 0f;
        
        for (int i = 1; i < gesturePoints.Count - 1; i++)
        {
            Vector2 dir1 = (gesturePoints[i] - gesturePoints[i-1]).normalized;
            Vector2 dir2 = (gesturePoints[i+1] - gesturePoints[i]).normalized;
            
            float angle = Vector2.SignedAngle(dir1, dir2);
            totalAngle += angle;
        }
        
        return Mathf.Abs(totalAngle) > 180f; // Más de media vuelta
    }
    
    bool IsRainbowPattern()
    {
        if (gesturePoints.Count < 5) return false;
        
        // Verificar si el patrón forma un arco hacia arriba
        float minY = float.MaxValue;
        float maxY = float.MinValue;
        
        foreach (Vector2 point in gesturePoints)
        {
            if (point.y < minY) minY = point.y;
            if (point.y > maxY) maxY = point.y;
        }
        
        float arcHeight = maxY - minY;
        return arcHeight > 50f; // Suficiente altura para ser considerado arco
    }
    
    public void ClearGesture()
    {
        gesturePoints.Clear();
        gestureTimestamps.Clear();
    }
    
    // Método para debugging - mostrar el gesto actual
    void OnDrawGizmos()
    {
        if (gesturePoints.Count < 2) return;
        
        Gizmos.color = Color.red;
        
        for (int i = 1; i < gesturePoints.Count; i++)
        {
            Vector3 worldPos1 = Camera.main.ScreenToWorldPoint(new Vector3(gesturePoints[i-1].x, gesturePoints[i-1].y, 10f));
            Vector3 worldPos2 = Camera.main.ScreenToWorldPoint(new Vector3(gesturePoints[i].x, gesturePoints[i].y, 10f));
            
            Gizmos.DrawLine(worldPos1, worldPos2);
        }
    }
}