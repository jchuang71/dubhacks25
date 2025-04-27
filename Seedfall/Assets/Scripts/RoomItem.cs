using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

public class RoomItem : MonoBehaviour
{

    public TMP_Text roomName;
    LobbyManager manager;
    private string roomNameOnly;

    private void Start()
    {
        manager = FindObjectOfType<LobbyManager>();
    }

    public void SetRoomName(string _roomName)
    {
        roomNameOnly = _roomName;
        roomName.text = _roomName;
    }
    public void SetRoomInfo(string _roomName, int currentPlayers, int maxPlayers)
    {
        roomNameOnly = _roomName;
        roomName.text = $"{_roomName} ({currentPlayers}/{maxPlayers})";
    }

    public void OnClickItem()
    {
        manager.JoinRoom(roomNameOnly);
    }

}
