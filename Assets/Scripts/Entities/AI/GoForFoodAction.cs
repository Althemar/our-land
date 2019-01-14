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
        if (nearestEntity) {
            bool targetIsStatic = !nearestEntity.movingEntity || nearestEntity.movingEntity == entity;
            Entity target = nearestEntity.movingEntity;
            int distanceOfHarvest = 1;
            if (targetIsStatic) {
                distanceOfHarvest = 0;
                target = nearestEntity.staticEntity;
            }
            if (nearestEntity.Coordinates.Distance(entity.Tile.Coordinates) != distanceOfHarvest) {
                entity.MoveTo(nearestEntity);
            } else {
                entity.Harvest(target);
                entity.remainingTurnsBeforeDie = entity.movingEntitySO.nbTurnsToDie;
                 if (entity.reserve == entity.population) {
                    entity.isHungry = false;          
                }
            }
        }
        
        if (entity.remainingTurnsBeforeDie == 0) {
            entity.DecreasePopulation();
            entity.remainingTurnsBeforeDie = entity.movingEntitySO.nbTurnsToDie;
        } else {
            entity.remainingTurnsBeforeDie -= 1;
        }
        
    }

    public override void OnExitState(StateController controller) {
        Debug.Log("On exit");
        MovingEntity entity = controller.entity as MovingEntity;
        entity.reserve = 0;
        entity.remainingTurnsBeforeHungry = entity.movingEntitySO.nbTurnsToBeHungry;
        entity.IncreasePopulation();   
        entity.remainingTurnsBeforeDie = entity.movingEntitySO.nbTurnsToDie;
    }

    public override void OnEnterState(StateController controller) {

    }
}
