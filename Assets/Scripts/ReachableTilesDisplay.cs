using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * ReachableTilesDisplay
 * Display a preview for the movement : )
 *      - Color the tiles in the path to the pointed tile
 */

public class ReachableTilesDisplay : MonoBehaviour
{
    private bool displaying;
    private Movable movable;
    private TileProperties currentPointedTile;
    private MotherShip motherShip;

    private Stack<TileProperties> currentPath;

    public PathUI canvasPath;

    public bool Displaying
    {
        get => displaying;
    }

    private void Start() {
        motherShip = GetComponent<MotherShip>();
    }

    public void InitReachableTiles(TileProperties tile, Movable movable) {
        if (movable.Moving) {
            return;
        }
        this.movable = movable;
        displaying = true;
    }

    public void UndisplayReachables() {
        displaying = false;
        canvasPath.pathPoints = new Vector3[0];
        canvasPath.UpdatePath();
    }

    public void ColorPath(Stack<TileProperties> path) {
        canvasPath.pathPoints = new Vector3[path.Count];
        canvasPath.pathTiles = new TileProperties[path.Count];
        int i = 0;
        while (path.Count > 0) {
            TileProperties tile = path.Pop();
            if (i <= 1) {
                tile.ActionPointCost = 0;
            }
           /* else {
                tile.ActionPointCost = motherShip.movementBaseCost
            }
            tile.ActionPointCost = i * motherShip*/
            canvasPath.pathPoints[i] = tile.Tilemap.GetCellCenterWorld(tile.Position);
            canvasPath.pathTiles[i++] = tile;
        }
        canvasPath.UpdatePath();
    }

    public void RefreshPath(TileProperties tile) {
        if (tile != currentPointedTile) {
            currentPointedTile = tile;
            currentPath = AStarSearch.Path(movable.CurrentTile, tile);
            movable.Path = currentPath;
            ColorPath(new Stack<TileProperties>(currentPath));
        }
    }
}

