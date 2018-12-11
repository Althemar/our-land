using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;
using RotaryHeart.Lib.SerializableDictionary;

[Serializable]
public class ResourcesToHarvest : SerializableDictionaryBase<ResourceType, int>
{
}

//[CreateAssetMenu(fileName = "Entity", menuName = "Entity", order = 1)]
public class EntitySO : ScriptableObject
{
    public string name;
    public Sprite sprite;

    [BoxGroup("Population")]
    public int basePopulation;
    [BoxGroup("Population")]
    public int populationMax;

    [BoxGroup("Population rates")]
    public float reproductionRate;
    [BoxGroup("Population rates")]
    public float deathRate;

    [BoxGroup("Food")]
    public float foodWhenHarvested;

    [ReorderableList]
    public List<CustomTile> availableTiles;

    [SerializeField]
    public ResourcesToHarvest resources;
   
}
