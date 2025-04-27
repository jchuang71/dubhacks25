using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine.UIElements;
using ExitGames.Client.Photon;

public class LobbyManager : MonoBehaviourPunCallbacks
{

    public GameObject playButton;

    //public GameObject playersPanel;
    public TMP_Text player1Text;
    public TMP_Text player2Text;

    public TMP_InputField roomInputField;
    public GameObject lobbyPanel;
    public GameObject roomPanel;
    public TMP_Text roomName;
    public GameObject roomsList;
    public GameObject leaveButton;

    public RoomItem roomItemPrefab;
    List<RoomItem> roomItemsList = new List<RoomItem>();
    public Transform contentObject;

    public float timeBetweenUpdates = 1.5f;
    float nextUpdateTime;

    public TMP_InputField joinRoomInputField;

    private void Start()
    {
        PhotonNetwork.JoinLobby();
    }

    public void OnClickJoinRoom()
    {
        if (!string.IsNullOrEmpty(joinRoomInputField.text))
        {
            PhotonNetwork.JoinRoom(joinRoomInputField.text);
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Failed to join room: {message}");
        // Optional: Show a message to the user that the room doesn't exist
        // Could add a UI Text component to display errors
    }

    public void OnClickCreate()
    {
        PhotonNetwork.CreateRoom(roomInputField.text, new RoomOptions() { MaxPlayers = 2 });
    }

    public void OnClickPlay()
    {
        Debug.Log("Play button clicked"); // ADDED: Debug log

        // Always load the Game scene
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Master client loading level for all players"); // ADDED: Debug info
            PhotonNetwork.LoadLevel("Game");
        }
        else
        {
            Debug.Log("Non-master client loading scene locally"); // ADDED: Debug info
            SceneManager.LoadScene("Game");
        }

    }


    public override void OnJoinedRoom()
    {
        lobbyPanel.SetActive(false);
        roomsList.SetActive(false);
        roomPanel.SetActive(true);
        leaveButton.SetActive(true);
        playButton.SetActive(true);
        //playersPanel.SetActive(true);
        roomName.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name +
                         " (" + PhotonNetwork.CurrentRoom.PlayerCount + "/" +
                         PhotonNetwork.CurrentRoom.MaxPlayers + ")";
        UpdatePlayerList();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {

        if(Time.time >= nextUpdateTime)
        {
            UpdateRoomList(roomList);
            nextUpdateTime = Time.time + timeBetweenUpdates;
        }
    }

    private void UpdateRoomList(List<RoomInfo> list)
    {
        foreach (RoomItem item in roomItemsList)
        {
            Destroy(item.gameObject);
        }
        roomItemsList.Clear();

        foreach (RoomInfo room in list)
        {
            if (!room.RemovedFromList)
            {
                RoomItem newRoom = Instantiate(roomItemPrefab, contentObject);
                newRoom.SetRoomInfo(room.Name, room.PlayerCount, room.MaxPlayers);
                roomItemsList.Add(newRoom);
            }
        }
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        roomPanel.SetActive(false);
        //playersPanel.SetActive(false);
        leaveButton.SetActive(false);
        lobbyPanel.SetActive(true);
        roomsList.SetActive(true);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        roomName.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name +
                         " (" + PhotonNetwork.CurrentRoom.PlayerCount + "/" +
                         PhotonNetwork.CurrentRoom.MaxPlayers + ")";
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        roomName.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name +
                         " (" + PhotonNetwork.CurrentRoom.PlayerCount + "/" +
                         PhotonNetwork.CurrentRoom.MaxPlayers + ")";
        UpdatePlayerList();
    }


    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server in Lobby scene");
        PhotonNetwork.JoinLobby();
    }

    //public override void OnJoinedLobby()
    //{
    //    Debug.Log("Joined lobby successfully"); // ADDED: Debug info
    //                                            // Make sure UI is in the right state when rejoining lobby
    //    lobbyPanel.SetActive(true);
    //    roomsList.SetActive(true);
    //    roomPanel.SetActive(false);
    //    leaveButton.SetActive(false);
    //}

    private void UpdatePlayerList()
    {
        // Reset player texts
        player1Text.text = "";
        player2Text.text = "";

        // Add text for each player in room
        if (PhotonNetwork.CurrentRoom != null)
        {
            Player[] players = PhotonNetwork.PlayerList;

            for (int i = 0; i < players.Length; i++)
            {
                Player player = players[i];
                string playerText = player.NickName;

                // Add "(You)" after your own name
                if (player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    playerText += " (You)";
                }

                // Assign to the appropriate text field
                if (i == 0)
                {
                    player1Text.text = playerText;
                }
                else if (i == 1)
                {
                    player2Text.text = playerText;
                }
            }
        }
    }

}
