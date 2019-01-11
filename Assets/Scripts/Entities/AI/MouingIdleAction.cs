using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Actions/MouingIdle")]
public class MouingIdleAction : Action 
{
    public override void Act (StateController controller)
    {
        MovingEntity entity = controller.entity as MovingEntity;
        if (entity.Tile.Tile.terrainType != CustomTile.TerrainType.Grass) {
            var nearest = entity.Tile.NearestBiomeWithoutEntities(CustomTile.TerrainType.Grass, -1);
            entity.MoveTo(nearest);
        }
            
        
    }
}