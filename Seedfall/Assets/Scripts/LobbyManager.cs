using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField roomNameInput;
    private bool isConnecting = false;

    void Start()
    {
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
        }
    }

    public void JoinOrCreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInput.text) || roomNameInput.text == "Input Room Name")
        {
            Debug.LogWarning("Room name is empty!");
            return;
        }

        // Check if we're connected to the master server
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            Debug.LogWarning("Not connected to Photon yet!");
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