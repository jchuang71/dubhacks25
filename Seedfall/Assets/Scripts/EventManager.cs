using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EventManager : MonoBehaviour
{
    [SerializeField] private EventDataList eventList;
    [SerializeField] private string jsonPath;
    
    void Start()
    {
        TextAsset jsonText = Resources.Load<TextAsset>("events");

        if(jsonText != null) 
        {
            eventList = JsonUtility.FromJson<EventDataList>(jsonText.text);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
