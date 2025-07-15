using UnityEngine;

[CreateAssetMenu(fileName = "New League", menuName = "Football Game/League Data")]
public class LeagueData : ScriptableObject
{
    [Header("League Basic Info")]
    public string leagueName;
    public string shortName; // Ej: "EPL", "La Liga", "Serie A"
    public string country;
    public int leagueId;
    public int tier = 1; // 1 = Primera división, 2 = Segunda, etc.
    
    [Header("Visual Identity")]
    public Sprite leagueLogo;
    public Color primaryColor = Color.white;
    public Color secondaryColor = Color.black;
    
    [Header("Competition Format")]
    public int numberOfTeams = 20;
    public int matchesPerSeason = 38;
    public bool hasPlayoffs = false;
    public int promotionSpots = 0;
    public int relegationSpots = 3;
    
    [Header("Season Settings")]
    public System.DateTime seasonStart = new System.DateTime(2025, 8, 1);
    public System.DateTime seasonEnd = new System.DateTime(2026, 5, 31);
    public int winterBreakDays = 0;
    
    [Header("Teams")]
    public TeamData[] teams;
    
    [Header("Prize Money")]
    public long championPrize = 50000000;
    public long runnerUpPrize = 30000000;
    public long participationBonus = 1000000;
    public long winBonus = 100000;
    public long drawBonus = 50000;
    
    [Header("Broadcasting")]
    public long tvRevenue = 100000000;
    public string[] broadcastPartners;
    
    [Header("Sponsorship")]
    public string mainSponsor;
    public string ballSponsor;
    public long sponsorshipRevenue = 50000000;
    
    [Header("Statistics")]
    public int foundedYear = 1888;
    public int totalSeasons = 137;
    public TeamData mostSuccessfulTeam;
    public int mostTitlesCount = 0;
    
    [Header("Records")]
    public int highestAttendance = 0;
    public string highestAttendanceMatch;
    public int mostGoalsInSeason = 0;
    public string mostGoalsPlayer;
    public int mostGoalsInMatch = 0;
    public string mostGoalsMatch;
    
    [Header("Competition Rules")]
    public bool hasExtraTime = false;
    public bool hasPenalties = false;
    public int maxSubstitutions = 5;
    public int maxForeignPlayers = 0; // 0 = sin límite
    
    // Propiedades calculadas
    public long totalPrizeMoney
    {
        get
        {
            return championPrize + runnerUpPrize + (participationBonus * numberOfTeams);
        }
    }
    
    public long totalRevenue
    {
        get
        {
            return tvRevenue + sponsorshipRevenue + totalPrizeMoney;
        }
    }
    
    public float averageTeamRating
    {
        get
        {
            if (teams == null || teams.Length == 0) return 0f;
            
            float totalRating = 0f;
            int validTeams = 0;
            
            foreach (TeamData team in teams)
            {
                if (team != null)
                {
                    totalRating += team.overallRating;
                    validTeams++;
                }
            }
            
            return validTeams > 0 ? totalRating / validTeams : 0f;
        }
    }
    
    public int totalMatches
    {
        get
        {
            return numberOfTeams * (numberOfTeams - 1);
        }
    }
    
    // Métodos para gestión de equipos
    public bool AddTeam(TeamData team)
    {
        if (teams.Length >= numberOfTeams)
        {
            Debug.LogWarning("League is full! Cannot add more teams.");
            return false;
        }
        
        // Buscar primera posición vacía
        for (int i = 0; i < teams.Length; i++)
        {
            if (teams[i] == null)
            {
                teams[i] = team;
                return true;
            }
        }
        
        return false;
    }
    
    public bool RemoveTeam(TeamData team)
    {
        for (int i = 0; i < teams.Length; i++)
        {
            if (teams[i] == team)
            {
                teams[i] = null;
                return true;
            }
        }
        
        return false;
    }
    
    public TeamData GetTeamById(int teamId)
    {
        foreach (TeamData team in teams)
        {
            if (team != null && team.teamId == teamId)
            {
                return team;
            }
        }
        
        return null;
    }
    
    public TeamData[] GetActiveTeams()
    {
        System.Collections.Generic.List<TeamData> activeTeams = new System.Collections.Generic.List<TeamData>();
        
        foreach (TeamData team in teams)
        {
            if (team != null)
            {
                activeTeams.Add(team);
            }
        }
        
        return activeTeams.ToArray();
    }
    
    // Métodos para calcular premios
    public long GetPositionPrize(int position)
    {
        switch (position)
        {
            case 1:
                return championPrize;
            case 2:
                return runnerUpPrize;
            default:
                return participationBonus;
        }
    }
    
    public long GetMatchPrize(int homeScore, int awayScore)
    {
        if (homeScore == awayScore)
        {
            return drawBonus;
        }
        else
        {
            return winBonus;
        }
    }
    
    // Generar liga con equipos aleatorios
    public static LeagueData GenerateRandomLeague(string name, string country, int teamCount = 20)
    {
        LeagueData league = ScriptableObject.CreateInstance<LeagueData>();
        
        league.leagueName = name;
        league.shortName = name.Substring(0, Mathf.Min(5, name.Length));
        league.country = country;
        league.leagueId = Random.Range(1000, 9999);
        league.numberOfTeams = teamCount;
        league.matchesPerSeason = teamCount * 2 - 2;
        
        // Generar equipos
        league.teams = new TeamData[teamCount];
        
        string[] cityNames = GenerateCityNames(country, teamCount);
        
        for (int i = 0; i < teamCount; i++)
        {
            string teamName = $"{cityNames[i]} FC";
            league.teams[i] = TeamData.GenerateRandomTeam(teamName, i + 1, country);
        }
        
        // Configurar premios basados en el país
        ConfigurePrizeMoney(league, country);
        
        // Configurar colores de liga
        league.primaryColor = new Color(Random.value, Random.value, Random.value);
        league.secondaryColor = new Color(Random.value, Random.value, Random.value);
        
        return league;
    }
    
    static string[] GenerateCityNames(string country, int count)
    {
        string[] baseCities = GetBaseCitiesForCountry(country);
        string[] result = new string[count];
        
        for (int i = 0; i < count; i++)
        {
            if (i < baseCities.Length)
            {
                result[i] = baseCities[i];
            }
            else
            {
                result[i] = $"Ciudad {i + 1}";
            }
        }
        
        return result;
    }
    
    static string[] GetBaseCitiesForCountry(string country)
    {
        switch (country.ToLower())
        {
            case "spain":
                return new string[] { "Madrid", "Barcelona", "Valencia", "Sevilla", "Bilbao", "Zaragoza", "Málaga", "Murcia", "Palma", "Las Palmas", "Gijón", "Cádiz", "Santander", "Pamplona", "Almería", "Leganés", "Getafe", "Elche", "Villarreal", "Real Sociedad" };
            case "england":
                return new string[] { "London", "Manchester", "Liverpool", "Birmingham", "Leeds", "Sheffield", "Newcastle", "Brighton", "Southampton", "Arsenal", "Chelsea", "Tottenham", "Everton", "Aston Villa", "West Ham", "Leicester", "Wolves", "Crystal Palace", "Burnley", "Norwich" };
            case "italy":
                return new string[] { "Milano", "Roma", "Napoli", "Torino", "Genova", "Palermo", "Bologna", "Firenze", "Verona", "Catania", "Venezia", "Trieste", "Brescia", "Parma", "Modena", "Reggio", "Livorno", "Pisa", "Perugia", "Pescara" };
            case "germany":
                return new string[] { "Berlin", "München", "Hamburg", "Köln", "Frankfurt", "Stuttgart", "Düsseldorf", "Dortmund", "Essen", "Leipzig", "Bremen", "Dresden", "Hannover", "Nürnberg", "Duisburg", "Bochum", "Wuppertal", "Bielefeld", "Bonn", "Münster" };
            case "france":
                return new string[] { "Paris", "Marseille", "Lyon", "Toulouse", "Nice", "Nantes", "Strasbourg", "Montpellier", "Bordeaux", "Lille", "Rennes", "Reims", "Le Havre", "Saint-Étienne", "Toulon", "Grenoble", "Dijon", "Angers", "Nîmes", "Villeurbanne" };
            default:
                return new string[] { "Ciudad1", "Ciudad2", "Ciudad3", "Ciudad4", "Ciudad5", "Ciudad6", "Ciudad7", "Ciudad8", "Ciudad9", "Ciudad10", "Ciudad11", "Ciudad12", "Ciudad13", "Ciudad14", "Ciudad15", "Ciudad16", "Ciudad17", "Ciudad18", "Ciudad19", "Ciudad20" };
        }
    }
    
    static void ConfigurePrizeMoney(LeagueData league, string country)
    {
        // Configurar premios basados en el prestigio del país
        switch (country.ToLower())
        {
            case "england":
                league.championPrize = 100000000;
                league.runnerUpPrize = 60000000;
                league.tvRevenue = 500000000;
                league.sponsorshipRevenue = 100000000;
                break;
            case "spain":
                league.championPrize = 80000000;
                league.runnerUpPrize = 50000000;
                league.tvRevenue = 400000000;
                league.sponsorshipRevenue = 80000000;
                break;
            case "germany":
                league.championPrize = 70000000;
                league.runnerUpPrize = 45000000;
                league.tvRevenue = 350000000;
                league.sponsorshipRevenue = 70000000;
                break;
            case "italy":
                league.championPrize = 60000000;
                league.runnerUpPrize = 40000000;
                league.tvRevenue = 300000000;
                league.sponsorshipRevenue = 60000000;
                break;
            case "france":
                league.championPrize = 50000000;
                league.runnerUpPrize = 35000000;
                league.tvRevenue = 250000000;
                league.sponsorshipRevenue = 50000000;
                break;
            default:
                league.championPrize = 30000000;
                league.runnerUpPrize = 20000000;
                league.tvRevenue = 150000000;
                league.sponsorshipRevenue = 30000000;
                break;
        }
        
        league.participationBonus = league.championPrize / 50;
        league.winBonus = league.championPrize / 500;
        league.drawBonus = league.winBonus / 2;
    }
    
    // Método para validar la liga
    public bool IsValidLeague()
    {
        if (string.IsNullOrEmpty(leagueName)) return false;
        if (numberOfTeams < 2) return false;
        if (teams == null || teams.Length != numberOfTeams) return false;
        
        int activeTeams = 0;
        foreach (TeamData team in teams)
        {
            if (team != null) activeTeams++;
        }
        
        return activeTeams >= 2;
    }
    
    // Método para obtener información completa de la liga
    public LeagueInfo GetLeagueInfo()
    {
        return new LeagueInfo
        {
            league = this,
            season = 2025,
            currentMatchday = 1,
            totalMatchdays = matchesPerSeason,
            leagueTable = new System.Collections.Generic.List<TeamStats>(),
            upcomingFixtures = new System.Collections.Generic.List<MatchFixture>(),
            lastResults = new System.Collections.Generic.List<MatchResult>()
        };
    }
}