using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

public class ConnectionUIController : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI connectionStatusText;
    [SerializeField] private GameObject connectionStatusPanel;
    [SerializeField] private Image connectionStatusIcon;
    [SerializeField] private TextMeshProUGUI errorMessageText;
    [SerializeField] private GameObject errorPanel;
    [SerializeField] private Button reconnectButton;

    [Header("Status Icons")]
    [SerializeField] private Sprite connectingIcon;
    [SerializeField] private Sprite connectedIcon;
    [SerializeField] private Sprite disconnectedIcon;
    [SerializeField] private Sprite errorIcon;

    [Header("Settings")]
    [SerializeField] private float errorDisplayTime = 5f;

    private Coroutine errorDisplayCoroutine;
    private bool isReconnecting = false;

    #region Unity Methods

    void Start()
    {
        // Hide error panel initially
        if (errorPanel != null)
            errorPanel.SetActive(false);

        // Add listener to reconnect button
        if (reconnectButton != null)
        {
            reconnectButton.onClick.RemoveAllListeners();
            reconnectButton.onClick.AddListener(OnReconnectClicked);
        }

        // Update UI based on current connection state
        UpdateConnectionStatus(PhotonNetwork.NetworkClientState);
    }

    #endregion

    #region UI Methods

    public void UpdateConnectionStatus(ClientState state)
    {
        if (connectionStatusText == null || connectionStatusIcon == null)
            return;

        string statusMessage = "Status: ";

        switch (state)
        {
            case ClientState.PeerCreated:
            case ClientState.Authenticating:
            // Remove: case ClientState.AuthenticatingWithServer:
            case ClientState.ConnectingToGameServer:
            case ClientState.ConnectingToMasterServer:
            case ClientState.ConnectingToNameServer:
                statusMessage += "Connecting...";
                connectionStatusIcon.sprite = connectingIcon;
                connectionStatusIcon.color = new Color(1f, 0.8f, 0f); // Yellow
                break;

            // Replace Connected with proper states
            case ClientState.Joined:
            case ClientState.JoinedLobby:
                statusMessage += "Connected";
                connectionStatusIcon.sprite = connectedIcon;
                connectionStatusIcon.color = new Color(0f, 0.8f, 0f); // Green
                break;

            case ClientState.Disconnected:
            case ClientState.DisconnectingFromGameServer:
            case ClientState.DisconnectingFromMasterServer:
            case ClientState.DisconnectingFromNameServer:
                statusMessage += "Disconnected";
                connectionStatusIcon.sprite = disconnectedIcon;
                connectionStatusIcon.color = new Color(0.8f, 0f, 0f); // Red
                break;

            default:
                statusMessage += state.ToString();
                connectionStatusIcon.sprite = null;
                connectionStatusIcon.color = Color.white;
                break;
        }

        // Add room info if in a room
        if (PhotonNetwork.InRoom)
        {
            statusMessage += $"\nRoom: {PhotonNetwork.CurrentRoom.Name} ({PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers})";
        }

        connectionStatusText.text = statusMessage;
    }

    public void ShowErrorMessage(string message, bool showReconnectButton = true)
    {
        if (errorPanel == null || errorMessageText == null)
            return;

        // Cancel any existing error display coroutine
        if (errorDisplayCoroutine != null)
            StopCoroutine(errorDisplayCoroutine);

        // Set error message
        errorMessageText.text = message;

        // Show reconnect button if specified
        if (reconnectButton != null)
            reconnectButton.gameObject.SetActive(showReconnectButton);

        // Show error panel
        errorPanel.SetActive(true);

        // Start coroutine to hide error after delay
        errorDisplayCoroutine = StartCoroutine(HideErrorAfterDelay());
    }

    private IEnumerator HideErrorAfterDelay()
    {
        yield return new WaitForSeconds(errorDisplayTime);

        if (errorPanel != null)
            errorPanel.SetActive(false);

        errorDisplayCoroutine = null;
    }

    private void OnReconnectClicked()
    {
        if (isReconnecting)
            return;

        // Hide error panel
        if (errorPanel != null)
            errorPanel.SetActive(false);

        // Reconnect to Photon
        StartCoroutine(ReconnectToServer());
    }

    private IEnumerator ReconnectToServer()
    {
        isReconnecting = true;

        // Show connecting status
        UpdateConnectionStatus(ClientState.ConnectingToNameServer);

        // Disconnect if currently connected
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();

            // Wait until disconnected
            while (PhotonNetwork.IsConnected)
                yield return null;
        }

        // Try to reconnect
        PhotonNetwork.ConnectUsingSettings();

        isReconnecting = false;
    }

    #endregion

    #region Photon Callbacks

    public override void OnConnectedToMaster()
    {
        UpdateConnectionStatus(PhotonNetwork.NetworkClientState);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        UpdateConnectionStatus(PhotonNetwork.NetworkClientState);

        // Show error message if disconnection wasn't intentional
        if (cause != DisconnectCause.DisconnectByClientLogic)
        {
            ShowErrorMessage($"Disconnected: {cause}", true);
        }
    }

    public override void OnJoinedRoom()
    {
        UpdateConnectionStatus(PhotonNetwork.NetworkClientState);
    }

    public override void OnLeftRoom()
    {
        UpdateConnectionStatus(PhotonNetwork.NetworkClientState);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        ShowErrorMessage($"Failed to join room: {message}", false);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        ShowErrorMessage($"Failed to create room: {message}", false);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        ShowErrorMessage($"Failed to join random room: {message}", false);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdateConnectionStatus(PhotonNetwork.NetworkClientState);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateConnectionStatus(PhotonNetwork.NetworkClientState);
    }

    #endregion
}