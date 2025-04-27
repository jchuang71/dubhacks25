using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerItem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private GameObject hostIcon;
    [SerializeField] private GameObject readyIcon;
    [SerializeField] private Image playerAvatar;
    [SerializeField] private Image backgroundImage;

    [Header("Visual Settings")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hostColor = new Color(1f, 0.95f, 0.8f);
    [SerializeField] private Color localPlayerColor = new Color(0.8f, 1f, 0.8f);

    // Player properties
    private string playerName;
    private bool isHost;
    private bool isReady;
    private bool isLocalPlayer;

    public void Initialize(string playerName, bool isHost, bool isReady = false)
    {
        this.playerName = playerName;
        this.isHost = isHost;
        this.isReady = isReady;
        this.isLocalPlayer = playerName == Photon.Pun.PhotonNetwork.NickName;

        // Set UI elements
        if (playerNameText != null)
            playerNameText.text = playerName;

        if (hostIcon != null)
            hostIcon.SetActive(isHost);

        if (readyIcon != null)
            readyIcon.SetActive(isReady);

        // Update visual style
        UpdateVisualState();
    }

    public void SetReady(bool ready)
    {
        isReady = ready;

        if (readyIcon != null)
            readyIcon.SetActive(isReady);
    }

    private void UpdateVisualState()
    {
        if (backgroundImage == null)
            return;

        // Change background color based on player state
        if (isLocalPlayer)
            backgroundImage.color = localPlayerColor;
        else if (isHost)
            backgroundImage.color = hostColor;
        else
            backgroundImage.color = normalColor;
    }

    // Optional: Method to update the player's avatar if your game supports this feature
    public void SetAvatar(Sprite avatarSprite)
    {
        if (playerAvatar != null && avatarSprite != null)
        {
            playerAvatar.sprite = avatarSprite;
            playerAvatar.gameObject.SetActive(true);
        }
    }

    // Update player status (called from GameManager when properties change)
    public void UpdateStatus(bool isHost, bool isReady)
    {
        this.isHost = isHost;
        this.isReady = isReady;

        if (hostIcon != null)
            hostIcon.SetActive(isHost);

        if (readyIcon != null)
            readyIcon.SetActive(isReady);

        UpdateVisualState();
    }
}