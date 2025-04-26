using Unity.VisualScripting;
using Photon.Pun;
using UnityEngine;
using TMPro;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager ManagerInstance; // Singleton reference to this manager object available to all other scripts
    public TextMeshProUGUI connectionInfoText;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    // Update is called once per frame
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

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server!");
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
