using UnityEngine;
using TMPro; // Import the TextMeshPro namespace for text handling

public class BannerNotification : MonoBehaviour
{
    public GameObject bannerObject; // The banner GameObject to show/hide
    public float displayTime = 2f; // Time in seconds to display the banner
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bannerObject.SetActive(false);
    }

    public void ShowBanner(string message)
    {
        bannerObject.GetComponentInChildren<TMP_Text>().text = message; // Set the message text
        bannerObject.SetActive(true); // Instantly show the banner
        Invoke(nameof(HideBanner), displayTime); // Hide after delay
    }
    
    private void HideBanner()
    {
        bannerObject.SetActive(false); // Instantly hide the banner
    }
}
