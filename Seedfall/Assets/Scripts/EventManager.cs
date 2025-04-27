using System;
using UnityEngine;
using System.Collections;
using Unity.Mathematics;

public class EventManager : MonoBehaviour
{
    [SerializeField] private EventDataList eventList;
    [SerializeField] private float randomEventIntervalMin;
    [SerializeField] private float randomEventIntervalMax;

    public bool eventsEnabled;
    void Start()
    {
        TextAsset jsonText = Resources.Load<TextAsset>("events");

        if(jsonText != null) 
        {
            eventList = JsonUtility.FromJson<EventDataList>(jsonText.text);
            eventsEnabled = true;
            StartCoroutine(RandomEvents());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator RandomEvents() 
    {
        while(eventsEnabled)
        {
            float randomTime = UnityEngine.Random.Range(randomEventIntervalMin, randomEventIntervalMax);
            yield return new WaitForSeconds(randomTime);
            int randomEventIndex = UnityEngine.Random.Range(0, eventList.events.Count - 1);
            EventData currentEvent = eventList.events[randomEventIndex];
            
            Debug.Log(currentEvent.title);
            AdjustPollution(currentEvent);
            AdjustMoney(currentEvent);
            DeforestTiles(currentEvent);

            Color textColor;
            if (currentEvent.type == "bad")
                textColor = new Color(1, 0.5f, 0.5f, 1);
            else
                textColor = new Color(0.5f, 1, 0.5f, 1);

            GameManager.ManagerInstance.GetComponent<BannerNotification>().ShowBanner(currentEvent.title, currentEvent.effect, textColor);
        }
    }

    void AdjustPollution(EventData ev) 
    {
        float pollution = GameManager.ManagerInstance.pollutionInterval;
        pollution = pollution - (pollution * ev.pollution / 200);
        GameManager.ManagerInstance.pollutionInterval = pollution;
    }

    void AdjustMoney(EventData ev)
    {
        GameManager.ManagerInstance.GetComponent<Money>().AddAmount(ev.money);
    }

    void DeforestTiles(EventData ev)
    {
        VegetationArea vArea = GameManager.ManagerInstance.vegetationArea;
            for(int i = 0; i < ev.deforest; i++) {
                int randomTileIndex = UnityEngine.Random.Range(0, vArea.tiles.Count);
                vArea.tiles[randomTileIndex].GetComponent<VegetationTile>().ChangeState("Deforested");
            }
    }
}
