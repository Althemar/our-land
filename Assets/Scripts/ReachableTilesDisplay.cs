using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * ReachableTilesDisplay
 * Display a preview for the movement : )
 *      - Color the tiles in the path to the pointed tile
 */
 
[RequireComponent(typeof(HexagonsOutline))]
public class ReachableTilesDisplay : MonoBehaviour
{
    private bool displaying;                   
    private Movable movable;                    
    private TileProperties currentPointedTile;      // current pointed tile by the mouse. used to refresh the path   

    private List<TileProperties> reachables;    
    private Stack<TileProperties> currentPath;  

    public PathUI canvasPath;
    public HexagonsOutline outline;

    public bool Displaying
    {
        get => displaying;
    }

    public TileProperties CurrentTile
    {
        get => currentPointedTile;
    }

    private void Start() {
        outline = GetComponent<HexagonsOutline>();
    }


    public void InitReachableTiles(List<TileProperties> reachables, TileProperties tile, Movable movable) {
        if (movable.Moving) {
            return;
        }
        this.movable = movable;
        this.reachables = reachables;
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
            canvasPath.pathPoints[i] = tile.Tilemap.GetCellCenterWorld(tile.Position);
            canvasPath.pathTiles[i++] = tile;
        }
        canvasPath.UpdatePath();
    }

    public void RefreshPath(TileProperties tile) {
        if (tile != currentPointedTile) {
            currentPointedTile = tile;
            if (reachables.Contains(currentPointedTile)) {
                currentPath = AStarSearch.Path(movable.CurrentTile, tile);
                ColorPath(new Stack<TileProperties>(currentPath));
            } else {
                canvasPath.pathPoints = new Vector3[0];
                canvasPath.UpdatePath();
            }
        }
    }
}
