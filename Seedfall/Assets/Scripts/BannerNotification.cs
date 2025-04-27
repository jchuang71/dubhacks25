using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting; // Import the TextMeshPro namespace for text handling

public class BannerNotification : MonoBehaviour
{
    public GameObject bannerObject; // The banner GameObject to show/hide
    public float displayTime = 2f; // Time in seconds to display the banner

    private TextMeshProUGUI titleText;
    private TextMeshProUGUI effectText;
    private Image panel;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        panel = bannerObject.GetComponent<Image>();
        titleText = bannerObject.transform.Find("EventTitle").GetComponent<TextMeshProUGUI>();
        effectText = bannerObject.transform.Find("EventEffect").GetComponent<TextMeshProUGUI>();
        titleText.color = new Color(titleText.color.r, titleText.color.g, titleText.color.b, 0);
        panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, 0);
        effectText.color = new Color(effectText.color.r, effectText.color.g, effectText.color.b, 0);
    }

    public void ShowBanner(string title, string effect, Color color)
    {
        titleText.text = title; // Set the message text
        effectText.text = effect;
        effectText.color = color;

        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        titleText.color = new Color(titleText.color.r, titleText.color.g, titleText.color.b, 0);
        effectText.color = new Color(effectText.color.r, effectText.color.g, effectText.color.b, 0);
        panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, 0);
        while (titleText.color.a < 1.0f)
        {
            titleText.color = new Color(titleText.color.r, titleText.color.g, titleText.color.b, titleText.color.a + (Time.deltaTime / 1.0f));
            effectText.color = new Color(effectText.color.r, effectText.color.g, effectText.color.b, effectText.color.a + (Time.deltaTime / 1.0f));
            panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, panel.color.a + (Time.deltaTime / 1.0f));
            yield return null;
        }

        yield return new WaitForSeconds(2.0f);
        StartCoroutine(FadeOut());  
    }

    IEnumerator FadeOut()
    {
        while (titleText.color.a > 0.0f)
        {
            titleText.color = new Color(titleText.color.r, titleText.color.g, titleText.color.b, titleText.color.a - (Time.deltaTime / 1.0f));
            effectText.color = new Color(effectText.color.r, effectText.color.g, effectText.color.b, effectText.color.a - (Time.deltaTime / 1.0f));
            panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, panel.color.a - (Time.deltaTime / 1.0f));
            yield return null;
        }
    }
}
