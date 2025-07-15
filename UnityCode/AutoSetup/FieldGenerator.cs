using UnityEngine;

public class FieldGenerator : MonoBehaviour
{
    [Header("Field Dimensions")]
    public float fieldLength = 100f;
    public float fieldWidth = 64f;
    public float goalWidth = 7.32f;
    public float goalHeight = 2.44f;
    public float goalDepth = 2f;
    
    [Header("Materials")]
    public Material grassMaterial;
    public Material lineMaterial;
    public Material goalMaterial;
    public Material netMaterial;
    
    [Header("Auto Generate")]
    public bool autoGenerateOnStart = true;
    
    void Start()
    {
        if (autoGenerateOnStart)
        {
            GenerateField();
        }
    }
    
    [ContextMenu("Generate Field")]
    public void GenerateField()
    {
        // Clear existing field
        ClearField();
        
        // Create field container
        GameObject fieldContainer = new GameObject("Football Field");
        fieldContainer.transform.position = Vector3.zero;
        
        // Generate grass
        GenerateGrass(fieldContainer);
        
        // Generate lines
        GenerateLines(fieldContainer);
        
        // Generate goals
        GenerateGoals(fieldContainer);
        
        // Generate center circle
        GenerateCenterCircle(fieldContainer);
        
        // Generate penalty areas
        GeneratePenaltyAreas(fieldContainer);
        
        // Generate corner areas
        GenerateCornerAreas(fieldContainer);
        
        // Setup colliders
        SetupColliders(fieldContainer);
        
        Debug.Log("Football field generated successfully!");
    }
    
    void ClearField()
    {
        GameObject existingField = GameObject.Find("Football Field");
        if (existingField != null)
        {
            DestroyImmediate(existingField);
        }
    }
    
    void GenerateGrass(GameObject parent)
    {
        GameObject grass = GameObject.CreatePrimitive(PrimitiveType.Plane);
        grass.name = "Grass";
        grass.transform.parent = parent.transform;
        grass.transform.localScale = new Vector3(fieldWidth / 10f, 1f, fieldLength / 10f);
        grass.transform.position = Vector3.zero;
        
        // Create grass material if not assigned
        if (grassMaterial == null)
        {
            grassMaterial = CreateGrassMaterial();
        }
        
        grass.GetComponent<Renderer>().material = grassMaterial;
        grass.tag = "Ground";
        grass.layer = LayerMask.NameToLayer("Ground");
    }
    
    void GenerateLines(GameObject parent)
    {
        GameObject linesContainer = new GameObject("Field Lines");
        linesContainer.transform.parent = parent.transform;
        
        // Create line material if not assigned
        if (lineMaterial == null)
        {
            lineMaterial = CreateLineMaterial();
        }
        
        // Sidelines
        CreateLine(linesContainer, "Left Sideline", 
                  new Vector3(-fieldWidth/2, 0.01f, 0), 
                  new Vector3(0.1f, 0.01f, fieldLength), 
                  lineMaterial);
        
        CreateLine(linesContainer, "Right Sideline", 
                  new Vector3(fieldWidth/2, 0.01f, 0), 
                  new Vector3(0.1f, 0.01f, fieldLength), 
                  lineMaterial);
        
        // Goal lines
        CreateLine(linesContainer, "Goal Line 1", 
                  new Vector3(0, 0.01f, -fieldLength/2), 
                  new Vector3(fieldWidth, 0.01f, 0.1f), 
                  lineMaterial);
        
        CreateLine(linesContainer, "Goal Line 2", 
                  new Vector3(0, 0.01f, fieldLength/2), 
                  new Vector3(fieldWidth, 0.01f, 0.1f), 
                  lineMaterial);
        
        // Center line
        CreateLine(linesContainer, "Center Line", 
                  new Vector3(0, 0.01f, 0), 
                  new Vector3(fieldWidth, 0.01f, 0.1f), 
                  lineMaterial);
    }
    
    void CreateLine(GameObject parent, string name, Vector3 position, Vector3 scale, Material material)
    {
        GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cube);
        line.name = name;
        line.transform.parent = parent.transform;
        line.transform.position = position;
        line.transform.localScale = scale;
        line.GetComponent<Renderer>().material = material;
        
        // Remove collider for lines
        DestroyImmediate(line.GetComponent<Collider>());
    }
    
    void GenerateGoals(GameObject parent)
    {
        GameObject goalsContainer = new GameObject("Goals");
        goalsContainer.transform.parent = parent.transform;
        
        // Create goal material if not assigned
        if (goalMaterial == null)
        {
            goalMaterial = CreateGoalMaterial();
        }
        
        if (netMaterial == null)
        {
            netMaterial = CreateNetMaterial();
        }
        
        // Goal 1
        CreateGoal(goalsContainer, "Goal 1", new Vector3(0, 0, -fieldLength/2 - goalDepth/2), 1);
        
        // Goal 2
        CreateGoal(goalsContainer, "Goal 2", new Vector3(0, 0, fieldLength/2 + goalDepth/2), -1);
    }
    
    void CreateGoal(GameObject parent, string name, Vector3 position, int direction)
    {
        GameObject goal = new GameObject(name);
        goal.transform.parent = parent.transform;
        goal.transform.position = position;
        
        // Goal posts
        CreateGoalPost(goal, "Left Post", new Vector3(-goalWidth/2, goalHeight/2, 0));
        CreateGoalPost(goal, "Right Post", new Vector3(goalWidth/2, goalHeight/2, 0));
        CreateGoalPost(goal, "Crossbar", new Vector3(0, goalHeight, 0));
        
        // Goal net
        CreateGoalNet(goal, "Net", Vector3.zero, direction);
        
        // Goal detector
        CreateGoalDetector(goal, "Goal Detector", new Vector3(0, goalHeight/2, goalDepth/4 * direction));
    }
    
    void CreateGoalPost(GameObject parent, string name, Vector3 position)
    {
        GameObject post = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        post.name = name;
        post.transform.parent = parent.transform;
        post.transform.position = position;
        
        if (name == "Crossbar")
        {
            post.transform.localScale = new Vector3(0.1f, goalWidth/2, 0.1f);
            post.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else
        {
            post.transform.localScale = new Vector3(0.1f, goalHeight, 0.1f);
        }
        
        post.GetComponent<Renderer>().material = goalMaterial;
        post.tag = "Post";
        
        // Add sound effect
        AudioSource audioSource = post.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }
    
    void CreateGoalNet(GameObject parent, string name, Vector3 position, int direction)
    {
        GameObject net = GameObject.CreatePrimitive(PrimitiveType.Cube);
        net.name = name;
        net.transform.parent = parent.transform;
        net.transform.position = position;
        net.transform.localScale = new Vector3(goalWidth, goalHeight, goalDepth);
        net.GetComponent<Renderer>().material = netMaterial;
        
        // Make net trigger
        net.GetComponent<Collider>().isTrigger = true;
        
        // Remove mesh renderer for net effect
        MeshRenderer netRenderer = net.GetComponent<MeshRenderer>();
        netRenderer.material.color = new Color(1, 1, 1, 0.3f);
    }
    
    void CreateGoalDetector(GameObject parent, string name, Vector3 position)
    {
        GameObject detector = GameObject.CreatePrimitive(PrimitiveType.Cube);
        detector.name = name;
        detector.transform.parent = parent.transform;
        detector.transform.position = position;
        detector.transform.localScale = new Vector3(goalWidth, goalHeight, goalDepth/2);
        
        // Make invisible
        detector.GetComponent<Renderer>().enabled = false;
        detector.GetComponent<Collider>().isTrigger = true;
        detector.tag = "Goal";
        
        // Add goal detector script
        detector.AddComponent<GoalDetector>();
    }
    
    void GenerateCenterCircle(GameObject parent)
    {
        GameObject centerCircle = new GameObject("Center Circle");
        centerCircle.transform.parent = parent.transform;
        centerCircle.transform.position = Vector3.zero;
        
        // Create circle using multiple line segments
        int segments = 32;
        float radius = 9.15f;
        
        for (int i = 0; i < segments; i++)
        {
            float angle1 = (float)i / segments * 2 * Mathf.PI;
            float angle2 = (float)(i + 1) / segments * 2 * Mathf.PI;
            
            Vector3 pos1 = new Vector3(Mathf.Cos(angle1) * radius, 0.01f, Mathf.Sin(angle1) * radius);
            Vector3 pos2 = new Vector3(Mathf.Cos(angle2) * radius, 0.01f, Mathf.Sin(angle2) * radius);
            
            CreateCircleSegment(centerCircle, $"Circle Segment {i}", pos1, pos2);
        }
    }
    
    void CreateCircleSegment(GameObject parent, string name, Vector3 start, Vector3 end)
    {
        GameObject segment = GameObject.CreatePrimitive(PrimitiveType.Cube);
        segment.name = name;
        segment.transform.parent = parent.transform;
        
        Vector3 direction = (end - start).normalized;
        float distance = Vector3.Distance(start, end);
        
        segment.transform.position = start + direction * distance / 2;
        segment.transform.localScale = new Vector3(0.1f, 0.01f, distance);
        segment.transform.LookAt(end);
        segment.GetComponent<Renderer>().material = lineMaterial;
        
        // Remove collider
        DestroyImmediate(segment.GetComponent<Collider>());
    }
    
    void GeneratePenaltyAreas(GameObject parent)
    {
        GameObject penaltyContainer = new GameObject("Penalty Areas");
        penaltyContainer.transform.parent = parent.transform;
        
        // Penalty area 1
        CreatePenaltyArea(penaltyContainer, "Penalty Area 1", new Vector3(0, 0, -fieldLength/2 + 16.5f));
        
        // Penalty area 2
        CreatePenaltyArea(penaltyContainer, "Penalty Area 2", new Vector3(0, 0, fieldLength/2 - 16.5f));
        
        // Goal areas
        CreateGoalArea(penaltyContainer, "Goal Area 1", new Vector3(0, 0, -fieldLength/2 + 5.5f));
        CreateGoalArea(penaltyContainer, "Goal Area 2", new Vector3(0, 0, fieldLength/2 - 5.5f));
    }
    
    void CreatePenaltyArea(GameObject parent, string name, Vector3 position)
    {
        GameObject area = new GameObject(name);
        area.transform.parent = parent.transform;
        area.transform.position = position;
        
        // Penalty area lines
        CreateLine(area, "Penalty Line Front", position, new Vector3(40.32f, 0.01f, 0.1f), lineMaterial);
        CreateLine(area, "Penalty Line Left", position + new Vector3(-20.16f, 0, 8.25f), new Vector3(0.1f, 0.01f, 16.5f), lineMaterial);
        CreateLine(area, "Penalty Line Right", position + new Vector3(20.16f, 0, 8.25f), new Vector3(0.1f, 0.01f, 16.5f), lineMaterial);
        
        // Penalty spot
        CreatePenaltySpot(area, "Penalty Spot", position + new Vector3(0, 0, 5.5f));
    }
    
    void CreateGoalArea(GameObject parent, string name, Vector3 position)
    {
        GameObject area = new GameObject(name);
        area.transform.parent = parent.transform;
        area.transform.position = position;
        
        // Goal area lines
        CreateLine(area, "Goal Area Line Front", position, new Vector3(18.32f, 0.01f, 0.1f), lineMaterial);
        CreateLine(area, "Goal Area Line Left", position + new Vector3(-9.16f, 0, 2.75f), new Vector3(0.1f, 0.01f, 5.5f), lineMaterial);
        CreateLine(area, "Goal Area Line Right", position + new Vector3(9.16f, 0, 2.75f), new Vector3(0.1f, 0.01f, 5.5f), lineMaterial);
    }
    
    void CreatePenaltySpot(GameObject parent, string name, Vector3 position)
    {
        GameObject spot = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        spot.name = name;
        spot.transform.parent = parent.transform;
        spot.transform.position = position;
        spot.transform.localScale = new Vector3(0.3f, 0.01f, 0.3f);
        spot.GetComponent<Renderer>().material = lineMaterial;
        
        // Remove collider
        DestroyImmediate(spot.GetComponent<Collider>());
    }
    
    void GenerateCornerAreas(GameObject parent)
    {
        GameObject cornerContainer = new GameObject("Corner Areas");
        cornerContainer.transform.parent = parent.transform;
        
        // Corner arcs
        CreateCornerArc(cornerContainer, "Corner Arc 1", new Vector3(-fieldWidth/2, 0, -fieldLength/2));
        CreateCornerArc(cornerContainer, "Corner Arc 2", new Vector3(fieldWidth/2, 0, -fieldLength/2));
        CreateCornerArc(cornerContainer, "Corner Arc 3", new Vector3(-fieldWidth/2, 0, fieldLength/2));
        CreateCornerArc(cornerContainer, "Corner Arc 4", new Vector3(fieldWidth/2, 0, fieldLength/2));
    }
    
    void CreateCornerArc(GameObject parent, string name, Vector3 position)
    {
        GameObject arc = new GameObject(name);
        arc.transform.parent = parent.transform;
        arc.transform.position = position;
        
        // Create arc using line segments
        int segments = 8;
        float radius = 1f;
        
        for (int i = 0; i < segments; i++)
        {
            float angle1 = (float)i / segments * Mathf.PI / 2;
            float angle2 = (float)(i + 1) / segments * Mathf.PI / 2;
            
            Vector3 pos1 = new Vector3(Mathf.Cos(angle1) * radius, 0.01f, Mathf.Sin(angle1) * radius);
            Vector3 pos2 = new Vector3(Mathf.Cos(angle2) * radius, 0.01f, Mathf.Sin(angle2) * radius);
            
            CreateCircleSegment(arc, $"Arc Segment {i}", pos1, pos2);
        }
    }
    
    void SetupColliders(GameObject parent)
    {
        // Add field boundary colliders
        GameObject boundaries = new GameObject("Field Boundaries");
        boundaries.transform.parent = parent.transform;
        
        // Invisible walls around field
        CreateBoundaryWall(boundaries, "Left Wall", new Vector3(-fieldWidth/2 - 1, 2.5f, 0), new Vector3(1, 5, fieldLength + 10));
        CreateBoundaryWall(boundaries, "Right Wall", new Vector3(fieldWidth/2 + 1, 2.5f, 0), new Vector3(1, 5, fieldLength + 10));
        CreateBoundaryWall(boundaries, "Back Wall 1", new Vector3(0, 2.5f, -fieldLength/2 - 5), new Vector3(fieldWidth + 10, 5, 1));
        CreateBoundaryWall(boundaries, "Back Wall 2", new Vector3(0, 2.5f, fieldLength/2 + 5), new Vector3(fieldWidth + 10, 5, 1));
    }
    
    void CreateBoundaryWall(GameObject parent, string name, Vector3 position, Vector3 scale)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = name;
        wall.transform.parent = parent.transform;
        wall.transform.position = position;
        wall.transform.localScale = scale;
        
        // Make invisible
        wall.GetComponent<Renderer>().enabled = false;
        wall.tag = "Boundary";
    }
    
    // Material creation methods
    Material CreateGrassMaterial()
    {
        Material material = new Material(Shader.Find("Standard"));
        material.name = "Grass Material";
        material.color = new Color(0.2f, 0.7f, 0.2f, 1f);
        material.SetFloat("_Metallic", 0f);
        material.SetFloat("_Smoothness", 0.2f);
        return material;
    }
    
    Material CreateLineMaterial()
    {
        Material material = new Material(Shader.Find("Unlit/Color"));
        material.name = "Line Material";
        material.color = Color.white;
        return material;
    }
    
    Material CreateGoalMaterial()
    {
        Material material = new Material(Shader.Find("Standard"));
        material.name = "Goal Material";
        material.color = Color.white;
        material.SetFloat("_Metallic", 0.5f);
        material.SetFloat("_Smoothness", 0.8f);
        return material;
    }
    
    Material CreateNetMaterial()
    {
        Material material = new Material(Shader.Find("Standard"));
        material.name = "Net Material";
        material.color = new Color(1f, 1f, 1f, 0.3f);
        material.SetFloat("_Mode", 3); // Transparent mode
        material.SetFloat("_Metallic", 0f);
        material.SetFloat("_Smoothness", 0f);
        return material;
    }
}