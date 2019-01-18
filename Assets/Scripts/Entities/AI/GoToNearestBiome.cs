using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/GoToNearestTile")]
public class GoToNearestBiome : Action
{
    public List<CustomTile.TerrainType> terrains;
    public bool goOnTile = true;
    public int range;

    private MovingEntity entity;

    public override void Act(StateController controller) {
        entity = controller.entity as MovingEntity;
        HashSet<TileProperties> visited = new HashSet<TileProperties>();
        visited.Add(entity.Tile);

        Queue<TileProperties> fringes = new Queue<TileProperties>();

        fringes.Enqueue(entity.Tile);
        while (fringes.Count > 0) {
            TileProperties previousTile = fringes.Dequeue();
            TileProperties[] neighbors = previousTile.GetNeighbors();
            for (int j = 0; j < neighbors.Length; j++) {
                TileProperties neighbor = neighbors[j];
                if (neighbor && neighbor.Tile && !visited.Contains(neighbor)) {
                    if (terrains.Contains(neighbor.Tile.terrainType)) {
                        TryMoveToBiome(neighbor);
                        return;
                    }
                    else {
                        fringes.Enqueue(neighbor);
                    }
                    visited.Add(neighbor);
                }
            }
        }
    }

    public void TryMoveToBiome(TileProperties tileToGo) {
        if(tileToGo.Coordinates.Distance(entity.Tile.Coordinates) <= range) {
            return;
        }
        List<TileProperties> inRange = tileToGo.InRange(range);
        inRange.Shuffle();
        foreach (TileProperties tile in inRange) {
            if ((!goOnTile && tile == tileToGo) || !tile.Tile || !tile.IsWalkable() || tile.asLake || tile.windOrigin ) {
                continue;
            }
            entity.MoveTo(tile, null);
            break;
        }
    }


    public override void OnEnterState(StateController controller) {
    }

    public override void OnExitState(StateController controller) {
    }
}
