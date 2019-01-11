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
        currentPointedTile = null;
        canvasPath.pathPoints = new Vector3[0];
        canvasPath.UpdatePath();
    }

    public void ColorPath(Stack<TileProperties> path) {
        canvasPath.pathPoints = new Vector3[path.Count];
        canvasPath.pathTiles = new TileProperties[path.Count];
        int i = 0;
        TileProperties previousTile = null;
        TileProperties currentTile;
        while (path.Count > 0) {
            currentTile = path.Pop();
            
            currentTile.ActionPointCost = i;
            previousTile = currentTile;
            canvasPath.pathPoints[i] = currentTile.Tilemap.GetCellCenterWorld(currentTile.Position);
            canvasPath.pathTiles[i++] = currentTile;
        }
        canvasPath.UpdatePath();
    }

    public void RefreshPath(TileProperties tile) {
        if (tile != currentPointedTile) {
            currentPointedTile = tile;
            currentPath = AStarSearch.Path(movable.CurrentTile, tile, null, motherShip.Movable);
            movable.Path = currentPath;
            ColorPath(new Stack<TileProperties>(currentPath));
        }
    }
}

