using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField roomNameInput;
    private bool isConnecting = false;
    public TextMeshProUGUI connectionStatusText;
    public Button joinButton;

    void Start()
    {
        // Disable button until connected
        if (joinButton != null)
            joinButton.interactable = false;

        // Update status text
        if (connectionStatusText != null)
            connectionStatusText.text = "Connecting to server...";

        // Only connect if we're not already connected
        if (PhotonNetwork.NetworkClientState == ClientState.Disconnected)
        {
            isConnecting = true;
            PhotonNetwork.ConnectUsingSettings();
        }
        else if (PhotonNetwork.IsConnected)
        {
            // Already connected, make sure we're in the lobby
            PhotonNetwork.JoinLobby();

            // Update UI
            if (connectionStatusText != null)
                connectionStatusText.text = "Connected! Enter room name.";
            if (joinButton != null)
                joinButton.interactable = true;
        }
    }

    void Update()
    {
        // Update the status text with current connection state
        if (connectionStatusText != null)
        {
            string statusText = $"Status: {PhotonNetwork.NetworkClientState}";

            // Add additional info if in a room
            if (PhotonNetwork.InRoom)
            {
                statusText += $"\nRoom: {PhotonNetwork.CurrentRoom.Name}";
            }

            connectionStatusText.text = statusText;
        }
    }

    public void JoinOrCreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInput.text) || roomNameInput.text == "Input Room Name")
        {
            Debug.LogWarning("Room name is empty!");
            // Show user feedback here (could add a UI text element to display this)
            return;
        }

        // Check if we're connected to the master server
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            Debug.LogWarning("Not connected to Photon yet! Attempting to connect...");
            // Try to connect again if not connected
            if (PhotonNetwork.NetworkClientState == ClientState.Disconnected)
            {
                PhotonNetwork.ConnectUsingSettings();
                // Show connecting feedback to user
            }
            return;
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;

        PhotonNetwork.JoinOrCreateRoom(roomNameInput.text, roomOptions, TypedLobby.Default);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server!");

        // Update UI
        if (connectionStatusText != null)
            connectionStatusText.text = "Connected to server!";

        // Join the default lobby after connecting to master
        if (isConnecting)
        {
            isConnecting = false;
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Photon Lobby");

        // Update UI
        if (connectionStatusText != null)
            connectionStatusText.text = "Ready! Enter room name.";
        if (joinButton != null)
            joinButton.interactable = true;
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room: " + PhotonNetwork.CurrentRoom.Name);

        // Load the game scene, automatically synced for all clients
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("GameScene");
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Disconnected from Photon: {cause}");
        isConnecting = false;

        // Update UI
        if (connectionStatusText != null)
            connectionStatusText.text = $"Disconnected: {cause}. Retrying...";
        if (joinButton != null)
            joinButton.interactable = false;

        // Try to reconnect
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Failed to join room: {message}");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Failed to create room: {message}");
    }
}