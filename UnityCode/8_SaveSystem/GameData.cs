using UnityEngine;
using System.Collections.Generic;

public class GameData : MonoBehaviour
{
    public static GameData Instance;
    
    [Header("Game Settings")]
    public GameSettings gameSettings;
    
    [Header("Player Progress")]
    public PlayerProfile playerProfile;
    
    [Header("Current Game State")]
    public TeamData selectedTeam;
    public LeagueData currentLeague;
    
    [Header("Save Data")]
    public List<SeasonData> seasonHistory = new List<SeasonData>();
    public List<MatchResult> matchHistory = new List<MatchResult>();
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGameData();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void LoadGameData()
    {
        // Cargar configuraciones
        gameSettings = LoadGameSettings();
        
        // Cargar perfil del jugador
        playerProfile = LoadPlayerProfile();
        
        // Aplicar configuraciones
        ApplyGameSettings();
    }
    
    void ApplyGameSettings()
    {
        if (gameSettings != null)
        {
            // Aplicar configuraciones de audio
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetMasterVolume(gameSettings.masterVolume);
                AudioManager.Instance.SetMusicVolume(gameSettings.musicVolume);
                AudioManager.Instance.SetSFXVolume(gameSettings.sfxVolume);
            }
            
            // Aplicar configuraciones de calidad
            QualitySettings.SetQualityLevel(gameSettings.qualityLevel);
            
            // Aplicar configuraciones de pantalla
            Screen.SetResolution(gameSettings.screenWidth, gameSettings.screenHeight, gameSettings.fullscreen);
        }
    }
    
    public void SaveGameData()
    {
        SaveGameSettings();
        SavePlayerProfile();
        SaveSeasonHistory();
        SaveMatchHistory();
    }
    
    void SaveGameSettings()
    {
        if (gameSettings != null)
        {
            string json = JsonUtility.ToJson(gameSettings);
            PlayerPrefs.SetString("GameSettings", json);
        }
    }
    
    GameSettings LoadGameSettings()
    {
        if (PlayerPrefs.HasKey("GameSettings"))
        {
            string json = PlayerPrefs.GetString("GameSettings");
            return JsonUtility.FromJson<GameSettings>(json);
        }
        else
        {
            return new GameSettings(); // Configuración por defecto
        }
    }
    
    void SavePlayerProfile()
    {
        if (playerProfile != null)
        {
            string json = JsonUtility.ToJson(playerProfile);
            PlayerPrefs.SetString("PlayerProfile", json);
        }
    }
    
    PlayerProfile LoadPlayerProfile()
    {
        if (PlayerPrefs.HasKey("PlayerProfile"))
        {
            string json = PlayerPrefs.GetString("PlayerProfile");
            return JsonUtility.FromJson<PlayerProfile>(json);
        }
        else
        {
            return new PlayerProfile(); // Perfil por defecto
        }
    }
    
    void SaveSeasonHistory()
    {
        string json = JsonUtility.ToJson(new SerializableList<SeasonData>(seasonHistory));
        PlayerPrefs.SetString("SeasonHistory", json);
    }
    
    void SaveMatchHistory()
    {
        string json = JsonUtility.ToJson(new SerializableList<MatchResult>(matchHistory));
        PlayerPrefs.SetString("MatchHistory", json);
    }
    
    public void AddSeasonData(SeasonData season)
    {
        seasonHistory.Add(season);
        SaveSeasonHistory();
    }
    
    public void AddMatchResult(MatchResult match)
    {
        matchHistory.Add(match);
        
        // Mantener solo los últimos 100 partidos
        if (matchHistory.Count > 100)
        {
            matchHistory.RemoveAt(0);
        }
        
        SaveMatchHistory();
    }
    
    public void UpdatePlayerProfile(PlayerProfile profile)
    {
        playerProfile = profile;
        SavePlayerProfile();
    }
    
    public List<MatchResult> GetRecentMatches(int count = 10)
    {
        int startIndex = Mathf.Max(0, matchHistory.Count - count);
        return matchHistory.GetRange(startIndex, Mathf.Min(count, matchHistory.Count));
    }
    
    public SeasonData GetCurrentSeasonData()
    {
        return seasonHistory.Count > 0 ? seasonHistory[seasonHistory.Count - 1] : null;
    }
    
    public void ResetGameData()
    {
        PlayerPrefs.DeleteAll();
        
        gameSettings = new GameSettings();
        playerProfile = new PlayerProfile();
        seasonHistory.Clear();
        matchHistory.Clear();
        
        SaveGameData();
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveGameData();
        }
    }
    
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            SaveGameData();
        }
    }
    
    void OnDestroy()
    {
        SaveGameData();
    }
}

[System.Serializable]
public class GameSettings
{
    public float masterVolume = 1f;
    public float musicVolume = 0.7f;
    public float sfxVolume = 1f;
    public float crowdVolume = 0.8f;
    public float commentaryVolume = 0.9f;
    
    public int qualityLevel = 2;
    public int screenWidth = 1920;
    public int screenHeight = 1080;
    public bool fullscreen = true;
    
    public string language = "Spanish";
    public bool subtitlesEnabled = true;
    public float gameplaySpeed = 1f;
    public int difficulty = 1; // 0=Fácil, 1=Normal, 2=Difícil
    
    public bool autoSave = true;
    public bool showTutorial = true;
    public bool vibrateOnMobile = true;
}

[System.Serializable]
public class PlayerProfile
{
    public string playerName = "Jugador";
    public int level = 1;
    public long totalXP = 0;
    public long coinsEarned = 0;
    public long coinsSpent = 0;
    
    public int matchesPlayed = 0;
    public int matchesWon = 0;
    public int matchesDrawn = 0;
    public int matchesLost = 0;
    
    public int goalsScored = 0;
    public int assistsMade = 0;
    public int cleanSheets = 0;
    
    public int trophiesWon = 0;
    public int cupsWon = 0;
    public int leaguesWon = 0;
    
    public List<string> unlockedTeams = new List<string>();
    public List<string> unlockedStadiums = new List<string>();
    public List<string> achievements = new List<string>();
    
    public string favoriteTeam = "";
    public string favoritePlayer = "";
    public string favoriteFormation = "";
    
    public System.DateTime lastPlayed = System.DateTime.Now;
    public System.DateTime accountCreated = System.DateTime.Now;
    
    public float GetWinPercentage()
    {
        return matchesPlayed > 0 ? (float)matchesWon / matchesPlayed * 100f : 0f;
    }
    
    public float GetGoalsPerMatch()
    {
        return matchesPlayed > 0 ? (float)goalsScored / matchesPlayed : 0f;
    }
    
    public long GetCurrentCoins()
    {
        return coinsEarned - coinsSpent;
    }
    
    public int GetLevelFromXP()
    {
        return Mathf.FloorToInt(totalXP / 1000f) + 1;
    }
    
    public float GetLevelProgress()
    {
        return (totalXP % 1000f) / 1000f;
    }
    
    public void AddXP(long xp)
    {
        totalXP += xp;
        level = GetLevelFromXP();
    }
    
    public void AddCoins(long coins)
    {
        coinsEarned += coins;
    }
    
    public bool SpendCoins(long coins)
    {
        if (GetCurrentCoins() >= coins)
        {
            coinsSpent += coins;
            return true;
        }
        return false;
    }
    
    public void UnlockTeam(string teamName)
    {
        if (!unlockedTeams.Contains(teamName))
        {
            unlockedTeams.Add(teamName);
        }
    }
    
    public void UnlockStadium(string stadiumName)
    {
        if (!unlockedStadiums.Contains(stadiumName))
        {
            unlockedStadiums.Add(stadiumName);
        }
    }
    
    public void AddAchievement(string achievement)
    {
        if (!achievements.Contains(achievement))
        {
            achievements.Add(achievement);
        }
    }
    
    public bool HasAchievement(string achievement)
    {
        return achievements.Contains(achievement);
    }
}

[System.Serializable]
public class SerializableList<T>
{
    public List<T> items;
    
    public SerializableList(List<T> list)
    {
        items = list;
    }
}