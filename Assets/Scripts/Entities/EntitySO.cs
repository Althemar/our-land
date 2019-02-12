using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;
using RotaryHeart.Lib.SerializableDictionary;

[System.Serializable]
public class ArrayRessources {
    public int[] gain;
}

[Serializable]
public class ResourcesToHarvest : SerializableDictionaryBase<ResourceType, ArrayRessources>
{
}

[System.Serializable]
public struct RandomBonus
{
    public int minBonus;
    public int maxBonus;
}

[System.Serializable]
public struct ArrayRandomBonus
{
    public RandomBonus[] randomBonus;
}


[Serializable]
public class ResourcesRandomBonus : SerializableDictionaryBase<ResourceType, ArrayRandomBonus>
{
}

//[CreateAssetMenu(fileName = "Entity", menuName = "Entity", order = 1)]
public class EntitySO : ScriptableObject
{
    public string name;
    public Sprite sprite;

    //[BoxGroup("Population")]
    //public int basePopulation;
    [BoxGroup("Population")]
    public int populationMax;

    [BoxGroup("Population")]
    public int nbTurnsBeforeReproduction;

    [BoxGroup("Population")]
    public bool reproduceAtEachHarvest;
    public bool renewWhenZero;


    [BoxGroup("Population rates")]
    public int reproductionRate;
    [BoxGroup("Population rates")]
    public int deathRate;




    [BoxGroup("Food")]
    [SerializeField]
    public ResourcesToHarvest resources;
    [BoxGroup("Food")]
    [SerializeField]
    public ResourcesRandomBonus randomBonus;
    

    [ReorderableList]
    public List<CustomTile> availableTiles;

   

    public string harvestSound;
}
