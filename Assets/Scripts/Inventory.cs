using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ResourcesGet : SerializableDictionaryBase<ResourceType, float> { }

public class Inventory : MonoBehaviour
{
    public ResourcesGet resources;
    public CallbackFunction OnInventoryChange;

    public void AddItem(ResourceType type, float amount) {
        if (!resources.ContainsKey(type)) {
            resources.Add(type, 0);
        }
        resources[type] += amount;
        
        OnInventoryChange();
    }

    public float GetResource(ResourceType type) {
        return resources[type];
    }
}
