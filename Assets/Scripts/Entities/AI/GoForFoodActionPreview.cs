using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "PluggableAI/Actions/GoForFoodPreview")]
public class GoForFoodActionPreview : Action {
    public override void Act(StateController controller) {


        MovingEntity entity = controller.entity as MovingEntity;
        

        if (!entity.hasFled) {
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
                    Stack<TileProperties> path = entity.MoveTo(nearestEntity, null, true);
                    if (path != null && path.Count > 1) {
                        TileProperties[] aPath = path.ToArray();
                        entity.previewTile.movablePreview = null;
                        HexagonalGrid.Instance.Tilemap.SetColor(entity.previewTile.Position, Color.white);

                        entity.previewTile = aPath[1];
                        entity.previewTile.movablePreview = entity.movable;
                        HexagonalGrid.Instance.Tilemap.SetColor(entity.previewTile.Position, Color.red);
                        entity.UpdateSprite(entity.Tile.Coordinates.Direction(aPath[1].Coordinates));
                    }
                }
                else if (rightDistance) {
                    entity.UpdateSprite(entity.Tile.Coordinates.Direction(target.Tile.Coordinates));
                }
            }
        }


    }

    public override void OnExitState(StateController controller) {
    }

    public override void OnEnterState(StateController controller) {

    }
}
