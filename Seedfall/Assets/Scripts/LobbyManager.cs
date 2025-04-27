using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using ExitGames.Client.Photon;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("Connection")]
    [SerializeField] private TextMeshProUGUI connectionStatusText;
    [SerializeField] private Button joinButton;
    [SerializeField] private float reconnectDelay = 5f;
    private bool isConnecting = false;
    private float reconnectTimer = 0f;

    [Header("Room Creation")]
    [SerializeField] private TMP_InputField roomNameInput;
    [SerializeField] private TMP_Dropdown maxPlayersDropdown;
    [SerializeField] private Toggle isPrivateToggle;

    [Header("Room Listing")]
    [SerializeField] private GameObject roomListContent;
    [SerializeField] private GameObject roomItemPrefab;
    [SerializeField] private Button refreshButton;
    [SerializeField] private TextMeshProUGUI noRoomsText;
    [SerializeField] private float refreshCooldown = 2f;

    private List<RoomInfo> cachedRoomList = new List<RoomInfo>();
    private List<GameObject> roomItemsList = new List<GameObject>();
    private float nextRefreshTime = 0f;
    private const string GAME_MODE_KEY = "gameMode";
    private const string ROOM_CREATED_TIME_KEY = "createdAt";

    #region Unity Methods

    void Start()
    {
        // Set up UI elements
        SetupUIElements();

        // Connect to Photon if not already connected
        if (PhotonNetwork.NetworkClientState == ClientState.Disconnected)
        {
            ConnectToServer();
        }
        else if (PhotonNetwork.IsConnected)
        {
            OnConnectedToMaster();
        }

        // Add listeners to UI elements
        AddButtonListeners();
    }

    void Update()
    {
        // Update connection status text
        UpdateConnectionUI();

        // Handle reconnection attempts
        HandleReconnection();

        // Refresh room list periodically
        HandleRoomListRefresh();
    }

    void OnDestroy()
    {
        // Remove listeners when destroyed
        RemoveButtonListeners();
    }

    #endregion

    #region UI Methods

    private void SetupUIElements()
    {
        // Initialize UI elements and set initial states
        // Set up dropdown options for max players
        // Disable buttons until connected
    }

    private void AddButtonListeners()
    {
        // Add listeners to buttons
    }

    private void RemoveButtonListeners()
    {
        // Remove listeners from buttons
    }

    private void UpdateConnectionUI()
    {
        // Update connection status text based on Photon state
        if (connectionStatusText != null)
        {
            string statusText = "Status: " + PhotonNetwork.NetworkClientState.ToString();

            // Add room info if in a room
            if (PhotonNetwork.InRoom)
            {
                statusText += $"\nRoom: {PhotonNetwork.CurrentRoom.Name} ({PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers})";
            }

            connectionStatusText.text = statusText;
        }

        // Update button interactability based on connection state
        if (joinButton != null)
        {
            joinButton.interactable = PhotonNetwork.IsConnectedAndReady;
        }
    }

    private void ClearRoomList()
    {
        // Destroy all room items and clear the list
        foreach (GameObject roomItem in roomItemsList)
        {
            Destroy(roomItem);
        }
        roomItemsList.Clear();
    }

    private void UpdateRoomList(List<RoomInfo> roomList)
    {
        // Clear existing room items
        ClearRoomList();

        // Show "no rooms" text if list is empty
        if (roomList.Count == 0)
        {
            if (noRoomsText != null)
                noRoomsText.gameObject.SetActive(true);
        }
        else
        {
            // Hide "no rooms" text
            if (noRoomsText != null)
                noRoomsText.gameObject.SetActive(false);

            // Create new room items for each available room
            foreach (RoomInfo info in roomList)
            {
                // Only show rooms that are open and visible
                if (info.IsOpen && info.IsVisible)
                {
                    CreateRoomItem(info);
                }
            }
        }
    }


    private void CreateRoomItem(RoomInfo info)
    {
        // Instantiate room item prefab
        GameObject roomItem = Instantiate(roomItemPrefab, roomListContent.transform);
        roomItemsList.Add(roomItem);

        // Set room name and player count
        RoomItem roomItemScript = roomItem.GetComponent<RoomItem>();
        if (roomItemScript != null)
        {
            roomItemScript.Initialize(info.Name, $"{info.PlayerCount}/{info.MaxPlayers}");

            // Add click listener
            roomItemScript.SetJoinCallback(() => JoinRoom(info.Name));
        }
    }


    #endregion

    #region Photon Connection Methods

    private void ConnectToServer()
    {
        isConnecting = true;
        connectionStatusText.text = "Connecting to server...";

        // Disable buttons during connection
        if (joinButton != null)
            joinButton.interactable = false;

        // Connect to Photon using settings from the inspector
        PhotonNetwork.ConnectUsingSettings();
    }

    public void JoinOrCreateRoom()
    {
        // Validate input first
        if (string.IsNullOrEmpty(roomNameInput.text) || roomNameInput.text == "Input Room Name")
        {
            ShowErrorMessage("Please enter a valid room name");
            return;
        }

        if (!PhotonNetwork.IsConnectedAndReady)
        {
            ShowErrorMessage("Not connected to server. Attempting to reconnect...");
            ConnectToServer();
            return;
        }

        // Set up room options
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = GetMaxPlayersFromDropdown(),
            IsVisible = !isPrivateToggle.isOn,
            PublishUserId = true // Needed to identify players
        };

        // Add custom room properties
        ExitGames.Client.Photon.Hashtable customProps = new ExitGames.Client.Photon.Hashtable
        {
            { ROOM_CREATED_TIME_KEY, System.DateTime.UtcNow.ToString() },
            // Add more custom properties as needed
        };

        roomOptions.CustomRoomProperties = customProps;
        roomOptions.CustomRoomPropertiesForLobby = new string[] { ROOM_CREATED_TIME_KEY };

        // Try to join existing room, or create a new one if it doesn't exist
        connectionStatusText.text = "Joining room...";
        PhotonNetwork.JoinOrCreateRoom(roomNameInput.text, roomOptions, TypedLobby.Default);
    }

    private void ShowErrorMessage(string message)
    {
        // Display error message to user
        Debug.LogWarning(message);
        connectionStatusText.text = "Error: " + message;
    }

    private byte GetMaxPlayersFromDropdown()
    {
        // Get selected max players from dropdown
        if (maxPlayersDropdown != null)
        {
            // Convert dropdown value to player count (2-8)
            return (byte)(maxPlayersDropdown.value + 2);
        }
        return 4; // Default value
    }

    private void HandleReconnection()
    {
        // If disconnected and not already attempting to reconnect
        if (PhotonNetwork.NetworkClientState == ClientState.Disconnected && !isConnecting)
        {
            reconnectTimer -= Time.deltaTime;

            if (reconnectTimer <= 0)
            {
                reconnectTimer = reconnectDelay;
                ConnectToServer();
            }

            connectionStatusText.text = $"Disconnected. Reconnecting in {Mathf.CeilToInt(reconnectTimer)}...";
        }
    }

    private void HandleRoomListRefresh()
    {
        // Automatically refresh room list after cooldown period
        if (PhotonNetwork.InLobby && Time.time >= nextRefreshTime)
        {
            nextRefreshTime = Time.time + refreshCooldown;
            PhotonNetwork.GetCustomRoomList(TypedLobby.Default, string.Empty);
        }
    }

    public void RefreshRoomList()
    {
        // Manual refresh button handler
        if (PhotonNetwork.InLobby)
        {
            nextRefreshTime = Time.time + refreshCooldown;
            PhotonNetwork.GetCustomRoomList(TypedLobby.Default, string.Empty);
            connectionStatusText.text = "Refreshing room list...";
        }
    }

    #endregion

    #region Photon Callbacks

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server!");
        isConnecting = false;
        reconnectTimer = reconnectDelay;

        // Update UI
        connectionStatusText.text = "Connected to server!";

        if (joinButton != null)
            joinButton.interactable = true;

        // Join the default lobby to see room list
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Photon Lobby");

        // Clear cached room list when entering lobby
        cachedRoomList.Clear();
        ClearRoomList();

        connectionStatusText.text = "In lobby. Ready to join or create a room.";

        // Refresh the room list immediately
        PhotonNetwork.GetCustomRoomList(TypedLobby.Default, string.Empty);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // Don't update too frequently to avoid UI flicker
        if (Time.time < nextRefreshTime)
            return;

        Debug.Log($"Room list updated: {roomList.Count} rooms available");

        // Cache and update the room list
        UpdateCachedRoomList(roomList);
        UpdateRoomList(cachedRoomList);
    }

    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        // Update our cached room list
        foreach (RoomInfo info in roomList)
        {
            // Remove room from cached room list if it got closed or invisible
            if (info.RemovedFromList || !info.IsOpen || !info.IsVisible)
            {
                int index = cachedRoomList.FindIndex(x => x.Name == info.Name);
                if (index != -1)
                {
                    cachedRoomList.RemoveAt(index);
                }
            }
            else // Add or update room in cached room list
            {
                int index = cachedRoomList.FindIndex(x => x.Name == info.Name);
                if (index != -1)
                {
                    cachedRoomList[index] = info;
                }
                else
                {
                    cachedRoomList.Add(info);
                }
            }
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room: " + PhotonNetwork.CurrentRoom.Name);
        connectionStatusText.text = $"Joined room: {PhotonNetwork.CurrentRoom.Name}";

        // Only the master client loads the level for everyone
        if (PhotonNetwork.IsMasterClient)
        {
            // Choose scene based on player count
            LoadAppropriateGameScene();
        }
    }

    private void LoadAppropriateGameScene()
    {
        // Load scene based on player count
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        // Clamp player count to valid range (1-4)
        playerCount = Mathf.Clamp(playerCount, 1, 4);

        // Load the appropriate scene
        PhotonNetwork.LoadLevel("Room for " + playerCount);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"Failed to join room: {message}");
        ShowErrorMessage($"Failed to join room: {message}");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"Failed to create room: {message}");
        ShowErrorMessage($"Failed to create room: {message}");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning($"Disconnected from Photon: {cause}");
        isConnecting = false;

        // Update UI
        if (joinButton != null)
            joinButton.interactable = false;

        // Set reconnect timer
        reconnectTimer = reconnectDelay;

        // Clear room list
        cachedRoomList.Clear();
        ClearRoomList();

        connectionStatusText.text = $"Disconnected: {cause}";
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player {newPlayer.NickName} joined room");

        // If we're the master client and player count changed, we may need to load a different scene
        if (PhotonNetwork.IsMasterClient)
        {
            // Check if we need to load a different scene based on player count
            LoadAppropriateGameScene();
        }
    }

    #endregion

    #region Room Joining

    public void JoinRoom(string roomName)
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            connectionStatusText.text = $"Joining room {roomName}...";
            PhotonNetwork.JoinRoom(roomName);
        }
        else
        {
            ShowErrorMessage("Not connected to server");
        }
    }

    #endregion
}