using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/MouingIdle")]
public class MouingIdleAction : Action {

    public int fleeRange;

    public override void Act(StateController controller) {
        MovingEntity entity = controller.entity as MovingEntity;
        if (!entity.hasFled) {
            if (entity.Tile.Tile.terrainType != CustomTile.TerrainType.Grass || entity.Tile.staticEntity != null) {
                var nearest = entity.Tile.NearestBiomeWithoutEntities(CustomTile.TerrainType.Grass, -1);
                if (nearest) {
                    entity.MoveTo(nearest, null);
                }
            }

        }
    }

    public override void OnExitState(StateController controller) {

    }

    public override void OnEnterState(StateController controller) {

    }
}