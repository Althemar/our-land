using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Flee")]
public class FleeAction : Action
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
                if (fleeTile && !fleeTile.currentMovable && fleeTile.IsWalkable() && entity.movingEntitySO.availableTiles.Contains(fleeTile.Tile)) {
                   entity.MoveTo(fleeTile, null);
                   entity.hasFled = true;
                    return;
                }
            }
            
        }

    }

    public override void OnEnterState(StateController controller) {
    }

    public override void OnExitState(StateController controller) {
    }
    
}
