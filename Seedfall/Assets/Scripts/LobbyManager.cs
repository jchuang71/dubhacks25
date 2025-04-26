using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField roomNameInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // Connects to Photon cloud
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // if room name is not empty, create a new room with the specified name
    public void JoinOrCreateRoom()
    {
        if(roomNameInput.text != "" && roomNameInput.text != "Input Room Name")
        {
            RoomOptions roomOptions = new RoomOptions(); // PLZ ENSURE ROOM HAS CORRECT OPTIONS
            roomOptions.MaxPlayers = 2;
            PhotonNetwork.JoinOrCreateRoom(roomNameInput.text, roomOptions, TypedLobby.Default); // Will auto create a room if it doesn't exist
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room: " + PhotonNetwork.CurrentRoom.Name);
      
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("GameScene"); // Only MasterClient should load the scene
        }
    }
}
