using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class CareerManager : MonoBehaviour
{
    [Header("Career UI")]
    public GameObject careerMenuPanel;
    public GameObject newCareerPanel;
    public GameObject careerDashboardPanel;
    public GameObject transferMarketPanel;
    public GameObject trainingCenterPanel;
    public GameObject calendarPanel;
    public GameObject clubManagementPanel;
    
    [Header("Career Info")]
    public Text seasonText;
    public Text weekText;
    public Text clubNameText;
    public Text leaguePositionText;
    public Text budgetText;
    public Text reputationText;
    public Text nextMatchText;
    public Image clubLogo;
    
    [Header("Team Management")]
    public Transform playerListParent;
    public GameObject playerCardPrefab;
    public Text teamOverallText;
    public Text teamChemistryText;
    public Text teamMoraleText;
    
    [Header("Career Buttons")]
    public Button playNextMatchButton;
    public Button simulateMatchButton;
    public Button transferMarketButton;
    public Button trainingCenterButton;
    public Button clubManagementButton;
    public Button calendarButton;
    public Button saveCareerButton;
    
    [Header("New Career")]
    public Dropdown careerTypeDropdown;
    public Dropdown difficultyDropdown;
    public InputField managerNameInput;
    public Button createCareerButton;
    
    private CareerSaveData currentCareer;
    private List<PlayerSaveData> squadPlayers;
    private List<MatchFixture> fixtures;
    private LeagueTable leagueTable;
    
    void Start()
    {
        InitializeCareerManager();
    }
    
    void InitializeCareerManager()
    {
        // Setup button listeners
        playNextMatchButton.onClick.AddListener(PlayNextMatch);
        simulateMatchButton.onClick.AddListener(SimulateNextMatch);
        transferMarketButton.onClick.AddListener(ShowTransferMarket);
        trainingCenterButton.onClick.AddListener(ShowTrainingCenter);
        clubManagementButton.onClick.AddListener(ShowClubManagement);
        calendarButton.onClick.AddListener(ShowCalendar);
        saveCareerButton.onClick.AddListener(SaveCareer);
        createCareerButton.onClick.AddListener(CreateCareer);
        
        // Initialize dropdown options
        SetupCareerTypeDropdown();
        SetupDifficultyDropdown();
        
        squadPlayers = new List<PlayerSaveData>();
        fixtures = new List<MatchFixture>();
    }
    
    void SetupCareerTypeDropdown()
    {
        careerTypeDropdown.ClearOptions();
        List<string> careerTypes = new List<string>
        {
            "Manager Career",
            "Player Career",
            "Create a Club"
        };
        careerTypeDropdown.AddOptions(careerTypes);
    }
    
    void SetupDifficultyDropdown()
    {
        difficultyDropdown.ClearOptions();
        List<string> difficulties = new List<string>
        {
            "Beginner",
            "Amateur",
            "Semi-Pro",
            "Professional",
            "World Class",
            "Legendary"
        };
        difficultyDropdown.AddOptions(difficulties);
    }
    
    public void CreateNewCareer()
    {
        careerMenuPanel.SetActive(false);
        newCareerPanel.SetActive(true);
    }
    
    public void ContinueCareer()
    {
        UserProfile user = SaveSystem.Instance.LoadUserProfile(1);
        if (user != null && user.CurrentCareer > 0)
        {
            currentCareer = SaveSystem.Instance.LoadCareerProgress(user.CurrentCareer);
            if (currentCareer != null)
            {
                LoadCareerDashboard();
            }
        }
    }
    
    void CreateCareer()
    {
        if (string.IsNullOrEmpty(managerNameInput.text))
        {
            ShowNotification("Please enter a manager name");
            return;
        }
        
        // Create new career
        CareerSaveData newCareer = new CareerSaveData
        {
            UserId = 1,
            CurrentTeamId = 0, // Will be set when team is selected
            Season = 1,
            Week = 1,
            CareerType = careerTypeDropdown.options[careerTypeDropdown.value].text,
            Budget = 50000000,
            Reputation = 50,
            StartDate = System.DateTime.Now,
            LastSaved = System.DateTime.Now,
            CurrentLeague = "",
            LeaguePosition = 0,
            TrophiesWon = 0,
            IsActive = true
        };
        
        SaveSystem.Instance.SaveCareerProgress(newCareer);
        
        // Update user profile
        UserProfile user = SaveSystem.Instance.LoadUserProfile(1);
        user.CurrentCareer = newCareer.CareerId;
        SaveSystem.Instance.SaveUserProfile(user);
        
        currentCareer = newCareer;
        
        // Show team selection
        ShowTeamSelection();
    }
    
    void ShowTeamSelection()
    {
        TeamSelectManager teamSelectManager = FindObjectOfType<TeamSelectManager>();
        if (teamSelectManager != null)
        {
            teamSelectManager.ShowTeamSelectionForCareer(OnTeamSelected);
        }
    }
    
    void OnTeamSelected(TeamSaveData selectedTeam)
    {
        currentCareer.CurrentTeamId = selectedTeam.TeamId;
        currentCareer.CurrentLeague = selectedTeam.League;
        currentCareer.Budget = selectedTeam.Budget;
        
        SaveSystem.Instance.SaveCareerProgress(currentCareer);
        
        // Generate season fixtures
        GenerateSeasonFixtures();
        
        // Load squad
        LoadSquadPlayers();
        
        // Show career dashboard
        LoadCareerDashboard();
    }
    
    void LoadCareerDashboard()
    {
        newCareerPanel.SetActive(false);
        careerDashboardPanel.SetActive(true);
        
        UpdateCareerUI();
        UpdateSquadDisplay();
        UpdateNextMatchInfo();
    }
    
    void UpdateCareerUI()
    {
        TeamSaveData currentTeam = SaveSystem.Instance.GetAllTeams()
            .FirstOrDefault(t => t.TeamId == currentCareer.CurrentTeamId);
        
        if (currentTeam != null)
        {
            seasonText.text = $"Season {currentCareer.Season}";
            weekText.text = $"Week {currentCareer.Week}";
            clubNameText.text = currentTeam.TeamName;
            leaguePositionText.text = $"Position: {currentCareer.LeaguePosition}";
            budgetText.text = $"Budget: ${FormatMoney(currentCareer.Budget)}";
            reputationText.text = $"Reputation: {currentCareer.Reputation}%";
        }
    }
    
    void LoadSquadPlayers()
    {
        squadPlayers = SaveSystem.Instance.GetPlayersByTeam(currentCareer.CurrentTeamId);
    }
    
    void UpdateSquadDisplay()
    {
        // Clear existing player cards
        foreach (Transform child in playerListParent)
        {
            Destroy(child.gameObject);
        }
        
        // Calculate team stats
        int totalOverall = 0;
        int chemistry = 85; // Default chemistry
        int morale = 75; // Default morale
        
        foreach (PlayerSaveData player in squadPlayers)
        {
            // Create player card
            GameObject playerCard = Instantiate(playerCardPrefab, playerListParent);
            PlayerCard card = playerCard.GetComponent<PlayerCard>();
            if (card != null)
            {
                card.SetupPlayerCard(player);
            }
            
            totalOverall += player.Overall;
        }
        
        int teamOverall = squadPlayers.Count > 0 ? totalOverall / squadPlayers.Count : 0;
        
        teamOverallText.text = $"Overall: {teamOverall}";
        teamChemistryText.text = $"Chemistry: {chemistry}%";
        teamMoraleText.text = $"Morale: {morale}%";
    }
    
    void GenerateSeasonFixtures()
    {
        fixtures.Clear();
        
        // Get all teams in the same league
        List<TeamSaveData> leagueTeams = SaveSystem.Instance.GetAllTeams()
            .Where(t => t.League == currentCareer.CurrentLeague)
            .ToList();
        
        // Generate fixtures for the season
        for (int week = 1; week <= 38; week++)
        {
            for (int i = 0; i < leagueTeams.Count; i += 2)
            {
                if (i + 1 < leagueTeams.Count)
                {
                    MatchFixture fixture = new MatchFixture
                    {
                        Week = week,
                        HomeTeamId = leagueTeams[i].TeamId,
                        AwayTeamId = leagueTeams[i + 1].TeamId,
                        Competition = currentCareer.CurrentLeague,
                        IsPlayed = false,
                        Stadium = leagueTeams[i].StadiumName
                    };
                    
                    fixtures.Add(fixture);
                }
            }
        }
    }
    
    void UpdateNextMatchInfo()
    {
        MatchFixture nextMatch = fixtures.FirstOrDefault(f => !f.IsPlayed && 
            (f.HomeTeamId == currentCareer.CurrentTeamId || f.AwayTeamId == currentCareer.CurrentTeamId));
        
        if (nextMatch != null)
        {
            TeamSaveData opponent = SaveSystem.Instance.GetAllTeams()
                .FirstOrDefault(t => t.TeamId == (nextMatch.HomeTeamId == currentCareer.CurrentTeamId ? 
                    nextMatch.AwayTeamId : nextMatch.HomeTeamId));
            
            if (opponent != null)
            {
                string homeAway = nextMatch.HomeTeamId == currentCareer.CurrentTeamId ? "vs" : "at";
                nextMatchText.text = $"Next: {homeAway} {opponent.TeamName}";
            }
        }
        else
        {
            nextMatchText.text = "No upcoming matches";
        }
    }
    
    void PlayNextMatch()
    {
        MatchFixture nextMatch = fixtures.FirstOrDefault(f => !f.IsPlayed && 
            (f.HomeTeamId == currentCareer.CurrentTeamId || f.AwayTeamId == currentCareer.CurrentTeamId));
        
        if (nextMatch != null)
        {
            // Setup match
            TeamSaveData homeTeam = SaveSystem.Instance.GetAllTeams()
                .FirstOrDefault(t => t.TeamId == nextMatch.HomeTeamId);
            TeamSaveData awayTeam = SaveSystem.Instance.GetAllTeams()
                .FirstOrDefault(t => t.TeamId == nextMatch.AwayTeamId);
            
            GameModeManager.Instance.SetGameMode(GameMode.ManagerCareer);
            GameModeManager.Instance.SetSelectedTeams(homeTeam, awayTeam);
            
            // Set match settings
            MatchSettings settings = new MatchSettings
            {
                matchType = MatchType.Career,
                duration = 90,
                injuries = true,
                cards = true,
                stadium = nextMatch.Stadium
            };
            
            GameModeManager.Instance.SetMatchSettings(settings);
            GameModeManager.Instance.StartMatch();
        }
    }
    
    void SimulateNextMatch()
    {
        MatchFixture nextMatch = fixtures.FirstOrDefault(f => !f.IsPlayed && 
            (f.HomeTeamId == currentCareer.CurrentTeamId || f.AwayTeamId == currentCareer.CurrentTeamId));
        
        if (nextMatch != null)
        {
            // Simulate match result
            MatchResult result = SimulateMatch(nextMatch);
            
            // Save result
            SaveSystem.Instance.SaveMatchResult(result);
            
            // Update career progress
            ProcessMatchResult(result);
            
            // Mark fixture as played
            nextMatch.IsPlayed = true;
            nextMatch.Result = result;
            
            // Update UI
            UpdateCareerUI();
            UpdateNextMatchInfo();
            
            // Show result notification
            ShowMatchResultNotification(result);
        }
    }
    
    MatchResult SimulateMatch(MatchFixture fixture)
    {
        TeamSaveData homeTeam = SaveSystem.Instance.GetAllTeams()
            .FirstOrDefault(t => t.TeamId == fixture.HomeTeamId);
        TeamSaveData awayTeam = SaveSystem.Instance.GetAllTeams()
            .FirstOrDefault(t => t.TeamId == fixture.AwayTeamId);
        
        // Calculate team strengths
        float homeStrength = homeTeam.Overall + 5; // Home advantage
        float awayStrength = awayTeam.Overall;
        
        // Add randomness
        homeStrength += Random.Range(-10f, 10f);
        awayStrength += Random.Range(-10f, 10f);
        
        // Simulate goals
        int homeGoals = SimulateGoals(homeStrength);
        int awayGoals = SimulateGoals(awayStrength);
        
        // Determine result
        string result = "Draw";
        if (homeGoals > awayGoals)
        {
            result = fixture.HomeTeamId == currentCareer.CurrentTeamId ? "Win" : "Loss";
        }
        else if (awayGoals > homeGoals)
        {
            result = fixture.AwayTeamId == currentCareer.CurrentTeamId ? "Win" : "Loss";
        }
        
        return new MatchResult
        {
            HomeTeamId = fixture.HomeTeamId,
            AwayTeamId = fixture.AwayTeamId,
            HomeScore = homeGoals,
            AwayScore = awayGoals,
            Date = System.DateTime.Now,
            Competition = fixture.Competition,
            Stadium = fixture.Stadium,
            Attendance = homeTeam.StadiumCapacity - Random.Range(0, homeTeam.StadiumCapacity / 4),
            Result = result
        };
    }
    
    int SimulateGoals(float teamStrength)
    {
        // Simple goal simulation based on team strength
        float goalProbability = teamStrength / 100f;
        int goals = 0;
        
        for (int i = 0; i < 10; i++) // 10 attempts
        {
            if (Random.Range(0f, 1f) < goalProbability * 0.3f)
            {
                goals++;
            }
        }
        
        return Mathf.Min(goals, 6); // Max 6 goals
    }
    
    void ProcessMatchResult(MatchResult result)
    {
        // Update career stats
        if (result.Result == "Win")
        {
            currentCareer.Budget += 2000000; // Win bonus
            currentCareer.Reputation += 2;
        }
        else if (result.Result == "Loss")
        {
            currentCareer.Reputation -= 1;
        }
        
        // Advance week
        currentCareer.Week++;
        
        // Check for end of season
        if (currentCareer.Week > 38)
        {
            EndSeason();
        }
        
        SaveSystem.Instance.SaveCareerProgress(currentCareer);
    }
    
    void EndSeason()
    {
        // End of season processing
        currentCareer.Season++;
        currentCareer.Week = 1;
        
        // Award end of season bonuses
        int finalPosition = CalculateFinalLeaguePosition();
        currentCareer.LeaguePosition = finalPosition;
        
        if (finalPosition == 1)
        {
            currentCareer.TrophiesWon++;
            currentCareer.Budget += 50000000; // League winner bonus
            currentCareer.Reputation += 10;
        }
        else if (finalPosition <= 4)
        {
            currentCareer.Budget += 25000000; // Top 4 bonus
            currentCareer.Reputation += 5;
        }
        
        // Generate new season fixtures
        GenerateSeasonFixtures();
        
        ShowSeasonSummary(finalPosition);
    }
    
    int CalculateFinalLeaguePosition()
    {
        // Calculate final league position based on results
        // This is a simplified calculation
        int wins = 0;
        int draws = 0;
        int losses = 0;
        
        foreach (var fixture in fixtures.Where(f => f.IsPlayed))
        {
            if (fixture.Result != null)
            {
                if (fixture.Result.Result == "Win") wins++;
                else if (fixture.Result.Result == "Draw") draws++;
                else losses++;
            }
        }
        
        int points = wins * 3 + draws;
        
        // Simplified position calculation
        if (points >= 80) return 1;
        else if (points >= 70) return Random.Range(2, 5);
        else if (points >= 60) return Random.Range(5, 10);
        else if (points >= 50) return Random.Range(10, 15);
        else return Random.Range(15, 20);
    }
    
    void ShowSeasonSummary(int position)
    {
        SeasonSummaryManager.Instance.ShowSeasonSummary(currentCareer, position);
    }
    
    void ShowMatchResultNotification(MatchResult result)
    {
        string message = $"Match Result: {result.HomeScore}-{result.AwayScore} ({result.Result})";
        NotificationManager.Instance.ShowNotification(message);
    }
    
    void ShowTransferMarket()
    {
        TransferMarketManager transferMarket = FindObjectOfType<TransferMarketManager>();
        if (transferMarket != null)
        {
            transferMarket.ShowTransferMarket(currentCareer);
        }
    }
    
    void ShowTrainingCenter()
    {
        TrainingCenterManager trainingCenter = FindObjectOfType<TrainingCenterManager>();
        if (trainingCenter != null)
        {
            trainingCenter.ShowTrainingCenter(squadPlayers);
        }
    }
    
    void ShowClubManagement()
    {
        ClubManagementManager clubManager = FindObjectOfType<ClubManagementManager>();
        if (clubManager != null)
        {
            clubManager.ShowClubManagement(currentCareer);
        }
    }
    
    void ShowCalendar()
    {
        CalendarManager calendarManager = FindObjectOfType<CalendarManager>();
        if (calendarManager != null)
        {
            calendarManager.ShowCalendar(fixtures);
        }
    }
    
    void SaveCareer()
    {
        if (currentCareer != null)
        {
            currentCareer.LastSaved = System.DateTime.Now;
            SaveSystem.Instance.SaveCareerProgress(currentCareer);
            ShowNotification("Career saved successfully!");
        }
    }
    
    void ShowNotification(string message)
    {
        NotificationManager.Instance.ShowNotification(message);
    }
    
    string FormatMoney(int amount)
    {
        if (amount >= 1000000)
            return (amount / 1000000f).ToString("F1") + "M";
        if (amount >= 1000)
            return (amount / 1000f).ToString("F1") + "K";
        return amount.ToString();
    }
}

[System.Serializable]
public class MatchFixture
{
    public int Week;
    public int HomeTeamId;
    public int AwayTeamId;
    public string Competition;
    public bool IsPlayed;
    public string Stadium;
    public MatchResult Result;
}

[System.Serializable]
public class LeagueTable
{
    public List<LeagueTableEntry> Entries;
}

[System.Serializable]
public class LeagueTableEntry
{
    public int TeamId;
    public string TeamName;
    public int Played;
    public int Won;
    public int Drawn;
    public int Lost;
    public int GoalsFor;
    public int GoalsAgainst;
    public int GoalDifference;
    public int Points;
    public int Position;
}