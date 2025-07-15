using UnityEngine;
using UnityEngine.AI;

public class PlayerGenerator : MonoBehaviour
{
    [Header("Player Generation")]
    public bool autoGenerateOnStart = true;
    public int homeTeamPlayers = 11;
    public int awayTeamPlayers = 11;
    
    [Header("Player Materials")]
    public Material homeTeamMaterial;
    public Material awayTeamMaterial;
    public Material ballMaterial;
    
    [Header("Player Positions")]
    public Transform[] homePlayerPositions;
    public Transform[] awayPlayerPositions;
    
    void Start()
    {
        if (autoGenerateOnStart)
        {
            GenerateAllPlayers();
        }
    }
    
    [ContextMenu("Generate All Players")]
    public void GenerateAllPlayers()
    {
        // Clear existing players
        ClearExistingPlayers();
        
        // Generate ball first
        GenerateBall();
        
        // Generate home team
        GenerateTeam("Home Team", homeTeamPlayers, homeTeamMaterial, true);
        
        // Generate away team
        GenerateTeam("Away Team", awayTeamPlayers, awayTeamMaterial, false);
        
        Debug.Log("All players generated successfully!");
    }
    
    void ClearExistingPlayers()
    {
        // Remove existing players
        PlayerController_120fps[] existingPlayers = FindObjectsOfType<PlayerController_120fps>();
        foreach (PlayerController_120fps player in existingPlayers)
        {
            DestroyImmediate(player.gameObject);
        }
        
        // Remove existing ball
        BallController_120fps existingBall = FindObjectOfType<BallController_120fps>();
        if (existingBall != null)
        {
            DestroyImmediate(existingBall.gameObject);
        }
    }
    
    void GenerateBall()
    {
        GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.name = "Football";
        ball.transform.position = Vector3.zero;
        ball.transform.localScale = new Vector3(0.22f, 0.22f, 0.22f);
        
        // Add components
        Rigidbody rb = ball.AddComponent<Rigidbody>();
        rb.mass = 0.45f;
        rb.drag = 0.05f;
        rb.angularDrag = 0.3f;
        
        // Add ball controller
        BallController_120fps ballController = ball.AddComponent<BallController_120fps>();
        
        // Create ball material
        if (ballMaterial == null)
        {
            ballMaterial = CreateBallMaterial();
        }
        ball.GetComponent<Renderer>().material = ballMaterial;
        
        // Add trail renderer
        TrailRenderer trail = ball.AddComponent<TrailRenderer>();
        trail.time = 0.3f;
        trail.startWidth = 0.1f;
        trail.endWidth = 0.01f;
        trail.material = ballMaterial;
        
        // Add audio source
        AudioSource audioSource = ball.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = 0.5f;
        
        // Set tags and layers
        ball.tag = "Ball";
        ball.layer = LayerMask.NameToLayer("Ball");
        
        // Add physics material
        PhysicMaterial ballPhysics = new PhysicMaterial("Ball Physics");
        ballPhysics.bounciness = 0.6f;
        ballPhysics.dynamicFriction = 0.4f;
        ballPhysics.staticFriction = 0.4f;
        ball.GetComponent<SphereCollider>().material = ballPhysics;
    }
    
    void GenerateTeam(string teamName, int playerCount, Material teamMaterial, bool isHomeTeam)
    {
        GameObject teamContainer = new GameObject(teamName);
        
        // Create team material if not assigned
        if (teamMaterial == null)
        {
            teamMaterial = CreateTeamMaterial(isHomeTeam);
        }
        
        for (int i = 0; i < playerCount; i++)
        {
            GameObject player = GeneratePlayer(i, teamName, teamMaterial, isHomeTeam);
            player.transform.parent = teamContainer.transform;
            
            // Position player
            Vector3 playerPosition = GetPlayerPosition(i, isHomeTeam);
            player.transform.position = playerPosition;
        }
    }
    
    GameObject GeneratePlayer(int playerIndex, string teamName, Material material, bool isHomeTeam)
    {
        // Create player capsule
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = $"{teamName} Player {playerIndex + 1}";
        player.transform.localScale = new Vector3(0.8f, 1f, 0.8f);
        
        // Add components
        Rigidbody rb = player.AddComponent<Rigidbody>();
        rb.mass = 75f;
        rb.drag = 1f;
        rb.angularDrag = 5f;
        rb.freezeRotation = true;
        
        // Add NavMeshAgent
        NavMeshAgent agent = player.AddComponent<NavMeshAgent>();
        agent.speed = 5f;
        agent.acceleration = 10f;
        agent.angularSpeed = 720f;
        agent.radius = 0.5f;
        agent.height = 2f;
        
        // Add player controller
        PlayerController_120fps playerController = player.AddComponent<PlayerController_120fps>();
        
        // Add player AI
        PlayerAI playerAI = player.AddComponent<PlayerAI>();
        
        // Create player data
        PlayerData playerData = CreatePlayerData(playerIndex, isHomeTeam);
        playerController.playerData = playerData;
        
        // Set material
        player.GetComponent<Renderer>().material = material;
        
        // Add ball control point
        GameObject ballControlPoint = new GameObject("Ball Control Point");
        ballControlPoint.transform.parent = player.transform;
        ballControlPoint.transform.localPosition = new Vector3(0, 0, 0.8f);
        playerController.ballControlPoint = ballControlPoint.transform;
        
        // Add animator
        Animator animator = player.AddComponent<Animator>();
        playerController.animator = animator;
        
        // Add footstep audio
        player.AddComponent<FootstepAudio>();
        
        // Set tags and layers
        player.tag = "Player";
        if (isHomeTeam)
        {
            player.layer = LayerMask.NameToLayer("HomeTeam");
        }
        else
        {
            player.layer = LayerMask.NameToLayer("AwayTeam");
        }
        
        // Add physics materials
        PhysicMaterial playerPhysics = new PhysicMaterial("Player Physics");
        playerPhysics.bounciness = 0.1f;
        playerPhysics.dynamicFriction = 0.6f;
        playerPhysics.staticFriction = 0.6f;
        player.GetComponent<CapsuleCollider>().material = playerPhysics;
        
        return player;
    }
    
    PlayerData CreatePlayerData(int playerIndex, bool isHomeTeam)
    {
        PlayerData data = ScriptableObject.CreateInstance<PlayerData>();
        
        // Basic info
        data.playerName = $"Player {playerIndex + 1}";
        data.playerId = playerIndex + 1;
        data.teamId = isHomeTeam ? 1 : 2;
        data.age = Random.Range(18, 35);
        data.shirtNumber = playerIndex + 1;
        
        // Assign position based on index
        data.preferredPosition = GetPlayerPosition(playerIndex);
        
        // Generate random stats
        data.speed = Random.Range(40, 90);
        data.acceleration = Random.Range(40, 90);
        data.stamina = Random.Range(50, 95);
        data.strength = Random.Range(30, 90);
        data.agility = Random.Range(40, 90);
        data.jumping = Random.Range(30, 90);
        
        data.passing = Random.Range(40, 90);
        data.shooting = Random.Range(30, 90);
        data.dribbling = Random.Range(40, 90);
        data.ballControl = Random.Range(40, 90);
        data.crossing = Random.Range(30, 85);
        data.finishing = Random.Range(30, 85);
        data.technique = Random.Range(40, 90);
        
        data.vision = Random.Range(40, 90);
        data.positioning = Random.Range(40, 90);
        data.decision = Random.Range(40, 90);
        data.composure = Random.Range(40, 90);
        
        data.tackling = Random.Range(30, 85);
        data.marking = Random.Range(30, 85);
        data.interception = Random.Range(30, 85);
        
        // Special stats for goalkeeper
        if (playerIndex == 0)
        {
            data.preferredPosition = PlayerPosition.Goalkeeper;
            data.diving = Random.Range(60, 95);
            data.handling = Random.Range(60, 95);
            data.kicking = Random.Range(50, 90);
            data.reflexes = Random.Range(60, 95);
            data.positioning_gk = Random.Range(60, 95);
        }
        
        // Calculate overall
        data.CalculateOverallRating();
        
        return data;
    }
    
    PlayerPosition GetPlayerPosition(int index)
    {
        switch (index)
        {
            case 0: return PlayerPosition.Goalkeeper;
            case 1: case 2: return PlayerPosition.CenterBack;
            case 3: return PlayerPosition.LeftBack;
            case 4: return PlayerPosition.RightBack;
            case 5: return PlayerPosition.DefensiveMidfield;
            case 6: case 7: return PlayerPosition.CentralMidfield;
            case 8: return PlayerPosition.AttackingMidfield;
            case 9: return PlayerPosition.LeftWing;
            case 10: return PlayerPosition.RightWing;
            default: return PlayerPosition.Striker;
        }
    }
    
    Vector3 GetPlayerPosition(int index, bool isHomeTeam)
    {
        float side = isHomeTeam ? -1f : 1f;
        
        switch (index)
        {
            case 0: return new Vector3(0, 0, -45f * side); // Goalkeeper
            case 1: return new Vector3(-5f, 0, -30f * side); // Center Back 1
            case 2: return new Vector3(5f, 0, -30f * side); // Center Back 2
            case 3: return new Vector3(-20f, 0, -25f * side); // Left Back
            case 4: return new Vector3(20f, 0, -25f * side); // Right Back
            case 5: return new Vector3(0, 0, -15f * side); // Defensive Midfield
            case 6: return new Vector3(-10f, 0, -5f * side); // Central Midfield 1
            case 7: return new Vector3(10f, 0, -5f * side); // Central Midfield 2
            case 8: return new Vector3(0, 0, 10f * side); // Attacking Midfield
            case 9: return new Vector3(-25f, 0, 20f * side); // Left Wing
            case 10: return new Vector3(25f, 0, 20f * side); // Right Wing
            default: return new Vector3(0, 0, 35f * side); // Striker
        }
    }
    
    Material CreateTeamMaterial(bool isHomeTeam)
    {
        Material material = new Material(Shader.Find("Standard"));
        
        if (isHomeTeam)
        {
            material.name = "Home Team Material";
            material.color = Color.red;
        }
        else
        {
            material.name = "Away Team Material";
            material.color = Color.blue;
        }
        
        material.SetFloat("_Metallic", 0.2f);
        material.SetFloat("_Smoothness", 0.5f);
        return material;
    }
    
    Material CreateBallMaterial()
    {
        Material material = new Material(Shader.Find("Standard"));
        material.name = "Ball Material";
        material.color = Color.white;
        material.SetFloat("_Metallic", 0.1f);
        material.SetFloat("_Smoothness", 0.8f);
        return material;
    }
    
    [ContextMenu("Generate NavMesh")]
    public void GenerateNavMesh()
    {
        // This will need to be done manually in Unity
        Debug.Log("Please bake NavMesh manually: Window > AI > Navigation > Bake");
    }
}