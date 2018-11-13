using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Movable))]
public class ReachableTilesDisplay : MonoBehaviour
{
    List<TileProperties> reachables;
    Stack<TileProperties> currentPath;
    bool displaying;
    Movable movable;
    TileProperties currentTile;


    public bool Displaying
    {
        get => displaying;
    }

    public TileProperties CurrentTile
    {
        get => currentTile;
    }

    private void Start() {
        movable = GetComponent<Movable>();
    }


    public void InitReachableTiles(List<TileProperties> reachables, TileProperties tile, Movable movable) {
        this.movable = movable;
        this.reachables = reachables;
        displaying = true;
        for (int i = 0; i < reachables.Count; i++) {
            reachables[i].Tilemap.SetColor(reachables[i].Position, new Color(0,0,100,0.5f));
        }
    }

    public void UndisplayReachables() {
        displaying = false;
        for (int i = 0; i < reachables.Count; i++) {
            reachables[i].Tilemap.SetColor(reachables[i].Position, Color.white);
        }
    }

    public void ColorPath(Stack<TileProperties> path, Color color) {
        while (path.Count > 0) {
            TileProperties tile = path.Pop();
            tile.Tilemap.SetColor(tile.Position, color);
        }
    }

    public void RefreshPath(TileProperties tile) {
        if (tile != currentTile) {
            if (reachables.Contains(currentTile)) {
                ColorPath(currentPath, new Color(0, 0, 100, 0.5f));
            }
            currentTile = tile;
            if (reachables.Contains(currentTile)) {
                currentPath = AStarSearch.Path(movable.CurrentTile, tile);
                ColorPath(new Stack<TileProperties>(currentPath), new Color(100, 0, 0, 0.5f));
            }
        }
    }
}
