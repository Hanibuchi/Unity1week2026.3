using System.Collections.Generic;
using UnityEngine;

public class SaveTriggerManager : MonoBehaviour
{
    public static SaveTriggerManager Instance { get; private set; }

    private List<SaveTrigger> saveTriggers = new List<SaveTrigger>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void Register(SaveTrigger trigger)
    {
        if (!saveTriggers.Contains(trigger))
        {
            saveTriggers.Add(trigger);
        }
    }

    public void Unregister(SaveTrigger trigger)
    {
        if (saveTriggers.Contains(trigger))
        {
            saveTriggers.Remove(trigger);
        }
    }

    public SaveTrigger GetSaveTriggerByLocationName(string locationName)
    {
        foreach (var trigger in saveTriggers)
        {
            if (trigger.LocationName == locationName)
            {
                return trigger;
            }
        }
        return null;
    }
}
