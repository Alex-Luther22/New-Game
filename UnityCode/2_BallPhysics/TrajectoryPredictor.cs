using UnityEngine;

public class TrajectoryPredictor : MonoBehaviour
{
    [Header("Trajectory Settings")]
    public int trajectoryPoints = 50;
    public float timeStep = 0.1f;
    public float maxTrajectoryTime = 5f;
    
    [Header("Visualization")]
    public LineRenderer trajectoryLine;
    public GameObject trajectoryPointPrefab;
    public Color trajectoryColor = Color.yellow;
    public float lineWidth = 0.05f;
    
    private GameObject[] trajectoryPoints;
    private BallController ballController;
    
    void Start()
    {
        ballController = FindObjectOfType<BallController>();
        
        // Configurar LineRenderer
        if (trajectoryLine == null)
        {
            trajectoryLine = gameObject.AddComponent<LineRenderer>();
        }
        
        trajectoryLine.material = new Material(Shader.Find("Sprites/Default"));
        trajectoryLine.color = trajectoryColor;
        trajectoryLine.startWidth = lineWidth;
        trajectoryLine.endWidth = lineWidth;
        trajectoryLine.positionCount = trajectoryPoints;
        
        // Crear puntos de trayectoria
        trajectoryPoints = new GameObject[trajectoryPoints];
        for (int i = 0; i < trajectoryPoints; i++)
        {
            if (trajectoryPointPrefab != null)
            {
                trajectoryPoints[i] = Instantiate(trajectoryPointPrefab);
                trajectoryPoints[i].SetActive(false);
            }
        }
    }
    
    public void ShowTrajectory(Vector3 startPos, Vector3 initialVelocity, Vector3 spin)
    {
        Vector3[] trajectory = CalculateTrajectory(startPos, initialVelocity, spin);
        
        // Actualizar LineRenderer
        trajectoryLine.positionCount = trajectory.Length;
        trajectoryLine.SetPositions(trajectory);
        
        // Actualizar puntos de trayectoria
        for (int i = 0; i < trajectoryPoints.Length && i < trajectory.Length; i++)
        {
            if (trajectoryPoints[i] != null)
            {
                trajectoryPoints[i].transform.position = trajectory[i];
                trajectoryPoints[i].SetActive(true);
            }
        }
        
        // Ocultar puntos no utilizados
        for (int i = trajectory.Length; i < trajectoryPoints.Length; i++)
        {
            if (trajectoryPoints[i] != null)
            {
                trajectoryPoints[i].SetActive(false);
            }
        }
    }
    
    public void HideTrajectory()
    {
        trajectoryLine.positionCount = 0;
        
        foreach (GameObject point in trajectoryPoints)
        {
            if (point != null)
            {
                point.SetActive(false);
            }
        }
    }
    
    Vector3[] CalculateTrajectory(Vector3 startPos, Vector3 initialVelocity, Vector3 spin)
    {
        System.Collections.Generic.List<Vector3> points = new System.Collections.Generic.List<Vector3>();
        
        Vector3 currentPos = startPos;
        Vector3 currentVel = initialVelocity;
        Vector3 currentSpin = spin;
        
        float time = 0f;
        
        while (time < maxTrajectoryTime && currentPos.y >= 0)
        {
            points.Add(currentPos);
            
            // Aplicar gravedad
            currentVel += Physics.gravity * timeStep;
            
            // Aplicar resistencia del aire
            currentVel *= 0.98f;
            
            // Aplicar efecto Magnus (curva)
            if (currentSpin.magnitude > 0.1f && currentVel.magnitude > 1f)
            {
                Vector3 magnusForce = Vector3.Cross(currentSpin, currentVel.normalized) * 0.5f;
                currentVel += magnusForce * timeStep;
            }
            
            // Actualizar posición
            currentPos += currentVel * timeStep;
            
            // Reducir spin
            currentSpin *= 0.95f;
            
            time += timeStep;
        }
        
        // Añadir punto final en el suelo si es necesario
        if (currentPos.y < 0)
        {
            currentPos.y = 0;
            points.Add(currentPos);
        }
        
        return points.ToArray();
    }
    
    public Vector3 GetLandingPosition(Vector3 startPos, Vector3 initialVelocity, Vector3 spin)
    {
        Vector3[] trajectory = CalculateTrajectory(startPos, initialVelocity, spin);
        
        if (trajectory.Length > 0)
        {
            return trajectory[trajectory.Length - 1];
        }
        
        return startPos;
    }
    
    public float GetTimeToLanding(Vector3 startPos, Vector3 initialVelocity, Vector3 spin)
    {
        Vector3[] trajectory = CalculateTrajectory(startPos, initialVelocity, spin);
        
        return trajectory.Length * timeStep;
    }
    
    public bool WillHitTarget(Vector3 startPos, Vector3 initialVelocity, Vector3 spin, Vector3 targetPos, float tolerance = 1f)
    {
        Vector3 landingPos = GetLandingPosition(startPos, initialVelocity, spin);
        float distance = Vector3.Distance(landingPos, targetPos);
        
        return distance <= tolerance;
    }
    
    // Método para calcular la velocidad inicial necesaria para alcanzar un objetivo
    public Vector3 CalculateVelocityToTarget(Vector3 startPos, Vector3 targetPos, float arcHeight = 2f)
    {
        Vector3 direction = targetPos - startPos;
        float distance = new Vector3(direction.x, 0, direction.z).magnitude;
        float heightDiff = direction.y;
        
        // Calcular ángulo de lanzamiento
        float angle = Mathf.Atan2(heightDiff + arcHeight, distance);
        
        // Calcular velocidad inicial
        float gravity = Mathf.Abs(Physics.gravity.y);
        float velocity = Mathf.Sqrt(gravity * distance / Mathf.Sin(2 * angle));
        
        // Crear vector de velocidad
        Vector3 horizontalDirection = new Vector3(direction.x, 0, direction.z).normalized;
        Vector3 initialVelocity = horizontalDirection * velocity * Mathf.Cos(angle);
        initialVelocity.y = velocity * Mathf.Sin(angle);
        
        return initialVelocity;
    }
    
    // Método para predecir si el balón entrará en la portería
    public bool PredictGoal(Vector3 startPos, Vector3 initialVelocity, Vector3 spin, Bounds goalBounds)
    {
        Vector3[] trajectory = CalculateTrajectory(startPos, initialVelocity, spin);
        
        foreach (Vector3 point in trajectory)
        {
            if (goalBounds.Contains(point))
            {
                return true;
            }
        }
        
        return false;
    }
    
    void OnDrawGizmos()
    {
        if (trajectoryLine != null && trajectoryLine.positionCount > 0)
        {
            Gizmos.color = trajectoryColor;
            
            for (int i = 0; i < trajectoryLine.positionCount - 1; i++)
            {
                Vector3 startPoint = trajectoryLine.GetPosition(i);
                Vector3 endPoint = trajectoryLine.GetPosition(i + 1);
                
                Gizmos.DrawLine(startPoint, endPoint);
            }
        }
    }
}