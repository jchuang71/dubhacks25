using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class RoomItem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private Button joinButton;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private GameObject lockedIcon;

    [Header("Visual Settings")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color fullColor = new Color(1f, 0.8f, 0.8f);
    [SerializeField] private Color privateColor = new Color(0.8f, 0.8f, 1f);

    // Room properties
    private string roomName;
    private bool isPrivate;
    private bool isFull;
    private Action onJoinCallback;

    public void Initialize(string roomName, string playerCount, bool isPrivate = false, bool isFull = false)
    {
        this.roomName = roomName;
        this.isPrivate = isPrivate;
        this.isFull = isFull;

        // Set UI elements
        if (roomNameText != null)
            roomNameText.text = roomName;

        if (playerCountText != null)
            playerCountText.text = playerCount;

        if (lockedIcon != null)
            lockedIcon.SetActive(isPrivate);

        // Update visual style based on room state
        UpdateVisualState();

        // Add button listener
        if (joinButton != null)
        {
            // Clear existing listeners first
            joinButton.onClick.RemoveAllListeners();
            joinButton.onClick.AddListener(OnJoinButtonClicked);

            // Disable button if room is full
            joinButton.interactable = !isFull;
        }
    }

    private void UpdateVisualState()
    {
        if (backgroundImage == null)
            return;

        // Change background color based on room state
        if (isFull)
            backgroundImage.color = fullColor;
        else if (isPrivate)
            backgroundImage.color = privateColor;
        else
            backgroundImage.color = normalColor;
    }

    private void OnJoinButtonClicked()
    {
        // Call the callback set during initialization
        onJoinCallback?.Invoke();
    }

    public void SetJoinCallback(Action callback)
    {
        onJoinCallback = callback;
    }
}