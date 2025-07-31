using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using FootballMaster.PlayerSystem;
using FootballMaster.WebIntegration;

namespace FootballMaster.Achievements
{
    [Serializable]
    public class Achievement
    {
        public string id;
        public string name;
        public string description;
        public string icon;
        public AchievementCategory category;
        public AchievementRarity rarity;
        public int rewardXP;
        public int rewardCoins;
        public bool isUnlocked;
        public DateTime unlockedDate;
        public AchievementRequirement requirement;
        public bool isHidden; // Hidden until unlocked
        public string unlockCondition;
    }

    [Serializable]
    public class AchievementRequirement
    {
        public RequirementType type;
        public string statName;
        public float targetValue;
        public ComparisonType comparison;
        public Dictionary<string, object> additionalData;
    }

    public enum AchievementCategory
    {
        Scoring,
        Passing,
        Defending,
        Skills,
        Career,
        Special,
        Streak,
        Collection,
        Social,
        Challenges
    }

    public enum AchievementRarity
    {
        Common,
        Rare,
        Epic,
        Legendary,
        Mythic
    }

    public enum RequirementType
    {
        StatThreshold,
        SpecialMove,
        MatchCondition,
        CareerMilestone,
        CollectionComplete,
        Social,
        TimeBased,
        Combo
    }

    public enum ComparisonType
    {
        GreaterThan,
        GreaterThanOrEqual,
        Equal,
        LessThan,
        LessThanOrEqual
    }

    [Serializable]
    public class PlayerStatistics
    {
        // Basic Match Stats
        public int matchesPlayed;
        public int matchesWon;
        public int matchesDrawn;
        public int matchesLost;
        public int goalsScored;
        public int goalsConceded;
        public int assists;
        public int shots;
        public int shotsOnTarget;
        public int passes;
        public int passesCompleted;
        public int tackles;
        public int interceptions;
        public int saves; // For goalkeepers
        public int cleanSheets;
        public int yellowCards;
        public int redCards;
        
        // Skill Move Stats
        public int elasticoCount;
        public int rainbowFlicksCount;
        public int stepOversCount;
        public int nutmegsCount;
        public int roulettesCount;
        public int rabonasCount;
        public int bicycleKicksCount;
        public int heelFlicksCount;
        public int totalSkillMoves;
        public int successfulSkillMoves;
        public float skillMoveSuccessRate;
        
        // Special Achievement Stats
        public int hatTricks;
        public int perfectPasses; // 100% pass accuracy in a match
        public int longShots; // Goals from outside the box
        public int headerGoals;
        public int freeKickGoals;
        public int penaltyGoals;
        public int lastMinuteGoals; // Goals in 85th minute or later
        public int wonderGoals; // Spectacular goals
        public int comebackWins; // Wins when losing at half-time
        public int cleanSheetsStreak;
        public int goalScoringStreak;
        public int winningStreak;
        public int unbeatenStreak;
        
        // Distance and Position Stats
        public float totalDistanceRun;
        public float averageDistancePerMatch;
        public int sprintCount;
        public Dictionary<string, int> positionsPlayed;
        public Dictionary<string, int> formationsUsed;
        
        // Time-based Stats
        public float totalPlayTime;
        public int consecutiveDaysPlayed;
        public DateTime lastPlayDate;
        public DateTime firstPlayDate;
        
        // Career Stats
        public int level;
        public int experience;
        public int trophiesWon;
        public List<string> teamsManaged;
        public int transfersMade;
        public int playersDiscovered;
        public float careerRating;
        
        // Special Combos
        public int skillMoveCombos; // Multiple skill moves in sequence
        public int goalAndAssistSameMatch;
        public int doubleHatTricks; // Hat-trick in consecutive matches
        public int perfectMatches; // 100% pass accuracy + clean sheet + goal
    }

    public class AchievementManager : MonoBehaviour
    {
        [Header("Achievement System")]
        public List<Achievement> allAchievements = new List<Achievement>();
        public List<Achievement> unlockedAchievements = new List<Achievement>();
        
        [Header("Statistics")]
        public PlayerStatistics playerStats = new PlayerStatistics();
        
        [Header("UI References")]
        public GameObject achievementNotificationPrefab;
        public Transform notificationParent;
        public AudioClip achievementUnlockSound;
        
        [Header("Integration")]
        public UnityWebIntegration webIntegration;
        public bool syncWithBackend = true;
        
        // Events
        public event System.Action<Achievement> OnAchievementUnlocked;
        public event System.Action<PlayerStatistics> OnStatisticsUpdated;
        
        private AudioSource audioSource;
        private bool isInitialized = false;
        
        void Start()
        {
            InitializeAchievementSystem();
        }
        
        void InitializeAchievementSystem()
        {
            if (isInitialized) return;
            
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            
            // Load saved statistics
            LoadPlayerStatistics();
            
            // Initialize achievements database
            InitializeAchievements();
            
            // Load unlocked achievements
            LoadUnlockedAchievements();
            
            isInitialized = true;
            Debug.Log("Achievement System initialized");
        }
        
        void InitializeAchievements()
        {
            // Clear existing achievements
            allAchievements.Clear();
            
            // === SCORING ACHIEVEMENTS ===
            AddAchievement("first_goal", "First Blood", "Score your first goal", "⚽", 
                AchievementCategory.Scoring, AchievementRarity.Common, 100, 500,
                RequirementType.StatThreshold, "goalsScored", 1);
                
            AddAchievement("hat_trick_hero", "Hat-Trick Hero", "Score a hat-trick in a single match", "🎩", 
                AchievementCategory.Scoring, AchievementRarity.Rare, 500, 2000,
                RequirementType.StatThreshold, "hatTricks", 1);
                
            AddAchievement("goal_machine", "Goal Machine", "Score 100 goals", "🤖", 
                AchievementCategory.Scoring, AchievementRarity.Epic, 2000, 10000,
                RequirementType.StatThreshold, "goalsScored", 100);
                
            AddAchievement("legendary_scorer", "Legendary Scorer", "Score 500 goals", "👑", 
                AchievementCategory.Scoring, AchievementRarity.Legendary, 5000, 25000,
                RequirementType.StatThreshold, "goalsScored", 500);
                
            AddAchievement("bicycle_master", "Bicycle Kick Master", "Score 10 bicycle kick goals", "🚲", 
                AchievementCategory.Scoring, AchievementRarity.Epic, 1500, 7500,
                RequirementType.StatThreshold, "bicycleKicksCount", 10);
                
            AddAchievement("free_kick_specialist", "Free Kick Specialist", "Score 25 free kick goals", "🥅", 
                AchievementCategory.Scoring, AchievementRarity.Rare, 1000, 5000,
                RequirementType.StatThreshold, "freeKickGoals", 25);
                
            AddAchievement("header_king", "Header King", "Score 50 header goals", "🦌", 
                AchievementCategory.Scoring, AchievementRarity.Rare, 800, 4000,
                RequirementType.StatThreshold, "headerGoals", 50);
                
            AddAchievement("wonder_goal", "Wonder Goal", "Score your first spectacular goal", "✨", 
                AchievementCategory.Scoring, AchievementRarity.Rare, 300, 1500,
                RequirementType.StatThreshold, "wonderGoals", 1);
                
            // === SKILL MOVE ACHIEVEMENTS ===
            AddAchievement("first_skill", "First Flair", "Perform your first skill move", "🌟", 
                AchievementCategory.Skills, AchievementRarity.Common, 50, 250,
                RequirementType.SpecialMove, "totalSkillMoves", 1);
                
            AddAchievement("elastico_expert", "Elastico Expert", "Perform 50 elastico moves", "🔄", 
                AchievementCategory.Skills, AchievementRarity.Rare, 750, 3750,
                RequirementType.SpecialMove, "elasticoCount", 50);
                
            AddAchievement("rainbow_legend", "Rainbow Legend", "Perform 25 rainbow flicks", "🌈", 
                AchievementCategory.Skills, AchievementRarity.Epic, 1000, 5000,
                RequirementType.SpecialMove, "rainbowFlicksCount", 25);
                
            AddAchievement("nutmeg_king", "Nutmeg King", "Perform 100 nutmegs", "🥜", 
                AchievementCategory.Skills, AchievementRarity.Rare, 600, 3000,
                RequirementType.SpecialMove, "nutmegsCount", 100);
                
            AddAchievement("skill_master", "Skill Master", "Successfully perform 500 skill moves", "🎭", 
                AchievementCategory.Skills, AchievementRarity.Epic, 2500, 12500,
                RequirementType.StatThreshold, "successfulSkillMoves", 500);
                
            AddAchievement("flair_perfectionist", "Flair Perfectionist", "Achieve 90% skill move success rate (min 100 attempts)", "💯", 
                AchievementCategory.Skills, AchievementRarity.Legendary, 3000, 15000,
                RequirementType.StatThreshold, "skillMoveSuccessRate", 0.9f);
                
            AddAchievement("combo_master", "Combo Master", "Perform 50 skill move combos", "🎪", 
                AchievementCategory.Skills, AchievementRarity.Epic, 1500, 7500,
                RequirementType.StatThreshold, "skillMoveCombos", 50);
                
            // === PASSING ACHIEVEMENTS ===
            AddAchievement("first_assist", "Team Player", "Make your first assist", "🤝", 
                AchievementCategory.Passing, AchievementRarity.Common, 75, 375,
                RequirementType.StatThreshold, "assists", 1);
                
            AddAchievement("assist_master", "Assist Master", "Make 100 assists", "🎯", 
                AchievementCategory.Passing, AchievementRarity.Rare, 1200, 6000,
                RequirementType.StatThreshold, "assists", 100);
                
            AddAchievement("perfect_passer", "Perfect Passer", "Achieve 100% pass accuracy in a match (min 20 passes)", "🎪", 
                AchievementCategory.Passing, AchievementRarity.Rare, 500, 2500,
                RequirementType.StatThreshold, "perfectPasses", 1);
                
            AddAchievement("pass_master", "Pass Master", "Complete 1000 passes", "📈", 
                AchievementCategory.Passing, AchievementRarity.Epic, 1000, 5000,
                RequirementType.StatThreshold, "passesCompleted", 1000);
                
            // === DEFENDING ACHIEVEMENTS ===
            AddAchievement("first_tackle", "Defensive Debut", "Make your first tackle", "🛡️", 
                AchievementCategory.Defending, AchievementRarity.Common, 50, 250,
                RequirementType.StatThreshold, "tackles", 1);
                
            AddAchievement("clean_sheet", "Clean Sheet", "Keep your first clean sheet", "🥅", 
                AchievementCategory.Defending, AchievementRarity.Common, 100, 500,
                RequirementType.StatThreshold, "cleanSheets", 1);
                
            AddAchievement("defensive_wall", "Defensive Wall", "Keep 25 clean sheets", "🧱", 
                AchievementCategory.Defending, AchievementRarity.Rare, 800, 4000,
                RequirementType.StatThreshold, "cleanSheets", 25);
                
            AddAchievement("interception_master", "Interception Master", "Make 200 interceptions", "🕷️", 
                AchievementCategory.Defending, AchievementRarity.Rare, 600, 3000,
                RequirementType.StatThreshold, "interceptions", 200);
                
            // === STREAK ACHIEVEMENTS ===
            AddAchievement("winning_streak", "On Fire", "Win 5 matches in a row", "🔥", 
                AchievementCategory.Streak, AchievementRarity.Rare, 750, 3750,
                RequirementType.StatThreshold, "winningStreak", 5);
                
            AddAchievement("unbeatable", "Unbeatable", "Go 10 matches without losing", "⚔️", 
                AchievementCategory.Streak, AchievementRarity.Epic, 1500, 7500,
                RequirementType.StatThreshold, "unbeatenStreak", 10);
                
            AddAchievement("goal_streak", "Goal Machine", "Score in 10 consecutive matches", "⚽", 
                AchievementCategory.Streak, AchievementRarity.Epic, 1200, 6000,
                RequirementType.StatThreshold, "goalScoringStreak", 10);
                
            AddAchievement("perfect_season", "Perfect Season", "Win all matches in a season", "👑", 
                AchievementCategory.Streak, AchievementRarity.Mythic, 10000, 50000,
                RequirementType.MatchCondition, "perfectSeason", 1);
                
            // === SPECIAL ACHIEVEMENTS ===
            AddAchievement("comeback_king", "Comeback King", "Win 10 matches after being behind at half-time", "👑", 
                AchievementCategory.Special, AchievementRarity.Epic, 2000, 10000,
                RequirementType.StatThreshold, "comebackWins", 10);
                
            AddAchievement("last_minute_hero", "Last Minute Hero", "Score 20 goals in the 85th minute or later", "⏰", 
                AchievementCategory.Special, AchievementRarity.Rare, 1000, 5000,
                RequirementType.StatThreshold, "lastMinuteGoals", 20);
                
            AddAchievement("perfect_match", "Perfect Match", "Achieve 100% pass accuracy, clean sheet, and score a goal in the same match", "💎", 
                AchievementCategory.Special, AchievementRarity.Legendary, 5000, 25000,
                RequirementType.StatThreshold, "perfectMatches", 1);
                
            AddAchievement("world_traveler", "World Traveler", "Play matches in 20 different stadiums", "🌍", 
                AchievementCategory.Special, AchievementRarity.Rare, 800, 4000,
                RequirementType.StatThreshold, "stadiumsVisited", 20);
                
            // === CAREER ACHIEVEMENTS ===
            AddAchievement("new_manager", "New Manager", "Start your first career", "👔", 
                AchievementCategory.Career, AchievementRarity.Common, 200, 1000,
                RequirementType.CareerMilestone, "careerStarted", 1);
                
            AddAchievement("trophy_hunter", "Trophy Hunter", "Win your first trophy", "🏆", 
                AchievementCategory.Career, AchievementRarity.Rare, 1000, 5000,
                RequirementType.StatThreshold, "trophiesWon", 1);
                
            AddAchievement("legend", "Legend", "Reach level 50", "⭐", 
                AchievementCategory.Career, AchievementRarity.Legendary, 5000, 25000,
                RequirementType.StatThreshold, "level", 50);
                
            AddAchievement("transfer_master", "Transfer Master", "Complete 100 transfers", "🔄", 
                AchievementCategory.Career, AchievementRarity.Epic, 1500, 7500,
                RequirementType.StatThreshold, "transfersMade", 100);
                
            Debug.Log($"Initialized {allAchievements.Count} achievements");
        }
        
        void AddAchievement(string id, string name, string description, string icon, 
            AchievementCategory category, AchievementRarity rarity, int rewardXP, int rewardCoins,
            RequirementType reqType, string statName, float targetValue, bool isHidden = false)
        {
            var achievement = new Achievement
            {
                id = id,
                name = name,
                description = description,
                icon = icon,
                category = category,
                rarity = rarity,
                rewardXP = rewardXP,
                rewardCoins = rewardCoins,
                isUnlocked = false,
                isHidden = isHidden,
                requirement = new AchievementRequirement
                {
                    type = reqType,
                    statName = statName,
                    targetValue = targetValue,
                    comparison = ComparisonType.GreaterThanOrEqual,
                    additionalData = new Dictionary<string, object>()
                }
            };
            
            allAchievements.Add(achievement);
        }
        
        // === STATISTIC UPDATE METHODS ===
        public void OnGoalScored(bool isHeader = false, bool isBicycleKick = false, bool isFreeKick = false, 
            bool isPenalty = false, bool isLongShot = false, bool isLastMinute = false, bool isWonderGoal = false)
        {
            playerStats.goalsScored++;
            playerStats.shots++;
            playerStats.shotsOnTarget++;
            
            if (isHeader) playerStats.headerGoals++;
            if (isBicycleKick) playerStats.bicycleKicksCount++;
            if (isFreeKick) playerStats.freeKickGoals++;
            if (isPenalty) playerStats.penaltyGoals++;
            if (isLongShot) playerStats.longShots++;
            if (isLastMinute) playerStats.lastMinuteGoals++;
            if (isWonderGoal) playerStats.wonderGoals++;
            
            CheckAchievements();
            OnStatisticsUpdated?.Invoke(playerStats);
        }
        
        public void OnAssistMade()
        {
            playerStats.assists++;
            CheckAchievements();
            OnStatisticsUpdated?.Invoke(playerStats);
        }
        
        public void OnSkillMovePerformed(TrickType trickType, bool wasSuccessful)
        {
            playerStats.totalSkillMoves++;
            if (wasSuccessful) playerStats.successfulSkillMoves++;
            
            switch (trickType)
            {
                case TrickType.Elastico:
                    playerStats.elasticoCount++;
                    break;
                case TrickType.RainbowFlick:
                    playerStats.rainbowFlicksCount++;
                    break;
                case TrickType.StepOverLeft:
                case TrickType.StepOverRight:
                    playerStats.stepOversCount++;
                    break;
                case TrickType.Nutmeg:
                    playerStats.nutmegsCount++;
                    break;
                case TrickType.Roulette:
                    playerStats.roulettesCount++;
                    break;
                case TrickType.Rabona:
                    playerStats.rabonasCount++;
                    break;
                case TrickType.HeelFlick:
                    playerStats.heelFlicksCount++;
                    break;
            }
            
            // Update success rate
            if (playerStats.totalSkillMoves > 0)
            {
                playerStats.skillMoveSuccessRate = (float)playerStats.successfulSkillMoves / playerStats.totalSkillMoves;
            }
            
            CheckAchievements();
            OnStatisticsUpdated?.Invoke(playerStats);
        }
        
        public void OnMatchCompleted(bool won, bool drawn, bool lost, bool cleanSheet, 
            bool perfectPassing, bool comebackWin, int goalsScored, int goalsConceded)
        {
            playerStats.matchesPlayed++;
            
            if (won)
            {
                playerStats.matchesWon++;
                playerStats.winningStreak++;
                playerStats.unbeatenStreak++;
            }
            else if (drawn)
            {
                playerStats.matchesDrawn++;
                playerStats.winningStreak = 0;
                playerStats.unbeatenStreak++;
            }
            else if (lost)
            {
                playerStats.matchesLost++;
                playerStats.winningStreak = 0;
                playerStats.unbeatenStreak = 0;
            }
            
            if (cleanSheet)
            {
                playerStats.cleanSheets++;
                playerStats.cleanSheetsStreak++;
            }
            else
            {
                playerStats.cleanSheetsStreak = 0;
            }
            
            if (perfectPassing) playerStats.perfectPasses++;
            if (comebackWin) playerStats.comebackWins++;
            
            // Check for perfect match (100% passing + clean sheet + goal)
            if (perfectPassing && cleanSheet && goalsScored > 0)
            {
                playerStats.perfectMatches++;
            }
            
            CheckAchievements();
            OnStatisticsUpdated?.Invoke(playerStats);
        }
        
        public void OnTackleMade()
        {
            playerStats.tackles++;
            CheckAchievements();
            OnStatisticsUpdated?.Invoke(playerStats);
        }
        
        public void OnInterceptionMade()
        {
            playerStats.interceptions++;
            CheckAchievements();
            OnStatisticsUpdated?.Invoke(playerStats);
        }
        
        public void OnPassMade(bool wasCompleted)
        {
            playerStats.passes++;
            if (wasCompleted) playerStats.passesCompleted++;
            
            CheckAchievements();
            OnStatisticsUpdated?.Invoke(playerStats);
        }
        
        public void OnSkillComboPerformed()
        {
            playerStats.skillMoveCombos++;
            CheckAchievements();
            OnStatisticsUpdated?.Invoke(playerStats);
        }
        
        public void OnHatTrickScored()
        {
            playerStats.hatTricks++;
            CheckAchievements();
            OnStatisticsUpdated?.Invoke(playerStats);
        }
        
        public void OnTrophyWon()
        {
            playerStats.trophiesWon++;
            CheckAchievements();
            OnStatisticsUpdated?.Invoke(playerStats);
        }
        
        public void OnLevelUp(int newLevel)
        {
            playerStats.level = newLevel;
            CheckAchievements();
            OnStatisticsUpdated?.Invoke(playerStats);
        }
        
        public void OnTransferCompleted()
        {
            playerStats.transfersMade++;
            CheckAchievements();
            OnStatisticsUpdated?.Invoke(playerStats);
        }
        
        // === ACHIEVEMENT CHECKING ===
        void CheckAchievements()
        {
            foreach (var achievement in allAchievements)
            {
                if (!achievement.isUnlocked && CheckAchievementRequirement(achievement))
                {
                    UnlockAchievement(achievement);
                }
            }
        }
        
        bool CheckAchievementRequirement(Achievement achievement)
        {
            var req = achievement.requirement;
            
            switch (req.type)
            {
                case RequirementType.StatThreshold:
                    return CheckStatThreshold(req.statName, req.targetValue, req.comparison);
                    
                case RequirementType.SpecialMove:
                    return CheckSpecialMoveRequirement(req.statName, req.targetValue);
                    
                case RequirementType.MatchCondition:
                    return CheckMatchCondition(req.statName, req.targetValue);
                    
                case RequirementType.CareerMilestone:
                    return CheckCareerMilestone(req.statName, req.targetValue);
                    
                default:
                    return false;
            }
        }
        
        bool CheckStatThreshold(string statName, float targetValue, ComparisonType comparison)
        {
            float currentValue = GetStatValue(statName);
            
            switch (comparison)
            {
                case ComparisonType.GreaterThan:
                    return currentValue > targetValue;
                case ComparisonType.GreaterThanOrEqual:
                    return currentValue >= targetValue;
                case ComparisonType.Equal:
                    return Mathf.Approximately(currentValue, targetValue);
                case ComparisonType.LessThan:
                    return currentValue < targetValue;
                case ComparisonType.LessThanOrEqual:
                    return currentValue <= targetValue;
                default:
                    return false;
            }
        }
        
        bool CheckSpecialMoveRequirement(string statName, float targetValue)
        {
            return GetStatValue(statName) >= targetValue;
        }
        
        bool CheckMatchCondition(string conditionName, float targetValue)
        {
            // This would check specific match conditions
            // Implementation depends on the specific condition
            return false; // Placeholder
        }
        
        bool CheckCareerMilestone(string milestoneName, float targetValue)
        {
            // This would check career-specific milestones
            // Implementation depends on the specific milestone
            return false; // Placeholder
        }
        
        float GetStatValue(string statName)
        {
            switch (statName)
            {
                case "goalsScored": return playerStats.goalsScored;
                case "assists": return playerStats.assists;
                case "hatTricks": return playerStats.hatTricks;
                case "totalSkillMoves": return playerStats.totalSkillMoves;
                case "successfulSkillMoves": return playerStats.successfulSkillMoves;
                case "skillMoveSuccessRate": return playerStats.skillMoveSuccessRate;
                case "elasticoCount": return playerStats.elasticoCount;
                case "rainbowFlicksCount": return playerStats.rainbowFlicksCount;
                case "nutmegsCount": return playerStats.nutmegsCount;
                case "roulettesCount": return playerStats.roulettesCount;
                case "bicycleKicksCount": return playerStats.bicycleKicksCount;
                case "freeKickGoals": return playerStats.freeKickGoals;
                case "headerGoals": return playerStats.headerGoals;
                case "wonderGoals": return playerStats.wonderGoals;
                case "tackles": return playerStats.tackles;
                case "interceptions": return playerStats.interceptions;
                case "cleanSheets": return playerStats.cleanSheets;
                case "passesCompleted": return playerStats.passesCompleted;
                case "perfectPasses": return playerStats.perfectPasses;
                case "winningStreak": return playerStats.winningStreak;
                case "unbeatenStreak": return playerStats.unbeatenStreak;
                case "goalScoringStreak": return playerStats.goalScoringStreak;
                case "comebackWins": return playerStats.comebackWins;
                case "lastMinuteGoals": return playerStats.lastMinuteGoals;
                case "perfectMatches": return playerStats.perfectMatches;
                case "skillMoveCombos": return playerStats.skillMoveCombos;
                case "trophiesWon": return playerStats.trophiesWon;
                case "level": return playerStats.level;
                case "transfersMade": return playerStats.transfersMade;
                default: return 0f;
            }
        }
        
        void UnlockAchievement(Achievement achievement)
        {
            achievement.isUnlocked = true;
            achievement.unlockedDate = DateTime.Now;
            unlockedAchievements.Add(achievement);
            
            // Add rewards
            playerStats.experience += achievement.rewardXP;
            
            // Save progress
            SaveUnlockedAchievements();
            SavePlayerStatistics();
            
            // Show notification
            ShowAchievementNotification(achievement);
            
            // Play sound
            if (achievementUnlockSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(achievementUnlockSound);
            }
            
            // Sync with backend if enabled
            if (syncWithBackend && webIntegration != null)
            {
                webIntegration.UnlockAchievement(achievement.id);
            }
            
            // Fire event
            OnAchievementUnlocked?.Invoke(achievement);
            
            Debug.Log($"Achievement unlocked: {achievement.name} - {achievement.description}");
        }
        
        void ShowAchievementNotification(Achievement achievement)
        {
            if (achievementNotificationPrefab != null && notificationParent != null)
            {
                GameObject notification = Instantiate(achievementNotificationPrefab, notificationParent);
                
                // Configure notification (this would depend on your UI setup)
                var notificationScript = notification.GetComponent<AchievementNotification>();
                if (notificationScript != null)
                {
                    notificationScript.ShowAchievement(achievement);
                }
            }
        }
        
        // === SAVE/LOAD SYSTEM ===
        void SavePlayerStatistics()
        {
            string json = JsonUtility.ToJson(playerStats);
            PlayerPrefs.SetString("PlayerStatistics", json);
            PlayerPrefs.Save();
        }
        
        void LoadPlayerStatistics()
        {
            string json = PlayerPrefs.GetString("PlayerStatistics", "");
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    playerStats = JsonUtility.FromJson<PlayerStatistics>(json);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to load player statistics: {e.Message}");
                    playerStats = new PlayerStatistics();
                }
            }
            else
            {
                playerStats = new PlayerStatistics();
                playerStats.firstPlayDate = DateTime.Now;
            }
        }
        
        void SaveUnlockedAchievements()
        {
            var unlockedIds = unlockedAchievements.Select(a => a.id).ToList();
            string json = JsonUtility.ToJson(new SerializableList<string>(unlockedIds));
            PlayerPrefs.SetString("UnlockedAchievements", json);
            PlayerPrefs.Save();
        }
        
        void LoadUnlockedAchievements()
        {
            string json = PlayerPrefs.GetString("UnlockedAchievements", "");
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    var unlockedIds = JsonUtility.FromJson<SerializableList<string>>(json);
                    foreach (string id in unlockedIds.items)
                    {
                        var achievement = allAchievements.Find(a => a.id == id);
                        if (achievement != null && !achievement.isUnlocked)
                        {
                            achievement.isUnlocked = true;
                            unlockedAchievements.Add(achievement);
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to load unlocked achievements: {e.Message}");
                }
            }
        }
        
        // === PUBLIC API ===
        public List<Achievement> GetAchievementsByCategory(AchievementCategory category)
        {
            return allAchievements.Where(a => a.category == category).ToList();
        }
        
        public List<Achievement> GetUnlockedAchievements()
        {
            return unlockedAchievements;
        }
        
        public List<Achievement> GetLockedAchievements()
        {
            return allAchievements.Where(a => !a.isUnlocked && !a.isHidden).ToList();
        }
        
        public float GetCompletionPercentage()
        {
            return (float)unlockedAchievements.Count / allAchievements.Count * 100f;
        }
        
        public PlayerStatistics GetPlayerStatistics()
        {
            return playerStats;
        }
        
        public Achievement GetAchievementById(string id)
        {
            return allAchievements.Find(a => a.id == id);
        }
        
        public void ResetAllProgress()
        {
            playerStats = new PlayerStatistics();
            unlockedAchievements.Clear();
            
            foreach (var achievement in allAchievements)
            {
                achievement.isUnlocked = false;
            }
            
            PlayerPrefs.DeleteKey("PlayerStatistics");
            PlayerPrefs.DeleteKey("UnlockedAchievements");
            PlayerPrefs.Save();
            
            Debug.Log("All achievement progress reset");
        }
    }
    
    // Helper class for JSON serialization
    [System.Serializable]
    public class SerializableList<T>
    {
        public List<T> items;
        
        public SerializableList(List<T> items)
        {
            this.items = items;
        }
    }
    
    // Achievement notification UI component (basic implementation)
    public class AchievementNotification : MonoBehaviour
    {
        public void ShowAchievement(Achievement achievement)
        {
            // Implement your notification UI here
            Debug.Log($"Showing achievement notification: {achievement.name}");
            
            // Auto-destroy after a few seconds
            Destroy(gameObject, 5f);
        }
    }
}