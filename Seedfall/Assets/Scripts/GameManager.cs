using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using ExitGames.Client.Photon;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("References")]
    public static GameManager Instance;
    [SerializeField] private TextMeshProUGUI connectionInfoText;
    [SerializeField] private TextMeshProUGUI roomInfoText;
    [SerializeField] private Transform playerListContent;
    [SerializeField] private GameObject playerItemPrefab;
    [SerializeField] private GameObject leaveRoomButton;

    [Header("Player Spawning")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPoints;

    private Dictionary<Player, GameObject> playerListItems = new Dictionary<Player, GameObject>();
    private GameObject localPlayerInstance;

    #region Unity Methods

    void Awake()
    {
        // Implement singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        // Don't destroy when loading new scenes
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Don't try to connect again if we're already in a game scene
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        else if (PhotonNetwork.InRoom)
        {
            // We're already in a room, so update the UI
            UpdateRoomInfo();
            SpawnPlayer();
        }
    }

    void Update()
    {
        if (connectionInfoText != null)
        {
            UpdateConnectionInfo();
        }
    }

    #endregion

    #region UI Methods

    private void UpdateConnectionInfo()
    {
        string roomInfo = PhotonNetwork.InRoom ?
            $"Room: {PhotonNetwork.CurrentRoom.Name} ({PhotonNetwork.CurrentRoom.PlayerCount} players)" :
            "Not in room";

        connectionInfoText.text = $"Connection Status: {PhotonNetwork.NetworkClientState}\n{roomInfo}";
    }

    private void UpdateRoomInfo()
    {
        if (roomInfoText != null && PhotonNetwork.InRoom)
        {
            roomInfoText.text = $"Room: {PhotonNetwork.CurrentRoom.Name} | Players: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
        }
    }

    private void UpdatePlayerList()
    {
        // Clear existing player list
        foreach (var item in playerListItems)
        {
            Destroy(item.Value);
        }
        playerListItems.Clear();

        // Add all current players
        if (PhotonNetwork.InRoom)
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                CreatePlayerListItem(player);
            }
        }
    }

    private void CreatePlayerListItem(Player player)
    {
        GameObject playerItem = Instantiate(playerItemPrefab, playerListContent);

        // Set up player item UI
        PlayerItem playerItemScript = playerItem.GetComponent<PlayerItem>();
        if (playerItemScript != null)
        {
            playerItemScript.Initialize(player.NickName, player.IsMasterClient);
        }

        playerListItems[player] = playerItem;
    }

    #endregion

    #region Player Management

    public void SpawnPlayer()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("Missing player prefab reference!");
            return;
        }

        if (PhotonNetwork.InRoom && localPlayerInstance == null)
        {
            // Get a spawn position
            Vector3 spawnPosition = Vector3.zero;
            if (spawnPoints.Length > 0)
            {
                int spawnIndex = PhotonNetwork.LocalPlayer.ActorNumber % spawnPoints.Length;
                spawnPosition = spawnPoints[spawnIndex].position;
            }

            // Instantiate local player
            localPlayerInstance = PhotonNetwork.Instantiate(
                playerPrefab.name,
                spawnPosition,
                Quaternion.identity,
                0
            );

            Debug.Log($"Player spawned at {spawnPosition}");
        }
    }

    #endregion

    #region Room Management

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    #endregion

    #region Photon Callbacks

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server from GameManager!");
        if (connectionInfoText != null)
        {
            connectionInfoText.text = "Connected to Master Server";
        }

        // If we were in a room before, try to rejoin
        if (PhotonNetwork.AutomaticallySyncScene && !PhotonNetwork.InRoom)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Local player joined room: {PhotonNetwork.CurrentRoom.Name}");

        // Update UI
        UpdateRoomInfo();
        UpdatePlayerList();

        // Spawn player
        SpawnPlayer();
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Left room");

        // Clean up
        if (localPlayerInstance != null)
        {
            Destroy(localPlayerInstance);
            localPlayerInstance = null;
        }

        // Load lobby scene
        PhotonNetwork.LoadLevel("LobbyScene");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player {newPlayer.NickName} entered the room!");

        // Update UI
        UpdateRoomInfo();
        CreatePlayerListItem(newPlayer);

        // Update room display for all players
        string message = $"{newPlayer.NickName} joined the room";
        ChatMessage(message);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"Player {otherPlayer.NickName} left the room!");

        // Update UI
        UpdateRoomInfo();

        // Remove from player list
        if (playerListItems.TryGetValue(otherPlayer, out GameObject playerItem))
        {
            Destroy(playerItem);
            playerListItems.Remove(otherPlayer);
        }

        // Update room display for all players
        string message = $"{otherPlayer.NickName} left the room";
        ChatMessage(message);
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        // Handle room property updates
        Debug.Log("Room properties updated");
        UpdateRoomInfo();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log($"New master client: {newMasterClient.NickName}");

        // Update player list to show new host
        UpdatePlayerList();

        string message = $"{newMasterClient.NickName} is now the host";
        ChatMessage(message);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Disconnected from Photon: {cause}");
        if (connectionInfoText != null)
        {
            connectionInfoText.text = $"Disconnected: {cause}";
        }

        // Clean up
        if (localPlayerInstance != null)
        {
            Destroy(localPlayerInstance);
            localPlayerInstance = null;
        }

        // Load lobby scene
        if (cause != DisconnectCause.DisconnectByClientLogic)
        {
            PhotonNetwork.LoadLevel("LobbyScene");
        }
    }

    #endregion

    #region Helper Methods

    void ChatMessage(string message)
    {
        // Send a chat message to all players (you can implement this with RPC)
        Debug.Log($"[ROOM] {message}");
    }

    #endregion
}