using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MultiplayerUI : MonoBehaviour
{
    [Header("Connection Panel")]
    public GameObject connectionPanel;
    public Button connectButton;
    public Button disconnectButton;
    public Text connectionStatus;
    
    [Header("Room Panel")]
    public GameObject roomPanel;
    public Button createRoomButton;
    public Button joinRoomButton;
    public Button leaveRoomButton;
    public InputField roomNameInput;
    public InputField roomPasswordInput;
    public Toggle privateRoomToggle;
    
    [Header("Lobby Panel")]
    public GameObject lobbyPanel;
    public Transform playerListContent;
    public GameObject playerCardPrefab;
    public Button readyButton;
    public Button startGameButton;
    public Text roomInfoText;
    
    [Header("Chat System")]
    public GameObject chatPanel;
    public InputField chatInput;
    public Button sendChatButton;
    public ScrollRect chatScrollRect;
    public Transform chatContent;
    public GameObject chatMessagePrefab;
    
    [Header("Network Info")]
    public Text pingText;
    public Text playersCountText;
    public Text connectionQualityText;
    
    private NetworkManager networkManager;
    private List<GameObject> playerCards = new List<GameObject>();
    private bool isReady = false;
    
    void Start()
    {
        networkManager = NetworkManager.Instance;
        
        SetupButtons();
        SetupEventListeners();
        
        // Mostrar panel de conexión inicialmente
        ShowConnectionPanel();
        
        // Actualizar UI periódicamente
        InvokeRepeating("UpdateNetworkInfo", 1f, 1f);
    }
    
    void SetupButtons()
    {
        connectButton.onClick.AddListener(ConnectToServer);
        disconnectButton.onClick.AddListener(DisconnectFromServer);
        
        createRoomButton.onClick.AddListener(CreateRoom);
        joinRoomButton.onClick.AddListener(JoinRoom);
        leaveRoomButton.onClick.AddListener(LeaveRoom);
        
        readyButton.onClick.AddListener(ToggleReady);
        startGameButton.onClick.AddListener(StartGame);
        
        sendChatButton.onClick.AddListener(SendChatMessage);
        chatInput.onEndEdit.AddListener(OnChatInputEndEdit);
    }
    
    void SetupEventListeners()
    {
        if (networkManager != null)
        {
            networkManager.OnConnectedToServer += OnConnectedToServer;
            networkManager.OnDisconnectedFromServer += OnDisconnectedFromServer;
            networkManager.OnJoinedRoom += OnJoinedRoom;
            networkManager.OnLeftRoom += OnLeftRoom;
            networkManager.OnPlayerJoined += OnPlayerJoined;
            networkManager.OnPlayerLeft += OnPlayerLeft;
            networkManager.OnConnectionFailed += OnConnectionFailed;
        }
    }
    
    void ConnectToServer()
    {
        if (networkManager != null)
        {
            networkManager.ConnectToServer();
            connectionStatus.text = "Connecting...";
            connectButton.interactable = false;
        }
    }
    
    void DisconnectFromServer()
    {
        if (networkManager != null)
        {
            networkManager.DisconnectFromServer();
        }
    }
    
    void CreateRoom()
    {
        if (networkManager != null)
        {
            networkManager.roomName = roomNameInput.text;
            networkManager.isPrivateRoom = privateRoomToggle.isOn;
            networkManager.roomPassword = roomPasswordInput.text;
            
            networkManager.CreateRoom();
        }
    }
    
    void JoinRoom()
    {
        if (networkManager != null)
        {
            networkManager.JoinRoom(roomNameInput.text);
        }
    }
    
    void LeaveRoom()
    {
        if (networkManager != null)
        {
            networkManager.LeaveRoom();
        }
    }
    
    void ToggleReady()
    {
        isReady = !isReady;
        
        if (networkManager != null)
        {
            networkManager.SetPlayerReady(isReady);
        }
        
        // Actualizar UI del botón
        readyButton.GetComponentInChildren<Text>().text = isReady ? "Cancel Ready" : "Ready";
        readyButton.image.color = isReady ? Color.green : Color.white;
    }
    
    void StartGame()
    {
        if (networkManager != null && networkManager.isHost)
        {
            networkManager.StartNetworkGame();
        }
    }
    
    void SendChatMessage()
    {
        string message = chatInput.text.Trim();
        if (!string.IsNullOrEmpty(message))
        {
            if (networkManager != null)
            {
                networkManager.SendChatMessage(message);
            }
            
            chatInput.text = "";
        }
    }
    
    void OnChatInputEndEdit(string text)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SendChatMessage();
        }
    }
    
    void OnConnectedToServer()
    {
        connectionStatus.text = "Connected";
        connectionStatus.color = Color.green;
        
        connectButton.interactable = false;
        disconnectButton.interactable = true;
        
        ShowRoomPanel();
    }
    
    void OnDisconnectedFromServer()
    {
        connectionStatus.text = "Disconnected";
        connectionStatus.color = Color.red;
        
        connectButton.interactable = true;
        disconnectButton.interactable = false;
        
        ShowConnectionPanel();
    }
    
    void OnJoinedRoom()
    {
        ShowLobbyPanel();
        UpdatePlayerList();
        UpdateRoomInfo();
    }
    
    void OnLeftRoom()
    {
        ShowRoomPanel();
        ClearPlayerList();
        isReady = false;
    }
    
    void OnPlayerJoined(NetworkPlayer player)
    {
        UpdatePlayerList();
        ShowChatMessage($"{player.playerName} joined the room", Color.yellow);
    }
    
    void OnPlayerLeft(NetworkPlayer player)
    {
        UpdatePlayerList();
        ShowChatMessage($"{player.playerName} left the room", Color.yellow);
    }
    
    void OnConnectionFailed(string error)
    {
        connectionStatus.text = $"Connection failed: {error}";
        connectionStatus.color = Color.red;
        
        connectButton.interactable = true;
        disconnectButton.interactable = false;
    }
    
    void ShowConnectionPanel()
    {
        connectionPanel.SetActive(true);
        roomPanel.SetActive(false);
        lobbyPanel.SetActive(false);
    }
    
    void ShowRoomPanel()
    {
        connectionPanel.SetActive(false);
        roomPanel.SetActive(true);
        lobbyPanel.SetActive(false);
    }
    
    void ShowLobbyPanel()
    {
        connectionPanel.SetActive(false);
        roomPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }
    
    void UpdatePlayerList()
    {
        if (networkManager == null) return;
        
        ClearPlayerList();
        
        List<NetworkPlayer> players = networkManager.GetConnectedPlayers();
        
        foreach (NetworkPlayer player in players)
        {
            GameObject playerCard = Instantiate(playerCardPrefab, playerListContent);
            PlayerCardUI cardUI = playerCard.GetComponent<PlayerCardUI>();
            
            if (cardUI != null)
            {
                cardUI.SetupPlayer(player);
            }
            
            playerCards.Add(playerCard);
        }
        
        // Actualizar botón de inicio (solo para host)
        if (networkManager.isHost)
        {
            startGameButton.gameObject.SetActive(true);
            startGameButton.interactable = networkManager.AllPlayersReady();
        }
        else
        {
            startGameButton.gameObject.SetActive(false);
        }
    }
    
    void ClearPlayerList()
    {
        foreach (GameObject card in playerCards)
        {
            Destroy(card);
        }
        playerCards.Clear();
    }
    
    void UpdateRoomInfo()
    {
        if (networkManager != null)
        {
            string roomInfo = $"Room: {networkManager.roomName}\n";
            roomInfo += $"Players: {networkManager.GetConnectedPlayers().Count}/{networkManager.maxPlayers}\n";
            roomInfo += $"Host: {(networkManager.isHost ? "You" : "Other player")}";
            
            roomInfoText.text = roomInfo;
        }
    }
    
    void UpdateNetworkInfo()
    {
        if (networkManager != null && networkManager.isConnected)
        {
            pingText.text = $"Ping: {networkManager.GetPing():F0}ms";
            playersCountText.text = $"Players: {networkManager.GetConnectedPlayers().Count}";
            connectionQualityText.text = $"Quality: {networkManager.GetConnectionQuality()}";
            
            // Actualizar color basado en calidad
            string quality = networkManager.GetConnectionQuality();
            switch (quality)
            {
                case "Excelente":
                    connectionQualityText.color = Color.green;
                    break;
                case "Buena":
                    connectionQualityText.color = Color.yellow;
                    break;
                case "Regular":
                    connectionQualityText.color = Color.orange;
                    break;
                case "Mala":
                    connectionQualityText.color = Color.red;
                    break;
            }
        }
    }
    
    void ShowChatMessage(string message, Color color)
    {
        GameObject chatMessage = Instantiate(chatMessagePrefab, chatContent);
        Text messageText = chatMessage.GetComponent<Text>();
        
        if (messageText != null)
        {
            messageText.text = message;
            messageText.color = color;
        }
        
        // Scroll hacia abajo
        Canvas.ForceUpdateCanvases();
        chatScrollRect.verticalNormalizedPosition = 0f;
    }
    
    void OnDestroy()
    {
        if (networkManager != null)
        {
            networkManager.OnConnectedToServer -= OnConnectedToServer;
            networkManager.OnDisconnectedFromServer -= OnDisconnectedFromServer;
            networkManager.OnJoinedRoom -= OnJoinedRoom;
            networkManager.OnLeftRoom -= OnLeftRoom;
            networkManager.OnPlayerJoined -= OnPlayerJoined;
            networkManager.OnPlayerLeft -= OnPlayerLeft;
            networkManager.OnConnectionFailed -= OnConnectionFailed;
        }
    }
}

public class PlayerCardUI : MonoBehaviour
{
    [Header("Player Info")]
    public Text playerNameText;
    public Text teamNameText;
    public Text statusText;
    public Image readyIndicator;
    public Image hostIndicator;
    
    public void SetupPlayer(NetworkPlayer player)
    {
        playerNameText.text = player.playerName;
        teamNameText.text = player.selectedTeam?.shortName ?? "No Team";
        
        // Mostrar estado
        statusText.text = player.isReady ? "Ready" : "Not Ready";
        statusText.color = player.isReady ? Color.green : Color.red;
        
        // Mostrar indicador de ready
        readyIndicator.gameObject.SetActive(player.isReady);
        
        // Mostrar indicador de host
        hostIndicator.gameObject.SetActive(player.isHost);
    }
}