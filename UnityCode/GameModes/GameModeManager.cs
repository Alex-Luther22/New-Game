using System;
using System.Collections.Generic;
using UnityEngine;
using FootballMaster.Database;

namespace FootballMaster.GameModes
{
    [Serializable]
    public class GameModeData
    {
        public string id;
        public string name;
        public string description;
        public string icon;
        public int maxPlayers;
        public int duration;
        public bool isUnlocked;
        public Dictionary<string, object> settings;
    }

    [Serializable]
    public class MatchData
    {
        public string id;
        public string homeTeamId;
        public string awayTeamId;
        public int homeScore;
        public int awayScore;
        public string stadiumId;
        public string gameMode;
        public int duration;
        public int difficulty;
        public string weather;
        public string timeOfDay;
        public bool completed;
        public string playerId;
        public List<MatchEvent> matchEvents;
        public MatchStatistics statistics;
        public DateTime createdAt;
    }

    [Serializable]
    public class MatchEvent
    {
        public int minute;
        public string type;
        public string playerId;
        public string teamId;
        public string description;
        public Vector3 position;
    }

    [Serializable]
    public class MatchStatistics
    {
        public int possession;
        public int shots;
        public int shotsOnTarget;
        public int corners;
        public int fouls;
        public int yellowCards;
        public int redCards;
        public int passes;
        public int passAccuracy;
        public int tackles;
        public int interceptions;
    }

    [Serializable]
    public class TournamentData
    {
        public string id;
        public string name;
        public string type;
        public List<string> participatingTeams;
        public int currentRound;
        public int totalRounds;
        public List<string> matches;
        public string winnerId;
        public int prizeMoney;
        public string status;
        public DateTime createdAt;
    }

    public class GameModeManager : MonoBehaviour
    {
        [Header("Game Mode Configuration")]
        public TeamDatabase teamDatabase;
        public List<GameModeData> availableGameModes;
        
        [Header("Current Match")]
        public MatchData currentMatch;
        public TeamData homeTeam;
        public TeamData awayTeam;
        
        [Header("Tournament System")]
        public List<TournamentData> activeTournaments;
        
        private static GameModeManager instance;
        public static GameModeManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<GameModeManager>();
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
                InitializeGameModes();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeGameModes()
        {
            availableGameModes = new List<GameModeData>
            {
                new GameModeData
                {
                    id = "quick_match",
                    name = "Quick Match",
                    description = "Jump into a quick match with any team",
                    icon = "⚡",
                    maxPlayers = 2,
                    duration = 90,
                    isUnlocked = true,
                    settings = new Dictionary<string, object>
                    {
                        { "allowCustomTeams", true },
                        { "difficultyOptions", new List<string> { "Easy", "Normal", "Hard" } },
                        { "weatherOptions", new List<string> { "Sunny", "Rainy", "Cloudy" } }
                    }
                },
                new GameModeData
                {
                    id = "career",
                    name = "Career Mode",
                    description = "Build your legacy as a manager",
                    icon = "👔",
                    maxPlayers = 1,
                    duration = 0,
                    isUnlocked = true,
                    settings = new Dictionary<string, object>
                    {
                        { "maxSeasons", 25 },
                        { "transferMarket", true },
                        { "youthAcademy", true }
                    }
                },
                new GameModeData
                {
                    id = "tournament",
                    name = "Tournament",
                    description = "Compete in various tournaments",
                    icon = "🏆",
                    maxPlayers = 32,
                    duration = 0,
                    isUnlocked = true,
                    settings = new Dictionary<string, object>
                    {
                        { "tournamentTypes", new List<string> { "Cup", "League", "Champions" } },
                        { "groupStage", true },
                        { "knockoutRounds", true }
                    }
                },
                new GameModeData
                {
                    id = "futsal",
                    name = "Futsal",
                    description = "Fast-paced 5v5 indoor football",
                    icon = "🏟️",
                    maxPlayers = 2,
                    duration = 40,
                    isUnlocked = true,
                    settings = new Dictionary<string, object>
                    {
                        { "teamSize", 5 },
                        { "fieldSize", "small" },
                        { "ballType", "futsal" }
                    }
                },
                new GameModeData
                {
                    id = "online",
                    name = "Online Match",
                    description = "Play against other players online",
                    icon = "🌐",
                    maxPlayers = 2,
                    duration = 90,
                    isUnlocked = false,
                    settings = new Dictionary<string, object>
                    {
                        { "rankingSystem", true },
                        { "matchmaking", true },
                        { "spectatorMode", true }
                    }
                }
            };
        }

        public void StartQuickMatch(string homeTeamId, string awayTeamId, int difficulty = 3)
        {
            homeTeam = teamDatabase.GetTeamById(homeTeamId);
            awayTeam = teamDatabase.GetTeamById(awayTeamId);
            
            if (homeTeam == null || awayTeam == null)
            {
                Debug.LogError("Teams not found for quick match!");
                return;
            }

            currentMatch = new MatchData
            {
                id = Guid.NewGuid().ToString(),
                homeTeamId = homeTeamId,
                awayTeamId = awayTeamId,
                homeScore = 0,
                awayScore = 0,
                stadiumId = GetRandomStadium(),
                gameMode = "quick_match",
                duration = 90,
                difficulty = difficulty,
                weather = GetRandomWeather(),
                timeOfDay = GetRandomTimeOfDay(),
                completed = false,
                playerId = GetCurrentPlayerId(),
                matchEvents = new List<MatchEvent>(),
                statistics = new MatchStatistics(),
                createdAt = DateTime.Now
            };

            OnMatchStarted?.Invoke(currentMatch);
            Debug.Log($"Quick match started: {homeTeam.name} vs {awayTeam.name}");
        }

        public void StartCareerMatch(string opponentTeamId)
        {
            var careerManager = CareerManager.Instance;
            if (careerManager == null || careerManager.currentTeam == null)
            {
                Debug.LogError("Career mode not initialized!");
                return;
            }

            homeTeam = careerManager.currentTeam;
            awayTeam = teamDatabase.GetTeamById(opponentTeamId);
            
            if (awayTeam == null)
            {
                Debug.LogError("Opponent team not found!");
                return;
            }

            currentMatch = new MatchData
            {
                id = Guid.NewGuid().ToString(),
                homeTeamId = homeTeam.id,
                awayTeamId = awayTeam.id,
                homeScore = 0,
                awayScore = 0,
                stadiumId = homeTeam.stadiumName,
                gameMode = "career",
                duration = 90,
                difficulty = CalculateCareerDifficulty(awayTeam),
                weather = GetRandomWeather(),
                timeOfDay = GetRandomTimeOfDay(),
                completed = false,
                playerId = GetCurrentPlayerId(),
                matchEvents = new List<MatchEvent>(),
                statistics = new MatchStatistics(),
                createdAt = DateTime.Now
            };

            OnMatchStarted?.Invoke(currentMatch);
        }

        public void StartFutsalMatch(string homeTeamId, string awayTeamId)
        {
            homeTeam = teamDatabase.GetTeamById(homeTeamId);
            awayTeam = teamDatabase.GetTeamById(awayTeamId);
            
            if (homeTeam == null || awayTeam == null)
            {
                Debug.LogError("Teams not found for futsal match!");
                return;
            }

            currentMatch = new MatchData
            {
                id = Guid.NewGuid().ToString(),
                homeTeamId = homeTeamId,
                awayTeamId = awayTeamId,
                homeScore = 0,
                awayScore = 0,
                stadiumId = "futsal_arena",
                gameMode = "futsal",
                duration = 40,
                difficulty = 3,
                weather = "indoor",
                timeOfDay = "artificial",
                completed = false,
                playerId = GetCurrentPlayerId(),
                matchEvents = new List<MatchEvent>(),
                statistics = new MatchStatistics(),
                createdAt = DateTime.Now
            };

            OnMatchStarted?.Invoke(currentMatch);
        }

        public TournamentData CreateTournament(string tournamentName, string tournamentType, List<string> teamIds)
        {
            var tournament = new TournamentData
            {
                id = Guid.NewGuid().ToString(),
                name = tournamentName,
                type = tournamentType,
                participatingTeams = teamIds,
                currentRound = 1,
                totalRounds = CalculateTotalRounds(teamIds.Count),
                matches = new List<string>(),
                winnerId = "",
                prizeMoney = CalculatePrizeMoney(tournamentType),
                status = "upcoming",
                createdAt = DateTime.Now
            };

            activeTournaments.Add(tournament);
            GenerateTournamentMatches(tournament);
            
            OnTournamentCreated?.Invoke(tournament);
            return tournament;
        }

        private void GenerateTournamentMatches(TournamentData tournament)
        {
            if (tournament.type == "cup")
            {
                GenerateCupMatches(tournament);
            }
            else if (tournament.type == "league")
            {
                GenerateLeagueMatches(tournament);
            }
        }

        private void GenerateCupMatches(TournamentData tournament)
        {
            // Generate knockout tournament matches
            var teams = new List<string>(tournament.participatingTeams);
            
            while (teams.Count > 1)
            {
                var roundMatches = new List<string>();
                
                for (int i = 0; i < teams.Count; i += 2)
                {
                    if (i + 1 < teams.Count)
                    {
                        var match = new MatchData
                        {
                            id = Guid.NewGuid().ToString(),
                            homeTeamId = teams[i],
                            awayTeamId = teams[i + 1],
                            homeScore = 0,
                            awayScore = 0,
                            stadiumId = GetRandomStadium(),
                            gameMode = "tournament",
                            duration = 90,
                            difficulty = 3,
                            weather = GetRandomWeather(),
                            timeOfDay = GetRandomTimeOfDay(),
                            completed = false,
                            playerId = GetCurrentPlayerId(),
                            matchEvents = new List<MatchEvent>(),
                            statistics = new MatchStatistics(),
                            createdAt = DateTime.Now
                        };
                        
                        roundMatches.Add(match.id);
                        tournament.matches.Add(match.id);
                    }
                }
                
                // Simulate match results and advance winners
                teams = SimulateRoundResults(teams);
            }
        }

        private void GenerateLeagueMatches(TournamentData tournament)
        {
            // Generate round-robin tournament matches
            var teams = tournament.participatingTeams;
            
            for (int i = 0; i < teams.Count; i++)
            {
                for (int j = i + 1; j < teams.Count; j++)
                {
                    // Home match
                    var homeMatch = new MatchData
                    {
                        id = Guid.NewGuid().ToString(),
                        homeTeamId = teams[i],
                        awayTeamId = teams[j],
                        homeScore = 0,
                        awayScore = 0,
                        stadiumId = GetRandomStadium(),
                        gameMode = "tournament",
                        duration = 90,
                        difficulty = 3,
                        weather = GetRandomWeather(),
                        timeOfDay = GetRandomTimeOfDay(),
                        completed = false,
                        playerId = GetCurrentPlayerId(),
                        matchEvents = new List<MatchEvent>(),
                        statistics = new MatchStatistics(),
                        createdAt = DateTime.Now
                    };
                    
                    // Away match
                    var awayMatch = new MatchData
                    {
                        id = Guid.NewGuid().ToString(),
                        homeTeamId = teams[j],
                        awayTeamId = teams[i],
                        homeScore = 0,
                        awayScore = 0,
                        stadiumId = GetRandomStadium(),
                        gameMode = "tournament",
                        duration = 90,
                        difficulty = 3,
                        weather = GetRandomWeather(),
                        timeOfDay = GetRandomTimeOfDay(),
                        completed = false,
                        playerId = GetCurrentPlayerId(),
                        matchEvents = new List<MatchEvent>(),
                        statistics = new MatchStatistics(),
                        createdAt = DateTime.Now
                    };
                    
                    tournament.matches.Add(homeMatch.id);
                    tournament.matches.Add(awayMatch.id);
                }
            }
        }

        public void CompleteMatch(int homeGoals, int awayGoals)
        {
            if (currentMatch == null) return;
            
            currentMatch.homeScore = homeGoals;
            currentMatch.awayScore = awayGoals;
            currentMatch.completed = true;
            
            // Update career stats if in career mode
            if (currentMatch.gameMode == "career")
            {
                var careerManager = CareerManager.Instance;
                if (careerManager != null)
                {
                    bool isWin = homeGoals > awayGoals;
                    bool isDraw = homeGoals == awayGoals;
                    
                    // Update season stats
                    var stats = careerManager.currentCareer.seasonStats;
                    if (isWin) stats.wins++;
                    else if (isDraw) stats.draws++;
                    else stats.losses++;
                    
                    stats.goalsFor += homeGoals;
                    stats.goalsAgainst += awayGoals;
                    stats.matchesPlayed++;
                    
                    careerManager.SaveCareerData();
                }
            }
            
            OnMatchCompleted?.Invoke(currentMatch);
        }

        public void AddMatchEvent(int minute, string eventType, string playerId, string teamId, string description, Vector3 position)
        {
            if (currentMatch == null) return;
            
            var matchEvent = new MatchEvent
            {
                minute = minute,
                type = eventType,
                playerId = playerId,
                teamId = teamId,
                description = description,
                position = position
            };
            
            currentMatch.matchEvents.Add(matchEvent);
            OnMatchEvent?.Invoke(matchEvent);
        }

        public void UpdateMatchStatistics(MatchStatistics stats)
        {
            if (currentMatch == null) return;
            
            currentMatch.statistics = stats;
        }

        private int CalculateCareerDifficulty(TeamData opponentTeam)
        {
            var careerManager = CareerManager.Instance;
            if (careerManager == null) return 3;
            
            int myRating = careerManager.currentTeam.overallRating;
            int opponentRating = opponentTeam.overallRating;
            
            int ratingDiff = opponentRating - myRating;
            
            if (ratingDiff > 10) return 5;
            if (ratingDiff > 5) return 4;
            if (ratingDiff > -5) return 3;
            if (ratingDiff > -10) return 2;
            return 1;
        }

        private int CalculateTotalRounds(int teamCount)
        {
            return Mathf.CeilToInt(Mathf.Log(teamCount, 2));
        }

        private int CalculatePrizeMoney(string tournamentType)
        {
            switch (tournamentType)
            {
                case "cup": return 1000000;
                case "league": return 2000000;
                case "champions": return 5000000;
                default: return 500000;
            }
        }

        private List<string> SimulateRoundResults(List<string> teams)
        {
            List<string> winners = new List<string>();
            
            for (int i = 0; i < teams.Count; i += 2)
            {
                if (i + 1 < teams.Count)
                {
                    // Simple random winner selection
                    winners.Add(UnityEngine.Random.Range(0, 2) == 0 ? teams[i] : teams[i + 1]);
                }
                else
                {
                    winners.Add(teams[i]); // Bye
                }
            }
            
            return winners;
        }

        private string GetRandomStadium()
        {
            var stadiums = teamDatabase.stadiums;
            return stadiums[UnityEngine.Random.Range(0, stadiums.Count)].id;
        }

        private string GetRandomWeather()
        {
            string[] weatherOptions = { "sunny", "cloudy", "rainy", "windy" };
            return weatherOptions[UnityEngine.Random.Range(0, weatherOptions.Length)];
        }

        private string GetRandomTimeOfDay()
        {
            string[] timeOptions = { "day", "night", "afternoon", "evening" };
            return timeOptions[UnityEngine.Random.Range(0, timeOptions.Length)];
        }

        private string GetCurrentPlayerId()
        {
            return PlayerPrefs.GetString("current_player_id", "default_player");
        }

        public GameModeData GetGameMode(string gameModeId)
        {
            return availableGameModes.Find(mode => mode.id == gameModeId);
        }

        public void UnlockGameMode(string gameModeId)
        {
            var gameMode = GetGameMode(gameModeId);
            if (gameMode != null)
            {
                gameMode.isUnlocked = true;
                OnGameModeUnlocked?.Invoke(gameMode);
            }
        }

        // Events
        public System.Action<MatchData> OnMatchStarted;
        public System.Action<MatchData> OnMatchCompleted;
        public System.Action<MatchEvent> OnMatchEvent;
        public System.Action<TournamentData> OnTournamentCreated;
        public System.Action<GameModeData> OnGameModeUnlocked;
    }
}