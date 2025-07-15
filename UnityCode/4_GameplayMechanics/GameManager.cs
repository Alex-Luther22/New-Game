using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public float matchDuration = 90f; // minutos
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
    public CameraController cameraController;
    
    [Header("Audio")]
    public AudioSource stadiumAudio;
    public AudioClip goalSound;
    public AudioClip whistleSound;
    public AudioClip crowdCheer;
    
    private float currentTime;
    private bool isGamePaused = false;
    private bool isGameEnded = false;
    private GameState currentGameState;
    private BallController ballController;
    private PlayerController[] allPlayers;
    private int currentHalf = 1;
    
    // Eventos del juego
    public static System.Action<int, int> OnScoreChanged;
    public static System.Action<GameState> OnGameStateChanged;
    public static System.Action<float> OnTimeChanged;
    public static System.Action<GoalInfo> OnGoalScored;
    
    void Start()
    {
        InitializeGame();
    }
    
    void Update()
    {
        if (!isGamePaused && !isGameEnded)
        {
            UpdateGameTime();
            UpdateUI();
        }
        
        HandleInput();
    }
    
    void InitializeGame()
    {
        ballController = FindObjectOfType<BallController>();
        allPlayers = FindObjectsOfType<PlayerController>();
        
        currentTime = 0f;
        currentGameState = GameState.PreGame;
        
        // Configurar equipos
        SetupTeams();
        
        // Comenzar el juego
        StartCoroutine(StartGameSequence());
    }
    
    void SetupTeams()
    {
        // Configurar jugadores según sus equipos
        foreach (PlayerController player in allPlayers)
        {
            if (player.playerData.teamId == homeTeam.teamId)
            {
                // Configurar como equipo local
                player.gameObject.layer = LayerMask.NameToLayer("HomeTeam");
            }
            else if (player.playerData.teamId == awayTeam.teamId)
            {
                // Configurar como equipo visitante
                player.gameObject.layer = LayerMask.NameToLayer("AwayTeam");
            }
        }
    }
    
    IEnumerator StartGameSequence()
    {
        currentGameState = GameState.PreGame;
        OnGameStateChanged?.Invoke(currentGameState);
        
        // Mostrar información del partido
        gameStateText.text = "Preparándose para el partido...";
        yield return new WaitForSeconds(2f);
        
        // Posicionar jugadores para el saque inicial
        PositionPlayersForKickoff();
        
        gameStateText.text = "¡Comienza el partido!";
        PlaySound(whistleSound);
        
        yield return new WaitForSeconds(1f);
        
        // Comenzar el juego
        currentGameState = GameState.Playing;
        OnGameStateChanged?.Invoke(currentGameState);
        gameStateText.text = "";
    }
    
    void UpdateGameTime()
    {
        currentTime += Time.deltaTime;
        
        // Verificar final de tiempo
        if (currentTime >= halfTimeDuration * 60f && currentHalf == 1)
        {
            StartCoroutine(HalfTimeSequence());
        }
        else if (currentTime >= matchDuration * 60f && currentHalf == 2)
        {
            EndGame();
        }
    }
    
    IEnumerator HalfTimeSequence()
    {
        currentGameState = GameState.HalfTime;
        OnGameStateChanged?.Invoke(currentGameState);
        
        gameStateText.text = "¡Descanso!";
        PlaySound(whistleSound);
        
        yield return new WaitForSeconds(3f);
        
        // Preparar segundo tiempo
        currentHalf = 2;
        currentTime = halfTimeDuration * 60f;
        
        // Cambiar lados
        SwitchSides();
        
        gameStateText.text = "¡Comienza el segundo tiempo!";
        PlaySound(whistleSound);
        
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
        
        PlaySound(whistleSound);
        
        // Mostrar resultado final
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
    
    void UpdateUI()
    {
        // Actualizar marcador
        homeScoreText.text = homeScore.ToString();
        awayScoreText.text = awayScore.ToString();
        
        // Actualizar tiempo
        float displayTime = currentTime / 60f;
        int minutes = Mathf.FloorToInt(displayTime);
        int seconds = Mathf.FloorToInt((displayTime - minutes) * 60f);
        timeText.text = $"{minutes:00}:{seconds:00}";
        
        // Disparar evento de tiempo
        OnTimeChanged?.Invoke(displayTime);
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
    
    public void ScoreGoal(int teamId, PlayerController scorer)
    {
        if (teamId == homeTeam.teamId)
        {
            homeScore++;
        }
        else if (teamId == awayTeam.teamId)
        {
            awayScore++;
        }
        
        // Crear información del gol
        GoalInfo goalInfo = new GoalInfo
        {
            scorer = scorer,
            teamId = teamId,
            minute = Mathf.FloorToInt(currentTime / 60f),
            homeScore = homeScore,
            awayScore = awayScore
        };
        
        // Disparar eventos
        OnScoreChanged?.Invoke(homeScore, awayScore);
        OnGoalScored?.Invoke(goalInfo);
        
        // Efectos de gol
        StartCoroutine(GoalCelebration(goalInfo));
    }
    
    IEnumerator GoalCelebration(GoalInfo goalInfo)
    {
        currentGameState = GameState.GoalCelebration;
        OnGameStateChanged?.Invoke(currentGameState);
        
        // Sonidos y efectos
        PlaySound(goalSound);
        PlaySound(crowdCheer);
        
        // Mostrar información del gol
        string goalText = $"¡GOL! {goalInfo.scorer.playerData.playerName} - {goalInfo.minute}'";
        gameStateText.text = goalText;
        
        // Cámara sigue al goleador
        if (cameraController != null)
        {
            cameraController.FocusOnPlayer(goalInfo.scorer, 3f);
        }
        
        yield return new WaitForSeconds(3f);
        
        // Reiniciar desde el centro
        PositionPlayersForKickoff();
        
        gameStateText.text = "";
        currentGameState = GameState.Playing;
        OnGameStateChanged?.Invoke(currentGameState);
    }
    
    void PositionPlayersForKickoff()
    {
        // Posicionar jugadores para el saque inicial
        Vector3 ballPosition = Vector3.zero;
        
        if (ballController != null)
        {
            ballController.transform.position = ballPosition;
            ballController.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        
        // Posicionar jugadores en sus posiciones iniciales
        foreach (PlayerController player in allPlayers)
        {
            Vector3 startPosition = GetStartPosition(player);
            player.transform.position = startPosition;
            player.GetComponent<NavMeshAgent>().Warp(startPosition);
        }
    }
    
    Vector3 GetStartPosition(PlayerController player)
    {
        // Obtener posición inicial basada en la posición del jugador
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
        // Cambiar lados al inicio del segundo tiempo
        foreach (PlayerController player in allPlayers)
        {
            Vector3 currentPos = player.transform.position;
            Vector3 newPos = new Vector3(currentPos.x, currentPos.y, -currentPos.z);
            player.transform.position = newPos;
            player.GetComponent<NavMeshAgent>().Warp(newPos);
        }
        
        // Cambiar posición del balón
        if (ballController != null)
        {
            ballController.transform.position = Vector3.zero;
            ballController.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
    
    void PlaySound(AudioClip clip)
    {
        if (stadiumAudio != null && clip != null)
        {
            stadiumAudio.PlayOneShot(clip);
        }
    }
    
    // Métodos públicos para UI
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
            gameState = currentGameState
        };
    }
}

public enum GameState
{
    PreGame,
    Playing,
    HalfTime,
    Paused,
    GoalCelebration,
    GameEnded
}

[System.Serializable]
public class GoalInfo
{
    public PlayerController scorer;
    public int teamId;
    public int minute;
    public int homeScore;
    public int awayScore;
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
}