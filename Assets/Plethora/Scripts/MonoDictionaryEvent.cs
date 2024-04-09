using Boom.Values;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonoDictionaryEvent : MonoBehaviour
{
    [SerializeField] List<KeyValue<string, UnityEvent>> serializedEvents = new();
    private Dictionary<string, UnityEvent> events = new();

    public void AddListener(string key, UnityAction action)
    {
        if(events.TryGetValue(key, out var unityEvent) == false)
        {
            unityEvent = new();
            events.Add(key, unityEvent);
        }

        unityEvent.AddListener(action);
    }

    public void RemoveListener(string key, UnityAction unityAction) 
    {
        if (events.TryGetValue(key,out var unityEvent))
        {
            unityEvent.RemoveListener(unityAction);
        }
    }

    public void InvokeEvent(string key)
    {
        if (events.TryGetValue(key, out var unityEvent))
        {
            unityEvent.Invoke();
        }

        if(serializedEvents.Count > 0)
        {
            foreach (var item in serializedEvents)
            {
                if(item.key == key)
                {
                    item.value.Invoke();
                    break;
                }
            }
        }
    }
}
