using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "Entity", menuName = "Entity/Moving Entity", order = 1)]
public class MovingEntitySO : EntitySO
{
    public int movementPoints;

    [BoxGroup("Food")]
    public float foodConsumption;
    [BoxGroup("Food")]
    public float satietyThreshold;
    [BoxGroup("Food")]
    public float starvationThreshold;

    public float damageWhenEat;
    public GameObject eatFeedback;

    [BoxGroup("Food")]
    [ReorderableList]
    public List<EntitySO> foods;
}
