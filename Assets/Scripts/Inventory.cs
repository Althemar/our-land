using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Inventory : ScriptableObject
{
    public Dictionary<ResourceType, int> resources;
    public CallbackFunction OnInventoryChange;

    public void AddItem(ResourceType type, int amount) {
        if(resources == null)
            resources = new Dictionary<ResourceType, int>();

        if(!resources.ContainsKey(type))
            resources.Add(type, 0);
        resources[type] += amount;
        
        OnInventoryChange();
    }
}
