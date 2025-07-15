using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LeagueManager : MonoBehaviour
{
    [Header("League Settings")]
    public LeagueData currentLeague;
    public int currentSeason = 2025;
    public int currentMatchday = 1;
    public int totalMatchdays = 38;
    
    [Header("Season Progress")]
    public List<MatchResult> seasonResults = new List<MatchResult>();
    public List<TeamStats> leagueTable = new List<TeamStats>();
    
    [Header("Match Generation")]
    public bool autoGenerateFixtures = true;
    public List<MatchFixture> fixtures = new List<MatchFixture>();
    
    [Header("Events")]
    public System.Action<MatchResult> OnMatchCompleted;
    public System.Action<List<TeamStats>> OnTableUpdated;
    public System.Action<int> OnSeasonEnded;
    
    private GameManager gameManager;
    
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        
        if (currentLeague != null)
        {
            InitializeLeague();
        }
    }
    
    void InitializeLeague()
    {
        // Inicializar tabla de la liga
        InitializeLeagueTable();
        
        // Generar fixtures si está habilitado
        if (autoGenerateFixtures)
        {
            GenerateSeasonFixtures();
        }
        
        // Suscribirse a eventos del game manager
        if (gameManager != null)
        {
            GameManager.OnScoreChanged += OnMatchScoreChanged;
        }
    }
    
    void InitializeLeagueTable()
    {
        leagueTable.Clear();
        
        foreach (TeamData team in currentLeague.teams)
        {
            if (team != null)
            {
                TeamStats stats = new TeamStats
                {
                    team = team,
                    matchesPlayed = 0,
                    wins = 0,
                    draws = 0,
                    losses = 0,
                    goalsFor = 0,
                    goalsAgainst = 0,
                    points = 0
                };
                
                leagueTable.Add(stats);
            }
        }
    }
    
    void GenerateSeasonFixtures()
    {
        fixtures.Clear();
        
        TeamData[] teams = currentLeague.teams;
        int teamCount = teams.Length;
        
        // Generar fixtures usando el algoritmo round-robin
        for (int round = 0; round < (teamCount - 1) * 2; round++)
        {
            int matchday = (round / 2) + 1;
            bool isSecondLeg = round >= teamCount - 1;
            
            for (int match = 0; match < teamCount / 2; match++)
            {
                int home = (round + match) % (teamCount - 1);
                int away = (teamCount - 1 - match + round) % (teamCount - 1);
                
                // El último equipo siempre juega contra el calculado
                if (match == 0)
                {
                    away = teamCount - 1;
                }
                
                // Cambiar local y visitante en la segunda vuelta
                if (isSecondLeg)
                {
                    int temp = home;
                    home = away;
                    away = temp;
                }
                
                MatchFixture fixture = new MatchFixture
                {
                    homeTeam = teams[home],
                    awayTeam = teams[away],
                    matchday = matchday,
                    season = currentSeason,
                    isPlayed = false,
                    scheduledDate = System.DateTime.Now.AddDays(matchday * 7)
                };
                
                fixtures.Add(fixture);
            }
        }
        
        // Mezclar fixtures para mayor realismo
        ShuffleFixtures();
    }
    
    void ShuffleFixtures()
    {
        for (int i = 0; i < fixtures.Count; i++)
        {
            MatchFixture temp = fixtures[i];
            int randomIndex = Random.Range(i, fixtures.Count);
            fixtures[i] = fixtures[randomIndex];
            fixtures[randomIndex] = temp;
        }
    }
    
    void OnMatchScoreChanged(int homeScore, int awayScore)
    {
        // Este método se llama cuando cambia el marcador en un partido
        // Aquí podemos actualizar estadísticas en tiempo real
    }
    
    public void RegisterMatchResult(TeamData homeTeam, TeamData awayTeam, int homeScore, int awayScore)
    {
        MatchResult result = new MatchResult
        {
            homeTeam = homeTeam,
            awayTeam = awayTeam,
            homeScore = homeScore,
            awayScore = awayScore,
            matchday = currentMatchday,
            season = currentSeason,
            date = System.DateTime.Now
        };
        
        seasonResults.Add(result);
        
        // Actualizar estadísticas de los equipos
        UpdateTeamStats(homeTeam, awayTeam, homeScore, awayScore);
        
        // Actualizar tabla
        UpdateLeagueTable();
        
        // Disparar eventos
        OnMatchCompleted?.Invoke(result);
        OnTableUpdated?.Invoke(leagueTable);
        
        // Verificar si la temporada terminó
        if (IsSeasonComplete())
        {
            EndSeason();
        }
    }
    
    void UpdateTeamStats(TeamData homeTeam, TeamData awayTeam, int homeScore, int awayScore)
    {
        TeamStats homeStats = leagueTable.Find(t => t.team == homeTeam);
        TeamStats awayStats = leagueTable.Find(t => t.team == awayTeam);
        
        if (homeStats != null && awayStats != null)
        {
            // Actualizar estadísticas del equipo local
            homeStats.matchesPlayed++;
            homeStats.goalsFor += homeScore;
            homeStats.goalsAgainst += awayScore;
            
            // Actualizar estadísticas del equipo visitante
            awayStats.matchesPlayed++;
            awayStats.goalsFor += awayScore;
            awayStats.goalsAgainst += homeScore;
            
            // Determinar resultado
            if (homeScore > awayScore)
            {
                // Victoria local
                homeStats.wins++;
                homeStats.points += 3;
                awayStats.losses++;
            }
            else if (awayScore > homeScore)
            {
                // Victoria visitante
                awayStats.wins++;
                awayStats.points += 3;
                homeStats.losses++;
            }
            else
            {
                // Empate
                homeStats.draws++;
                homeStats.points += 1;
                awayStats.draws++;
                awayStats.points += 1;
            }
        }
    }
    
    void UpdateLeagueTable()
    {
        // Ordenar tabla por puntos, diferencia de goles, goles a favor
        leagueTable.Sort((a, b) => {
            if (a.points != b.points)
                return b.points.CompareTo(a.points);
            
            if (a.goalDifference != b.goalDifference)
                return b.goalDifference.CompareTo(a.goalDifference);
            
            return b.goalsFor.CompareTo(a.goalsFor);
        });
        
        // Actualizar posiciones
        for (int i = 0; i < leagueTable.Count; i++)
        {
            leagueTable[i].position = i + 1;
        }
    }
    
    bool IsSeasonComplete()
    {
        return seasonResults.Count >= currentLeague.teams.Length * (currentLeague.teams.Length - 1);
    }
    
    void EndSeason()
    {
        OnSeasonEnded?.Invoke(currentSeason);
        
        // Guardar resultados de temporada
        SaveSeasonResults();
        
        // Preparar nueva temporada
        PrepareNewSeason();
    }
    
    void SaveSeasonResults()
    {
        SeasonData seasonData = new SeasonData
        {
            season = currentSeason,
            leagueTable = new List<TeamStats>(leagueTable),
            results = new List<MatchResult>(seasonResults),
            champion = leagueTable[0].team,
            topScorer = GetTopScorer(),
            averageGoalsPerMatch = GetAverageGoalsPerMatch()
        };
        
        // Guardar datos de temporada (implementar persistencia)
        SaveSeasonData(seasonData);
    }
    
    void PrepareNewSeason()
    {
        currentSeason++;
        currentMatchday = 1;
        seasonResults.Clear();
        
        // Reiniciar estadísticas
        InitializeLeagueTable();
        
        // Generar nuevos fixtures
        if (autoGenerateFixtures)
        {
            GenerateSeasonFixtures();
        }
    }
    
    // Métodos para obtener estadísticas
    public PlayerData GetTopScorer()
    {
        // Implementar lógica para encontrar el máximo goleador
        // Esto requeriría tracking de goles por jugador
        return null;
    }
    
    public float GetAverageGoalsPerMatch()
    {
        if (seasonResults.Count == 0) return 0f;
        
        int totalGoals = 0;
        foreach (MatchResult result in seasonResults)
        {
            totalGoals += result.homeScore + result.awayScore;
        }
        
        return (float)totalGoals / seasonResults.Count;
    }
    
    public List<MatchFixture> GetUpcomingFixtures(int matchday)
    {
        return fixtures.Where(f => f.matchday == matchday && !f.isPlayed).ToList();
    }
    
    public List<MatchResult> GetMatchdayResults(int matchday)
    {
        return seasonResults.Where(r => r.matchday == matchday).ToList();
    }
    
    public TeamStats GetTeamStats(TeamData team)
    {
        return leagueTable.Find(t => t.team == team);
    }
    
    public List<TeamStats> GetLeagueTable()
    {
        return new List<TeamStats>(leagueTable);
    }
    
    // Simular partido entre dos equipos
    public MatchResult SimulateMatch(TeamData homeTeam, TeamData awayTeam)
    {
        // Lógica de simulación simplificada
        int homeStrength = homeTeam.overallRating + 3; // Ventaja de local
        int awayStrength = awayTeam.overallRating;
        
        // Calcular probabilidades
        float homeWinProb = (float)homeStrength / (homeStrength + awayStrength);
        float drawProb = 0.25f;
        float awayWinProb = 1f - homeWinProb - drawProb;
        
        // Generar resultado
        float random = Random.value;
        int homeScore, awayScore;
        
        if (random < homeWinProb)
        {
            // Victoria local
            homeScore = Random.Range(1, 4);
            awayScore = Random.Range(0, homeScore);
        }
        else if (random < homeWinProb + drawProb)
        {
            // Empate
            int goals = Random.Range(0, 3);
            homeScore = goals;
            awayScore = goals;
        }
        else
        {
            // Victoria visitante
            awayScore = Random.Range(1, 4);
            homeScore = Random.Range(0, awayScore);
        }
        
        return new MatchResult
        {
            homeTeam = homeTeam,
            awayTeam = awayTeam,
            homeScore = homeScore,
            awayScore = awayScore,
            matchday = currentMatchday,
            season = currentSeason,
            date = System.DateTime.Now
        };
    }
    
    // Simular toda la jornada
    public void SimulateMatchday(int matchday)
    {
        List<MatchFixture> matchdayFixtures = GetUpcomingFixtures(matchday);
        
        foreach (MatchFixture fixture in matchdayFixtures)
        {
            MatchResult result = SimulateMatch(fixture.homeTeam, fixture.awayTeam);
            RegisterMatchResult(result.homeTeam, result.awayTeam, result.homeScore, result.awayScore);
            
            // Marcar fixture como jugado
            fixture.isPlayed = true;
        }
        
        currentMatchday++;
    }
    
    // Métodos de persistencia (implementar según necesidades)
    void SaveSeasonData(SeasonData data)
    {
        // Implementar guardado de datos de temporada
        string json = JsonUtility.ToJson(data);
        // Guardar en PlayerPrefs, archivo, o base de datos
    }
    
    SeasonData LoadSeasonData(int season)
    {
        // Implementar carga de datos de temporada
        return null;
    }
    
    // Método para obtener información completa de la liga
    public LeagueInfo GetLeagueInfo()
    {
        return new LeagueInfo
        {
            league = currentLeague,
            season = currentSeason,
            currentMatchday = currentMatchday,
            totalMatchdays = totalMatchdays,
            leagueTable = leagueTable,
            upcomingFixtures = GetUpcomingFixtures(currentMatchday),
            lastResults = seasonResults.TakeLast(10).ToList()
        };
    }
}

[System.Serializable]
public class MatchResult
{
    public TeamData homeTeam;
    public TeamData awayTeam;
    public int homeScore;
    public int awayScore;
    public int matchday;
    public int season;
    public System.DateTime date;
    
    public string GetResultString()
    {
        return $"{homeTeam.shortName} {homeScore}-{awayScore} {awayTeam.shortName}";
    }
}

[System.Serializable]
public class MatchFixture
{
    public TeamData homeTeam;
    public TeamData awayTeam;
    public int matchday;
    public int season;
    public bool isPlayed;
    public System.DateTime scheduledDate;
    
    public string GetFixtureString()
    {
        return $"{homeTeam.shortName} vs {awayTeam.shortName}";
    }
}

[System.Serializable]
public class TeamStats
{
    public TeamData team;
    public int position;
    public int matchesPlayed;
    public int wins;
    public int draws;
    public int losses;
    public int goalsFor;
    public int goalsAgainst;
    public int points;
    
    public int goalDifference => goalsFor - goalsAgainst;
    public float pointsPerGame => matchesPlayed > 0 ? (float)points / matchesPlayed : 0f;
    public float winPercentage => matchesPlayed > 0 ? (float)wins / matchesPlayed * 100f : 0f;
    
    public string GetFormattedStats()
    {
        return $"{position}. {team.shortName} - {points}pts ({wins}W {draws}D {losses}L) {goalsFor}:{goalsAgainst}";
    }
}

[System.Serializable]
public class SeasonData
{
    public int season;
    public List<TeamStats> leagueTable;
    public List<MatchResult> results;
    public TeamData champion;
    public PlayerData topScorer;
    public float averageGoalsPerMatch;
}

[System.Serializable]
public class LeagueInfo
{
    public LeagueData league;
    public int season;
    public int currentMatchday;
    public int totalMatchdays;
    public List<TeamStats> leagueTable;
    public List<MatchFixture> upcomingFixtures;
    public List<MatchResult> lastResults;
}