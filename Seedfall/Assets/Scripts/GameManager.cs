using Unity.VisualScripting;
using Photon.Pun;
using UnityEngine;
using TMPro;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager ManagerInstance;
    public TextMeshProUGUI connectionInfoText;

    void Awake()
    {
        // Implement singleton pattern
        if (ManagerInstance == null)
        {
            ManagerInstance = this;
        }
        else if (ManagerInstance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Don't try to connect again if we're already in a game scene
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    void Update()
    {
        if (connectionInfoText != null)
        {
            string roomInfo = PhotonNetwork.InRoom ?
                $"Room: {PhotonNetwork.CurrentRoom.Name} ({PhotonNetwork.CurrentRoom.PlayerCount} players)" :
                "Not in room";

            connectionInfoText.text = $"Connection Status: {PhotonNetwork.NetworkClientState}\n{roomInfo}";
        }
    }

    // Add callback for when player enters the room 
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player {newPlayer.NickName} entered the room!");
    }

    // Add callback for when player leaves the room
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"Player {otherPlayer.NickName} left the room!");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server from GameManager!");
        if (connectionInfoText != null)
        {
            connectionInfoText.text = "Connected to Master Server";
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Disconnected from Photon: {cause}");
        if (connectionInfoText != null)
        {
            connectionInfoText.text = $"Disconnected: {cause}";
        }
    }
}