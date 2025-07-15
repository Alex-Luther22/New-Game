using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SQLite4Unity3d;

public class SaveSystem : MonoBehaviour
{
    private static SaveSystem instance;
    public static SaveSystem Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SaveSystem>();
                if (instance == null)
                {
                    GameObject go = new GameObject("SaveSystem");
                    instance = go.AddComponent<SaveSystem>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }

    private SQLiteConnection connection;
    private string dbPath;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDatabase();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void InitializeDatabase()
    {
        dbPath = Path.Combine(Application.persistentDataPath, "FootballMaster.db");
        connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        
        CreateTables();
        PopulateDefaultData();
    }

    void CreateTables()
    {
        // User profile table
        connection.CreateTable<UserProfile>();
        
        // Teams table
        connection.CreateTable<TeamSaveData>();
        
        // Players table
        connection.CreateTable<PlayerSaveData>();
        
        // Career mode table
        connection.CreateTable<CareerSaveData>();
        
        // Match results table
        connection.CreateTable<MatchResult>();
        
        // Tournament data table
        connection.CreateTable<TournamentSaveData>();
        
        // User settings table
        connection.CreateTable<UserSettings>();
        
        // Achievements table
        connection.CreateTable<Achievement>();
        
        // Transfer history table
        connection.CreateTable<TransferRecord>();
        
        // Training data table
        connection.CreateTable<TrainingData>();
    }

    void PopulateDefaultData()
    {
        // Check if default data already exists
        if (connection.Table<TeamSaveData>().Count() == 0)
        {
            CreateDefaultTeams();
            CreateDefaultPlayers();
            CreateDefaultTournaments();
            CreateDefaultAchievements();
        }
    }

    void CreateDefaultTeams()
    {
        List<TeamSaveData> defaultTeams = new List<TeamSaveData>
        {
            // Premier League
            new TeamSaveData
            {
                TeamId = 1,
                TeamName = "Red Manchester",
                ShortName = "RM",
                Country = "England",
                City = "Manchester",
                League = "Premier League",
                Overall = 89,
                Attack = 92,
                Midfield = 87,
                Defense = 86,
                Budget = 200000000,
                StadiumName = "Old Theater",
                StadiumCapacity = 76000,
                HomeKitPrimary = "#FF0000",
                HomeKitSecondary = "#FFFFFF",
                AwayKitPrimary = "#FFFFFF",
                AwayKitSecondary = "#FF0000",
                Founded = 1878,
                Manager = "Erik ten Hag",
                Trophies = 68
            },
            new TeamSaveData
            {
                TeamId = 2,
                TeamName = "Blue Manchester",
                ShortName = "BM",
                Country = "England",
                City = "Manchester",
                League = "Premier League",
                Overall = 91,
                Attack = 89,
                Midfield = 93,
                Defense = 88,
                Budget = 300000000,
                StadiumName = "Etihad Stadium",
                StadiumCapacity = 55000,
                HomeKitPrimary = "#00BFFF",
                HomeKitSecondary = "#FFFFFF",
                AwayKitPrimary = "#FFFFFF",
                AwayKitSecondary = "#00BFFF",
                Founded = 1880,
                Manager = "Pep Guardiola",
                Trophies = 34
            },
            new TeamSaveData
            {
                TeamId = 3,
                TeamName = "Arsenal London",
                ShortName = "ARS",
                Country = "England",
                City = "London",
                League = "Premier League",
                Overall = 87,
                Attack = 88,
                Midfield = 86,
                Defense = 85,
                Budget = 150000000,
                StadiumName = "Emirates Stadium",
                StadiumCapacity = 60000,
                HomeKitPrimary = "#FF0000",
                HomeKitSecondary = "#FFFFFF",
                AwayKitPrimary = "#FFFFFF",
                AwayKitSecondary = "#FF0000",
                Founded = 1886,
                Manager = "Mikel Arteta",
                Trophies = 47
            },
            new TeamSaveData
            {
                TeamId = 4,
                TeamName = "Chelsea London",
                ShortName = "CHE",
                Country = "England",
                City = "London",
                League = "Premier League",
                Overall = 86,
                Attack = 85,
                Midfield = 87,
                Defense = 84,
                Budget = 180000000,
                StadiumName = "Stamford Bridge",
                StadiumCapacity = 42000,
                HomeKitPrimary = "#0000FF",
                HomeKitSecondary = "#FFFFFF",
                AwayKitPrimary = "#FFFFFF",
                AwayKitSecondary = "#0000FF",
                Founded = 1905,
                Manager = "Frank Lampard",
                Trophies = 34
            },
            new TeamSaveData
            {
                TeamId = 5,
                TeamName = "Liverpool Red",
                ShortName = "LIV",
                Country = "England",
                City = "Liverpool",
                League = "Premier League",
                Overall = 88,
                Attack = 90,
                Midfield = 86,
                Defense = 87,
                Budget = 170000000,
                StadiumName = "Anfield",
                StadiumCapacity = 54000,
                HomeKitPrimary = "#FF0000",
                HomeKitSecondary = "#FFFFFF",
                AwayKitPrimary = "#FFFFFF",
                AwayKitSecondary = "#FF0000",
                Founded = 1892,
                Manager = "Jurgen Klopp",
                Trophies = 66
            },
            new TeamSaveData
            {
                TeamId = 6,
                TeamName = "Tottenham London",
                ShortName = "TOT",
                Country = "England",
                City = "London",
                League = "Premier League",
                Overall = 84,
                Attack = 86,
                Midfield = 82,
                Defense = 83,
                Budget = 130000000,
                StadiumName = "Tottenham Stadium",
                StadiumCapacity = 62000,
                HomeKitPrimary = "#FFFFFF",
                HomeKitSecondary = "#000080",
                AwayKitPrimary = "#000080",
                AwayKitSecondary = "#FFFFFF",
                Founded = 1882,
                Manager = "Antonio Conte",
                Trophies = 26
            },
            
            // La Liga
            new TeamSaveData
            {
                TeamId = 7,
                TeamName = "Real Madrid",
                ShortName = "RM",
                Country = "Spain",
                City = "Madrid",
                League = "La Liga",
                Overall = 92,
                Attack = 93,
                Midfield = 91,
                Defense = 89,
                Budget = 350000000,
                StadiumName = "Santiago Bernabeu",
                StadiumCapacity = 85000,
                HomeKitPrimary = "#FFFFFF",
                HomeKitSecondary = "#FFD700",
                AwayKitPrimary = "#000000",
                AwayKitSecondary = "#FFFFFF",
                Founded = 1902,
                Manager = "Carlo Ancelotti",
                Trophies = 97
            },
            new TeamSaveData
            {
                TeamId = 8,
                TeamName = "Barcelona FC",
                ShortName = "BAR",
                Country = "Spain",
                City = "Barcelona",
                League = "La Liga",
                Overall = 90,
                Attack = 91,
                Midfield = 92,
                Defense = 87,
                Budget = 280000000,
                StadiumName = "Camp Nou",
                StadiumCapacity = 99000,
                HomeKitPrimary = "#FF0000",
                HomeKitSecondary = "#0000FF",
                AwayKitPrimary = "#FFD700",
                AwayKitSecondary = "#FF0000",
                Founded = 1899,
                Manager = "Ronald Koeman",
                Trophies = 95
            },
            new TeamSaveData
            {
                TeamId = 9,
                TeamName = "Atletico Madrid",
                ShortName = "ATM",
                Country = "Spain",
                City = "Madrid",
                League = "La Liga",
                Overall = 88,
                Attack = 86,
                Midfield = 88,
                Defense = 91,
                Budget = 160000000,
                StadiumName = "Wanda Metropolitano",
                StadiumCapacity = 70000,
                HomeKitPrimary = "#FF0000",
                HomeKitSecondary = "#FFFFFF",
                AwayKitPrimary = "#000080",
                AwayKitSecondary = "#FFFFFF",
                Founded = 1903,
                Manager = "Diego Simeone",
                Trophies = 31
            },
            
            // Serie A
            new TeamSaveData
            {
                TeamId = 10,
                TeamName = "Juventus Turin",
                ShortName = "JUV",
                Country = "Italy",
                City = "Turin",
                League = "Serie A",
                Overall = 87,
                Attack = 85,
                Midfield = 88,
                Defense = 89,
                Budget = 200000000,
                StadiumName = "Allianz Stadium",
                StadiumCapacity = 41000,
                HomeKitPrimary = "#FFFFFF",
                HomeKitSecondary = "#000000",
                AwayKitPrimary = "#000000",
                AwayKitSecondary = "#FFFFFF",
                Founded = 1897,
                Manager = "Massimiliano Allegri",
                Trophies = 70
            },
            new TeamSaveData
            {
                TeamId = 11,
                TeamName = "AC Milan",
                ShortName = "MIL",
                Country = "Italy",
                City = "Milan",
                League = "Serie A",
                Overall = 86,
                Attack = 87,
                Midfield = 85,
                Defense = 86,
                Budget = 180000000,
                StadiumName = "San Siro",
                StadiumCapacity = 80000,
                HomeKitPrimary = "#FF0000",
                HomeKitSecondary = "#000000",
                AwayKitPrimary = "#FFFFFF",
                AwayKitSecondary = "#FF0000",
                Founded = 1899,
                Manager = "Stefano Pioli",
                Trophies = 49
            },
            new TeamSaveData
            {
                TeamId = 12,
                TeamName = "Inter Milan",
                ShortName = "INT",
                Country = "Italy",
                City = "Milan",
                League = "Serie A",
                Overall = 88,
                Attack = 89,
                Midfield = 87,
                Defense = 88,
                Budget = 190000000,
                StadiumName = "San Siro",
                StadiumCapacity = 80000,
                HomeKitPrimary = "#0000FF",
                HomeKitSecondary = "#000000",
                AwayKitPrimary = "#FFFFFF",
                AwayKitSecondary = "#0000FF",
                Founded = 1908,
                Manager = "Simone Inzaghi",
                Trophies = 40
            },
            
            // Bundesliga
            new TeamSaveData
            {
                TeamId = 13,
                TeamName = "Bayern Munich",
                ShortName = "BAY",
                Country = "Germany",
                City = "Munich",
                League = "Bundesliga",
                Overall = 91,
                Attack = 92,
                Midfield = 90,
                Defense = 89,
                Budget = 320000000,
                StadiumName = "Allianz Arena",
                StadiumCapacity = 75000,
                HomeKitPrimary = "#FF0000",
                HomeKitSecondary = "#FFFFFF",
                AwayKitPrimary = "#FFFFFF",
                AwayKitSecondary = "#FF0000",
                Founded = 1900,
                Manager = "Julian Nagelsmann",
                Trophies = 82
            },
            new TeamSaveData
            {
                TeamId = 14,
                TeamName = "Borussia Dortmund",
                ShortName = "BVB",
                Country = "Germany",
                City = "Dortmund",
                League = "Bundesliga",
                Overall = 86,
                Attack = 88,
                Midfield = 84,
                Defense = 85,
                Budget = 140000000,
                StadiumName = "Signal Iduna Park",
                StadiumCapacity = 81000,
                HomeKitPrimary = "#FFD700",
                HomeKitSecondary = "#000000",
                AwayKitPrimary = "#000000",
                AwayKitSecondary = "#FFD700",
                Founded = 1909,
                Manager = "Edin Terzic",
                Trophies = 17
            },
            
            // Ligue 1
            new TeamSaveData
            {
                TeamId = 15,
                TeamName = "Paris Saint-Germain",
                ShortName = "PSG",
                Country = "France",
                City = "Paris",
                League = "Ligue 1",
                Overall = 90,
                Attack = 94,
                Midfield = 88,
                Defense = 87,
                Budget = 400000000,
                StadiumName = "Parc des Princes",
                StadiumCapacity = 48000,
                HomeKitPrimary = "#000080",
                HomeKitSecondary = "#FF0000",
                AwayKitPrimary = "#FFFFFF",
                AwayKitSecondary = "#000080",
                Founded = 1970,
                Manager = "Christophe Galtier",
                Trophies = 47
            }
        };

        connection.InsertAll(defaultTeams);
    }

    void CreateDefaultPlayers()
    {
        List<PlayerSaveData> defaultPlayers = new List<PlayerSaveData>
        {
            // Real Madrid Players
            new PlayerSaveData
            {
                PlayerId = 1,
                PlayerName = "Karim Benzema",
                Age = 36,
                Position = "ST",
                TeamId = 7,
                Overall = 91,
                Pace = 77,
                Shooting = 95,
                Passing = 83,
                Dribbling = 88,
                Defense = 39,
                Physical = 78,
                Value = 25000000,
                Wage = 350000,
                ShirtNumber = 9,
                Nationality = "France",
                PreferredFoot = "Right"
            },
            new PlayerSaveData
            {
                PlayerId = 2,
                PlayerName = "Vinicius Jr",
                Age = 23,
                Position = "LW",
                TeamId = 7,
                Overall = 86,
                Pace = 95,
                Shooting = 83,
                Passing = 78,
                Dribbling = 92,
                Defense = 31,
                Physical = 68,
                Value = 120000000,
                Wage = 200000,
                ShirtNumber = 7,
                Nationality = "Brazil",
                PreferredFoot = "Right"
            },
            new PlayerSaveData
            {
                PlayerId = 3,
                PlayerName = "Luka Modric",
                Age = 38,
                Position = "CM",
                TeamId = 7,
                Overall = 88,
                Pace = 74,
                Shooting = 76,
                Passing = 95,
                Dribbling = 90,
                Defense = 72,
                Physical = 65,
                Value = 10000000,
                Wage = 300000,
                ShirtNumber = 10,
                Nationality = "Croatia",
                PreferredFoot = "Right"
            },
            new PlayerSaveData
            {
                PlayerId = 4,
                PlayerName = "Thibaut Courtois",
                Age = 31,
                Position = "GK",
                TeamId = 7,
                Overall = 89,
                Pace = 43,
                Shooting = 17,
                Passing = 53,
                Dribbling = 31,
                Defense = 18,
                Physical = 90,
                Value = 60000000,
                Wage = 280000,
                ShirtNumber = 1,
                Nationality = "Belgium",
                PreferredFoot = "Left"
            },
            
            // Barcelona Players
            new PlayerSaveData
            {
                PlayerId = 5,
                PlayerName = "Robert Lewandowski",
                Age = 35,
                Position = "ST",
                TeamId = 8,
                Overall = 91,
                Pace = 78,
                Shooting = 94,
                Passing = 79,
                Dribbling = 86,
                Defense = 43,
                Physical = 85,
                Value = 45000000,
                Wage = 400000,
                ShirtNumber = 9,
                Nationality = "Poland",
                PreferredFoot = "Right"
            },
            new PlayerSaveData
            {
                PlayerId = 6,
                PlayerName = "Pedri",
                Age = 21,
                Position = "CM",
                TeamId = 8,
                Overall = 85,
                Pace = 79,
                Shooting = 70,
                Passing = 91,
                Dribbling = 87,
                Defense = 59,
                Physical = 66,
                Value = 90000000,
                Wage = 150000,
                ShirtNumber = 8,
                Nationality = "Spain",
                PreferredFoot = "Right"
            },
            new PlayerSaveData
            {
                PlayerId = 7,
                PlayerName = "Gavi",
                Age = 19,
                Position = "CM",
                TeamId = 8,
                Overall = 82,
                Pace = 82,
                Shooting = 68,
                Passing = 86,
                Dribbling = 85,
                Defense = 64,
                Physical = 70,
                Value = 80000000,
                Wage = 100000,
                ShirtNumber = 6,
                Nationality = "Spain",
                PreferredFoot = "Right"
            },
            
            // Manchester City Players
            new PlayerSaveData
            {
                PlayerId = 8,
                PlayerName = "Erling Haaland",
                Age = 23,
                Position = "ST",
                TeamId = 2,
                Overall = 91,
                Pace = 89,
                Shooting = 94,
                Passing = 65,
                Dribbling = 80,
                Defense = 45,
                Physical = 88,
                Value = 180000000,
                Wage = 375000,
                ShirtNumber = 9,
                Nationality = "Norway",
                PreferredFoot = "Left"
            },
            new PlayerSaveData
            {
                PlayerId = 9,
                PlayerName = "Kevin De Bruyne",
                Age = 32,
                Position = "CAM",
                TeamId = 2,
                Overall = 91,
                Pace = 76,
                Shooting = 86,
                Passing = 93,
                Dribbling = 88,
                Defense = 64,
                Physical = 78,
                Value = 85000000,
                Wage = 320000,
                ShirtNumber = 17,
                Nationality = "Belgium",
                PreferredFoot = "Right"
            },
            new PlayerSaveData
            {
                PlayerId = 10,
                PlayerName = "Pep Guardiola",
                Age = 28,
                Position = "CDM",
                TeamId = 2,
                Overall = 89,
                Pace = 65,
                Shooting = 72,
                Passing = 91,
                Dribbling = 83,
                Defense = 84,
                Physical = 82,
                Value = 70000000,
                Wage = 220000,
                ShirtNumber = 16,
                Nationality = "Spain",
                PreferredFoot = "Right"
            }
        };

        connection.InsertAll(defaultPlayers);
    }

    void CreateDefaultTournaments()
    {
        List<TournamentSaveData> defaultTournaments = new List<TournamentSaveData>
        {
            new TournamentSaveData
            {
                TournamentId = 1,
                TournamentName = "Premier League",
                TournamentType = "League",
                Season = "2024-25",
                Country = "England",
                Prize = 50000000,
                Prestige = 95,
                Status = "Active"
            },
            new TournamentSaveData
            {
                TournamentId = 2,
                TournamentName = "La Liga",
                TournamentType = "League",
                Season = "2024-25",
                Country = "Spain",
                Prize = 45000000,
                Prestige = 94,
                Status = "Active"
            },
            new TournamentSaveData
            {
                TournamentId = 3,
                TournamentName = "Champions League",
                TournamentType = "Cup",
                Season = "2024-25",
                Country = "Europe",
                Prize = 120000000,
                Prestige = 100,
                Status = "Active"
            },
            new TournamentSaveData
            {
                TournamentId = 4,
                TournamentName = "World Cup",
                TournamentType = "International",
                Season = "2026",
                Country = "World",
                Prize = 200000000,
                Prestige = 100,
                Status = "Upcoming"
            }
        };

        connection.InsertAll(defaultTournaments);
    }

    void CreateDefaultAchievements()
    {
        List<Achievement> defaultAchievements = new List<Achievement>
        {
            new Achievement
            {
                AchievementId = 1,
                Title = "First Goal",
                Description = "Score your first goal",
                Type = "Gameplay",
                Reward = 1000,
                IsUnlocked = false
            },
            new Achievement
            {
                AchievementId = 2,
                Title = "Hat Trick Hero",
                Description = "Score 3 goals in a single match",
                Type = "Gameplay",
                Reward = 5000,
                IsUnlocked = false
            },
            new Achievement
            {
                AchievementId = 3,
                Title = "League Champion",
                Description = "Win your first league title",
                Type = "Career",
                Reward = 50000,
                IsUnlocked = false
            },
            new Achievement
            {
                AchievementId = 4,
                Title = "Master of Skills",
                Description = "Perform all 16 skill moves",
                Type = "Skills",
                Reward = 10000,
                IsUnlocked = false
            }
        };

        connection.InsertAll(defaultAchievements);
    }

    // Save/Load Methods
    public void SaveUserProfile(UserProfile profile)
    {
        var existing = connection.Table<UserProfile>().FirstOrDefault(p => p.UserId == profile.UserId);
        if (existing != null)
        {
            connection.Update(profile);
        }
        else
        {
            connection.Insert(profile);
        }
    }

    public UserProfile LoadUserProfile(int userId)
    {
        return connection.Table<UserProfile>().FirstOrDefault(p => p.UserId == userId);
    }

    public List<TeamSaveData> GetAllTeams()
    {
        return connection.Table<TeamSaveData>().ToList();
    }

    public List<PlayerSaveData> GetPlayersByTeam(int teamId)
    {
        return connection.Table<PlayerSaveData>().Where(p => p.TeamId == teamId).ToList();
    }

    public void SaveCareerProgress(CareerSaveData career)
    {
        var existing = connection.Table<CareerSaveData>().FirstOrDefault(c => c.CareerId == career.CareerId);
        if (existing != null)
        {
            connection.Update(career);
        }
        else
        {
            connection.Insert(career);
        }
    }

    public CareerSaveData LoadCareerProgress(int careerId)
    {
        return connection.Table<CareerSaveData>().FirstOrDefault(c => c.CareerId == careerId);
    }

    public void SaveMatchResult(MatchResult result)
    {
        connection.Insert(result);
    }

    public List<MatchResult> GetMatchHistory(int limit = 20)
    {
        return connection.Table<MatchResult>().OrderByDescending(m => m.Date).Take(limit).ToList();
    }

    public void UnlockAchievement(int achievementId)
    {
        var achievement = connection.Table<Achievement>().FirstOrDefault(a => a.AchievementId == achievementId);
        if (achievement != null && !achievement.IsUnlocked)
        {
            achievement.IsUnlocked = true;
            achievement.UnlockedDate = DateTime.Now;
            connection.Update(achievement);
        }
    }

    public List<Achievement> GetAchievements()
    {
        return connection.Table<Achievement>().ToList();
    }

    public void SaveSettings(UserSettings settings)
    {
        var existing = connection.Table<UserSettings>().FirstOrDefault(s => s.UserId == settings.UserId);
        if (existing != null)
        {
            connection.Update(settings);
        }
        else
        {
            connection.Insert(settings);
        }
    }

    public UserSettings LoadSettings(int userId)
    {
        return connection.Table<UserSettings>().FirstOrDefault(s => s.UserId == userId);
    }

    void OnDestroy()
    {
        connection?.Close();
    }
}

// Database Models
[Table("UserProfile")]
public class UserProfile
{
    [PrimaryKey, AutoIncrement]
    public int UserId { get; set; }
    public string PlayerName { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public int Coins { get; set; }
    public int Gems { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime LastPlayed { get; set; }
    public int TotalMatches { get; set; }
    public int TotalWins { get; set; }
    public int TotalGoals { get; set; }
    public string FavoriteTeam { get; set; }
    public int CurrentCareer { get; set; }
}

[Table("TeamSaveData")]
public class TeamSaveData
{
    [PrimaryKey]
    public int TeamId { get; set; }
    public string TeamName { get; set; }
    public string ShortName { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
    public string League { get; set; }
    public int Overall { get; set; }
    public int Attack { get; set; }
    public int Midfield { get; set; }
    public int Defense { get; set; }
    public int Budget { get; set; }
    public string StadiumName { get; set; }
    public int StadiumCapacity { get; set; }
    public string HomeKitPrimary { get; set; }
    public string HomeKitSecondary { get; set; }
    public string AwayKitPrimary { get; set; }
    public string AwayKitSecondary { get; set; }
    public int Founded { get; set; }
    public string Manager { get; set; }
    public int Trophies { get; set; }
}

[Table("PlayerSaveData")]
public class PlayerSaveData
{
    [PrimaryKey]
    public int PlayerId { get; set; }
    public string PlayerName { get; set; }
    public int Age { get; set; }
    public string Position { get; set; }
    public int TeamId { get; set; }
    public int Overall { get; set; }
    public int Pace { get; set; }
    public int Shooting { get; set; }
    public int Passing { get; set; }
    public int Dribbling { get; set; }
    public int Defense { get; set; }
    public int Physical { get; set; }
    public int Value { get; set; }
    public int Wage { get; set; }
    public int ShirtNumber { get; set; }
    public string Nationality { get; set; }
    public string PreferredFoot { get; set; }
    public int Goals { get; set; }
    public int Assists { get; set; }
    public int Matches { get; set; }
}

[Table("CareerSaveData")]
public class CareerSaveData
{
    [PrimaryKey, AutoIncrement]
    public int CareerId { get; set; }
    public int UserId { get; set; }
    public int CurrentTeamId { get; set; }
    public int Season { get; set; }
    public int Week { get; set; }
    public string CareerType { get; set; } // Manager, Player, Create Club
    public int Budget { get; set; }
    public int Reputation { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime LastSaved { get; set; }
    public string CurrentLeague { get; set; }
    public int LeaguePosition { get; set; }
    public int TrophiesWon { get; set; }
    public bool IsActive { get; set; }
}

[Table("MatchResult")]
public class MatchResult
{
    [PrimaryKey, AutoIncrement]
    public int MatchId { get; set; }
    public int HomeTeamId { get; set; }
    public int AwayTeamId { get; set; }
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }
    public DateTime Date { get; set; }
    public string Competition { get; set; }
    public string Stadium { get; set; }
    public int Attendance { get; set; }
    public string Result { get; set; } // Win, Draw, Loss
}

[Table("TournamentSaveData")]
public class TournamentSaveData
{
    [PrimaryKey]
    public int TournamentId { get; set; }
    public string TournamentName { get; set; }
    public string TournamentType { get; set; }
    public string Season { get; set; }
    public string Country { get; set; }
    public int Prize { get; set; }
    public int Prestige { get; set; }
    public string Status { get; set; }
}

[Table("Achievement")]
public class Achievement
{
    [PrimaryKey]
    public int AchievementId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public int Reward { get; set; }
    public bool IsUnlocked { get; set; }
    public DateTime UnlockedDate { get; set; }
}

[Table("UserSettings")]
public class UserSettings
{
    [PrimaryKey]
    public int UserId { get; set; }
    public float MasterVolume { get; set; }
    public float MusicVolume { get; set; }
    public float SFXVolume { get; set; }
    public int GraphicsQuality { get; set; }
    public bool HapticFeedback { get; set; }
    public bool ShowTutorials { get; set; }
    public string Language { get; set; }
    public int ControlSensitivity { get; set; }
}

[Table("TransferRecord")]
public class TransferRecord
{
    [PrimaryKey, AutoIncrement]
    public int TransferId { get; set; }
    public int PlayerId { get; set; }
    public int FromTeamId { get; set; }
    public int ToTeamId { get; set; }
    public int TransferFee { get; set; }
    public DateTime TransferDate { get; set; }
    public string TransferType { get; set; } // Buy, Sell, Loan
}

[Table("TrainingData")]
public class TrainingData
{
    [PrimaryKey, AutoIncrement]
    public int TrainingId { get; set; }
    public int PlayerId { get; set; }
    public string TrainingType { get; set; }
    public int ImprovementPoints { get; set; }
    public DateTime TrainingDate { get; set; }
    public int Cost { get; set; }
}