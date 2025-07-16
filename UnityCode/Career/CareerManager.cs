using System;
using System.Collections.Generic;
using UnityEngine;
using FootballMaster.Database;

namespace FootballMaster.Career
{
    [Serializable]
    public class CareerData
    {
        public string id;
        public string userId;
        public string currentTeamId;
        public int currentSeason;
        public int reputation;
        public int budget;
        public List<ObjectiveData> objectives;
        public SeasonStats seasonStats;
        public List<TransferData> transferHistory;
        public DateTime contractEndDate;
        public DateTime createdAt;
    }

    [Serializable]
    public class ObjectiveData
    {
        public string type;
        public string description;
        public int target;
        public int reward;
        public bool completed;
        public DateTime deadline;
    }

    [Serializable]
    public class SeasonStats
    {
        public int matchesPlayed;
        public int wins;
        public int draws;
        public int losses;
        public int goalsFor;
        public int goalsAgainst;
        public int leaguePosition;
        public int points;
        public List<string> trophiesWon;
    }

    [Serializable]
    public class TransferData
    {
        public string playerId;
        public string playerName;
        public string fromTeamId;
        public string toTeamId;
        public int transferFee;
        public DateTime transferDate;
        public string transferType; // "buy", "sell", "loan"
    }

    public class CareerManager : MonoBehaviour
    {
        [Header("Career Settings")]
        public TeamDatabase teamDatabase;
        public int maxSeasonsPerCareer = 25;
        public int startingBudget = 5000000;
        public int startingReputation = 1;
        
        [Header("Current Career")]
        public CareerData currentCareer;
        public TeamData currentTeam;
        
        [Header("Season Management")]
        public int matchesPerSeason = 38;
        public List<string> availableTournaments;
        
        private static CareerManager instance;
        public static CareerManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<CareerManager>();
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
            LoadCareerData();
        }

        public void StartNewCareer(string teamId, string userId)
        {
            TeamData selectedTeam = teamDatabase.GetTeamById(teamId);
            if (selectedTeam == null)
            {
                Debug.LogError($"Team with ID {teamId} not found!");
                return;
            }

            currentCareer = new CareerData
            {
                id = Guid.NewGuid().ToString(),
                userId = userId,
                currentTeamId = teamId,
                currentSeason = 1,
                reputation = startingReputation,
                budget = startingBudget,
                objectives = GenerateSeasonObjectives(selectedTeam),
                seasonStats = new SeasonStats(),
                transferHistory = new List<TransferData>(),
                contractEndDate = DateTime.Now.AddYears(2),
                createdAt = DateTime.Now
            };

            currentTeam = selectedTeam;
            SaveCareerData();
            
            Debug.Log($"Started new career with {selectedTeam.name}");
            OnCareerStarted?.Invoke(currentCareer);
        }

        public void LoadCareerData()
        {
            string careerJson = PlayerPrefs.GetString("current_career", "");
            if (!string.IsNullOrEmpty(careerJson))
            {
                currentCareer = JsonUtility.FromJson<CareerData>(careerJson);
                currentTeam = teamDatabase.GetTeamById(currentCareer.currentTeamId);
            }
        }

        public void SaveCareerData()
        {
            if (currentCareer != null)
            {
                string careerJson = JsonUtility.ToJson(currentCareer);
                PlayerPrefs.SetString("current_career", careerJson);
                PlayerPrefs.Save();
            }
        }

        public void AdvanceSeason()
        {
            if (currentCareer == null) return;

            currentCareer.currentSeason++;
            currentCareer.seasonStats = new SeasonStats();
            currentCareer.objectives = GenerateSeasonObjectives(currentTeam);
            
            // Update team based on performance
            UpdateTeamAfterSeason();
            
            SaveCareerData();
            OnSeasonAdvanced?.Invoke(currentCareer.currentSeason);
        }

        private void UpdateTeamAfterSeason()
        {
            // Update budget based on performance
            int performanceBonus = CalculatePerformanceBonus();
            currentCareer.budget += performanceBonus;
            
            // Update reputation based on objectives completed
            int objectivesCompleted = currentCareer.objectives.FindAll(obj => obj.completed).Count;
            currentCareer.reputation += objectivesCompleted;
            currentCareer.reputation = Mathf.Clamp(currentCareer.reputation, 1, 10);
            
            // Update contract
            if (DateTime.Now > currentCareer.contractEndDate)
            {
                OnContractExpired?.Invoke();
            }
        }

        private int CalculatePerformanceBonus()
        {
            int bonus = 0;
            
            // League position bonus
            if (currentCareer.seasonStats.leaguePosition <= 4)
            {
                bonus += 2000000; // Champions League bonus
            }
            else if (currentCareer.seasonStats.leaguePosition <= 6)
            {
                bonus += 1000000; // Europa League bonus
            }
            
            // Win ratio bonus
            float winRatio = (float)currentCareer.seasonStats.wins / currentCareer.seasonStats.matchesPlayed;
            bonus += (int)(winRatio * 1000000);
            
            // Trophies bonus
            bonus += currentCareer.seasonStats.trophiesWon.Count * 500000;
            
            return bonus;
        }

        private List<ObjectiveData> GenerateSeasonObjectives(TeamData team)
        {
            List<ObjectiveData> objectives = new List<ObjectiveData>();
            
            // League position objective
            int targetPosition = GetTargetLeaguePosition(team);
            objectives.Add(new ObjectiveData
            {
                type = "league_position",
                description = $"Finish in top {targetPosition} of the league",
                target = targetPosition,
                reward = 1000000,
                completed = false,
                deadline = DateTime.Now.AddMonths(9)
            });
            
            // Cup objective
            objectives.Add(new ObjectiveData
            {
                type = "cup_progress",
                description = "Reach quarter-finals in domestic cup",
                target = 8, // Quarter-finals
                reward = 500000,
                completed = false,
                deadline = DateTime.Now.AddMonths(6)
            });
            
            // Youth development objective
            objectives.Add(new ObjectiveData
            {
                type = "youth_development",
                description = "Promote 2 youth players to first team",
                target = 2,
                reward = 300000,
                completed = false,
                deadline = DateTime.Now.AddMonths(9)
            });
            
            // Financial objective
            objectives.Add(new ObjectiveData
            {
                type = "financial",
                description = "Maintain positive transfer balance",
                target = 0,
                reward = 200000,
                completed = false,
                deadline = DateTime.Now.AddMonths(9)
            });
            
            return objectives;
        }

        private int GetTargetLeaguePosition(TeamData team)
        {
            if (team.prestige >= 9) return 4;
            if (team.prestige >= 7) return 6;
            if (team.prestige >= 5) return 10;
            return 15;
        }

        public void CompleteObjective(string objectiveType)
        {
            if (currentCareer == null) return;
            
            ObjectiveData objective = currentCareer.objectives.Find(obj => obj.type == objectiveType);
            if (objective != null && !objective.completed)
            {
                objective.completed = true;
                currentCareer.budget += objective.reward;
                SaveCareerData();
                OnObjectiveCompleted?.Invoke(objective);
            }
        }

        public void UpdateSeasonStats(int wins, int draws, int losses, int goalsFor, int goalsAgainst)
        {
            if (currentCareer == null) return;
            
            currentCareer.seasonStats.wins = wins;
            currentCareer.seasonStats.draws = draws;
            currentCareer.seasonStats.losses = losses;
            currentCareer.seasonStats.goalsFor = goalsFor;
            currentCareer.seasonStats.goalsAgainst = goalsAgainst;
            currentCareer.seasonStats.matchesPlayed = wins + draws + losses;
            currentCareer.seasonStats.points = (wins * 3) + draws;
            
            SaveCareerData();
        }

        public void AddTransfer(TransferData transfer)
        {
            if (currentCareer == null) return;
            
            currentCareer.transferHistory.Add(transfer);
            
            // Update budget
            if (transfer.transferType == "buy")
            {
                currentCareer.budget -= transfer.transferFee;
            }
            else if (transfer.transferType == "sell")
            {
                currentCareer.budget += transfer.transferFee;
            }
            
            SaveCareerData();
            OnTransferCompleted?.Invoke(transfer);
        }

        public void AddTrophy(string trophyName)
        {
            if (currentCareer == null) return;
            
            currentCareer.seasonStats.trophiesWon.Add(trophyName);
            SaveCareerData();
            OnTrophyWon?.Invoke(trophyName);
        }

        public bool CanAffordPlayer(int transferFee)
        {
            return currentCareer != null && currentCareer.budget >= transferFee;
        }

        public void ChangeTeam(string newTeamId)
        {
            if (currentCareer == null) return;
            
            TeamData newTeam = teamDatabase.GetTeamById(newTeamId);
            if (newTeam == null) return;
            
            currentCareer.currentTeamId = newTeamId;
            currentTeam = newTeam;
            currentCareer.contractEndDate = DateTime.Now.AddYears(2);
            
            SaveCareerData();
            OnTeamChanged?.Invoke(newTeam);
        }

        public float GetCareerProgress()
        {
            if (currentCareer == null) return 0f;
            return (float)currentCareer.currentSeason / maxSeasonsPerCareer;
        }

        public int GetTotalTrophies()
        {
            if (currentCareer == null) return 0;
            
            int totalTrophies = 0;
            // This would need to be tracked across seasons
            return totalTrophies;
        }

        // Events
        public System.Action<CareerData> OnCareerStarted;
        public System.Action<int> OnSeasonAdvanced;
        public System.Action<ObjectiveData> OnObjectiveCompleted;
        public System.Action<TransferData> OnTransferCompleted;
        public System.Action<string> OnTrophyWon;
        public System.Action<TeamData> OnTeamChanged;
        public System.Action OnContractExpired;
    }
}