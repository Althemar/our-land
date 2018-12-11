using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ResourcesGet : SerializableDictionaryBase<ResourceType, int> { }

[CreateAssetMenu]
public class Inventory : ScriptableObject
{
    public ResourcesGet resources;
    public CallbackFunction OnInventoryChange;

    public void AddItem(ResourceType type, int amount) {
        if(!resources.ContainsKey(type))
            resources.Add(type, 0);
        resources[type] += amount;
        
        OnInventoryChange();
    }
}
