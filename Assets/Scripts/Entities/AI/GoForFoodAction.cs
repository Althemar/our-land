using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu (menuName = "PluggableAI/Actions/GoForFood")]
public class GoForFoodAction : Action 
{
    public override void Act (StateController controller)
    {
        MovingEntity entity = controller.entity as MovingEntity;
        var nearestEntity = entity.Tile.NearestEntity(entity.movingEntitySO.foods.ToArray(), -1);
        int distanceOfHarvest = 1;
        if (!nearestEntity.movingEntity || nearestEntity.movingEntity == this) distanceOfHarvest = 0;
        if (nearestEntity.Coordinates.Distance(entity.Tile.Coordinates) != distanceOfHarvest) {
            entity.MoveTo(nearestEntity);
        }  else {
            //eat it
        }
    }
}
