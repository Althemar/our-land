using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMovable : MonoBehaviour
{
    public enum DebugMode
    {
        Coordinates,
        Neighbors, 
        Distance,
        Path
    }

    public Movable movable;
    public HexagonalGridPositions gridPositions;

    List<TileProperties> coloredTiles;
    

    private void Start() {
        movable.DebugMovable = this;
        coloredTiles = new List<TileProperties>();
    }

    public void UpdateDebug(DebugMode mode = DebugMode.Neighbors) {
        for (int i = 0; i < coloredTiles.Count; i++) {

            coloredTiles[i].Tilemap.SetColor(coloredTiles[i].Position, Color.white);
        }
        coloredTiles.Clear();

        if (mode == DebugMode.Neighbors) {
            

            TileProperties[] neighbors = movable.CurrentTile.GetNeighbors();
            for (int i = 0; i < 6; i++) {
                if (neighbors[i] != null) {
                    neighbors[i].Tilemap.SetColor(neighbors[i].Position, Color.red);
                    coloredTiles.Add(neighbors[i]);
                }
            }
        }
        if (mode == DebugMode.Path) {
            Stack<TileProperties> path = movable.Path;

            while (path.Count > 0) {
                TileProperties tileOnPath = path.Pop();
                tileOnPath.Tilemap.SetColor(tileOnPath.Position, Color.red);
                coloredTiles.Add(tileOnPath);
            }
        }
    }

    
}
    

