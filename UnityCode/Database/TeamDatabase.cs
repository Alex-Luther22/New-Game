using System;
using System.Collections.Generic;
using UnityEngine;

namespace FootballMaster.Database
{
    [Serializable]
    public class PlayerData
    {
        public string id;
        public string name;
        public Position position;
        public int overallRating;
        public int pace;
        public int shooting;
        public int passing;
        public int defending;
        public int physicality;
        public int age;
        public string nationality;
        public int value;
        public int stamina;
        public int skillMoves;
        public int weakFoot;
        public bool isCustom;
        public DateTime createdAt;
    }

    [Serializable]
    public class TeamData
    {
        public string id;
        public string name;
        public string shortName;
        public string country;
        public string league;
        public int overallRating;
        public int attackRating;
        public int midfieldRating;
        public int defenseRating;
        public List<PlayerData> players;
        public Formation formation;
        public Color primaryColor;
        public Color secondaryColor;
        public string stadiumName;
        public int stadiumCapacity;
        public int budget;
        public int prestige;
        public DateTime createdAt;
    }

    [Serializable]
    public class StadiumData
    {
        public string id;
        public string name;
        public int capacity;
        public string country;
        public string city;
        public string surfaceType;
        public string roofType;
        public int atmosphereRating;
        public List<string> weatherConditions;
        public List<string> uniqueFeatures;
        public DateTime createdAt;
    }

    public enum Position
    {
        Goalkeeper,
        Defender,
        Midfielder,
        Forward
    }

    public enum Formation
    {
        F_4_4_2,
        F_4_3_3,
        F_3_5_2,
        F_4_2_3_1,
        F_5_4_1
    }

    [CreateAssetMenu(fileName = "TeamDatabase", menuName = "Football Master/Team Database")]
    public class TeamDatabase : ScriptableObject
    {
        [Header("Teams Configuration")]
        public List<TeamData> teams = new List<TeamData>();
        
        [Header("Stadiums Configuration")]
        public List<StadiumData> stadiums = new List<StadiumData>();
        
        [Header("Player Names Database")]
        public List<string> goalkeeperNames = new List<string>();
        public List<string> defenderNames = new List<string>();
        public List<string> midfielderNames = new List<string>();
        public List<string> forwardNames = new List<string>();
        
        [Header("Countries")]
        public List<string> countries = new List<string>();
        
        private void OnEnable()
        {
            if (teams.Count == 0)
            {
                InitializeDefaultTeams();
            }
            
            if (stadiums.Count == 0)
            {
                InitializeDefaultStadiums();
            }
        }

        private void InitializeDefaultTeams()
        {
            // Premier League Teams
            teams.Add(new TeamData
            {
                id = "london_red",
                name = "London Red",
                shortName = "LRD",
                country = "England",
                league = "Premier League",
                overallRating = 88,
                attackRating = 90,
                midfieldRating = 87,
                defenseRating = 86,
                formation = Formation.F_4_3_3,
                primaryColor = Color.red,
                secondaryColor = Color.white,
                stadiumName = "Emirates Arena",
                stadiumCapacity = 60000,
                budget = 200000000,
                prestige = 9,
                createdAt = DateTime.Now,
                players = GenerateTeamPlayers(88, Position.Goalkeeper, 3)
            });

            teams.Add(new TeamData
            {
                id = "manchester_blue",
                name = "Manchester Blue",
                shortName = "MCB",
                country = "England",
                league = "Premier League",
                overallRating = 91,
                attackRating = 93,
                midfieldRating = 90,
                defenseRating = 89,
                formation = Formation.F_4_3_3,
                primaryColor = new Color(0.42f, 0.8f, 0.87f),
                secondaryColor = Color.white,
                stadiumName = "Etihad Stadium",
                stadiumCapacity = 55000,
                budget = 250000000,
                prestige = 10,
                createdAt = DateTime.Now,
                players = GenerateTeamPlayers(91, Position.Goalkeeper, 3)
            });

            // Spanish La Liga Teams
            teams.Add(new TeamData
            {
                id = "madrid_white",
                name = "Madrid White",
                shortName = "MDW",
                country = "Spain",
                league = "La Liga",
                overallRating = 92,
                attackRating = 94,
                midfieldRating = 91,
                defenseRating = 90,
                formation = Formation.F_4_3_3,
                primaryColor = Color.white,
                secondaryColor = new Color(1f, 0.84f, 0f),
                stadiumName = "Santiago Bernabéu",
                stadiumCapacity = 81000,
                budget = 300000000,
                prestige = 10,
                createdAt = DateTime.Now,
                players = GenerateTeamPlayers(92, Position.Goalkeeper, 3)
            });

            teams.Add(new TeamData
            {
                id = "barcelona_fc",
                name = "Barcelona FC",
                shortName = "BAR",
                country = "Spain",
                league = "La Liga",
                overallRating = 90,
                attackRating = 92,
                midfieldRating = 89,
                defenseRating = 88,
                formation = Formation.F_4_3_3,
                primaryColor = new Color(0.65f, 0f, 0.27f),
                secondaryColor = new Color(0f, 0.3f, 0.6f),
                stadiumName = "Camp Nou",
                stadiumCapacity = 99000,
                budget = 280000000,
                prestige = 10,
                createdAt = DateTime.Now,
                players = GenerateTeamPlayers(90, Position.Goalkeeper, 3)
            });

            // German Bundesliga Teams
            teams.Add(new TeamData
            {
                id = "bayern_munich",
                name = "Bayern Munich",
                shortName = "BAY",
                country = "Germany",
                league = "Bundesliga",
                overallRating = 90,
                attackRating = 92,
                midfieldRating = 89,
                defenseRating = 88,
                formation = Formation.F_4_2_3_1,
                primaryColor = Color.red,
                secondaryColor = Color.white,
                stadiumName = "Allianz Arena",
                stadiumCapacity = 75000,
                budget = 250000000,
                prestige = 9,
                createdAt = DateTime.Now,
                players = GenerateTeamPlayers(90, Position.Goalkeeper, 3)
            });

            // Add more teams...
        }

        private void InitializeDefaultStadiums()
        {
            stadiums.Add(new StadiumData
            {
                id = "emirates_arena",
                name = "Emirates Arena",
                capacity = 60000,
                country = "England",
                city = "London",
                surfaceType = "hybrid_grass",
                roofType = "open",
                atmosphereRating = 8,
                weatherConditions = new List<string> { "rainy", "cloudy", "sunny" },
                uniqueFeatures = new List<string> { "historic_stands", "steep_stands" },
                createdAt = DateTime.Now
            });

            stadiums.Add(new StadiumData
            {
                id = "camp_nou",
                name = "Camp Nou",
                capacity = 99000,
                country = "Spain",
                city = "Barcelona",
                surfaceType = "natural_grass",
                roofType = "open",
                atmosphereRating = 10,
                weatherConditions = new List<string> { "sunny", "windy" },
                uniqueFeatures = new List<string> { "massive_capacity", "iconic_architecture" },
                createdAt = DateTime.Now
            });

            stadiums.Add(new StadiumData
            {
                id = "allianz_arena",
                name = "Allianz Arena",
                capacity = 75000,
                country = "Germany",
                city = "Munich",
                surfaceType = "natural_grass",
                roofType = "closed",
                atmosphereRating = 9,
                weatherConditions = new List<string> { "cold", "snow", "sunny" },
                uniqueFeatures = new List<string> { "color_changing_exterior", "modern_design" },
                createdAt = DateTime.Now
            });
        }

        private List<PlayerData> GenerateTeamPlayers(int teamRating, Position startPosition, int count)
        {
            List<PlayerData> players = new List<PlayerData>();
            
            // Generate players for all positions
            Position[] positions = { Position.Goalkeeper, Position.Defender, Position.Midfielder, Position.Forward };
            int[] positionCounts = { 3, 8, 8, 6 };
            
            for (int i = 0; i < positions.Length; i++)
            {
                for (int j = 0; j < positionCounts[i]; j++)
                {
                    players.Add(GeneratePlayer(positions[i], teamRating, j == 0));
                }
            }
            
            return players;
        }

        private PlayerData GeneratePlayer(Position position, int teamRating, bool isStarter)
        {
            List<string> names = GetNamesByPosition(position);
            
            int baseRating = teamRating;
            if (!isStarter)
            {
                baseRating = Mathf.Max(50, teamRating - UnityEngine.Random.Range(5, 15));
            }
            
            return new PlayerData
            {
                id = Guid.NewGuid().ToString(),
                name = names[UnityEngine.Random.Range(0, names.Count)],
                position = position,
                overallRating = Mathf.Clamp(baseRating + UnityEngine.Random.Range(-3, 4), 50, 99),
                pace = UnityEngine.Random.Range(40, 95),
                shooting = UnityEngine.Random.Range(30, 95),
                passing = UnityEngine.Random.Range(40, 95),
                defending = UnityEngine.Random.Range(30, 95),
                physicality = UnityEngine.Random.Range(40, 95),
                age = UnityEngine.Random.Range(18, 35),
                nationality = GetRandomCountry(),
                value = UnityEngine.Random.Range(100000, 50000000),
                stamina = UnityEngine.Random.Range(65, 95),
                skillMoves = UnityEngine.Random.Range(1, 6),
                weakFoot = UnityEngine.Random.Range(1, 6),
                isCustom = false,
                createdAt = DateTime.Now
            };
        }

        private List<string> GetNamesByPosition(Position position)
        {
            switch (position)
            {
                case Position.Goalkeeper:
                    return goalkeeperNames.Count > 0 ? goalkeeperNames : new List<string> { "Martinez", "Silva", "Johnson", "Mueller" };
                case Position.Defender:
                    return defenderNames.Count > 0 ? defenderNames : new List<string> { "Smith", "Jones", "Williams", "Davis" };
                case Position.Midfielder:
                    return midfielderNames.Count > 0 ? midfielderNames : new List<string> { "Robinson", "Walker", "Young", "Allen" };
                case Position.Forward:
                    return forwardNames.Count > 0 ? forwardNames : new List<string> { "Roberts", "Carter", "Mitchell", "Perez" };
                default:
                    return new List<string> { "Player" };
            }
        }

        private string GetRandomCountry()
        {
            List<string> defaultCountries = new List<string> 
            { 
                "England", "Spain", "Germany", "Italy", "France", "Brazil", "Argentina", 
                "Portugal", "Netherlands", "Belgium", "Croatia", "Colombia", "Mexico" 
            };
            
            List<string> countryList = countries.Count > 0 ? countries : defaultCountries;
            return countryList[UnityEngine.Random.Range(0, countryList.Count)];
        }

        public TeamData GetTeamById(string id)
        {
            return teams.Find(team => team.id == id);
        }

        public List<TeamData> GetTeamsByLeague(string league)
        {
            return teams.FindAll(team => team.league == league);
        }

        public List<TeamData> GetTeamsByCountry(string country)
        {
            return teams.FindAll(team => team.country == country);
        }

        public StadiumData GetStadiumById(string id)
        {
            return stadiums.Find(stadium => stadium.id == id);
        }

        public List<string> GetAvailableLeagues()
        {
            HashSet<string> leagues = new HashSet<string>();
            foreach (var team in teams)
            {
                leagues.Add(team.league);
            }
            return new List<string>(leagues);
        }

        public List<string> GetAvailableCountries()
        {
            HashSet<string> countries = new HashSet<string>();
            foreach (var team in teams)
            {
                countries.Add(team.country);
            }
            return new List<string>(countries);
        }
    }
}