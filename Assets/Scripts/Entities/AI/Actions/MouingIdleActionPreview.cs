using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/MouingIdlePreview")]
public class MouingIdleActionPreview : Action {

    public override void Act(StateController controller) {
        MovingEntity entity = controller.entity as MovingEntity;
        if (!entity.hasFled) {
            if (entity.Tile.Tile.terrainType != CustomTile.TerrainType.Grass || entity.Tile.staticEntity != null) {
                var nearest = entity.Tile.NearestBiomeWithoutEntities(CustomTile.TerrainType.Grass, -1, true);
                if (nearest) {
                    Stack<TileProperties> path = entity.MoveTo(nearest, null, true);
                    if (path != null && path.Count > 1) {
                        TileProperties[] aPath = path.ToArray();

                        entity.SetPreviewTile(aPath[1]);

                        entity.UpdateSprite(entity.Tile.Coordinates.Direction(aPath[1].Coordinates));
                        return;
                    }
                }
            }
            entity.UpdateSprite(HexDirection.E, true);

        }
    }


    public override void OnExitState(StateController controller) {

    }

    public override void OnEnterState(StateController controller) {

    }
}