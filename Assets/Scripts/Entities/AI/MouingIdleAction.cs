using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/MouingIdle")]
public class MouingIdleAction : Action {
    public override void Act(StateController controller) {
        MovingEntity entity = controller.entity as MovingEntity;
        if (entity.Tile.Tile.terrainType != CustomTile.TerrainType.Grass || entity.Tile.staticEntity != null) {
            var nearest = entity.Tile.NearestBiomeWithoutEntities(CustomTile.TerrainType.Grass, -1);
            if (nearest)
                entity.MoveTo(nearest, Nothing);
        }
        entity.remainingTurnsBeforeHungry -= 1;
        if (entity.remainingTurnsBeforeHungry == 0) {
            entity.isHungry = true;
        }
    }

    private void Nothing() {
        
    }

    public override void OnExitState(StateController controller) {

    }

    public override void OnEnterState(StateController controller) {

    }
}