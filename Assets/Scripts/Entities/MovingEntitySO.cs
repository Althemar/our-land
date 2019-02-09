using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;
using RotaryHeart.Lib.SerializableDictionary;


[Serializable]
public class Foods : SerializableDictionaryBase<EntitySO, int>
{
}

[CreateAssetMenu(fileName = "Entity", menuName = "Entity/Moving Entity", order = 1)]
public class MovingEntitySO : EntitySO
{
    public int movementPoints;
    public int nbTurnsToDie;

    [BoxGroup("Food")]
    public int nbTurnsToBeHungry;

    public float damageWhenEat;
    public GameObject eatFeedback;

    //[BoxGroup("Food")]
    //[ReorderableList]
    //public List<EntitySO> foods;

    [BoxGroup("Food")]
    public Foods foods;

    [BoxGroup("Food")]
    [ReorderableList]
    public List<EntitySO> predators;
}
