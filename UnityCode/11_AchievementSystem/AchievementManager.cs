using UnityEngine;
using System.Collections.Generic;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;
    
    [Header("Achievement Settings")]
    public List<Achievement> allAchievements = new List<Achievement>();
    public GameObject achievementPopupPrefab;
    public Transform achievementContainer;
    public AudioClip achievementSound;
    
    [Header("Progress Tracking")]
    public Dictionary<string, int> progressTracker = new Dictionary<string, int>();
    public List<string> unlockedAchievements = new List<string>();
    
    // Eventos
    public System.Action<Achievement> OnAchievementUnlocked;
    public System.Action<Achievement, int> OnAchievementProgress;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAchievements();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeAchievements()
    {
        CreateDefaultAchievements();
        LoadAchievementProgress();
        
        // Suscribirse a eventos del juego
        SubscribeToGameEvents();
    }
    
    void CreateDefaultAchievements()
    {
        // Logros de goles
        AddAchievement(new Achievement
        {
            id = "first_goal",
            title = "Primer Gol",
            description = "Marca tu primer gol",
            icon = null,
            targetValue = 1,
            currentValue = 0,
            rewardCoins = 100,
            rewardXP = 50,
            isUnlocked = false,
            type = AchievementType.Goals
        });
        
        AddAchievement(new Achievement
        {
            id = "hat_trick",
            title = "Hat Trick",
            description = "Marca 3 goles en un partido",
            icon = null,
            targetValue = 3,
            currentValue = 0,
            rewardCoins = 500,
            rewardXP = 200,
            isUnlocked = false,
            type = AchievementType.Goals
        });
        
        AddAchievement(new Achievement
        {
            id = "goal_machine",
            title = "Máquina de Goles",
            description = "Marca 100 goles",
            icon = null,
            targetValue = 100,
            currentValue = 0,
            rewardCoins = 2000,
            rewardXP = 1000,
            isUnlocked = false,
            type = AchievementType.Goals
        });
        
        // Logros de partidos
        AddAchievement(new Achievement
        {
            id = "first_win",
            title = "Primera Victoria",
            description = "Gana tu primer partido",
            icon = null,
            targetValue = 1,
            currentValue = 0,
            rewardCoins = 200,
            rewardXP = 100,
            isUnlocked = false,
            type = AchievementType.Matches
        });
        
        AddAchievement(new Achievement
        {
            id = "winning_streak",
            title = "Racha Ganadora",
            description = "Gana 5 partidos seguidos",
            icon = null,
            targetValue = 5,
            currentValue = 0,
            rewardCoins = 800,
            rewardXP = 400,
            isUnlocked = false,
            type = AchievementType.Matches
        });
        
        AddAchievement(new Achievement
        {
            id = "century_club",
            title = "Club del Siglo",
            description = "Juega 100 partidos",
            icon = null,
            targetValue = 100,
            currentValue = 0,
            rewardCoins = 1500,
            rewardXP = 750,
            isUnlocked = false,
            type = AchievementType.Matches
        });
        
        // Logros de habilidades
        AddAchievement(new Achievement
        {
            id = "trick_master",
            title = "Maestro de Trucos",
            description = "Realiza 50 trucos exitosos",
            icon = null,
            targetValue = 50,
            currentValue = 0,
            rewardCoins = 1000,
            rewardXP = 500,
            isUnlocked = false,
            type = AchievementType.Skills
        });
        
        AddAchievement(new Achievement
        {
            id = "assist_king",
            title = "Rey de Asistencias",
            description = "Realiza 25 asistencias",
            icon = null,
            targetValue = 25,
            currentValue = 0,
            rewardCoins = 750,
            rewardXP = 375,
            isUnlocked = false,
            type = AchievementType.Skills
        });
        
        // Logros de ligas
        AddAchievement(new Achievement
        {
            id = "champion",
            title = "Campeón",
            description = "Gana tu primera liga",
            icon = null,
            targetValue = 1,
            currentValue = 0,
            rewardCoins = 5000,
            rewardXP = 2000,
            isUnlocked = false,
            type = AchievementType.Trophies
        });
        
        AddAchievement(new Achievement
        {
            id = "dynasty",
            title = "Dinastía",
            description = "Gana 3 ligas consecutivas",
            icon = null,
            targetValue = 3,
            currentValue = 0,
            rewardCoins = 10000,
            rewardXP = 5000,
            isUnlocked = false,
            type = AchievementType.Trophies
        });
        
        // Logros especiales
        AddAchievement(new Achievement
        {
            id = "clean_sheet",
            title = "Portería Imbatida",
            description = "Mantén la portería en cero en 10 partidos",
            icon = null,
            targetValue = 10,
            currentValue = 0,
            rewardCoins = 600,
            rewardXP = 300,
            isUnlocked = false,
            type = AchievementType.Defense
        });
        
        AddAchievement(new Achievement
        {
            id = "long_shot_master",
            title = "Maestro de Larga Distancia",
            description = "Marca 5 goles desde fuera del área",
            icon = null,
            targetValue = 5,
            currentValue = 0,
            rewardCoins = 800,
            rewardXP = 400,
            isUnlocked = false,
            type = AchievementType.Skills
        });
    }
    
    void AddAchievement(Achievement achievement)
    {
        allAchievements.Add(achievement);
        
        if (!progressTracker.ContainsKey(achievement.id))
        {
            progressTracker[achievement.id] = 0;
        }
    }
    
    void SubscribeToGameEvents()
    {
        // Suscribirse a eventos del GameManager
        GameManager.OnGoalScored += OnGoalScored;
        GameManager.OnGameStateChanged += OnGameStateChanged;
        
        // Suscribirse a eventos de jugadores
        // PlayerStats eventos, etc.
    }
    
    void OnGoalScored(GoalInfo goalInfo)
    {
        UpdateAchievementProgress("first_goal", 1);
        UpdateAchievementProgress("goal_machine", 1);
        
        // Verificar hat trick
        PlayerStats scorerStats = goalInfo.scorer.GetComponent<PlayerStats>();
        if (scorerStats != null && scorerStats.goals >= 3)
        {
            UpdateAchievementProgress("hat_trick", 1);
        }
    }
    
    void OnGameStateChanged(GameState newState)
    {
        if (newState == GameState.GameEnded)
        {
            // Verificar victoria
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                GameInfo gameInfo = gameManager.GetGameInfo();
                
                // Verificar quién ganó
                if (gameInfo.homeScore > gameInfo.awayScore)
                {
                    UpdateAchievementProgress("first_win", 1);
                    UpdateAchievementProgress("winning_streak", 1);
                }
                else if (gameInfo.homeScore < gameInfo.awayScore)
                {
                    // Resetear racha ganadora
                    ResetAchievementProgress("winning_streak");
                }
                
                // Partido jugado
                UpdateAchievementProgress("century_club", 1);
                
                // Verificar portería imbatida
                if (gameInfo.awayScore == 0)
                {
                    UpdateAchievementProgress("clean_sheet", 1);
                }
            }
        }
    }
    
    public void UpdateAchievementProgress(string achievementId, int progress)
    {
        Achievement achievement = GetAchievementById(achievementId);
        if (achievement != null && !achievement.isUnlocked)
        {
            achievement.currentValue += progress;
            progressTracker[achievementId] = achievement.currentValue;
            
            // Disparar evento de progreso
            OnAchievementProgress?.Invoke(achievement, progress);
            
            // Verificar si se desbloqueó
            if (achievement.currentValue >= achievement.targetValue)
            {
                UnlockAchievement(achievement);
            }
            
            SaveAchievementProgress();
        }
    }
    
    public void ResetAchievementProgress(string achievementId)
    {
        Achievement achievement = GetAchievementById(achievementId);
        if (achievement != null)
        {
            achievement.currentValue = 0;
            progressTracker[achievementId] = 0;
            SaveAchievementProgress();
        }
    }
    
    void UnlockAchievement(Achievement achievement)
    {
        if (achievement.isUnlocked) return;
        
        achievement.isUnlocked = true;
        unlockedAchievements.Add(achievement.id);
        
        // Dar recompensas
        if (GameData.Instance != null && GameData.Instance.playerProfile != null)
        {
            GameData.Instance.playerProfile.AddCoins(achievement.rewardCoins);
            GameData.Instance.playerProfile.AddXP(achievement.rewardXP);
            GameData.Instance.playerProfile.AddAchievement(achievement.id);
        }
        
        // Mostrar popup
        ShowAchievementPopup(achievement);
        
        // Reproducir sonido
        if (AudioManager.Instance != null && achievementSound != null)
        {
            AudioManager.Instance.PlaySFX(achievementSound);
        }
        
        // Disparar evento
        OnAchievementUnlocked?.Invoke(achievement);
        
        // Guardar progreso
        SaveAchievementProgress();
        
        Debug.Log($"Achievement Unlocked: {achievement.title}");
    }
    
    void ShowAchievementPopup(Achievement achievement)
    {
        if (achievementPopupPrefab != null && achievementContainer != null)
        {
            GameObject popup = Instantiate(achievementPopupPrefab, achievementContainer);
            AchievementPopup popupScript = popup.GetComponent<AchievementPopup>();
            
            if (popupScript != null)
            {
                popupScript.ShowAchievement(achievement);
            }
            
            // Destruir después de unos segundos
            Destroy(popup, 5f);
        }
    }
    
    public Achievement GetAchievementById(string id)
    {
        return allAchievements.Find(a => a.id == id);
    }
    
    public List<Achievement> GetAchievementsByType(AchievementType type)
    {
        return allAchievements.FindAll(a => a.type == type);
    }
    
    public List<Achievement> GetUnlockedAchievements()
    {
        return allAchievements.FindAll(a => a.isUnlocked);
    }
    
    public List<Achievement> GetLockedAchievements()
    {
        return allAchievements.FindAll(a => !a.isUnlocked);
    }
    
    public float GetCompletionPercentage()
    {
        int unlockedCount = GetUnlockedAchievements().Count;
        return (float)unlockedCount / allAchievements.Count * 100f;
    }
    
    public int GetTotalRewardCoins()
    {
        int total = 0;
        foreach (Achievement achievement in GetUnlockedAchievements())
        {
            total += achievement.rewardCoins;
        }
        return total;
    }
    
    public int GetTotalRewardXP()
    {
        int total = 0;
        foreach (Achievement achievement in GetUnlockedAchievements())
        {
            total += achievement.rewardXP;
        }
        return total;
    }
    
    void SaveAchievementProgress()
    {
        AchievementSaveData saveData = new AchievementSaveData
        {
            progressTracker = progressTracker,
            unlockedAchievements = unlockedAchievements
        };
        
        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString("AchievementProgress", json);
    }
    
    void LoadAchievementProgress()
    {
        if (PlayerPrefs.HasKey("AchievementProgress"))
        {
            string json = PlayerPrefs.GetString("AchievementProgress");
            AchievementSaveData saveData = JsonUtility.FromJson<AchievementSaveData>(json);
            
            progressTracker = saveData.progressTracker;
            unlockedAchievements = saveData.unlockedAchievements;
            
            // Aplicar progreso a los logros
            foreach (Achievement achievement in allAchievements)
            {
                if (progressTracker.ContainsKey(achievement.id))
                {
                    achievement.currentValue = progressTracker[achievement.id];
                }
                
                achievement.isUnlocked = unlockedAchievements.Contains(achievement.id);
            }
        }
    }
    
    // Métodos para logros específicos
    public void OnTrickPerformed(TrickType trickType)
    {
        UpdateAchievementProgress("trick_master", 1);
    }
    
    public void OnAssistMade()
    {
        UpdateAchievementProgress("assist_king", 1);
    }
    
    public void OnLeagueWon()
    {
        UpdateAchievementProgress("champion", 1);
        UpdateAchievementProgress("dynasty", 1);
    }
    
    public void OnLongShotGoal()
    {
        UpdateAchievementProgress("long_shot_master", 1);
    }
    
    public void ResetAllAchievements()
    {
        foreach (Achievement achievement in allAchievements)
        {
            achievement.currentValue = 0;
            achievement.isUnlocked = false;
        }
        
        progressTracker.Clear();
        unlockedAchievements.Clear();
        
        PlayerPrefs.DeleteKey("AchievementProgress");
        
        Debug.Log("All achievements reset!");
    }
}

[System.Serializable]
public class Achievement
{
    public string id;
    public string title;
    public string description;
    public Sprite icon;
    public int targetValue;
    public int currentValue;
    public int rewardCoins;
    public int rewardXP;
    public bool isUnlocked;
    public AchievementType type;
    
    public float GetProgress()
    {
        return targetValue > 0 ? (float)currentValue / targetValue : 0f;
    }
    
    public string GetProgressText()
    {
        return $"{currentValue}/{targetValue}";
    }
}

[System.Serializable]
public class AchievementSaveData
{
    public Dictionary<string, int> progressTracker;
    public List<string> unlockedAchievements;
}

public enum AchievementType
{
    Goals,
    Matches,
    Skills,
    Trophies,
    Defense,
    Special
}

public class AchievementPopup : MonoBehaviour
{
    [Header("UI Elements")]
    public UnityEngine.UI.Text titleText;
    public UnityEngine.UI.Text descriptionText;
    public UnityEngine.UI.Text rewardText;
    public UnityEngine.UI.Image iconImage;
    public Animator popupAnimator;
    
    public void ShowAchievement(Achievement achievement)
    {
        titleText.text = achievement.title;
        descriptionText.text = achievement.description;
        rewardText.text = $"+{achievement.rewardCoins} monedas, +{achievement.rewardXP} XP";
        
        if (achievement.icon != null)
        {
            iconImage.sprite = achievement.icon;
        }
        
        if (popupAnimator != null)
        {
            popupAnimator.SetTrigger("Show");
        }
    }
}