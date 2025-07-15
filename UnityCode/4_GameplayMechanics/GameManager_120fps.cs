using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager_120fps : MonoBehaviour
{
    [Header("Game Settings")]
    public float matchDuration = 90f;
    public float halfTimeDuration = 45f;
    public int extraTimeMinutes = 5;
    
    [Header("Teams")]
    public TeamData homeTeam;
    public TeamData awayTeam;
    
    [Header("Score")]
    public int homeScore = 0;
    public int awayScore = 0;
    
    [Header("UI References")]
    public Text homeScoreText;
    public Text awayScoreText;
    public Text timeText;
    public Text gameStateText;
    public GameObject pauseMenu;
    public GameObject endGameMenu;
    
    [Header("Camera")]
    public CameraController_120fps cameraController;
    
    [Header("Audio")]
    public AudioSource stadiumAudio;
    public AudioClip goalSound;
    public AudioClip whistleSound;
    public AudioClip crowdCheer;
    
    [Header("120fps Optimizations")]
    public bool enablePerformanceMode = true;
    public bool useObjectPooling = true;
    public int maxSimultaneousEffects = 10;
    
    // Game state
    private float currentTime;
    private bool isGamePaused = false;
    private bool isGameEnded = false;
    private GameState currentGameState;
    private int currentHalf = 1;
    
    // Component references
    private BallController_120fps ballController;
    private PlayerController_120fps[] allPlayers;
    private OfflineRule offlineRule;
    
    // Performance optimizations
    private Queue<GameObject> effectPool;
    private Dictionary<string, AudioClip> audioClips;
    private float lastUpdateTime;
    private float uiUpdateInterval = 0.033f; // 30fps for UI
    
    // Game statistics
    private GameStats gameStats;
    private List<GameEvent> gameEvents;
    
    // Events
    public static System.Action<int, int> OnScoreChanged;
    public static System.Action<GameState> OnGameStateChanged;
    public static System.Action<float> OnTimeChanged;
    public static System.Action<GoalInfo> OnGoalScored;
    public static System.Action<PlayerController_120fps> OnPlayerFouled;
    public static System.Action<OffsideInfo> OnOffsideDetected;
    
    void Start()
    {
        InitializeGame();
        SetupPerformanceOptimizations();
    }
    
    void InitializeGame()
    {
        ballController = FindObjectOfType<BallController_120fps>();
        allPlayers = FindObjectsOfType<PlayerController_120fps>();
        offlineRule = GetComponent<OfflineRule>();
        
        currentTime = 0f;
        currentGameState = GameState.PreGame;
        
        // Initialize game statistics
        gameStats = new GameStats();
        gameEvents = new List<GameEvent>();
        
        // Setup teams
        SetupTeams();
        
        // Initialize audio clips dictionary
        InitializeAudioClips();
        
        // Start game sequence
        StartCoroutine(StartGameSequence());
    }
    
    void SetupPerformanceOptimizations()
    {
        if (useObjectPooling)
        {
            InitializeEffectPool();
        }
        
        // Set target framerate for 120fps
        Application.targetFrameRate = 120;
        QualitySettings.vSyncCount = 0;
        
        // Optimize physics timestep
        Time.fixedDeltaTime = 1f / 120f;
    }
    
    void InitializeEffectPool()
    {
        effectPool = new Queue<GameObject>();
        
        for (int i = 0; i < maxSimultaneousEffects; i++)
        {
            GameObject effect = new GameObject("PooledEffect");
            effect.SetActive(false);
            effectPool.Enqueue(effect);
        }
    }
    
    void InitializeAudioClips()
    {
        audioClips = new Dictionary<string, AudioClip>
        {
            { "goal", goalSound },
            { "whistle", whistleSound },
            { "crowd", crowdCheer }
        };
    }
    
    void SetupTeams()
    {
        foreach (PlayerController_120fps player in allPlayers)
        {
            if (player.playerData.teamId == homeTeam.teamId)
            {
                player.gameObject.layer = LayerMask.NameToLayer("HomeTeam");
            }
            else if (player.playerData.teamId == awayTeam.teamId)
            {
                player.gameObject.layer = LayerMask.NameToLayer("AwayTeam");
            }
        }
    }
    
    void Update()
    {
        float deltaTime = Time.deltaTime;
        
        if (!isGamePaused && !isGameEnded)
        {
            // High-frequency updates (120fps)
            UpdateGameTime(deltaTime);
            CheckGameRules();
            
            // Lower-frequency updates (30fps)
            if (Time.time - lastUpdateTime >= uiUpdateInterval)
            {
                UpdateUI();
                UpdateGameStats();
                lastUpdateTime = Time.time;
            }
        }
        
        HandleInput();
    }
    
    void UpdateGameTime(float deltaTime)
    {
        currentTime += deltaTime;
        
        // Check for half-time
        if (currentTime >= halfTimeDuration * 60f && currentHalf == 1)
        {
            StartCoroutine(HalfTimeSequence());
        }
        // Check for full-time
        else if (currentTime >= matchDuration * 60f && currentHalf == 2)
        {
            EndGame();
        }
    }
    
    void CheckGameRules()
    {
        if (offlineRule != null)
        {
            // Check for offside
            CheckOffside();
            
            // Check for fouls
            CheckFouls();
            
            // Check for out of bounds
            CheckOutOfBounds();
        }
    }
    
    void CheckOffside()
    {
        foreach (PlayerController_120fps player in allPlayers)
        {
            if (player.playerData.teamId != homeTeam.teamId && player.playerData.teamId != awayTeam.teamId)
                continue;
                
            bool isOffside = offlineRule.IsPlayerOffside(player, ballController.transform.position);
            
            if (isOffside)
            {
                OffsideInfo offsideInfo = new OffsideInfo
                {
                    player = player,
                    position = player.transform.position,
                    minute = Mathf.FloorToInt(currentTime / 60f)
                };
                
                OnOffsideDetected?.Invoke(offsideInfo);
                HandleOffside(offsideInfo);
            }
        }
    }
    
    void CheckFouls()
    {
        // Advanced foul detection system
        for (int i = 0; i < allPlayers.Length; i++)
        {
            for (int j = i + 1; j < allPlayers.Length; j++)
            {
                PlayerController_120fps player1 = allPlayers[i];
                PlayerController_120fps player2 = allPlayers[j];
                
                if (player1.playerData.teamId != player2.playerData.teamId)
                {
                    float distance = Vector3.Distance(player1.transform.position, player2.transform.position);
                    
                    if (distance < 1.5f && IsPlayerAggressive(player1, player2))
                    {
                        HandleFoul(player1, player2);
                    }
                }
            }
        }
    }
    
    bool IsPlayerAggressive(PlayerController_120fps player1, PlayerController_120fps player2)
    {
        // Check if player is making aggressive moves
        float player1Speed = player1.GetComponent<Rigidbody>().velocity.magnitude;
        float player2Speed = player2.GetComponent<Rigidbody>().velocity.magnitude;
        
        return player1Speed > 5f && player2Speed < 2f;
    }
    
    void HandleFoul(PlayerController_120fps aggressor, PlayerController_120fps victim)
    {
        OnPlayerFouled?.Invoke(victim);
        
        // Add to game events
        GameEvent foulEvent = new GameEvent
        {
            eventType = GameEventType.Foul,
            player = aggressor,
            minute = Mathf.FloorToInt(currentTime / 60f),
            position = aggressor.transform.position
        };
        
        gameEvents.Add(foulEvent);
        
        // Stop play briefly
        StartCoroutine(HandleFoulSequence(aggressor, victim));
    }
    
    IEnumerator HandleFoulSequence(PlayerController_120fps aggressor, PlayerController_120fps victim)
    {
        currentGameState = GameState.Foul;
        OnGameStateChanged?.Invoke(currentGameState);
        
        PlayAudioClip("whistle");
        
        gameStateText.text = $"Falta de {aggressor.playerData.playerName}";
        
        yield return new WaitForSeconds(2f);
        
        gameStateText.text = "";
        currentGameState = GameState.Playing;
        OnGameStateChanged?.Invoke(currentGameState);
    }
    
    void HandleOffside(OffsideInfo offsideInfo)
    {
        StartCoroutine(HandleOffsideSequence(offsideInfo));
    }
    
    IEnumerator HandleOffsideSequence(OffsideInfo offsideInfo)
    {
        currentGameState = GameState.Offside;
        OnGameStateChanged?.Invoke(currentGameState);
        
        PlayAudioClip("whistle");
        
        gameStateText.text = $"¡Fuera de juego! {offsideInfo.player.playerData.playerName}";
        
        yield return new WaitForSeconds(2f);
        
        // Reset ball position
        ballController.transform.position = offsideInfo.position;
        ballController.GetComponent<Rigidbody>().velocity = Vector3.zero;
        
        gameStateText.text = "";
        currentGameState = GameState.Playing;
        OnGameStateChanged?.Invoke(currentGameState);
    }
    
    void CheckOutOfBounds()
    {
        Vector3 ballPosition = ballController.transform.position;
        
        // Check if ball is out of bounds
        if (Mathf.Abs(ballPosition.x) > 30f || Mathf.Abs(ballPosition.z) > 50f)
        {
            HandleOutOfBounds(ballPosition);
        }
    }
    
    void HandleOutOfBounds(Vector3 ballPosition)
    {
        // Determine if it's a corner kick, goal kick, or throw-in
        if (Mathf.Abs(ballPosition.z) > 50f)
        {
            // Corner kick or goal kick
            if (Mathf.Abs(ballPosition.x) < 7.32f) // Goal area
            {
                StartCoroutine(HandleGoalKick());
            }
            else
            {
                StartCoroutine(HandleCornerKick());
            }
        }
        else
        {
            // Throw-in
            StartCoroutine(HandleThrowIn());
        }
    }
    
    IEnumerator HandleGoalKick()
    {
        currentGameState = GameState.GoalKick;
        OnGameStateChanged?.Invoke(currentGameState);
        
        gameStateText.text = "Saque de puerta";
        
        yield return new WaitForSeconds(1f);
        
        // Position ball for goal kick
        ballController.transform.position = new Vector3(0, 0, -45f);
        ballController.GetComponent<Rigidbody>().velocity = Vector3.zero;
        
        gameStateText.text = "";
        currentGameState = GameState.Playing;
        OnGameStateChanged?.Invoke(currentGameState);
    }
    
    IEnumerator HandleCornerKick()
    {
        currentGameState = GameState.CornerKick;
        OnGameStateChanged?.Invoke(currentGameState);
        
        gameStateText.text = "Saque de esquina";
        
        yield return new WaitForSeconds(1f);
        
        // Position ball for corner kick
        Vector3 ballPos = ballController.transform.position;
        ballController.transform.position = new Vector3(Mathf.Sign(ballPos.x) * 30f, 0, Mathf.Sign(ballPos.z) * 50f);
        ballController.GetComponent<Rigidbody>().velocity = Vector3.zero;
        
        gameStateText.text = "";
        currentGameState = GameState.Playing;
        OnGameStateChanged?.Invoke(currentGameState);
    }
    
    IEnumerator HandleThrowIn()
    {
        currentGameState = GameState.ThrowIn;
        OnGameStateChanged?.Invoke(currentGameState);
        
        gameStateText.text = "Saque de banda";
        
        yield return new WaitForSeconds(1f);
        
        // Position ball for throw-in
        Vector3 ballPos = ballController.transform.position;
        ballController.transform.position = new Vector3(Mathf.Sign(ballPos.x) * 30f, 0, ballPos.z);
        ballController.GetComponent<Rigidbody>().velocity = Vector3.zero;
        
        gameStateText.text = "";
        currentGameState = GameState.Playing;
        OnGameStateChanged?.Invoke(currentGameState);
    }
    
    void UpdateUI()
    {
        // Update score
        homeScoreText.text = homeScore.ToString();
        awayScoreText.text = awayScore.ToString();
        
        // Update time
        float displayTime = currentTime / 60f;
        int minutes = Mathf.FloorToInt(displayTime);
        int seconds = Mathf.FloorToInt((displayTime - minutes) * 60f);
        timeText.text = $"{minutes:00}:{seconds:00}";
        
        OnTimeChanged?.Invoke(displayTime);
    }
    
    void UpdateGameStats()
    {
        gameStats.totalPlayTime = currentTime;
        gameStats.homeTeamPossession = CalculatePossession(homeTeam.teamId);
        gameStats.awayTeamPossession = CalculatePossession(awayTeam.teamId);
    }
    
    float CalculatePossession(int teamId)
    {
        // Simple possession calculation based on closest player to ball
        PlayerController_120fps closestPlayer = FindClosestPlayerToBall();
        return closestPlayer != null && closestPlayer.playerData.teamId == teamId ? 1f : 0f;
    }
    
    PlayerController_120fps FindClosestPlayerToBall()
    {
        PlayerController_120fps closest = null;
        float closestDistance = float.MaxValue;
        
        foreach (PlayerController_120fps player in allPlayers)
        {
            float distance = Vector3.Distance(player.transform.position, ballController.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = player;
            }
        }
        
        return closest;
    }
    
    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    
    public void TogglePause()
    {
        isGamePaused = !isGamePaused;
        pauseMenu.SetActive(isGamePaused);
        
        if (isGamePaused)
        {
            Time.timeScale = 0f;
            currentGameState = GameState.Paused;
        }
        else
        {
            Time.timeScale = 1f;
            currentGameState = GameState.Playing;
        }
        
        OnGameStateChanged?.Invoke(currentGameState);
    }
    
    IEnumerator StartGameSequence()
    {
        currentGameState = GameState.PreGame;
        OnGameStateChanged?.Invoke(currentGameState);
        
        gameStateText.text = "Preparándose para el partido...";
        yield return new WaitForSeconds(2f);
        
        PositionPlayersForKickoff();
        
        gameStateText.text = "¡Comienza el partido!";
        PlayAudioClip("whistle");
        
        yield return new WaitForSeconds(1f);
        
        currentGameState = GameState.Playing;
        OnGameStateChanged?.Invoke(currentGameState);
        gameStateText.text = "";
    }
    
    IEnumerator HalfTimeSequence()
    {
        currentGameState = GameState.HalfTime;
        OnGameStateChanged?.Invoke(currentGameState);
        
        gameStateText.text = "¡Descanso!";
        PlayAudioClip("whistle");
        
        yield return new WaitForSeconds(3f);
        
        currentHalf = 2;
        currentTime = halfTimeDuration * 60f;
        
        SwitchSides();
        
        gameStateText.text = "¡Comienza el segundo tiempo!";
        PlayAudioClip("whistle");
        
        yield return new WaitForSeconds(1f);
        
        currentGameState = GameState.Playing;
        OnGameStateChanged?.Invoke(currentGameState);
        gameStateText.text = "";
    }
    
    void EndGame()
    {
        isGameEnded = true;
        currentGameState = GameState.GameEnded;
        OnGameStateChanged?.Invoke(currentGameState);
        
        PlayAudioClip("whistle");
        ShowFinalResult();
    }
    
    void ShowFinalResult()
    {
        endGameMenu.SetActive(true);
        
        string result;
        if (homeScore > awayScore)
        {
            result = $"¡{homeTeam.teamName} gana {homeScore}-{awayScore}!";
        }
        else if (awayScore > homeScore)
        {
            result = $"¡{awayTeam.teamName} gana {awayScore}-{homeScore}!";
        }
        else
        {
            result = $"¡Empate {homeScore}-{awayScore}!";
        }
        
        gameStateText.text = result;
    }
    
    public void ScoreGoal(int teamId, PlayerController_120fps scorer)
    {
        if (teamId == homeTeam.teamId)
        {
            homeScore++;
        }
        else if (teamId == awayTeam.teamId)
        {
            awayScore++;
        }
        
        GoalInfo goalInfo = new GoalInfo
        {
            scorer = scorer,
            teamId = teamId,
            minute = Mathf.FloorToInt(currentTime / 60f),
            homeScore = homeScore,
            awayScore = awayScore
        };
        
        OnScoreChanged?.Invoke(homeScore, awayScore);
        OnGoalScored?.Invoke(goalInfo);
        
        // Add to game events
        GameEvent goalEvent = new GameEvent
        {
            eventType = GameEventType.Goal,
            player = scorer,
            minute = goalInfo.minute,
            position = scorer.transform.position
        };
        
        gameEvents.Add(goalEvent);
        
        StartCoroutine(GoalCelebration(goalInfo));
    }
    
    IEnumerator GoalCelebration(GoalInfo goalInfo)
    {
        currentGameState = GameState.GoalCelebration;
        OnGameStateChanged?.Invoke(currentGameState);
        
        PlayAudioClip("goal");
        PlayAudioClip("crowd");
        
        string goalText = $"¡GOL! {goalInfo.scorer.playerData.playerName} - {goalInfo.minute}'";
        gameStateText.text = goalText;
        
        if (cameraController != null)
        {
            cameraController.FocusOnPlayer(goalInfo.scorer, 3f);
        }
        
        // Spawn celebration effect
        SpawnEffect("GoalCelebration", goalInfo.scorer.transform.position);
        
        yield return new WaitForSeconds(3f);
        
        PositionPlayersForKickoff();
        
        gameStateText.text = "";
        currentGameState = GameState.Playing;
        OnGameStateChanged?.Invoke(currentGameState);
    }
    
    void SpawnEffect(string effectName, Vector3 position)
    {
        if (useObjectPooling && effectPool.Count > 0)
        {
            GameObject effect = effectPool.Dequeue();
            effect.transform.position = position;
            effect.SetActive(true);
            
            StartCoroutine(ReturnEffectToPool(effect));
        }
    }
    
    IEnumerator ReturnEffectToPool(GameObject effect)
    {
        yield return new WaitForSeconds(3f);
        effect.SetActive(false);
        effectPool.Enqueue(effect);
    }
    
    void PositionPlayersForKickoff()
    {
        Vector3 ballPosition = Vector3.zero;
        
        if (ballController != null)
        {
            ballController.transform.position = ballPosition;
            ballController.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        
        foreach (PlayerController_120fps player in allPlayers)
        {
            Vector3 startPosition = GetStartPosition(player);
            player.transform.position = startPosition;
            player.GetComponent<NavMeshAgent>().Warp(startPosition);
        }
    }
    
    Vector3 GetStartPosition(PlayerController_120fps player)
    {
        Vector3 basePosition = Vector3.zero;
        bool isHomeTeam = player.playerData.teamId == homeTeam.teamId;
        float sideMultiplier = isHomeTeam ? -1f : 1f;
        
        switch (player.playerData.preferredPosition)
        {
            case PlayerPosition.Goalkeeper:
                basePosition = new Vector3(0, 0, -45f * sideMultiplier);
                break;
            case PlayerPosition.CenterBack:
                basePosition = new Vector3(0, 0, -30f * sideMultiplier);
                break;
            case PlayerPosition.LeftBack:
                basePosition = new Vector3(-20f, 0, -30f * sideMultiplier);
                break;
            case PlayerPosition.RightBack:
                basePosition = new Vector3(20f, 0, -30f * sideMultiplier);
                break;
            case PlayerPosition.DefensiveMidfield:
                basePosition = new Vector3(0, 0, -15f * sideMultiplier);
                break;
            case PlayerPosition.CentralMidfield:
                basePosition = new Vector3(0, 0, 0f * sideMultiplier);
                break;
            case PlayerPosition.AttackingMidfield:
                basePosition = new Vector3(0, 0, 15f * sideMultiplier);
                break;
            case PlayerPosition.LeftWing:
                basePosition = new Vector3(-25f, 0, 20f * sideMultiplier);
                break;
            case PlayerPosition.RightWing:
                basePosition = new Vector3(25f, 0, 20f * sideMultiplier);
                break;
            case PlayerPosition.Striker:
                basePosition = new Vector3(0, 0, 35f * sideMultiplier);
                break;
        }
        
        return basePosition;
    }
    
    void SwitchSides()
    {
        foreach (PlayerController_120fps player in allPlayers)
        {
            Vector3 currentPos = player.transform.position;
            Vector3 newPos = new Vector3(currentPos.x, currentPos.y, -currentPos.z);
            player.transform.position = newPos;
            player.GetComponent<NavMeshAgent>().Warp(newPos);
        }
        
        if (ballController != null)
        {
            ballController.transform.position = Vector3.zero;
            ballController.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
    
    void PlayAudioClip(string clipName)
    {
        if (audioClips.ContainsKey(clipName) && stadiumAudio != null)
        {
            stadiumAudio.PlayOneShot(audioClips[clipName]);
        }
    }
    
    // Public methods
    public void RestartGame()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
    
    public GameInfo GetGameInfo()
    {
        return new GameInfo
        {
            homeTeam = homeTeam,
            awayTeam = awayTeam,
            homeScore = homeScore,
            awayScore = awayScore,
            currentTime = currentTime,
            currentHalf = currentHalf,
            gameState = currentGameState,
            gameStats = gameStats
        };
    }
    
    public List<GameEvent> GetGameEvents()
    {
        return gameEvents;
    }
}

// Supporting classes and enums
public enum GameState
{
    PreGame,
    Playing,
    HalfTime,
    Paused,
    GoalCelebration,
    GameEnded,
    Foul,
    Offside,
    GoalKick,
    CornerKick,
    ThrowIn
}

public enum GameEventType
{
    Goal,
    Foul,
    Offside,
    YellowCard,
    RedCard,
    Substitution
}

[System.Serializable]
public class GoalInfo
{
    public PlayerController_120fps scorer;
    public int teamId;
    public int minute;
    public int homeScore;
    public int awayScore;
}

[System.Serializable]
public class OffsideInfo
{
    public PlayerController_120fps player;
    public Vector3 position;
    public int minute;
}

[System.Serializable]
public class GameEvent
{
    public GameEventType eventType;
    public PlayerController_120fps player;
    public int minute;
    public Vector3 position;
}

[System.Serializable]
public class GameStats
{
    public float totalPlayTime;
    public float homeTeamPossession;
    public float awayTeamPossession;
    public int totalShots;
    public int totalPasses;
    public int totalFouls;
}

[System.Serializable]
public class GameInfo
{
    public TeamData homeTeam;
    public TeamData awayTeam;
    public int homeScore;
    public int awayScore;
    public float currentTime;
    public int currentHalf;
    public GameState gameState;
    public GameStats gameStats;
}

// Offside rule implementation
public class OfflineRule : MonoBehaviour
{
    public bool IsPlayerOffside(PlayerController_120fps player, Vector3 ballPosition)
    {
        // Basic offside rule implementation
        if (player.playerData.preferredPosition == PlayerPosition.Goalkeeper)
            return false;
        
        // Check if player is in opponent's half
        bool isInOpponentHalf = (player.playerData.teamId == 1 && player.transform.position.z > 0) ||
                               (player.playerData.teamId == 2 && player.transform.position.z < 0);
        
        if (!isInOpponentHalf)
            return false;
        
        // Check if player is ahead of ball
        bool isAheadOfBall = (player.playerData.teamId == 1 && player.transform.position.z > ballPosition.z) ||
                            (player.playerData.teamId == 2 && player.transform.position.z < ballPosition.z);
        
        if (!isAheadOfBall)
            return false;
        
        // Check if player is ahead of second-to-last opponent
        PlayerController_120fps[] opponents = FindOpponents(player);
        if (opponents.Length < 2)
            return false;
        
        // Sort opponents by Z position
        System.Array.Sort(opponents, (a, b) => a.transform.position.z.CompareTo(b.transform.position.z));
        
        float secondLastOpponentZ = opponents[1].transform.position.z;
        
        bool isOffside = (player.playerData.teamId == 1 && player.transform.position.z > secondLastOpponentZ) ||
                        (player.playerData.teamId == 2 && player.transform.position.z < secondLastOpponentZ);
        
        return isOffside;
    }
    
    PlayerController_120fps[] FindOpponents(PlayerController_120fps player)
    {
        PlayerController_120fps[] allPlayers = FindObjectsOfType<PlayerController_120fps>();
        List<PlayerController_120fps> opponents = new List<PlayerController_120fps>();
        
        foreach (PlayerController_120fps p in allPlayers)
        {
            if (p.playerData.teamId != player.playerData.teamId)
            {
                opponents.Add(p);
            }
        }
        
        return opponents.ToArray();
    }
}