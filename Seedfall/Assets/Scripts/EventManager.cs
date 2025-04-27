using System;
using UnityEngine;
using System.Collections;
using Unity.Mathematics;

public class EventManager : MonoBehaviour
{
    [SerializeField] private EventDataList eventList;
    [SerializeField] private float randomEventIntervalMin;
    [SerializeField] private float randomEventIntervalMax;

    private bool eventsEnabled;
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
            
            float pollution = GameManager.ManagerInstance.pollutionInterval;
            pollution = pollution - (pollution * currentEvent.pollution / 100);
            GameManager.ManagerInstance.pollutionInterval = pollution;

            GameManager.ManagerInstance.GetComponent<Money>().AddAmount(currentEvent.money);

            VegetationArea vArea = GameManager.ManagerInstance.vegetationArea;
            for(int i = 0; i < currentEvent.deforest; i++) {
                int randomTileIndex = UnityEngine.Random.Range(0, vArea.tiles.Count);
                vArea.tiles[randomTileIndex].GetComponent<VegetationTile>().ChangeState("Deforested");
            }
        }
    }
}
