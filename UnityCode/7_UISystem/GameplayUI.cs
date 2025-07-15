using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameplayUI : MonoBehaviour
{
    [Header("HUD Elements")]
    public Text homeTeamName;
    public Text awayTeamName;
    public Text homeScore;
    public Text awayScore;
    public Text gameTime;
    public Text gameState;
    
    [Header("Player HUD")]
    public Text playerName;
    public Text playerPosition;
    public Slider staminaBar;
    public GameObject ballIndicator;
    
    [Header("Minimap")]
    public RawImage minimapImage;
    public Transform minimapCamera;
    
    [Header("Controls")]
    public GameObject touchControls;
    public Button pauseButton;
    public Button shootButton;
    public Button passButton;
    public Button trickButton;
    
    [Header("Pause Menu")]
    public GameObject pauseMenuPanel;
    public Button resumeButton;
    public Button restartButton;
    public Button settingsFromGameButton;
    public Button mainMenuButton;
    
    [Header("Goal Celebration")]
    public GameObject goalCelebrationPanel;
    public Text goalScorerText;
    public Text goalTimeText;
    public Animator goalAnimator;
    
    [Header("Match End")]
    public GameObject matchEndPanel;
    public Text finalScoreText;
    public Text matchResultText;
    public Button playAgainButton;
    public Button backToMenuButton;
    
    [Header("Statistics")]
    public GameObject statsPanel;
    public Text possessionText;
    public Text shotsText;
    public Text cornersText;
    public Text foulsText;
    public Button statsButton;
    
    [Header("Substitution")]
    public GameObject substitutionPanel;
    public Transform playerList;
    public Button confirmSubButton;
    public Button cancelSubButton;
    
    private GameManager gameManager;
    private PlayerController currentPlayer;
    private bool isGamePaused = false;
    
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        
        SetupButtons();
        SetupEventListeners();
        
        // Configurar controles táctiles
        SetupTouchControls();
        
        // Inicializar UI
        UpdateUI();
    }
    
    void SetupButtons()
    {
        pauseButton.onClick.AddListener(TogglePause);
        
        // Botones de pausa
        resumeButton.onClick.AddListener(TogglePause);
        restartButton.onClick.AddListener(RestartMatch);
        settingsFromGameButton.onClick.AddListener(ShowGameSettings);
        mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        
        // Botones de final de partido
        playAgainButton.onClick.AddListener(PlayAgain);
        backToMenuButton.onClick.AddListener(ReturnToMainMenu);
        
        // Botones de estadísticas
        statsButton.onClick.AddListener(ToggleStats);
        
        // Botones de sustitución
        confirmSubButton.onClick.AddListener(ConfirmSubstitution);
        cancelSubButton.onClick.AddListener(CancelSubstitution);
    }
    
    void SetupEventListeners()
    {
        // Suscribirse a eventos del GameManager
        GameManager.OnScoreChanged += UpdateScore;
        GameManager.OnTimeChanged += UpdateTime;
        GameManager.OnGameStateChanged += UpdateGameState;
        GameManager.OnGoalScored += ShowGoalCelebration;
    }
    
    void SetupTouchControls()
    {
        // Configurar controles táctiles basados en la plataforma
        if (Application.isMobilePlatform)
        {
            touchControls.SetActive(true);
            
            // Configurar botones táctiles
            shootButton.onClick.AddListener(OnShootButtonPressed);
            passButton.onClick.AddListener(OnPassButtonPressed);
            trickButton.onClick.AddListener(OnTrickButtonPressed);
        }
        else
        {
            touchControls.SetActive(false);
        }
    }
    
    void Update()
    {
        if (!isGamePaused)
        {
            UpdatePlayerHUD();
            UpdateMinimap();
        }
    }
    
    void UpdateUI()
    {
        if (gameManager != null)
        {
            GameInfo gameInfo = gameManager.GetGameInfo();
            
            homeTeamName.text = gameInfo.homeTeam.shortName;
            awayTeamName.text = gameInfo.awayTeam.shortName;
            
            UpdateScore(gameInfo.homeScore, gameInfo.awayScore);
            UpdateTime(gameInfo.currentTime);
        }
    }
    
    void UpdateScore(int home, int away)
    {
        homeScore.text = home.ToString();
        awayScore.text = away.ToString();
    }
    
    void UpdateTime(float time)
    {
        int minutes = Mathf.FloorToInt(time);
        int seconds = Mathf.FloorToInt((time - minutes) * 60f);
        gameTime.text = $"{minutes:00}:{seconds:00}";
    }
    
    void UpdateGameState(GameState state)
    {
        switch (state)
        {
            case GameState.PreGame:
                gameState.text = "Preparando...";
                break;
            case GameState.Playing:
                gameState.text = "";
                break;
            case GameState.HalfTime:
                gameState.text = "DESCANSO";
                break;
            case GameState.Paused:
                gameState.text = "PAUSADO";
                break;
            case GameState.GoalCelebration:
                gameState.text = "¡GOL!";
                break;
            case GameState.GameEnded:
                gameState.text = "FINAL";
                ShowMatchEnd();
                break;
        }
    }
    
    void UpdatePlayerHUD()
    {
        if (currentPlayer != null)
        {
            PlayerInfo playerInfo = currentPlayer.GetPlayerInfo();
            
            playerName.text = playerInfo.playerData.playerName;
            playerPosition.text = playerInfo.playerData.preferredPosition.ToString();
            staminaBar.value = playerInfo.stamina / 100f;
            
            // Mostrar indicador de balón
            ballIndicator.SetActive(playerInfo.hasBall);
        }
    }
    
    void UpdateMinimap()
    {
        // Actualizar posición de cámara del minimapa
        if (minimapCamera != null && Camera.main != null)
        {
            minimapCamera.position = new Vector3(
                Camera.main.transform.position.x,
                50f,
                Camera.main.transform.position.z
            );
        }
    }
    
    void ShowGoalCelebration(GoalInfo goalInfo)
    {
        goalCelebrationPanel.SetActive(true);
        goalScorerText.text = goalInfo.scorer.playerData.playerName;
        goalTimeText.text = $"{goalInfo.minute}'";
        
        if (goalAnimator != null)
        {
            goalAnimator.SetTrigger("ShowGoal");
        }
        
        // Ocultar después de 3 segundos
        Invoke("HideGoalCelebration", 3f);
    }
    
    void HideGoalCelebration()
    {
        goalCelebrationPanel.SetActive(false);
    }
    
    void ShowMatchEnd()
    {
        matchEndPanel.SetActive(true);
        
        if (gameManager != null)
        {
            GameInfo gameInfo = gameManager.GetGameInfo();
            finalScoreText.text = $"{gameInfo.homeScore} - {gameInfo.awayScore}";
            
            if (gameInfo.homeScore > gameInfo.awayScore)
            {
                matchResultText.text = $"¡{gameInfo.homeTeam.teamName} GANA!";
            }
            else if (gameInfo.awayScore > gameInfo.homeScore)
            {
                matchResultText.text = $"¡{gameInfo.awayTeam.teamName} GANA!";
            }
            else
            {
                matchResultText.text = "¡EMPATE!";
            }
        }
    }
    
    void TogglePause()
    {
        isGamePaused = !isGamePaused;
        pauseMenuPanel.SetActive(isGamePaused);
        
        if (gameManager != null)
        {
            gameManager.TogglePause();
        }
        
        AudioManager.Instance.PlayButtonSound();
    }
    
    void RestartMatch()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    void ShowGameSettings()
    {
        // Mostrar configuración del juego
        // Similar al menú principal pero adaptado para el juego
    }
    
    void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }
    
    void PlayAgain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    void ToggleStats()
    {
        statsPanel.SetActive(!statsPanel.activeSelf);
        
        if (statsPanel.activeSelf)
        {
            UpdateStatsPanel();
        }
    }
    
    void UpdateStatsPanel()
    {
        // Actualizar estadísticas del partido
        possessionText.text = "Posesión: 60% - 40%";
        shotsText.text = "Disparos: 8 - 5";
        cornersText.text = "Corners: 4 - 2";
        foulsText.text = "Faltas: 12 - 8";
    }
    
    void ConfirmSubstitution()
    {
        // Confirmar sustitución
        substitutionPanel.SetActive(false);
    }
    
    void CancelSubstitution()
    {
        substitutionPanel.SetActive(false);
    }
    
    // Métodos para controles táctiles
    void OnShootButtonPressed()
    {
        if (currentPlayer != null)
        {
            // Obtener dirección hacia la portería
            Vector3 goalDirection = Vector3.forward; // Simplificado
            currentPlayer.Shoot(goalDirection, 0.8f);
        }
    }
    
    void OnPassButtonPressed()
    {
        if (currentPlayer != null)
        {
            currentPlayer.PerformShortPass();
        }
    }
    
    void OnTrickButtonPressed()
    {
        if (currentPlayer != null)
        {
            currentPlayer.PerformTrick(TrickType.StepOverRight);
        }
    }
    
    // Método para cambiar jugador controlado
    public void SetCurrentPlayer(PlayerController player)
    {
        currentPlayer = player;
    }
    
    // Método para mostrar notificaciones
    public void ShowNotification(string message, float duration = 2f)
    {
        // Crear y mostrar notificación temporal
        GameObject notification = new GameObject("Notification");
        Text notificationText = notification.AddComponent<Text>();
        notificationText.text = message;
        notificationText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        notificationText.fontSize = 24;
        notificationText.color = Color.white;
        
        notification.transform.SetParent(transform);
        notification.transform.localPosition = Vector3.zero;
        
        Destroy(notification, duration);
    }
    
    void OnDestroy()
    {
        // Desuscribirse de eventos
        GameManager.OnScoreChanged -= UpdateScore;
        GameManager.OnTimeChanged -= UpdateTime;
        GameManager.OnGameStateChanged -= UpdateGameState;
        GameManager.OnGoalScored -= ShowGoalCelebration;
    }
}