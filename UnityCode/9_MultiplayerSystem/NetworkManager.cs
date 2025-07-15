using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance;
    
    [Header("Network Settings")]
    public int maxPlayers = 2;
    public string gameVersion = "1.0";
    public bool autoConnect = true;
    
    [Header("Room Settings")]
    public string roomName = "FootballMatch";
    public bool isPrivateRoom = false;
    public string roomPassword = "";
    
    [Header("Player Info")]
    public string playerName = "Player";
    public TeamData selectedTeam;
    
    [Header("Connection Status")]
    public bool isConnected = false;
    public bool isHost = false;
    public bool isInRoom = false;
    
    private List<NetworkPlayer> connectedPlayers = new List<NetworkPlayer>();
    private Dictionary<string, object> roomProperties = new Dictionary<string, object>();
    
    // Eventos de red
    public System.Action OnConnectedToServer;
    public System.Action OnDisconnectedFromServer;
    public System.Action OnJoinedRoom;
    public System.Action OnLeftRoom;
    public System.Action<NetworkPlayer> OnPlayerJoined;
    public System.Action<NetworkPlayer> OnPlayerLeft;
    public System.Action<string> OnConnectionFailed;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        if (autoConnect)
        {
            ConnectToServer();
        }
    }
    
    public void ConnectToServer()
    {
        Debug.Log("Connecting to server...");
        
        // Simular conexión (en implementación real usar Photon, Mirror, etc.)
        Invoke("SimulateConnection", 2f);
    }
    
    void SimulateConnection()
    {
        isConnected = true;
        OnConnectedToServer?.Invoke();
        Debug.Log("Connected to server!");
    }
    
    public void DisconnectFromServer()
    {
        if (isInRoom)
        {
            LeaveRoom();
        }
        
        isConnected = false;
        OnDisconnectedFromServer?.Invoke();
        Debug.Log("Disconnected from server!");
    }
    
    public void CreateRoom()
    {
        if (!isConnected)
        {
            OnConnectionFailed?.Invoke("Not connected to server");
            return;
        }
        
        roomProperties["teamCount"] = 2;
        roomProperties["gameMode"] = "QuickMatch";
        roomProperties["maxScore"] = 3;
        
        // Simular creación de sala
        isHost = true;
        isInRoom = true;
        
        NetworkPlayer hostPlayer = new NetworkPlayer
        {
            playerId = "host",
            playerName = playerName,
            selectedTeam = selectedTeam,
            isHost = true,
            isReady = false
        };
        
        connectedPlayers.Add(hostPlayer);
        OnJoinedRoom?.Invoke();
        
        Debug.Log("Room created successfully!");
    }
    
    public void JoinRoom(string roomId)
    {
        if (!isConnected)
        {
            OnConnectionFailed?.Invoke("Not connected to server");
            return;
        }
        
        // Simular unión a sala
        isHost = false;
        isInRoom = true;
        
        NetworkPlayer joinedPlayer = new NetworkPlayer
        {
            playerId = "guest",
            playerName = playerName,
            selectedTeam = selectedTeam,
            isHost = false,
            isReady = false
        };
        
        connectedPlayers.Add(joinedPlayer);
        OnJoinedRoom?.Invoke();
        
        Debug.Log($"Joined room: {roomId}");
    }
    
    public void LeaveRoom()
    {
        if (!isInRoom) return;
        
        connectedPlayers.Clear();
        isInRoom = false;
        isHost = false;
        
        OnLeftRoom?.Invoke();
        Debug.Log("Left room");
    }
    
    public void SetPlayerReady(bool ready)
    {
        NetworkPlayer localPlayer = GetLocalPlayer();
        if (localPlayer != null)
        {
            localPlayer.isReady = ready;
            SendPlayerUpdate(localPlayer);
        }
    }
    
    public void SendChatMessage(string message)
    {
        if (!isInRoom) return;
        
        ChatMessage chatMsg = new ChatMessage
        {
            playerId = GetLocalPlayer().playerId,
            playerName = GetLocalPlayer().playerName,
            message = message,
            timestamp = System.DateTime.Now
        };
        
        // Enviar mensaje a todos los jugadores
        BroadcastChatMessage(chatMsg);
    }
    
    void BroadcastChatMessage(ChatMessage message)
    {
        // En implementación real, enviar por red
        Debug.Log($"[{message.playerName}]: {message.message}");
    }
    
    public void SendPlayerInput(PlayerInput input)
    {
        if (!isInRoom) return;
        
        // Enviar input del jugador a otros clientes
        BroadcastPlayerInput(input);
    }
    
    void BroadcastPlayerInput(PlayerInput input)
    {
        // En implementación real, sincronizar input entre clientes
        // Aquí solo registramos el input localmente
    }
    
    public void SendGameEvent(GameEventData eventData)
    {
        if (!isInRoom) return;
        
        // Enviar evento del juego (gol, falta, etc.)
        BroadcastGameEvent(eventData);
    }
    
    void BroadcastGameEvent(GameEventData eventData)
    {
        Debug.Log($"Game Event: {eventData.eventType} - {eventData.description}");
    }
    
    void SendPlayerUpdate(NetworkPlayer player)
    {
        // Enviar actualización del jugador a otros clientes
        Debug.Log($"Player {player.playerName} ready: {player.isReady}");
    }
    
    public NetworkPlayer GetLocalPlayer()
    {
        return connectedPlayers.Find(p => p.playerId == (isHost ? "host" : "guest"));
    }
    
    public List<NetworkPlayer> GetConnectedPlayers()
    {
        return new List<NetworkPlayer>(connectedPlayers);
    }
    
    public bool AllPlayersReady()
    {
        foreach (NetworkPlayer player in connectedPlayers)
        {
            if (!player.isReady) return false;
        }
        return connectedPlayers.Count >= 2;
    }
    
    public void StartNetworkGame()
    {
        if (!isHost)
        {
            Debug.LogWarning("Only host can start the game!");
            return;
        }
        
        if (!AllPlayersReady())
        {
            Debug.LogWarning("Not all players are ready!");
            return;
        }
        
        // Iniciar juego en red
        GameEventData startEvent = new GameEventData
        {
            eventType = "GameStart",
            description = "Match starting...",
            timestamp = System.DateTime.Now
        };
        
        BroadcastGameEvent(startEvent);
        
        // Cargar escena del juego
        UnityEngine.SceneManagement.SceneManager.LoadScene("NetworkGameScene");
    }
    
    public void SynchronizeGameState(GameState state)
    {
        if (!isHost) return;
        
        // Sincronizar estado del juego entre todos los clientes
        GameStateSync syncData = new GameStateSync
        {
            gameTime = Time.time,
            homeScore = 0, // Obtener del GameManager
            awayScore = 0,
            ballPosition = Vector3.zero,
            currentState = state
        };
        
        BroadcastGameState(syncData);
    }
    
    void BroadcastGameState(GameStateSync syncData)
    {
        // En implementación real, enviar estado por red
        Debug.Log($"Game State: {syncData.currentState} - Time: {syncData.gameTime}");
    }
    
    public void HandlePlayerDisconnection(string playerId)
    {
        NetworkPlayer disconnectedPlayer = connectedPlayers.Find(p => p.playerId == playerId);
        if (disconnectedPlayer != null)
        {
            connectedPlayers.Remove(disconnectedPlayer);
            OnPlayerLeft?.Invoke(disconnectedPlayer);
            
            // Si el host se desconecta, migrar host
            if (disconnectedPlayer.isHost && connectedPlayers.Count > 0)
            {
                MigrateHost();
            }
        }
    }
    
    void MigrateHost()
    {
        if (connectedPlayers.Count > 0)
        {
            connectedPlayers[0].isHost = true;
            isHost = connectedPlayers[0].playerId == GetLocalPlayer().playerId;
            
            Debug.Log($"Host migrated to: {connectedPlayers[0].playerName}");
        }
    }
    
    public void SendPing()
    {
        if (!isConnected) return;
        
        // Enviar ping para medir latencia
        float pingTime = Time.time;
        // En implementación real, enviar ping y medir respuesta
    }
    
    public float GetPing()
    {
        // Retornar latencia simulada
        return Random.Range(50f, 150f);
    }
    
    public string GetConnectionQuality()
    {
        float ping = GetPing();
        
        if (ping < 50f) return "Excelente";
        if (ping < 100f) return "Buena";
        if (ping < 200f) return "Regular";
        return "Mala";
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && isInRoom)
        {
            // Notificar pausa a otros jugadores
            SendGameEvent(new GameEventData
            {
                eventType = "PlayerPaused",
                description = $"{playerName} paused the game",
                timestamp = System.DateTime.Now
            });
        }
    }
    
    void OnDestroy()
    {
        DisconnectFromServer();
    }
}

[System.Serializable]
public class NetworkPlayer
{
    public string playerId;
    public string playerName;
    public TeamData selectedTeam;
    public bool isHost;
    public bool isReady;
    public float ping;
    public System.DateTime lastSeen;
}

[System.Serializable]
public class ChatMessage
{
    public string playerId;
    public string playerName;
    public string message;
    public System.DateTime timestamp;
}

[System.Serializable]
public class PlayerInput
{
    public string playerId;
    public Vector2 moveDirection;
    public bool shootPressed;
    public bool passPressed;
    public bool trickPressed;
    public float timestamp;
}

[System.Serializable]
public class GameEventData
{
    public string eventType;
    public string description;
    public Vector3 position;
    public string playerId;
    public System.DateTime timestamp;
}

[System.Serializable]
public class GameStateSync
{
    public float gameTime;
    public int homeScore;
    public int awayScore;
    public Vector3 ballPosition;
    public GameState currentState;
}