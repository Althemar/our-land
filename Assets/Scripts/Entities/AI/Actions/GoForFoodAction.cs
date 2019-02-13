using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "PluggableAI/Actions/GoForFood")]
public class GoForFoodAction : Action {

    [ReorderableList]
    [BoxGroup("Grow entities near")]
    public List<EntitySO> targets;

    [ReorderableList]
    [BoxGroup("Grow entities near")]
    public List<EntitySO> entitiesToGrow;

    public override void Act(StateController controller) {
        MovingEntity entity = controller.entity as MovingEntity;
        
        if (!entity.hasFled) {
            var nearestEntity = entity.Tile.NearestFoodWithPriority(entity.movingEntitySO.foods);
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
        else {
            DecreasePop(entity);
        }
    }

    private void TryHarvest(MovingEntity entity, Entity target, int distanceOfHarvest) {
        if (target.Tile.Coordinates.Distance(entity.Tile.Coordinates) == distanceOfHarvest) {
            entity.harvestAnimation = true;

            Grow(target.Tile, target.entitySO);
            
            entity.Harvest(target);
            entity.ChangeAnimation("Eating", false, entity.EndEating);
            entity.remainingTurnsBeforeDie = entity.movingEntitySO.nbTurnsToDie;
            if (entity.movingEntitySO.reproduceAtEachHarvest) {
                entity.IncreasePopulation();
            }
            entity.UpdateSprite(entity.Tile.Coordinates.Direction(target.Tile.Coordinates), distanceOfHarvest == 0);


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

    public void Grow(TileProperties baseTile, EntitySO targetSO) {

        if (targets.Contains(targetSO)) {
            GrowEntitiesOnTile(baseTile);

            foreach (TileProperties neighbor in baseTile.GetNeighbors()) {
                GrowEntitiesOnTile(neighbor);
            }
        }
    }

    private void GrowEntitiesOnTile(TileProperties tile) {
        if (tile) {
            GrowEntity(tile.staticEntity);
            GrowEntity(tile.movingEntity);
        }
    }

    private void GrowEntity(Entity otherEntity) {
        if (otherEntity && entitiesToGrow.Contains(otherEntity.entitySO)) {
            otherEntity.IncreasePopulation();
            otherEntity.remainingTurnsBeforReproduction = otherEntity.entitySO.nbTurnsBeforeReproduction;
        }
    }
}
