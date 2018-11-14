using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Movable))]
public class ReachableTilesDisplay : MonoBehaviour
{
    public GameObject edgePrefab;

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
        if (movable.Moving) {
            return;
        }
        this.movable = movable;
        this.reachables = reachables;
        displaying = true;
        for (int i = 0; i < reachables.Count; i++) {
            reachables[i].Tilemap.SetColor(reachables[i].Position, new Color(0.6f, 0.6f, 1f,1f));
        }
        //GetLimits();
    }

    public void GetLimits() {

        HexagonalGrid newGrid = new GameObject().AddComponent<HexagonalGrid>();
        for (int i = 0; i < reachables.Count; i++) {
            newGrid.AddTile(new GameObject().AddComponent<TileProperties>());
        }
        newGrid.SetNeighbors();

        Dictionary<TileProperties, HexDirection> limits;
        TileProperties farest = reachables[reachables.Count - 1];
        TileProperties currentLimit;
        foreach (TileProperties neighbor in farest.GetNeighbors()) {
            if (neighbor == null) {
                currentLimit = neighbor;
            }
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
                ColorPath(currentPath, new Color(0.6f, 0.6f, 1f, 1f));
            }
            currentTile = tile;
            if (reachables.Contains(currentTile)) {
                currentPath = AStarSearch.Path(movable.CurrentTile, tile);
                ColorPath(new Stack<TileProperties>(currentPath), new Color(1f, 0.6f, 0.6f, 1f));
            }
        }
    }
}
