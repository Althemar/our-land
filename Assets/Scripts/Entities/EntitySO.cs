using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

//[CreateAssetMenu(fileName = "Entity", menuName = "Entity", order = 1)]
public class EntitySO : ScriptableObject
{
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
   
}
