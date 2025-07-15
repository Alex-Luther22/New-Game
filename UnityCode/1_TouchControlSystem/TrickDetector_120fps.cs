using UnityEngine;
using System.Collections.Generic;

public class TrickDetector_120fps : MonoBehaviour
{
    [Header("Detection Settings")]
    public float patternTimeWindow = 1f;
    public float patternTolerance = 50f;
    public float minimumGestureLength = 100f;
    public int maxPatternPoints = 20;
    
    [Header("Trick Patterns")]
    public bool enableAdvancedTricks = true;
    public float circleThreshold = 0.8f;
    public float zigzagThreshold = 0.6f;
    public float straightThreshold = 0.9f;
    
    // Pattern detection
    private List<Vector2> currentPattern = new List<Vector2>();
    private List<TrickPattern> trickPatterns = new List<TrickPattern>();
    private float lastPatternTime;
    
    // Optimization
    private Vector2 lastProcessedPoint;
    private float minDistanceBetweenPoints = 10f;
    
    void Start()
    {
        InitializeTrickPatterns();
    }
    
    void InitializeTrickPatterns()
    {
        // Initialize all trick patterns
        trickPatterns.Clear();
        
        // Step Over Right - Quick right swipe
        trickPatterns.Add(new TrickPattern
        {
            trickType = TrickType.StepOverRight,
            pattern = new Vector2[] {
                new Vector2(0, 0),
                new Vector2(100, 0)
            },
            tolerance = 30f,
            description = "Quick right swipe"
        });
        
        // Step Over Left - Quick left swipe
        trickPatterns.Add(new TrickPattern
        {
            trickType = TrickType.StepOverLeft,
            pattern = new Vector2[] {
                new Vector2(0, 0),
                new Vector2(-100, 0)
            },
            tolerance = 30f,
            description = "Quick left swipe"
        });
        
        // Roulette - Circular pattern
        trickPatterns.Add(new TrickPattern
        {
            trickType = TrickType.Roulette,
            pattern = new Vector2[] {
                new Vector2(0, 0),
                new Vector2(50, 0),
                new Vector2(50, 50),
                new Vector2(0, 50),
                new Vector2(-50, 50),
                new Vector2(-50, 0),
                new Vector2(-50, -50),
                new Vector2(0, -50),
                new Vector2(50, -50),
                new Vector2(50, 0)
            },
            tolerance = 40f,
            description = "Circular motion"
        });
        
        // Elastico - L-shaped pattern
        trickPatterns.Add(new TrickPattern
        {
            trickType = TrickType.Elastico,
            pattern = new Vector2[] {
                new Vector2(0, 0),
                new Vector2(80, 0),
                new Vector2(80, -80)
            },
            tolerance = 35f,
            description = "L-shaped motion"
        });
        
        // Nutmeg - Vertical line
        trickPatterns.Add(new TrickPattern
        {
            trickType = TrickType.Nutmeg,
            pattern = new Vector2[] {
                new Vector2(0, 0),
                new Vector2(0, 100)
            },
            tolerance = 25f,
            description = "Vertical swipe up"
        });
        
        // Rainbow Flick - Arc pattern
        trickPatterns.Add(new TrickPattern
        {
            trickType = TrickType.RainbowFlick,
            pattern = new Vector2[] {
                new Vector2(0, 0),
                new Vector2(30, 50),
                new Vector2(60, 80),
                new Vector2(90, 50),
                new Vector2(120, 0)
            },
            tolerance = 30f,
            description = "Arc upward motion"
        });
        
        // Heel Flick - Backward swipe
        trickPatterns.Add(new TrickPattern
        {
            trickType = TrickType.HeelFlick,
            pattern = new Vector2[] {
                new Vector2(0, 0),
                new Vector2(0, -100)
            },
            tolerance = 25f,
            description = "Backward swipe"
        });
        
        // Scorpion - S-shaped pattern
        trickPatterns.Add(new TrickPattern
        {
            trickType = TrickType.Scorpion,
            pattern = new Vector2[] {
                new Vector2(0, 0),
                new Vector2(50, 50),
                new Vector2(0, 100),
                new Vector2(-50, 150)
            },
            tolerance = 40f,
            description = "S-shaped motion"
        });
        
        // Rabona - Curve pattern
        trickPatterns.Add(new TrickPattern
        {
            trickType = TrickType.Rabona,
            pattern = new Vector2[] {
                new Vector2(0, 0),
                new Vector2(-50, 30),
                new Vector2(-70, 70),
                new Vector2(-50, 110),
                new Vector2(0, 140)
            },
            tolerance = 35f,
            description = "Curved motion"
        });
        
        // Bicycle - Figure-8 pattern
        trickPatterns.Add(new TrickPattern
        {
            trickType = TrickType.Bicycle,
            pattern = new Vector2[] {
                new Vector2(0, 0),
                new Vector2(40, 40),
                new Vector2(0, 80),
                new Vector2(-40, 40),
                new Vector2(0, 0),
                new Vector2(40, -40),
                new Vector2(0, -80),
                new Vector2(-40, -40),
                new Vector2(0, 0)
            },
            tolerance = 45f,
            description = "Figure-8 motion"
        });
        
        // Chop - Sharp diagonal
        trickPatterns.Add(new TrickPattern
        {
            trickType = TrickType.Chop,
            pattern = new Vector2[] {
                new Vector2(0, 0),
                new Vector2(70, -70)
            },
            tolerance = 30f,
            description = "Sharp diagonal down-right"
        });
        
        // Cut Inside - Curved inward
        trickPatterns.Add(new TrickPattern
        {
            trickType = TrickType.CutInside,
            pattern = new Vector2[] {
                new Vector2(0, 0),
                new Vector2(50, 0),
                new Vector2(70, 30),
                new Vector2(50, 60),
                new Vector2(0, 60)
            },
            tolerance = 35f,
            description = "Curved inward motion"
        });
        
        // Fake Shot - Forward then stop
        trickPatterns.Add(new TrickPattern
        {
            trickType = TrickType.FakeShot,
            pattern = new Vector2[] {
                new Vector2(0, 0),
                new Vector2(0, 80),
                new Vector2(0, 80) // Stop at same position
            },
            tolerance = 20f,
            description = "Forward then stop"
        });
        
        // Body Feint - Side to side
        trickPatterns.Add(new TrickPattern
        {
            trickType = TrickType.BodyFeint,
            pattern = new Vector2[] {
                new Vector2(0, 0),
                new Vector2(60, 0),
                new Vector2(-60, 0)
            },
            tolerance = 25f,
            description = "Side to side motion"
        });
        
        // Dummy - Zigzag pattern
        trickPatterns.Add(new TrickPattern
        {
            trickType = TrickType.Dummy,
            pattern = new Vector2[] {
                new Vector2(0, 0),
                new Vector2(40, 40),
                new Vector2(0, 80),
                new Vector2(-40, 120),
                new Vector2(0, 160)
            },
            tolerance = 30f,
            description = "Zigzag motion"
        });
        
        // Spin - Quick circle
        trickPatterns.Add(new TrickPattern
        {
            trickType = TrickType.Spin,
            pattern = new Vector2[] {
                new Vector2(0, 0),
                new Vector2(30, 30),
                new Vector2(0, 60),
                new Vector2(-30, 30),
                new Vector2(0, 0)
            },
            tolerance = 25f,
            description = "Quick circular motion"
        });
    }
    
    public void AddPatternPoint(Vector2 screenPoint)
    {
        // Optimize by filtering close points
        if (currentPattern.Count > 0)
        {
            float distance = Vector2.Distance(screenPoint, lastProcessedPoint);
            if (distance < minDistanceBetweenPoints)
            {
                return;
            }
        }
        
        currentPattern.Add(screenPoint);
        lastProcessedPoint = screenPoint;
        lastPatternTime = Time.time;
        
        // Limit pattern size for performance
        if (currentPattern.Count > maxPatternPoints)
        {
            currentPattern.RemoveAt(0);
        }
    }
    
    public TrickType DetectTrick(TouchInfo touchInfo)
    {
        // Add current touch to pattern
        if (touchInfo.phase == TouchPhase.Moved)
        {
            AddPatternPoint(touchInfo.position);
        }
        
        // Detect trick on touch end
        if (touchInfo.phase == TouchPhase.Ended)
        {
            TrickType detectedTrick = AnalyzeCurrentPattern();
            ClearPattern();
            return detectedTrick;
        }
        
        return TrickType.StepOverRight; // Default/none
    }
    
    TrickType AnalyzeCurrentPattern()
    {
        if (currentPattern.Count < 2)
        {
            return TrickType.StepOverRight; // Not enough points
        }
        
        // Check if pattern is long enough
        float totalLength = CalculatePatternLength();
        if (totalLength < minimumGestureLength)
        {
            return TrickType.StepOverRight; // Too short
        }
        
        // Normalize pattern to start from origin
        List<Vector2> normalizedPattern = NormalizePattern(currentPattern);
        
        // Check against all known patterns
        TrickType bestMatch = TrickType.StepOverRight;
        float bestScore = 0f;
        
        foreach (TrickPattern trickPattern in trickPatterns)
        {
            float score = CalculatePatternSimilarity(normalizedPattern, trickPattern);
            if (score > bestScore && score > 0.7f) // Minimum similarity threshold
            {
                bestScore = score;
                bestMatch = trickPattern.trickType;
            }
        }
        
        return bestMatch;
    }
    
    float CalculatePatternLength()
    {
        float totalLength = 0f;
        for (int i = 1; i < currentPattern.Count; i++)
        {
            totalLength += Vector2.Distance(currentPattern[i-1], currentPattern[i]);
        }
        return totalLength;
    }
    
    List<Vector2> NormalizePattern(List<Vector2> pattern)
    {
        if (pattern.Count == 0) return new List<Vector2>();
        
        List<Vector2> normalized = new List<Vector2>();
        Vector2 startPoint = pattern[0];
        
        foreach (Vector2 point in pattern)
        {
            normalized.Add(point - startPoint);
        }
        
        return normalized;
    }
    
    float CalculatePatternSimilarity(List<Vector2> userPattern, TrickPattern trickPattern)
    {
        if (userPattern.Count == 0 || trickPattern.pattern.Length == 0)
        {
            return 0f;
        }
        
        // Resample both patterns to same length for comparison
        List<Vector2> resampledUser = ResamplePattern(userPattern, trickPattern.pattern.Length);
        List<Vector2> resampledTarget = new List<Vector2>(trickPattern.pattern);
        
        // Calculate similarity score
        float totalDistance = 0f;
        float maxDistance = 0f;
        
        for (int i = 0; i < resampledUser.Count && i < resampledTarget.Count; i++)
        {
            float distance = Vector2.Distance(resampledUser[i], resampledTarget[i]);
            totalDistance += distance;
            maxDistance = Mathf.Max(maxDistance, distance);
        }
        
        // Normalize score
        float averageDistance = totalDistance / resampledUser.Count;
        float similarity = 1f - (averageDistance / trickPattern.tolerance);
        
        return Mathf.Clamp01(similarity);
    }
    
    List<Vector2> ResamplePattern(List<Vector2> pattern, int targetLength)
    {
        if (pattern.Count == 0 || targetLength == 0)
        {
            return new List<Vector2>();
        }
        
        List<Vector2> resampled = new List<Vector2>();
        
        for (int i = 0; i < targetLength; i++)
        {
            float t = (float)i / (targetLength - 1);
            int index = Mathf.FloorToInt(t * (pattern.Count - 1));
            index = Mathf.Clamp(index, 0, pattern.Count - 1);
            
            resampled.Add(pattern[index]);
        }
        
        return resampled;
    }
    
    void ClearPattern()
    {
        currentPattern.Clear();
        lastPatternTime = 0f;
    }
    
    void Update()
    {
        // Clear old pattern if too much time passed
        if (Time.time - lastPatternTime > patternTimeWindow && currentPattern.Count > 0)
        {
            ClearPattern();
        }
    }
    
    // Public methods for debugging
    public List<Vector2> GetCurrentPattern()
    {
        return new List<Vector2>(currentPattern);
    }
    
    public int GetPatternCount()
    {
        return currentPattern.Count;
    }
    
    public void SetPatternTolerance(float tolerance)
    {
        patternTolerance = tolerance;
        
        // Update all pattern tolerances
        foreach (TrickPattern pattern in trickPatterns)
        {
            pattern.tolerance = tolerance;
        }
    }
    
    void OnDrawGizmos()
    {
        // Draw current pattern in scene view
        if (currentPattern.Count > 1)
        {
            Gizmos.color = Color.red;
            for (int i = 1; i < currentPattern.Count; i++)
            {
                Vector3 from = Camera.main.ScreenToWorldPoint(new Vector3(currentPattern[i-1].x, currentPattern[i-1].y, 10f));
                Vector3 to = Camera.main.ScreenToWorldPoint(new Vector3(currentPattern[i].x, currentPattern[i].y, 10f));
                Gizmos.DrawLine(from, to);
            }
        }
    }
}

[System.Serializable]
public class TrickPattern
{
    public TrickType trickType;
    public Vector2[] pattern;
    public float tolerance;
    public string description;
}