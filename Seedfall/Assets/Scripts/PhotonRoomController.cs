using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using TMPro;

public class PhotonRoomController : MonoBehaviourPunCallbacks
{
    [Header("Room Configuration")]
    public static PhotonRoomController Instance;
    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private bool autoSyncScene = true;
    [SerializeField] private int minPlayersToStart = 1;
    [SerializeField] private float roomTimeoutWhenEmpty = 60f;

    [Header("Game Settings")]
    [SerializeField] private string[] gameModes = { "Standard", "Team Deathmatch", "Capture the Flag" };
    [SerializeField] private string defaultGameMode = "Standard";
    [SerializeField] private int defaultTimeLimit = 300; // 5 minutes in seconds
    [SerializeField] private int defaultScoreLimit = 10;

    // Room property keys
    private const string GAME_MODE_KEY = "gameMode";
    private const string TIME_LIMIT_KEY = "timeLimit";
    private const string SCORE_LIMIT_KEY = "scoreLimit";
    private const string GAME_STARTED_KEY = "gameStarted";
    private const string MAP_NAME_KEY = "mapName";

    // Property change event codes
    private const byte GAME_SETTINGS_CHANGED = 1;
    private const byte PLAYER_READY_CHANGED = 2;

    // Local tracking of players ready status
    private Dictionary<string, bool> playersReadyStatus = new Dictionary<string, bool>();
    private bool isGameStarted = false;

    #region Unity Methods

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Set up automatic scene synchronization
        PhotonNetwork.AutomaticallySyncScene = autoSyncScene;
    }

    void Start()
    {
        // Update UI if we're already in a room
        if (PhotonNetwork.InRoom)
        {
            UpdateRoomInfoUI();
            InitializeRoomProperties();
        }
    }

    void Update()
    {
        // Update player count UI if we're in a room
        if (PhotonNetwork.InRoom && playerCountText != null)
        {
            playerCountText.text = $"Players: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
        }
    }

    #endregion

    #region Room Management

    // Initialize default room properties if we're the master client
    private void InitializeRoomProperties()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        // Get existing properties or create new ones
        Hashtable roomProps = PhotonNetwork.CurrentRoom.CustomProperties;
        bool propsChanged = false;

        // Set default game mode if not already set
        if (!roomProps.ContainsKey(GAME_MODE_KEY))
        {
            roomProps[GAME_MODE_KEY] = defaultGameMode;
            propsChanged = true;
        }

        // Set default time limit if not already set
        if (!roomProps.ContainsKey(TIME_LIMIT_KEY))
        {
            roomProps[TIME_LIMIT_KEY] = defaultTimeLimit;
            propsChanged = true;
        }

        // Set default score limit if not already set
        if (!roomProps.ContainsKey(SCORE_LIMIT_KEY))
        {
            roomProps[SCORE_LIMIT_KEY] = defaultScoreLimit;
            propsChanged = true;
        }

        // Set game started flag if not already set
        if (!roomProps.ContainsKey(GAME_STARTED_KEY))
        {
            roomProps[GAME_STARTED_KEY] = false;
            propsChanged = true;
        }

        // Set map name based on current scene if not already set
        if (!roomProps.ContainsKey(MAP_NAME_KEY))
        {
            roomProps[MAP_NAME_KEY] = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            propsChanged = true;
        }

        // Only update if properties changed
        if (propsChanged)
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);
        }
    }

    // Update room settings (master client only)
    public void UpdateRoomSettings(string gameMode, int timeLimit, int scoreLimit)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogWarning("Only the host can change room settings!");
            return;
        }

        Hashtable roomProps = new Hashtable
        {
            { GAME_MODE_KEY, gameMode },
            { TIME_LIMIT_KEY, timeLimit },
            { SCORE_LIMIT_KEY, scoreLimit }
        };

        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);

        // Notify all players about the change
        RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(GAME_SETTINGS_CHANGED, null, eventOptions, sendOptions);
    }

    // Start the game (master client only)
    public void StartGame()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogWarning("Only the host can start the game!");
            return;
        }

        // Check if we have enough players
        if (PhotonNetwork.CurrentRoom.PlayerCount < minPlayersToStart)
        {
            Debug.LogWarning($"Need at least {minPlayersToStart} players to start!");
            return;
        }

        // Check if all players are ready
        if (!AreAllPlayersReady())
        {
            Debug.LogWarning("Not all players are ready!");
            return;
        }

        // Set game started property
        Hashtable roomProps = new Hashtable
        {
            { GAME_STARTED_KEY, true }
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);

        // Load the game scene
        string gameScene = GetAppropriateGameScene();
        PhotonNetwork.LoadLevel(gameScene);
    }

    // Choose game scene based on player count and other factors
    private string GetAppropriateGameScene()
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        playerCount = Mathf.Clamp(playerCount, 1, 4); // Limit to available scenes

        // Get game mode
        string gameMode = (string)PhotonNetwork.CurrentRoom.CustomProperties[GAME_MODE_KEY];

        // For now, just return the basic room scene based on player count
        return $"Room for {playerCount}";

        // In a more advanced implementation, you could combine game mode and player count:
        // return $"{gameMode}_Room_{playerCount}";
    }

    // Helper method to check if all players are ready
    private bool AreAllPlayersReady()
    {
        // If no players have set ready status yet, return false
        if (playersReadyStatus.Count == 0)
            return false;

        // If not all players have set ready status, return false
        if (playersReadyStatus.Count < PhotonNetwork.CurrentRoom.PlayerCount)
            return false;

        // Check if any player is not ready
        foreach (bool isReady in playersReadyStatus.Values)
        {
            if (!isReady)
                return false;
        }

        return true;
    }

    // Set local player ready status
    public void SetPlayerReady(bool isReady)
    {
        // Store ready status in player properties
        Hashtable playerProps = new Hashtable
        {
            { "IsReady", isReady }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);

        // Update local tracking
        string playerId = PhotonNetwork.LocalPlayer.UserId;
        playersReadyStatus[playerId] = isReady;

        // Notify all players
        object[] content = new object[] { PhotonNetwork.LocalPlayer.UserId, isReady };
        RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(PLAYER_READY_CHANGED, content, eventOptions, sendOptions);
    }

    // Update the room info UI
    private void UpdateRoomInfoUI()
    {
        if (roomNameText != null && PhotonNetwork.InRoom)
        {
            roomNameText.text = $"Room: {PhotonNetwork.CurrentRoom.Name}";
        }

        if (playerCountText != null && PhotonNetwork.InRoom)
        {
            playerCountText.text = $"Players: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
        }
    }

    #endregion

    #region Photon Callbacks

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined room: {PhotonNetwork.CurrentRoom.Name}");

        // Update UI
        UpdateRoomInfoUI();

        // Initialize properties if master client
        if (PhotonNetwork.IsMasterClient)
        {
            InitializeRoomProperties();
        }

        // Reset ready status tracking
        playersReadyStatus.Clear();

        // Initialize ready status of existing players
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue("IsReady", out object isReadyObj))
            {
                playersReadyStatus[player.UserId] = (bool)isReadyObj;
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player {newPlayer.NickName} entered the room");

        // Update UI
        UpdateRoomInfoUI();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"Player {otherPlayer.NickName} left the room");

        // Update UI
        UpdateRoomInfoUI();

        // Remove player from ready tracking
        if (playersReadyStatus.ContainsKey(otherPlayer.UserId))
        {
            playersReadyStatus.Remove(otherPlayer.UserId);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        // Check if ready status changed
        if (changedProps.TryGetValue("IsReady", out object isReadyObj))
        {
            bool isReady = (bool)isReadyObj;
            playersReadyStatus[targetPlayer.UserId] = isReady;

            Debug.Log($"Player {targetPlayer.NickName} is now {(isReady ? "ready" : "not ready")}");

            // Update UI or notify GameManager
            // You could implement an event system here to notify other components
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        // Check if game started status changed
        if (propertiesThatChanged.TryGetValue(GAME_STARTED_KEY, out object gameStartedObj))
        {
            isGameStarted = (bool)gameStartedObj;

            if (isGameStarted)
            {
                Debug.Log("Game is starting!");
                // Any additional logic when game starts
            }
        }

        // Handle other property changes (game mode, time limit, etc.)
        if (propertiesThatChanged.ContainsKey(GAME_MODE_KEY) ||
            propertiesThatChanged.ContainsKey(TIME_LIMIT_KEY) ||
            propertiesThatChanged.ContainsKey(SCORE_LIMIT_KEY))
        {
            Debug.Log("Room settings updated");
            // Update UI or notify GameManager
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log($"New master client: {newMasterClient.NickName}");

        // If we are the new master client, initialize room properties
        if (newMasterClient.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            InitializeRoomProperties();
        }
    }

    #endregion

    #region Public Methods

    // Get current game mode
    public string GetGameMode()
    {
        if (PhotonNetwork.InRoom &&
            PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(GAME_MODE_KEY, out object gameModeObj))
        {
            return (string)gameModeObj;
        }
        return defaultGameMode;
    }

    // Get current time limit
    public int GetTimeLimit()
    {
        if (PhotonNetwork.InRoom &&
            PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(TIME_LIMIT_KEY, out object timeLimitObj))
        {
            return (int)timeLimitObj;
        }
        return defaultTimeLimit;
    }

    // Get current score limit
    public int GetScoreLimit()
    {
        if (PhotonNetwork.InRoom &&
            PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(SCORE_LIMIT_KEY, out object scoreLimitObj))
        {
            return (int)scoreLimitObj;
        }
        return defaultScoreLimit;
    }

    // Check if player is ready
    public bool IsPlayerReady(string userId)
    {
        if (playersReadyStatus.TryGetValue(userId, out bool isReady))
        {
            return isReady;
        }
        return false;
    }

    // Check if current player is room host
    public bool IsLocalPlayerHost()
    {
        return PhotonNetwork.IsMasterClient;
    }

    #endregion
}