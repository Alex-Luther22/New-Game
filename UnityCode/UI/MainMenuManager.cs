using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class MainMenuManager : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject mainMenuPanel;
    public GameObject playMenuPanel;
    public GameObject careerMenuPanel;
    public GameObject settingsPanel;
    public GameObject achievementsPanel;
    public GameObject teamSelectPanel;
    public GameObject trainingPanel;
    public GameObject statisticsPanel;
    public GameObject storePanel;
    
    [Header("Main Menu Buttons")]
    public Button playButton;
    public Button careerButton;
    public Button trainingButton;
    public Button settingsButton;
    public Button achievementsButton;
    public Button statisticsButton;
    public Button storeButton;
    public Button exitButton;
    
    [Header("Play Menu Buttons")]
    public Button quickMatchButton;
    public Button tournamentButton;
    public Button friendlyButton;
    public Button onlineButton;
    
    [Header("Career Menu Buttons")]
    public Button newCareerButton;
    public Button continueCareerButton;
    public Button managerModeButton;
    public Button playerModeButton;
    public Button createClubButton;
    
    [Header("UI Elements")]
    public Text playerNameText;
    public Text levelText;
    public Text coinsText;
    public Text gemsText;
    public Image levelProgressBar;
    public Image playerAvatar;
    
    [Header("Audio")]
    public AudioSource menuAudioSource;
    public AudioClip buttonClickSound;
    public AudioClip backgroundMusic;
    
    [Header("Animation")]
    public Animator menuAnimator;
    public Animator logoAnimator;
    
    private UserProfile currentUser;
    private string currentPanel = "main";
    
    void Start()
    {
        InitializeMainMenu();
        LoadUserData();
        PlayBackgroundMusic();
        ShowMainMenu();
    }
    
    void InitializeMainMenu()
    {
        // Setup button listeners
        playButton.onClick.AddListener(() => ShowPlayMenu());
        careerButton.onClick.AddListener(() => ShowCareerMenu());
        trainingButton.onClick.AddListener(() => ShowTrainingMenu());
        settingsButton.onClick.AddListener(() => ShowSettingsMenu());
        achievementsButton.onClick.AddListener(() => ShowAchievementsMenu());
        statisticsButton.onClick.AddListener(() => ShowStatisticsMenu());
        storeButton.onClick.AddListener(() => ShowStoreMenu());
        exitButton.onClick.AddListener(() => ExitGame());
        
        // Play menu buttons
        quickMatchButton.onClick.AddListener(() => StartQuickMatch());
        tournamentButton.onClick.AddListener(() => ShowTournamentMenu());
        friendlyButton.onClick.AddListener(() => StartFriendlyMatch());
        onlineButton.onClick.AddListener(() => StartOnlineMatch());
        
        // Career menu buttons
        newCareerButton.onClick.AddListener(() => CreateNewCareer());
        continueCareerButton.onClick.AddListener(() => ContinueCareer());
        managerModeButton.onClick.AddListener(() => StartManagerMode());
        playerModeButton.onClick.AddListener(() => StartPlayerMode());
        createClubButton.onClick.AddListener(() => CreateClubMode());
        
        // Hide all panels initially
        HideAllPanels();
    }
    
    void LoadUserData()
    {
        currentUser = SaveSystem.Instance.LoadUserProfile(1);
        if (currentUser == null)
        {
            // Create new user
            currentUser = new UserProfile
            {
                PlayerName = "Player",
                Level = 1,
                Experience = 0,
                Coins = 10000,
                Gems = 100,
                CreatedDate = System.DateTime.Now,
                LastPlayed = System.DateTime.Now,
                TotalMatches = 0,
                TotalWins = 0,
                TotalGoals = 0,
                FavoriteTeam = "Real Madrid",
                CurrentCareer = 0
            };
            SaveSystem.Instance.SaveUserProfile(currentUser);
        }
        
        UpdateUI();
    }
    
    void UpdateUI()
    {
        playerNameText.text = currentUser.PlayerName;
        levelText.text = $"Level {currentUser.Level}";
        coinsText.text = FormatNumber(currentUser.Coins);
        gemsText.text = FormatNumber(currentUser.Gems);
        
        // Update level progress bar
        int expForNextLevel = GetExperienceForLevel(currentUser.Level + 1);
        int expForCurrentLevel = GetExperienceForLevel(currentUser.Level);
        float progress = (float)(currentUser.Experience - expForCurrentLevel) / (expForNextLevel - expForCurrentLevel);
        levelProgressBar.fillAmount = progress;
        
        // Update last played
        currentUser.LastPlayed = System.DateTime.Now;
        SaveSystem.Instance.SaveUserProfile(currentUser);
    }
    
    int GetExperienceForLevel(int level)
    {
        return level * 1000 + (level - 1) * 500;
    }
    
    string FormatNumber(int number)
    {
        if (number >= 1000000)
            return (number / 1000000f).ToString("F1") + "M";
        if (number >= 1000)
            return (number / 1000f).ToString("F1") + "K";
        return number.ToString();
    }
    
    void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && menuAudioSource != null)
        {
            menuAudioSource.clip = backgroundMusic;
            menuAudioSource.loop = true;
            menuAudioSource.Play();
        }
    }
    
    void PlayButtonSound()
    {
        if (buttonClickSound != null && menuAudioSource != null)
        {
            menuAudioSource.PlayOneShot(buttonClickSound);
        }
    }
    
    void HideAllPanels()
    {
        mainMenuPanel.SetActive(false);
        playMenuPanel.SetActive(false);
        careerMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        achievementsPanel.SetActive(false);
        teamSelectPanel.SetActive(false);
        trainingPanel.SetActive(false);
        statisticsPanel.SetActive(false);
        storePanel.SetActive(false);
    }
    
    void ShowMainMenu()
    {
        PlayButtonSound();
        HideAllPanels();
        mainMenuPanel.SetActive(true);
        currentPanel = "main";
        
        if (menuAnimator != null)
        {
            menuAnimator.SetTrigger("ShowMain");
        }
    }
    
    void ShowPlayMenu()
    {
        PlayButtonSound();
        HideAllPanels();
        playMenuPanel.SetActive(true);
        currentPanel = "play";
        
        if (menuAnimator != null)
        {
            menuAnimator.SetTrigger("ShowPlay");
        }
    }
    
    void ShowCareerMenu()
    {
        PlayButtonSound();
        HideAllPanels();
        careerMenuPanel.SetActive(true);
        currentPanel = "career";
        
        // Check if user has existing career
        CareerSaveData existingCareer = SaveSystem.Instance.LoadCareerProgress(currentUser.CurrentCareer);
        continueCareerButton.interactable = existingCareer != null;
        
        if (menuAnimator != null)
        {
            menuAnimator.SetTrigger("ShowCareer");
        }
    }
    
    void ShowTrainingMenu()
    {
        PlayButtonSound();
        HideAllPanels();
        trainingPanel.SetActive(true);
        currentPanel = "training";
        
        TrainingManager trainingManager = FindObjectOfType<TrainingManager>();
        if (trainingManager != null)
        {
            trainingManager.InitializeTraining();
        }
    }
    
    void ShowSettingsMenu()
    {
        PlayButtonSound();
        HideAllPanels();
        settingsPanel.SetActive(true);
        currentPanel = "settings";
        
        SettingsManager settingsManager = FindObjectOfType<SettingsManager>();
        if (settingsManager != null)
        {
            settingsManager.LoadSettings();
        }
    }
    
    void ShowAchievementsMenu()
    {
        PlayButtonSound();
        HideAllPanels();
        achievementsPanel.SetActive(true);
        currentPanel = "achievements";
        
        AchievementManager achievementManager = FindObjectOfType<AchievementManager>();
        if (achievementManager != null)
        {
            achievementManager.LoadAchievements();
        }
    }
    
    void ShowStatisticsMenu()
    {
        PlayButtonSound();
        HideAllPanels();
        statisticsPanel.SetActive(true);
        currentPanel = "statistics";
        
        StatisticsManager statisticsManager = FindObjectOfType<StatisticsManager>();
        if (statisticsManager != null)
        {
            statisticsManager.LoadStatistics();
        }
    }
    
    void ShowStoreMenu()
    {
        PlayButtonSound();
        HideAllPanels();
        storePanel.SetActive(true);
        currentPanel = "store";
        
        StoreManager storeManager = FindObjectOfType<StoreManager>();
        if (storeManager != null)
        {
            storeManager.LoadStore();
        }
    }
    
    void ShowTeamSelectMenu()
    {
        PlayButtonSound();
        HideAllPanels();
        teamSelectPanel.SetActive(true);
        currentPanel = "teamselect";
        
        TeamSelectManager teamSelectManager = FindObjectOfType<TeamSelectManager>();
        if (teamSelectManager != null)
        {
            teamSelectManager.LoadTeams();
        }
    }
    
    void StartQuickMatch()
    {
        PlayButtonSound();
        ShowTeamSelectMenu();
        
        // Set quick match mode
        GameModeManager.Instance.SetGameMode(GameMode.QuickMatch);
    }
    
    void StartFriendlyMatch()
    {
        PlayButtonSound();
        ShowTeamSelectMenu();
        
        // Set friendly mode
        GameModeManager.Instance.SetGameMode(GameMode.Friendly);
    }
    
    void StartOnlineMatch()
    {
        PlayButtonSound();
        // TODO: Implement online multiplayer
        ShowNotificationMessage("Online multiplayer coming soon!");
    }
    
    void ShowTournamentMenu()
    {
        PlayButtonSound();
        
        TournamentManager tournamentManager = FindObjectOfType<TournamentManager>();
        if (tournamentManager != null)
        {
            tournamentManager.ShowTournaments();
        }
    }
    
    void CreateNewCareer()
    {
        PlayButtonSound();
        
        CareerManager careerManager = FindObjectOfType<CareerManager>();
        if (careerManager != null)
        {
            careerManager.CreateNewCareer();
        }
    }
    
    void ContinueCareer()
    {
        PlayButtonSound();
        
        CareerManager careerManager = FindObjectOfType<CareerManager>();
        if (careerManager != null)
        {
            careerManager.ContinueCareer();
        }
    }
    
    void StartManagerMode()
    {
        PlayButtonSound();
        GameModeManager.Instance.SetGameMode(GameMode.ManagerCareer);
        ShowTeamSelectMenu();
    }
    
    void StartPlayerMode()
    {
        PlayButtonSound();
        GameModeManager.Instance.SetGameMode(GameMode.PlayerCareer);
        
        PlayerCreatorManager playerCreator = FindObjectOfType<PlayerCreatorManager>();
        if (playerCreator != null)
        {
            playerCreator.ShowPlayerCreator();
        }
    }
    
    void CreateClubMode()
    {
        PlayButtonSound();
        GameModeManager.Instance.SetGameMode(GameMode.CreateClub);
        
        ClubCreatorManager clubCreator = FindObjectOfType<ClubCreatorManager>();
        if (clubCreator != null)
        {
            clubCreator.ShowClubCreator();
        }
    }
    
    void ExitGame()
    {
        PlayButtonSound();
        
        // Show confirmation dialog
        ConfirmationDialog.Show("Exit Game", "Are you sure you want to exit?", 
            () => {
                #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                #else
                    Application.Quit();
                #endif
            },
            () => {
                // Cancel - do nothing
            });
    }
    
    void ShowNotificationMessage(string message)
    {
        NotificationManager.Instance.ShowNotification(message);
    }
    
    public void AddCoins(int amount)
    {
        currentUser.Coins += amount;
        SaveSystem.Instance.SaveUserProfile(currentUser);
        UpdateUI();
    }
    
    public void AddGems(int amount)
    {
        currentUser.Gems += amount;
        SaveSystem.Instance.SaveUserProfile(currentUser);
        UpdateUI();
    }
    
    public void AddExperience(int amount)
    {
        currentUser.Experience += amount;
        
        // Check for level up
        int newLevel = CalculateLevel(currentUser.Experience);
        if (newLevel > currentUser.Level)
        {
            currentUser.Level = newLevel;
            ShowLevelUpNotification(newLevel);
        }
        
        SaveSystem.Instance.SaveUserProfile(currentUser);
        UpdateUI();
    }
    
    int CalculateLevel(int experience)
    {
        int level = 1;
        while (experience >= GetExperienceForLevel(level + 1))
        {
            level++;
        }
        return level;
    }
    
    void ShowLevelUpNotification(int newLevel)
    {
        LevelUpManager.Instance.ShowLevelUp(newLevel);
    }
    
    void Update()
    {
        // Handle back button on Android
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleBackButton();
        }
    }
    
    void HandleBackButton()
    {
        switch (currentPanel)
        {
            case "main":
                ExitGame();
                break;
            case "play":
            case "career":
            case "training":
            case "settings":
            case "achievements":
            case "statistics":
            case "store":
                ShowMainMenu();
                break;
            case "teamselect":
                ShowPlayMenu();
                break;
        }
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            // Save user data when app is paused
            SaveSystem.Instance.SaveUserProfile(currentUser);
        }
    }
    
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            // Save user data when app loses focus
            SaveSystem.Instance.SaveUserProfile(currentUser);
        }
    }
}