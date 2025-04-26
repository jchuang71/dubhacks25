using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;

public class NewMonoBehaviourScript : MonoBehaviourPunCallbacks
{
    public InputField roomNameInput;

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
    void CreateRoom()
    {
        if(roomNameInput.text != "")
        {
            RoomOptions roomOptions = new RoomOptions(); // PLZ ENSURE ROOM HAS CORRECT OPTIONS
            roomOptions.MaxPlayers = 2;
            PhotonNetwork.JoinOrCreateRoom(roomNameInput.text, roomOptions, TypedLobby.Default); // Will auto create a room if it doesn't exist
        }
    }
}
