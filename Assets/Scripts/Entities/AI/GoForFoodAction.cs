using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "PluggableAI/Actions/GoForFood")]
public class GoForFoodAction : Action {
    public override void Act(StateController controller) {

        
        MovingEntity entity = controller.entity as MovingEntity;

        var nearestEntity = entity.Tile.NearestEntity(entity.movingEntitySO.foods.ToArray(), -1);
        if (nearestEntity) {
            bool targetIsStatic = !nearestEntity.movingEntity || nearestEntity.movingEntity == entity;

            Entity target;
            int distanceOfHarvest;

            if (targetIsStatic) {
                distanceOfHarvest = 0;
                target = nearestEntity.staticEntity;
            }
            else {
                distanceOfHarvest = 1;
                target = nearestEntity.movingEntity;
            }

            int distance = target.Tile.Coordinates.Distance(entity.Tile.Coordinates);
            bool rightDistance = distance <= distanceOfHarvest;

            if (!entity.hasFled && !rightDistance) {
                entity.MoveTo(nearestEntity, () => TryHarvest(entity, target, distanceOfHarvest));
            }
            else if (rightDistance) {
                entity.isHungry = false;
                TryHarvest(entity, target, distanceOfHarvest);
            }
        }
        else {
            DecreasePop(entity);
        }
        
    }

    private void TryHarvest(MovingEntity entity, Entity target, int distanceOfHarvest) {
        if (target.Tile.Coordinates.Distance(entity.Tile.Coordinates) == distanceOfHarvest) {
            entity.harvestAnimation = true;

            entity.Harvest(target);
            entity.ChangeAnimation("Eating", false, entity.EndEating);
            entity.remainingTurnsBeforeDie = entity.movingEntitySO.nbTurnsToDie;
            entity.isHungry = false;
            /*/if (entity.reserve == entity.population) {
                entity.isHungry = false;
            }*/

            if (entity.movingEntitySO.reproduceAtEachHarvest) {
                entity.IncreasePopulation();
            }


            entity.isHungry = false;
        }

        DecreasePop(entity);
    }

    private void DecreasePop(MovingEntity entity) {
        if (entity.remainingTurnsBeforeDie == 0) {
            entity.DecreasePopulation();
            entity.remainingTurnsBeforeDie = entity.movingEntitySO.nbTurnsToDie;
        }
        else {
            entity.remainingTurnsBeforeDie -= 1;
        }
    }

    public override void OnExitState(StateController controller) {
        MovingEntity entity = controller.entity as MovingEntity;
        entity.reserve = 0;
        entity.remainingTurnsBeforeHungry = entity.movingEntitySO.nbTurnsToBeHungry;
        entity.remainingTurnsBeforeDie = entity.movingEntitySO.nbTurnsToDie;
    }

    public override void OnEnterState(StateController controller) {

    }
}
