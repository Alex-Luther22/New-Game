using System;
using System.Collections.Generic;
using UnityEngine;

namespace FootballMaster.Achievements
{
    [Serializable]
    public class AchievementData
    {
        public string id;
        public string name;
        public string description;
        public string icon;
        public string category;
        public AchievementRequirement requirement;
        public int rewardXP;
        public int rewardCoins;
        public AchievementRarity rarity;
        public string unlockCondition;
        public bool isUnlocked;
        public DateTime unlockedAt;
        public DateTime createdAt;
    }

    [Serializable]
    public class AchievementRequirement
    {
        public string type;
        public int targetValue;
        public int currentValue;
        public Dictionary<string, object> conditions;
    }

    public enum AchievementRarity
    {
        Common,
        Rare,
        Epic,
        Legendary
    }

    public enum AchievementCategory
    {
        Scoring,
        Defending,
        Streak,
        Season,
        Career,
        Trophies,
        Transfers,
        Skills,
        Exploration,
        Progress
    }

    [CreateAssetMenu(fileName = "AchievementDatabase", menuName = "Football Master/Achievement Database")]
    public class AchievementDatabase : ScriptableObject
    {
        public List<AchievementData> achievements = new List<AchievementData>();
        
        private void OnEnable()
        {
            if (achievements.Count == 0)
            {
                InitializeDefaultAchievements();
            }
        }

        private void InitializeDefaultAchievements()
        {
            achievements.AddRange(new List<AchievementData>
            {
                // Scoring Achievements
                new AchievementData
                {
                    id = "first_goal",
                    name = "First Goal",
                    description = "Score your first goal",
                    icon = "⚽",
                    category = "Scoring",
                    requirement = new AchievementRequirement
                    {
                        type = "goals_scored",
                        targetValue = 1,
                        currentValue = 0,
                        conditions = new Dictionary<string, object>()
                    },
                    rewardXP = 100,
                    rewardCoins = 500,
                    rarity = AchievementRarity.Common,
                    unlockCondition = "goals_scored >= 1",
                    isUnlocked = false,
                    createdAt = DateTime.Now
                },
                new AchievementData
                {
                    id = "hat_trick_hero",
                    name = "Hat-trick Hero",
                    description = "Score 3 goals in a single match",
                    icon = "🎩",
                    category = "Scoring",
                    requirement = new AchievementRequirement
                    {
                        type = "goals_in_match",
                        targetValue = 3,
                        currentValue = 0,
                        conditions = new Dictionary<string, object> { { "single_match", true } }
                    },
                    rewardXP = 500,
                    rewardCoins = 2000,
                    rarity = AchievementRarity.Rare,
                    unlockCondition = "goals_in_match >= 3",
                    isUnlocked = false,
                    createdAt = DateTime.Now
                },
                new AchievementData
                {
                    id = "goal_machine",
                    name = "Goal Machine",
                    description = "Score 100 goals in your career",
                    icon = "🏭",
                    category = "Scoring",
                    requirement = new AchievementRequirement
                    {
                        type = "career_goals",
                        targetValue = 100,
                        currentValue = 0,
                        conditions = new Dictionary<string, object>()
                    },
                    rewardXP = 2000,
                    rewardCoins = 10000,
                    rarity = AchievementRarity.Epic,
                    unlockCondition = "career_goals >= 100",
                    isUnlocked = false,
                    createdAt = DateTime.Now
                },
                
                // Defending Achievements
                new AchievementData
                {
                    id = "clean_sheet",
                    name = "Clean Sheet",
                    description = "Win a match without conceding",
                    icon = "🥅",
                    category = "Defending",
                    requirement = new AchievementRequirement
                    {
                        type = "clean_sheets",
                        targetValue = 1,
                        currentValue = 0,
                        conditions = new Dictionary<string, object>()
                    },
                    rewardXP = 200,
                    rewardCoins = 1000,
                    rarity = AchievementRarity.Common,
                    unlockCondition = "clean_sheets >= 1",
                    isUnlocked = false,
                    createdAt = DateTime.Now
                },
                new AchievementData
                {
                    id = "defensive_wall",
                    name = "Defensive Wall",
                    description = "Keep 10 clean sheets",
                    icon = "🛡️",
                    category = "Defending",
                    requirement = new AchievementRequirement
                    {
                        type = "clean_sheets",
                        targetValue = 10,
                        currentValue = 0,
                        conditions = new Dictionary<string, object>()
                    },
                    rewardXP = 1000,
                    rewardCoins = 5000,
                    rarity = AchievementRarity.Rare,
                    unlockCondition = "clean_sheets >= 10",
                    isUnlocked = false,
                    createdAt = DateTime.Now
                },
                
                // Streak Achievements
                new AchievementData
                {
                    id = "winning_streak",
                    name = "Winning Streak",
                    description = "Win 5 matches in a row",
                    icon = "🔥",
                    category = "Streak",
                    requirement = new AchievementRequirement
                    {
                        type = "win_streak",
                        targetValue = 5,
                        currentValue = 0,
                        conditions = new Dictionary<string, object> { { "consecutive", true } }
                    },
                    rewardXP = 750,
                    rewardCoins = 3000,
                    rarity = AchievementRarity.Rare,
                    unlockCondition = "win_streak >= 5",
                    isUnlocked = false,
                    createdAt = DateTime.Now
                },
                new AchievementData
                {
                    id = "unbeaten_season",
                    name = "Unbeaten Season",
                    description = "Go an entire season without losing",
                    icon = "👑",
                    category = "Season",
                    requirement = new AchievementRequirement
                    {
                        type = "unbeaten_season",
                        targetValue = 1,
                        currentValue = 0,
                        conditions = new Dictionary<string, object> { { "full_season", true } }
                    },
                    rewardXP = 5000,
                    rewardCoins = 25000,
                    rarity = AchievementRarity.Legendary,
                    unlockCondition = "unbeaten_season == true",
                    isUnlocked = false,
                    createdAt = DateTime.Now
                },
                
                // Career Achievements
                new AchievementData
                {
                    id = "new_manager",
                    name = "New Manager",
                    description = "Start your first career",
                    icon = "👔",
                    category = "Career",
                    requirement = new AchievementRequirement
                    {
                        type = "career_started",
                        targetValue = 1,
                        currentValue = 0,
                        conditions = new Dictionary<string, object>()
                    },
                    rewardXP = 200,
                    rewardCoins = 1000,
                    rarity = AchievementRarity.Common,
                    unlockCondition = "career_started == true",
                    isUnlocked = false,
                    createdAt = DateTime.Now
                },
                new AchievementData
                {
                    id = "legendary_manager",
                    name = "Legendary Manager",
                    description = "Reach maximum reputation",
                    icon = "⭐",
                    category = "Career",
                    requirement = new AchievementRequirement
                    {
                        type = "reputation",
                        targetValue = 10,
                        currentValue = 0,
                        conditions = new Dictionary<string, object>()
                    },
                    rewardXP = 10000,
                    rewardCoins = 50000,
                    rarity = AchievementRarity.Legendary,
                    unlockCondition = "reputation >= 10",
                    isUnlocked = false,
                    createdAt = DateTime.Now
                },
                
                // Trophy Achievements
                new AchievementData
                {
                    id = "first_trophy",
                    name = "First Trophy",
                    description = "Win your first trophy",
                    icon = "🏆",
                    category = "Trophies",
                    requirement = new AchievementRequirement
                    {
                        type = "trophies_won",
                        targetValue = 1,
                        currentValue = 0,
                        conditions = new Dictionary<string, object>()
                    },
                    rewardXP = 800,
                    rewardCoins = 3000,
                    rarity = AchievementRarity.Rare,
                    unlockCondition = "trophies_won >= 1",
                    isUnlocked = false,
                    createdAt = DateTime.Now
                },
                new AchievementData
                {
                    id = "treble_winner",
                    name = "Treble Winner",
                    description = "Win 3 trophies in one season",
                    icon = "🏅",
                    category = "Trophies",
                    requirement = new AchievementRequirement
                    {
                        type = "trophies_per_season",
                        targetValue = 3,
                        currentValue = 0,
                        conditions = new Dictionary<string, object> { { "single_season", true } }
                    },
                    rewardXP = 3000,
                    rewardCoins = 15000,
                    rarity = AchievementRarity.Epic,
                    unlockCondition = "trophies_per_season >= 3",
                    isUnlocked = false,
                    createdAt = DateTime.Now
                },
                
                // Skills Achievements
                new AchievementData
                {
                    id = "skill_master",
                    name = "Skill Master",
                    description = "Successfully perform 100 skill moves",
                    icon = "🌟",
                    category = "Skills",
                    requirement = new AchievementRequirement
                    {
                        type = "skill_moves_successful",
                        targetValue = 100,
                        currentValue = 0,
                        conditions = new Dictionary<string, object>()
                    },
                    rewardXP = 1000,
                    rewardCoins = 4000,
                    rarity = AchievementRarity.Rare,
                    unlockCondition = "skill_moves_successful >= 100",
                    isUnlocked = false,
                    createdAt = DateTime.Now
                },
                new AchievementData
                {
                    id = "trick_master",
                    name = "Trick Master",
                    description = "Master all 16 skill moves",
                    icon = "🎭",
                    category = "Skills",
                    requirement = new AchievementRequirement
                    {
                        type = "unique_tricks_mastered",
                        targetValue = 16,
                        currentValue = 0,
                        conditions = new Dictionary<string, object>()
                    },
                    rewardXP = 2500,
                    rewardCoins = 12000,
                    rarity = AchievementRarity.Epic,
                    unlockCondition = "unique_tricks_mastered >= 16",
                    isUnlocked = false,
                    createdAt = DateTime.Now
                },
                
                // Exploration Achievements
                new AchievementData
                {
                    id = "world_traveler",
                    name = "World Traveler",
                    description = "Play in 20 different stadiums",
                    icon = "🌍",
                    category = "Exploration",
                    requirement = new AchievementRequirement
                    {
                        type = "stadiums_played",
                        targetValue = 20,
                        currentValue = 0,
                        conditions = new Dictionary<string, object>()
                    },
                    rewardXP = 2000,
                    rewardCoins = 10000,
                    rarity = AchievementRarity.Epic,
                    unlockCondition = "stadiums_played >= 20",
                    isUnlocked = false,
                    createdAt = DateTime.Now
                },
                
                // Progress Achievements
                new AchievementData
                {
                    id = "level_up",
                    name = "Level Up",
                    description = "Reach level 10",
                    icon = "📈",
                    category = "Progress",
                    requirement = new AchievementRequirement
                    {
                        type = "level",
                        targetValue = 10,
                        currentValue = 0,
                        conditions = new Dictionary<string, object>()
                    },
                    rewardXP = 1000,
                    rewardCoins = 5000,
                    rarity = AchievementRarity.Common,
                    unlockCondition = "level >= 10",
                    isUnlocked = false,
                    createdAt = DateTime.Now
                }
            });
        }

        public AchievementData GetAchievementById(string id)
        {
            return achievements.Find(achievement => achievement.id == id);
        }

        public List<AchievementData> GetAchievementsByCategory(AchievementCategory category)
        {
            return achievements.FindAll(achievement => achievement.category == category.ToString());
        }

        public List<AchievementData> GetAchievementsByRarity(AchievementRarity rarity)
        {
            return achievements.FindAll(achievement => achievement.rarity == rarity);
        }

        public List<AchievementData> GetUnlockedAchievements()
        {
            return achievements.FindAll(achievement => achievement.isUnlocked);
        }

        public List<AchievementData> GetLockedAchievements()
        {
            return achievements.FindAll(achievement => !achievement.isUnlocked);
        }
    }

    public class AchievementManager : MonoBehaviour
    {
        [Header("Achievement Configuration")]
        public AchievementDatabase achievementDatabase;
        
        [Header("Player Stats Tracking")]
        public Dictionary<string, int> playerStats = new Dictionary<string, int>();
        public List<string> unlockedAchievements = new List<string>();
        
        private static AchievementManager instance;
        public static AchievementManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<AchievementManager>();
                }
                return instance;
            }
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            LoadAchievementData();
            InitializePlayerStats();
        }

        private void InitializePlayerStats()
        {
            // Initialize all tracked stats
            string[] statKeys = {
                "goals_scored", "assists", "matches_played", "wins", "draws", "losses",
                "clean_sheets", "win_streak", "current_streak", "career_goals",
                "skill_moves_successful", "unique_tricks_mastered", "stadiums_played",
                "trophies_won", "trophies_per_season", "reputation", "level",
                "transfers_completed", "seasons_played", "cards_received"
            };

            foreach (string key in statKeys)
            {
                if (!playerStats.ContainsKey(key))
                {
                    playerStats[key] = 0;
                }
            }
        }

        public void UpdatePlayerStat(string statName, int value)
        {
            if (playerStats.ContainsKey(statName))
            {
                playerStats[statName] = value;
            }
            else
            {
                playerStats.Add(statName, value);
            }

            CheckAchievements();
            SaveAchievementData();
        }

        public void IncrementPlayerStat(string statName, int increment = 1)
        {
            if (playerStats.ContainsKey(statName))
            {
                playerStats[statName] += increment;
            }
            else
            {
                playerStats.Add(statName, increment);
            }

            CheckAchievements();
            SaveAchievementData();
        }

        private void CheckAchievements()
        {
            foreach (var achievement in achievementDatabase.achievements)
            {
                if (achievement.isUnlocked) continue;

                if (IsAchievementCompleted(achievement))
                {
                    UnlockAchievement(achievement);
                }
                else
                {
                    UpdateAchievementProgress(achievement);
                }
            }
        }

        private bool IsAchievementCompleted(AchievementData achievement)
        {
            var requirement = achievement.requirement;
            
            if (!playerStats.ContainsKey(requirement.type))
            {
                return false;
            }

            switch (requirement.type)
            {
                case "goals_scored":
                case "career_goals":
                case "clean_sheets":
                case "win_streak":
                case "skill_moves_successful":
                case "unique_tricks_mastered":
                case "stadiums_played":
                case "trophies_won":
                case "level":
                case "reputation":
                case "transfers_completed":
                    return playerStats[requirement.type] >= requirement.targetValue;
                
                case "goals_in_match":
                    return playerStats.ContainsKey("goals_current_match") && 
                           playerStats["goals_current_match"] >= requirement.targetValue;
                
                case "trophies_per_season":
                    return playerStats.ContainsKey("trophies_current_season") && 
                           playerStats["trophies_current_season"] >= requirement.targetValue;
                
                case "unbeaten_season":
                    return playerStats.ContainsKey("current_season_losses") && 
                           playerStats["current_season_losses"] == 0 &&
                           playerStats.ContainsKey("current_season_matches") &&
                           playerStats["current_season_matches"] >= 30;
                
                case "career_started":
                    return playerStats.ContainsKey("career_started") && 
                           playerStats["career_started"] == 1;
                
                default:
                    return false;
            }
        }

        private void UpdateAchievementProgress(AchievementData achievement)
        {
            var requirement = achievement.requirement;
            
            if (playerStats.ContainsKey(requirement.type))
            {
                requirement.currentValue = playerStats[requirement.type];
            }
        }

        private void UnlockAchievement(AchievementData achievement)
        {
            achievement.isUnlocked = true;
            achievement.unlockedAt = DateTime.Now;
            
            if (!unlockedAchievements.Contains(achievement.id))
            {
                unlockedAchievements.Add(achievement.id);
            }

            // Award rewards
            AwardAchievementRewards(achievement);
            
            // Trigger events
            OnAchievementUnlocked?.Invoke(achievement);
            
            Debug.Log($"Achievement Unlocked: {achievement.name}");
        }

        private void AwardAchievementRewards(AchievementData achievement)
        {
            // Award XP
            IncrementPlayerStat("experience", achievement.rewardXP);
            
            // Award coins
            IncrementPlayerStat("coins", achievement.rewardCoins);
            
            // Check for level up
            CheckLevelUp();
        }

        private void CheckLevelUp()
        {
            int currentXP = playerStats.ContainsKey("experience") ? playerStats["experience"] : 0;
            int currentLevel = playerStats.ContainsKey("level") ? playerStats["level"] : 1;
            
            int requiredXP = CalculateRequiredXP(currentLevel);
            
            if (currentXP >= requiredXP)
            {
                playerStats["level"] = currentLevel + 1;
                OnLevelUp?.Invoke(currentLevel + 1);
            }
        }

        private int CalculateRequiredXP(int level)
        {
            return level * 1000 + (level - 1) * 500;
        }

        public void ResetMatchStats()
        {
            playerStats["goals_current_match"] = 0;
            playerStats["assists_current_match"] = 0;
            playerStats["cards_current_match"] = 0;
        }

        public void ResetSeasonStats()
        {
            playerStats["trophies_current_season"] = 0;
            playerStats["current_season_losses"] = 0;
            playerStats["current_season_matches"] = 0;
        }

        public void OnMatchCompleted(int goals, int assists, bool isWin, bool isDraw, bool isCleanSheet)
        {
            // Update match stats
            playerStats["goals_current_match"] = goals;
            playerStats["assists_current_match"] = assists;
            
            // Update career stats
            IncrementPlayerStat("goals_scored", goals);
            IncrementPlayerStat("assists", assists);
            IncrementPlayerStat("matches_played", 1);
            IncrementPlayerStat("current_season_matches", 1);
            
            if (isWin)
            {
                IncrementPlayerStat("wins", 1);
                IncrementPlayerStat("current_streak", 1);
                
                // Update win streak
                int currentStreak = playerStats["current_streak"];
                if (currentStreak > playerStats["win_streak"])
                {
                    playerStats["win_streak"] = currentStreak;
                }
            }
            else
            {
                playerStats["current_streak"] = 0;
                
                if (isDraw)
                {
                    IncrementPlayerStat("draws", 1);
                }
                else
                {
                    IncrementPlayerStat("losses", 1);
                    IncrementPlayerStat("current_season_losses", 1);
                }
            }
            
            if (isCleanSheet)
            {
                IncrementPlayerStat("clean_sheets", 1);
            }
            
            CheckAchievements();
        }

        public void OnSkillMovePerformed(string skillType, bool successful)
        {
            if (successful)
            {
                IncrementPlayerStat("skill_moves_successful", 1);
                
                // Track unique tricks
                string uniqueKey = $"trick_{skillType}_mastered";
                if (!playerStats.ContainsKey(uniqueKey))
                {
                    playerStats[uniqueKey] = 1;
                    IncrementPlayerStat("unique_tricks_mastered", 1);
                }
            }
        }

        public void OnTrophyWon(string trophyType)
        {
            IncrementPlayerStat("trophies_won", 1);
            IncrementPlayerStat("trophies_current_season", 1);
        }

        public void OnCareerStarted()
        {
            UpdatePlayerStat("career_started", 1);
        }

        public void OnStadiumPlayed(string stadiumId)
        {
            string stadiumKey = $"stadium_{stadiumId}_played";
            if (!playerStats.ContainsKey(stadiumKey))
            {
                playerStats[stadiumKey] = 1;
                IncrementPlayerStat("stadiums_played", 1);
            }
        }

        public void OnTransferCompleted()
        {
            IncrementPlayerStat("transfers_completed", 1);
        }

        public void OnSeasonCompleted()
        {
            IncrementPlayerStat("seasons_played", 1);
            ResetSeasonStats();
        }

        public List<AchievementData> GetAchievementsByCategory(AchievementCategory category)
        {
            return achievementDatabase.GetAchievementsByCategory(category);
        }

        public List<AchievementData> GetUnlockedAchievements()
        {
            return achievementDatabase.GetUnlockedAchievements();
        }

        public int GetAchievementProgress()
        {
            int totalAchievements = achievementDatabase.achievements.Count;
            int unlockedCount = unlockedAchievements.Count;
            return totalAchievements > 0 ? (int)((float)unlockedCount / totalAchievements * 100) : 0;
        }

        public int GetPlayerStat(string statName)
        {
            return playerStats.ContainsKey(statName) ? playerStats[statName] : 0;
        }

        private void SaveAchievementData()
        {
            string achievementJson = JsonUtility.ToJson(new SerializableAchievementData
            {
                playerStats = playerStats,
                unlockedAchievements = unlockedAchievements
            });
            
            PlayerPrefs.SetString("achievement_data", achievementJson);
            PlayerPrefs.Save();
        }

        private void LoadAchievementData()
        {
            string achievementJson = PlayerPrefs.GetString("achievement_data", "");
            if (!string.IsNullOrEmpty(achievementJson))
            {
                var data = JsonUtility.FromJson<SerializableAchievementData>(achievementJson);
                playerStats = data.playerStats ?? new Dictionary<string, int>();
                unlockedAchievements = data.unlockedAchievements ?? new List<string>();
                
                // Update achievement database
                foreach (string achievementId in unlockedAchievements)
                {
                    var achievement = achievementDatabase.GetAchievementById(achievementId);
                    if (achievement != null)
                    {
                        achievement.isUnlocked = true;
                    }
                }
            }
        }

        // Events
        public System.Action<AchievementData> OnAchievementUnlocked;
        public System.Action<int> OnLevelUp;
        public System.Action<int> OnXPGained;
        public System.Action<int> OnCoinsGained;
    }

    [Serializable]
    public class SerializableAchievementData
    {
        public Dictionary<string, int> playerStats;
        public List<string> unlockedAchievements;
    }
}