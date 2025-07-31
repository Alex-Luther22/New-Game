using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace FootballMaster.WebIntegration
{
    [Serializable]
    public class WebAPIConfig
    {
        public string baseUrl = "https://your-backend-url.com/api";
        public string apiKey = "";
        public int timeoutSeconds = 30;
        public bool enableOfflineMode = true;
        public bool enableDataSync = true;
    }

    [Serializable]
    public class UserProfileWeb
    {
        public string id;
        public string username;
        public string email;
        public int level;
        public int experience;
        public string favorite_team_id;
        public List<string> career_teams;
        public List<string> achievements;
        public int total_matches;
        public int total_wins;
        public int total_draws;
        public int total_losses;
        public int total_goals_scored;
        public int total_goals_conceded;
        public string preferred_formation;
        public Dictionary<string, object> control_settings;
        public DateTime created_at;
        public DateTime last_login;
    }

    [Serializable]
    public class TeamWeb
    {
        public string id;
        public string name;
        public string short_name;
        public string country;
        public string league;
        public int overall_rating;
        public int attack_rating;
        public int midfield_rating;
        public int defense_rating;
        public List<PlayerWeb> players;
        public string formation;
        public string primary_color;
        public string secondary_color;
        public string stadium_name;
        public int stadium_capacity;
        public int budget;
        public int prestige;
        public DateTime created_at;
    }

    [Serializable]
    public class PlayerWeb
    {
        public string id;
        public string name;
        public string position;
        public int overall_rating;
        public int pace;
        public int shooting;
        public int passing;
        public int defending;
        public int physicality;
        public int age;
        public string nationality;
        public int value;
        public int stamina;
        public int skill_moves;
        public int weak_foot;
        public bool is_custom;
        public DateTime created_at;
    }

    [Serializable]
    public class MatchResultWeb
    {
        public string id;
        public string home_team_id;
        public string away_team_id;
        public int home_score;
        public int away_score;
        public string stadium_id;
        public string game_mode;
        public int duration;
        public int difficulty;
        public string weather;
        public string time_of_day;
        public bool completed;
        public string player_id;
        public List<Dictionary<string, object>> match_events;
        public Dictionary<string, object> statistics;
        public DateTime created_at;
    }

    public class UnityWebIntegration : MonoBehaviour
    {
        [Header("Configuration")]
        public WebAPIConfig config;
        
        [Header("Authentication")]
        public string currentUserId;
        public string authToken;
        public bool isLoggedIn = false;
        
        [Header("Sync Settings")]
        public bool enableAutoSync = true;
        public float syncIntervalMinutes = 5f;
        public bool syncOnlyOnWiFi = true;
        
        [Header("Offline Mode")]
        public bool isOfflineMode = false;
        public int offlineDataMaxSize = 1000; // Max offline operations
        
        // Events
        public event System.Action<UserProfileWeb> OnUserProfileLoaded;
        public event System.Action<List<TeamWeb>> OnTeamsLoaded;
        public event System.Action<string> OnDataSyncCompleted;
        public event System.Action<string> OnError;
        
        // Internal
        private Queue<Dictionary<string, object>> offlineOperations = new Queue<Dictionary<string, object>>();
        private UserProfileWeb cachedUserProfile;
        private List<TeamWeb> cachedTeams = new List<TeamWeb>();
        private bool isSyncing = false;
        
        private void Start()
        {
            InitializeIntegration();
            
            if (enableAutoSync)
            {
                InvokeRepeating(nameof(PerformAutoSync), syncIntervalMinutes * 60f, syncIntervalMinutes * 60f);
            }
        }
        
        void InitializeIntegration()
        {
            // Load cached data from PlayerPrefs
            LoadCachedData();
            
            // Check internet connectivity
            CheckConnectivity();
            
            Debug.Log("Unity-Web Integration initialized");
        }
        
        void LoadCachedData()
        {
            // Load cached user profile
            string cachedProfile = PlayerPrefs.GetString("CachedUserProfile", "");
            if (!string.IsNullOrEmpty(cachedProfile))
            {
                try
                {
                    cachedUserProfile = JsonConvert.DeserializeObject<UserProfileWeb>(cachedProfile);
                    if (cachedUserProfile != null)
                    {
                        currentUserId = cachedUserProfile.id;
                        isLoggedIn = true;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to load cached profile: {e.Message}");
                }
            }
            
            // Load cached teams
            string cachedTeamsData = PlayerPrefs.GetString("CachedTeams", "");
            if (!string.IsNullOrEmpty(cachedTeamsData))
            {
                try
                {
                    cachedTeams = JsonConvert.DeserializeObject<List<TeamWeb>>(cachedTeamsData);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to load cached teams: {e.Message}");
                }
            }
        }
        
        void CheckConnectivity()
        {
            isOfflineMode = Application.internetReachability == NetworkReachability.NotReachable;
            
            if (isOfflineMode && config.enableOfflineMode)
            {
                Debug.Log("Starting in offline mode");
            }
            else if (isOfflineMode)
            {
                OnError?.Invoke("No internet connection and offline mode is disabled");
            }
        }
        
        // Authentication
        public void LoginUser(string username, string password)
        {
            StartCoroutine(LoginUserCoroutine(username, password));
        }
        
        IEnumerator LoginUserCoroutine(string username, string password)
        {
            var loginData = new Dictionary<string, string>
            {
                ["username"] = username,
                ["password"] = password
            };
            
            string jsonData = JsonConvert.SerializeObject(loginData);
            
            using (UnityWebRequest request = new UnityWebRequest($"{config.baseUrl}/auth/login", "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.timeout = config.timeoutSeconds;
                
                yield return request.SendWebRequest();
                
                if (request.result == UnityWebRequest.Result.Success)
                {
                    var response = JsonConvert.DeserializeObject<Dictionary<string, object>>(request.downloadHandler.text);
                    
                    if (response.ContainsKey("token") && response.ContainsKey("user"))
                    {
                        authToken = response["token"].ToString();
                        var userJson = JsonConvert.SerializeObject(response["user"]);
                        cachedUserProfile = JsonConvert.DeserializeObject<UserProfileWeb>(userJson);
                        
                        currentUserId = cachedUserProfile.id;
                        isLoggedIn = true;
                        
                        // Cache the profile
                        PlayerPrefs.SetString("CachedUserProfile", userJson);
                        PlayerPrefs.SetString("AuthToken", authToken);
                        PlayerPrefs.Save();
                        
                        OnUserProfileLoaded?.Invoke(cachedUserProfile);
                        
                        // Load additional data
                        LoadUserTeams();
                    }
                }
                else
                {
                    OnError?.Invoke($"Login failed: {request.error}");
                }
            }
        }
        
        public void CreateUser(string username, string email, string password)
        {
            StartCoroutine(CreateUserCoroutine(username, email, password));
        }
        
        IEnumerator CreateUserCoroutine(string username, string email, string password)
        {
            var userData = new Dictionary<string, string>
            {
                ["username"] = username,
                ["email"] = email,
                ["password"] = password
            };
            
            string jsonData = JsonConvert.SerializeObject(userData);
            
            using (UnityWebRequest request = new UnityWebRequest($"{config.baseUrl}/users", "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.timeout = config.timeoutSeconds;
                
                yield return request.SendWebRequest();
                
                if (request.result == UnityWebRequest.Result.Success)
                {
                    var response = JsonConvert.DeserializeObject<Dictionary<string, object>>(request.downloadHandler.text);
                    
                    if (response.ContainsKey("user_id"))
                    {
                        // Auto-login after creation
                        LoginUser(username, password);
                    }
                }
                else
                {
                    OnError?.Invoke($"User creation failed: {request.error}");
                }
            }
        }
        
        // Data Loading
        public void LoadUserTeams()
        {
            if (isOfflineMode)
            {
                OnTeamsLoaded?.Invoke(cachedTeams);
                return;
            }
            
            StartCoroutine(LoadUserTeamsCoroutine());
        }
        
        IEnumerator LoadUserTeamsCoroutine()
        {
            using (UnityWebRequest request = UnityWebRequest.Get($"{config.baseUrl}/teams"))
            {
                request.SetRequestHeader("Authorization", $"Bearer {authToken}");
                request.timeout = config.timeoutSeconds;
                
                yield return request.SendWebRequest();
                
                if (request.result == UnityWebRequest.Result.Success)
                {
                    var teamsData = JsonConvert.DeserializeObject<List<TeamWeb>>(request.downloadHandler.text);
                    cachedTeams = teamsData;
                    
                    // Cache the teams
                    PlayerPrefs.SetString("CachedTeams", request.downloadHandler.text);
                    PlayerPrefs.Save();
                    
                    OnTeamsLoaded?.Invoke(cachedTeams);
                }
                else
                {
                    OnError?.Invoke($"Failed to load teams: {request.error}");
                    // Fallback to cached data
                    OnTeamsLoaded?.Invoke(cachedTeams);
                }
            }
        }
        
        public void LoadTeamById(string teamId, System.Action<TeamWeb> callback)
        {
            // Check cache first
            var cachedTeam = cachedTeams.Find(t => t.id == teamId);
            if (cachedTeam != null && isOfflineMode)
            {
                callback?.Invoke(cachedTeam);
                return;
            }
            
            StartCoroutine(LoadTeamByIdCoroutine(teamId, callback));
        }
        
        IEnumerator LoadTeamByIdCoroutine(string teamId, System.Action<TeamWeb> callback)
        {
            using (UnityWebRequest request = UnityWebRequest.Get($"{config.baseUrl}/teams/{teamId}"))
            {
                request.SetRequestHeader("Authorization", $"Bearer {authToken}");
                request.timeout = config.timeoutSeconds;
                
                yield return request.SendWebRequest();
                
                if (request.result == UnityWebRequest.Result.Success)
                {
                    var teamData = JsonConvert.DeserializeObject<TeamWeb>(request.downloadHandler.text);
                    
                    // Update cache
                    var existingIndex = cachedTeams.FindIndex(t => t.id == teamId);
                    if (existingIndex >= 0)
                    {
                        cachedTeams[existingIndex] = teamData;
                    }
                    else
                    {
                        cachedTeams.Add(teamData);
                    }
                    
                    callback?.Invoke(teamData);
                }
                else
                {
                    OnError?.Invoke($"Failed to load team: {request.error}");
                    callback?.Invoke(null);
                }
            }
        }
        
        // Match Data Submission
        public void SubmitMatchResult(MatchResultWeb matchResult)
        {
            if (isOfflineMode)
            {
                QueueOfflineOperation("submit_match", matchResult);
                return;
            }
            
            StartCoroutine(SubmitMatchResultCoroutine(matchResult));
        }
        
        IEnumerator SubmitMatchResultCoroutine(MatchResultWeb matchResult)
        {
            string jsonData = JsonConvert.SerializeObject(matchResult);
            
            using (UnityWebRequest request = new UnityWebRequest($"{config.baseUrl}/matches", "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", $"Bearer {authToken}");
                request.timeout = config.timeoutSeconds;
                
                yield return request.SendWebRequest();
                
                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("Match result submitted successfully");
                    
                    // Update user profile stats
                    UpdateUserProfileStats(matchResult);
                }
                else
                {
                    OnError?.Invoke($"Failed to submit match result: {request.error}");
                    
                    // Queue for retry
                    QueueOfflineOperation("submit_match", matchResult);
                }
            }
        }
        
        void UpdateUserProfileStats(MatchResultWeb matchResult)
        {
            if (cachedUserProfile != null)
            {
                cachedUserProfile.total_matches++;
                
                // Assuming player is always home team for simplicity
                if (matchResult.home_score > matchResult.away_score)
                {
                    cachedUserProfile.total_wins++;
                }
                else if (matchResult.home_score == matchResult.away_score)
                {
                    cachedUserProfile.total_draws++;
                }
                else
                {
                    cachedUserProfile.total_losses++;
                }
                
                cachedUserProfile.total_goals_scored += matchResult.home_score;
                cachedUserProfile.total_goals_conceded += matchResult.away_score;
                
                // Add experience points
                int xpGain = 50; // Base XP
                xpGain += matchResult.home_score * 10; // Bonus for goals
                if (matchResult.home_score > matchResult.away_score) xpGain += 100; // Win bonus
                
                cachedUserProfile.experience += xpGain;
                
                // Level up check
                int requiredXP = cachedUserProfile.level * 1000;
                if (cachedUserProfile.experience >= requiredXP)
                {
                    cachedUserProfile.level++;
                    Debug.Log($"Level up! New level: {cachedUserProfile.level}");
                }
                
                // Save updated profile
                string updatedProfileJson = JsonConvert.SerializeObject(cachedUserProfile);
                PlayerPrefs.SetString("CachedUserProfile", updatedProfileJson);
                PlayerPrefs.Save();
            }
        }
        
        public void UnlockAchievement(string achievementId)
        {
            if (isOfflineMode)
            {
                QueueOfflineOperation("unlock_achievement", new { achievement_id = achievementId });
                return;
            }
            
            StartCoroutine(UnlockAchievementCoroutine(achievementId));
        }
        
        IEnumerator UnlockAchievementCoroutine(string achievementId)
        {
            using (UnityWebRequest request = new UnityWebRequest($"{config.baseUrl}/users/{currentUserId}/achievements/{achievementId}", "POST"))
            {
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Authorization", $"Bearer {authToken}");
                request.timeout = config.timeoutSeconds;
                
                yield return request.SendWebRequest();
                
                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log($"Achievement unlocked: {achievementId}");
                    
                    // Update cached profile
                    if (cachedUserProfile != null && !cachedUserProfile.achievements.Contains(achievementId))
                    {
                        cachedUserProfile.achievements.Add(achievementId);
                        
                        string updatedProfileJson = JsonConvert.SerializeObject(cachedUserProfile);
                        PlayerPrefs.SetString("CachedUserProfile", updatedProfileJson);
                        PlayerPrefs.Save();
                    }
                }
                else
                {
                    OnError?.Invoke($"Failed to unlock achievement: {request.error}");
                    QueueOfflineOperation("unlock_achievement", new { achievement_id = achievementId });
                }
            }
        }
        
        // Offline Operations
        void QueueOfflineOperation(string operationType, object data)
        {
            if (offlineOperations.Count >= offlineDataMaxSize)
            {
                offlineOperations.Dequeue(); // Remove oldest operation
            }
            
            var operation = new Dictionary<string, object>
            {
                ["type"] = operationType,
                ["data"] = data,
                ["timestamp"] = DateTime.UtcNow.ToString("O")
            };
            
            offlineOperations.Enqueue(operation);
            
            // Save offline operations
            SaveOfflineOperations();
            
            Debug.Log($"Queued offline operation: {operationType}");
        }
        
        void SaveOfflineOperations()
        {
            var operationsList = new List<Dictionary<string, object>>(offlineOperations.ToArray());
            string json = JsonConvert.SerializeObject(operationsList);
            PlayerPrefs.SetString("OfflineOperations", json);
            PlayerPrefs.Save();
        }
        
        void LoadOfflineOperations()
        {
            string json = PlayerPrefs.GetString("OfflineOperations", "");
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    var operationsList = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json);
                    offlineOperations = new Queue<Dictionary<string, object>>(operationsList);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to load offline operations: {e.Message}");
                }
            }
        }
        
        // Sync Operations
        void PerformAutoSync()
        {
            if (!enableAutoSync || isSyncing || isOfflineMode) return;
            
            if (syncOnlyOnWiFi && Application.internetReachability != NetworkReachability.ReachableViaLocalAreaNetwork)
            {
                return;
            }
            
            StartCoroutine(SyncOfflineOperations());
        }
        
        public void ForceSyncNow()
        {
            if (!isSyncing)
            {
                StartCoroutine(SyncOfflineOperations());
            }
        }
        
        IEnumerator SyncOfflineOperations()
        {
            isSyncing = true;
            int syncedOperations = 0;
            int failedOperations = 0;
            
            while (offlineOperations.Count > 0)
            {
                var operation = offlineOperations.Dequeue();
                bool success = false;
                
                switch (operation["type"].ToString())
                {
                    case "submit_match":
                        var matchData = JsonConvert.DeserializeObject<MatchResultWeb>(operation["data"].ToString());
                        yield return StartCoroutine(SubmitMatchResultCoroutine(matchData));
                        success = true; // Simplified - should check actual result
                        break;
                        
                    case "unlock_achievement":
                        var achievementData = (Dictionary<string, object>)operation["data"];
                        yield return StartCoroutine(UnlockAchievementCoroutine(achievementData["achievement_id"].ToString()));
                        success = true;
                        break;
                }
                
                if (success)
                {
                    syncedOperations++;
                }
                else
                {
                    failedOperations++;
                    // Re-queue failed operation
                    offlineOperations.Enqueue(operation);
                }
                
                yield return new WaitForSeconds(0.5f); // Prevent API spam
            }
            
            SaveOfflineOperations();
            isSyncing = false;
            
            string syncResult = $"Sync completed: {syncedOperations} successful, {failedOperations} failed";
            Debug.Log(syncResult);
            OnDataSyncCompleted?.Invoke(syncResult);
        }
        
        // Utility Methods
        public bool IsConnectedToInternet()
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }
        
        public UserProfileWeb GetCachedUserProfile()
        {
            return cachedUserProfile;
        }
        
        public List<TeamWeb> GetCachedTeams()
        {
            return cachedTeams;
        }
        
        public int GetPendingOperationsCount()
        {
            return offlineOperations.Count;
        }
        
        public void ClearCache()
        {
            PlayerPrefs.DeleteKey("CachedUserProfile");
            PlayerPrefs.DeleteKey("CachedTeams");
            PlayerPrefs.DeleteKey("OfflineOperations");
            PlayerPrefs.DeleteKey("AuthToken");
            PlayerPrefs.Save();
            
            cachedUserProfile = null;
            cachedTeams.Clear();
            offlineOperations.Clear();
            
            Debug.Log("Cache cleared");
        }
        
        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                SaveOfflineOperations();
            }
            else
            {
                LoadOfflineOperations();
                CheckConnectivity();
            }
        }
        
        void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                SaveOfflineOperations();
            }
            else
            {
                CheckConnectivity();
            }
        }
    }
}