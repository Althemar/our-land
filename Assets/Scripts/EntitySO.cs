using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Entity", menuName = "Entity", order = 1)]
public class EntitySO : ScriptableObject
{
    public Sprite sprite;

    public int basePopulation;
    public int populationMax;

    public float reproductionRate;
    public float deathRate;
}
