using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameModeManager : MonoBehaviour
{
    private static GameModeManager instance;
    public static GameModeManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameModeManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("GameModeManager");
                    instance = go.AddComponent<GameModeManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }

    [Header("Game Mode Settings")]
    public GameMode currentGameMode;
    public MatchSettings currentMatchSettings;
    public TeamSaveData selectedHomeTeam;
    public TeamSaveData selectedAwayTeam;
    public List<PlayerSaveData> selectedPlayers;
    
    [Header("Scene Management")]
    public string mainMenuScene = "MainMenu";
    public string gameplayScene = "Gameplay";
    public string careerScene = "Career";
    public string trainingScene = "Training";
    public string tournamentScene = "Tournament";
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGameMode();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeGameMode()
    {
        currentGameMode = GameMode.QuickMatch;
        currentMatchSettings = new MatchSettings();
        selectedPlayers = new List<PlayerSaveData>();
    }
    
    public void SetGameMode(GameMode mode)
    {
        currentGameMode = mode;
        Debug.Log($"Game mode set to: {mode}");
    }
    
    public void SetMatchSettings(MatchSettings settings)
    {
        currentMatchSettings = settings;
    }
    
    public void SetSelectedTeams(TeamSaveData homeTeam, TeamSaveData awayTeam)
    {
        selectedHomeTeam = homeTeam;
        selectedAwayTeam = awayTeam;
    }
    
    public void StartMatch()
    {
        switch (currentGameMode)
        {
            case GameMode.QuickMatch:
                StartQuickMatch();
                break;
            case GameMode.Friendly:
                StartFriendlyMatch();
                break;
            case GameMode.Tournament:
                StartTournamentMatch();
                break;
            case GameMode.ManagerCareer:
                StartCareerMatch();
                break;
            case GameMode.PlayerCareer:
                StartPlayerCareerMatch();
                break;
            case GameMode.Training:
                StartTrainingMode();
                break;
            case GameMode.OnlineMultiplayer:
                StartOnlineMatch();
                break;
        }
    }
    
    void StartQuickMatch()
    {
        // Quick match with selected teams
        currentMatchSettings.matchType = MatchType.QuickMatch;
        currentMatchSettings.duration = 90;
        currentMatchSettings.difficulty = DifficultyLevel.Medium;
        currentMatchSettings.weather = WeatherType.Clear;
        currentMatchSettings.timeOfDay = TimeOfDay.Afternoon;
        
        LoadGameplayScene();
    }
    
    void StartFriendlyMatch()
    {
        // Friendly match with custom settings
        currentMatchSettings.matchType = MatchType.Friendly;
        currentMatchSettings.duration = currentMatchSettings.duration > 0 ? currentMatchSettings.duration : 90;
        currentMatchSettings.difficulty = currentMatchSettings.difficulty;
        currentMatchSettings.weather = currentMatchSettings.weather;
        currentMatchSettings.timeOfDay = currentMatchSettings.timeOfDay;
        
        LoadGameplayScene();
    }
    
    void StartTournamentMatch()
    {
        // Tournament match
        currentMatchSettings.matchType = MatchType.Tournament;
        currentMatchSettings.duration = 90;
        currentMatchSettings.extraTime = true;
        currentMatchSettings.penalties = true;
        
        LoadGameplayScene();
    }
    
    void StartCareerMatch()
    {
        // Career mode match
        currentMatchSettings.matchType = MatchType.Career;
        currentMatchSettings.duration = 90;
        currentMatchSettings.injuries = true;
        currentMatchSettings.cards = true;
        
        LoadGameplayScene();
    }
    
    void StartPlayerCareerMatch()
    {
        // Player career match
        currentMatchSettings.matchType = MatchType.PlayerCareer;
        currentMatchSettings.duration = 90;
        currentMatchSettings.playerControlOnly = true;
        
        LoadGameplayScene();
    }
    
    void StartTrainingMode()
    {
        // Training mode
        currentMatchSettings.matchType = MatchType.Training;
        currentMatchSettings.duration = 0; // No time limit
        currentMatchSettings.freePractice = true;
        
        SceneManager.LoadScene(trainingScene);
    }
    
    void StartOnlineMatch()
    {
        // Online multiplayer match
        currentMatchSettings.matchType = MatchType.OnlineMultiplayer;
        currentMatchSettings.duration = 90;
        currentMatchSettings.onlineMode = true;
        
        LoadGameplayScene();
    }
    
    void LoadGameplayScene()
    {
        // Save current game state
        SaveCurrentGameState();
        
        // Load gameplay scene
        SceneManager.LoadScene(gameplayScene);
    }
    
    void SaveCurrentGameState()
    {
        // Save game state to PlayerPrefs for scene transition
        PlayerPrefs.SetString("GameMode", currentGameMode.ToString());
        PlayerPrefs.SetString("MatchSettings", JsonUtility.ToJson(currentMatchSettings));
        
        if (selectedHomeTeam != null)
        {
            PlayerPrefs.SetString("HomeTeam", JsonUtility.ToJson(selectedHomeTeam));
        }
        
        if (selectedAwayTeam != null)
        {
            PlayerPrefs.SetString("AwayTeam", JsonUtility.ToJson(selectedAwayTeam));
        }
        
        PlayerPrefs.Save();
    }
    
    public void LoadGameState()
    {
        // Load game state from PlayerPrefs
        if (PlayerPrefs.HasKey("GameMode"))
        {
            System.Enum.TryParse(PlayerPrefs.GetString("GameMode"), out currentGameMode);
        }
        
        if (PlayerPrefs.HasKey("MatchSettings"))
        {
            string settingsJson = PlayerPrefs.GetString("MatchSettings");
            currentMatchSettings = JsonUtility.FromJson<MatchSettings>(settingsJson);
        }
        
        if (PlayerPrefs.HasKey("HomeTeam"))
        {
            string homeTeamJson = PlayerPrefs.GetString("HomeTeam");
            selectedHomeTeam = JsonUtility.FromJson<TeamSaveData>(homeTeamJson);
        }
        
        if (PlayerPrefs.HasKey("AwayTeam"))
        {
            string awayTeamJson = PlayerPrefs.GetString("AwayTeam");
            selectedAwayTeam = JsonUtility.FromJson<TeamSaveData>(awayTeamJson);
        }
    }
    
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
    
    public void ShowPostMatchResults(MatchResult result)
    {
        // Save match result
        SaveSystem.Instance.SaveMatchResult(result);
        
        // Update user stats
        UpdateUserStats(result);
        
        // Show results screen
        PostMatchManager.Instance.ShowResults(result);
    }
    
    void UpdateUserStats(MatchResult result)
    {
        UserProfile user = SaveSystem.Instance.LoadUserProfile(1);
        if (user != null)
        {
            user.TotalMatches++;
            
            if (result.Result == "Win")
            {
                user.TotalWins++;
                user.Coins += CalculateMatchReward(result);
                user.Experience += 100;
            }
            else if (result.Result == "Draw")
            {
                user.Coins += CalculateMatchReward(result) / 2;
                user.Experience += 50;
            }
            else
            {
                user.Experience += 25;
            }
            
            SaveSystem.Instance.SaveUserProfile(user);
        }
    }
    
    int CalculateMatchReward(MatchResult result)
    {
        int baseReward = 1000;
        
        switch (currentGameMode)
        {
            case GameMode.QuickMatch:
                return baseReward;
            case GameMode.Friendly:
                return baseReward / 2;
            case GameMode.Tournament:
                return baseReward * 3;
            case GameMode.ManagerCareer:
                return baseReward * 2;
            case GameMode.PlayerCareer:
                return baseReward * 2;
            case GameMode.OnlineMultiplayer:
                return baseReward * 4;
            default:
                return baseReward;
        }
    }
    
    public bool IsCareerMode()
    {
        return currentGameMode == GameMode.ManagerCareer || currentGameMode == GameMode.PlayerCareer;
    }
    
    public bool IsOnlineMode()
    {
        return currentGameMode == GameMode.OnlineMultiplayer;
    }
    
    public bool IsTrainingMode()
    {
        return currentGameMode == GameMode.Training;
    }
    
    public void PauseGame()
    {
        Time.timeScale = 0f;
    }
    
    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }
    
    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
    }
}

[System.Serializable]
public class MatchSettings
{
    public MatchType matchType = MatchType.QuickMatch;
    public int duration = 90;
    public DifficultyLevel difficulty = DifficultyLevel.Medium;
    public WeatherType weather = WeatherType.Clear;
    public TimeOfDay timeOfDay = TimeOfDay.Afternoon;
    public bool extraTime = false;
    public bool penalties = false;
    public bool injuries = false;
    public bool cards = false;
    public bool offsideRule = true;
    public bool foulsEnabled = true;
    public bool playerControlOnly = false;
    public bool freePractice = false;
    public bool onlineMode = false;
    public string stadium = "";
    public int attendance = 0;
}

public enum GameMode
{
    QuickMatch,
    Friendly,
    Tournament,
    ManagerCareer,
    PlayerCareer,
    CreateClub,
    Training,
    OnlineMultiplayer
}

public enum MatchType
{
    QuickMatch,
    Friendly,
    Tournament,
    Career,
    PlayerCareer,
    Training,
    OnlineMultiplayer
}

public enum DifficultyLevel
{
    Beginner,
    Easy,
    Medium,
    Hard,
    Legendary
}

public enum WeatherType
{
    Clear,
    Cloudy,
    Rainy,
    Snowy,
    Foggy
}

public enum TimeOfDay
{
    Morning,
    Afternoon,
    Evening,
    Night
}