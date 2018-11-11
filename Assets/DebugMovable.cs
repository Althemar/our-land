using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMovable : MonoBehaviour
{
    public enum DebugMode
    {
        Coordinates,
        Neighbors
    }

    List<TileProperties> coloredTiles;

    DebugMode mode = DebugMode.Neighbors;
    Movable movable;

    private void Start() {
        movable = GetComponent<Movable>();
        coloredTiles = new List<TileProperties>();
    }

    public void UpdateDebug() {
        if (mode == DebugMode.Neighbors) {
            for (int i = 0; i < coloredTiles.Count; i++) {

                coloredTiles[i].Tilemap.SetColor(coloredTiles[i].Position, Color.white);
            }
            coloredTiles.Clear();

            TileProperties[] neighbors = movable.CurrentTile.GetNeighbors();
            for (int i = 0; i < 6; i++) {
                if (neighbors[i] != null) {
                    neighbors[i].Tilemap.SetColor(neighbors[i].Position, Color.red);
                    coloredTiles.Add(neighbors[i]);
                }
            }
        }
    }

    
}
    

