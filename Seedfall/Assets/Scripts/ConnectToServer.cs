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

    private void Start()
    {
        // Set the initial text in the input field to the default userName
        if (userNameInputField != null)
        {
            userNameInputField.text = userName;
        }
    }

    public void OnClickConnectToServer()
    {

        if (!string.IsNullOrEmpty(userNameInputField.text))
        {
            userName = userNameInputField.text;
        }

        PhotonNetwork.NickName = userName; // Set the player's nickname to the default username
        connectButtonText.text = "Connecting..."; // Change the button text to indicate connection in progress
        Debug.Log("directly before connecting awaiting connect...");
        PhotonNetwork.ConnectUsingSettings(); // Connect to Photon server using the settings defined in the PhotonServerSettings file
        Debug.Log("Connecting to server...");
    }

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("Lobby"); // Load the lobby scene after connecting to the server
    }
}
