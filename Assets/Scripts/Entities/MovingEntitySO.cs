using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Entity", menuName = "Entity/Moving Entity", order = 1)]
public class MovingEntitySO : EntitySO
{
    public int movementPoints;

    public float foodConsumption;

    public float satietyThreshold;
    public float starvationThreshold;

    public List<EntitySO> foods;
}
