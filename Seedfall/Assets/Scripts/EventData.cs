using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EventData
{
    public int eventId;
    public string title;
    public string text;
    public string effect;
    public float pollution;
    public float money;
    public int deforest;
}

[Serializable]
public class EventDataList 
{
    public List<EventData> events;
}
