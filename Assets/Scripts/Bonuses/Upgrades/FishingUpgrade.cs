using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingUpgrade : Upgrade
{
    public EntitiesHarvestableUI entitiesHarvestable;

    public override void DoUpgrade() {
        GameManager.Instance.motherShip.canFish = true;

        foreach (TileProperties neighbor in GameManager.Instance.motherShip.Movable.CurrentTile.neighbors) {
            if (neighbor.movingEntity && neighbor.movingEntity.entitySO == GameManager.Instance.fishPrefab.entitySO) {
                Vector3 position = neighbor.transform.position;
                position.y -= 1;
                entitiesHarvestable.AddButton(neighbor.movingEntity, position);
            }
        }
    }
}
