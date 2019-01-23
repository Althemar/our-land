﻿using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/FleePreview")]
public class FleeActionPreview : Action
{
    public int fleeRange;

    public override void Act(StateController controller) {

        MovingEntity entity = controller.entity as MovingEntity;

        entity.hasFled = false;
        TileProperties nearestPredatorTile = entity.Tile.NearestEntity(entity.movingEntitySO.predators.ToArray(), fleeRange);

        if (nearestPredatorTile) {
            int distance = nearestPredatorTile.Coordinates.Distance(entity.Tile.Coordinates);
            if (distance <= fleeRange) {
                TileProperties fleeTile = HexagonalGrid.Instance.GetTile(entity.Tile.Coordinates.Opposite(nearestPredatorTile.Coordinates));
                if (fleeTile && !fleeTile.movablePreview && fleeTile.IsWalkable() && entity.movingEntitySO.availableTiles.Contains(fleeTile.Tile)) {
                    entity.hasFled = true;

                    entity.SetPreviewTile(fleeTile);

                    entity.UpdateSprite(entity.Tile.Coordinates.Direction(fleeTile.Coordinates));
                }
            }
            
        }

    }

    public override void OnEnterState(StateController controller) {
    }

    public override void OnExitState(StateController controller) {
    }
    
}
