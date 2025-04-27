using UnityEngine;
using UnityEngine.SceneManagement; // For scene management
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    [SerializeField] public TMP_InputField userNameInputField; // Input field for the player to enter their username
    [SerializeField] public TMP_Text connectButtonText; // Button to initiate connection to the server
    string userName = "Player"; // Default username for the player
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void OnClickConnectToServer()
    {
        PhotonNetwork.NickName = userName; // Set the player's nickname to the default username
        connectButtonText.text = "Connecting..."; // Change the button text to indicate connection in progress
        PhotonNetwork.ConnectUsingSettings(); // Connect to Photon server using the settings defined in the PhotonServerSettings file
        Debug.Log("Connecting to server...");
    }

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("LobbyScene"); // Load the lobby scene after connecting to the server
    }
}
