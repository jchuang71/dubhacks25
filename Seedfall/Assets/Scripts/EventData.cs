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
}

[Serializable]
public class EventDataList 
{
    public List<EventData> events;
}
