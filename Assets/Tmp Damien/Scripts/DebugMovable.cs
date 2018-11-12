using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMovable : MonoBehaviour
{
    public enum DebugMode
    {
        Neighbors,
        Path,
        None
    }

    public Movable movable;
    public HexagonalGridPositions gridPositions;

    List<TileProperties> coloredTiles;

    DebugMode mode = DebugMode.Neighbors;

    public DebugMode Mode
    {
        get => mode;
        set => mode = value;
    }

    private void Start() {
        movable.DebugMovable = this;
        coloredTiles = new List<TileProperties>();
    }

    public void UpdateDebug() {
        for (int i = 0; i < coloredTiles.Count; i++) {

            coloredTiles[i].Tilemap.SetColor(coloredTiles[i].Position, Color.white);
        }
        coloredTiles.Clear();

        if (mode == DebugMode.Neighbors) {
            

            TileProperties[] neighbors = movable.TargetTile.GetNeighbors();
            for (int i = 0; i < 6; i++) {
                if (neighbors[i] != null) {
                    neighbors[i].Tilemap.SetColor(neighbors[i].Position, Color.red);
                    coloredTiles.Add(neighbors[i]);
                }
            }
        }
        if (mode == DebugMode.Path) {
            Stack<TileProperties> path = new Stack<TileProperties>(movable.Path);

            while (path.Count > 0) {
                TileProperties tileOnPath = path.Pop();
                tileOnPath.Tilemap.SetColor(tileOnPath.Position, Color.red);
                coloredTiles.Add(tileOnPath);
            }
        }
    }

    public void SwitchMode() {
        mode++;
        if ((int)mode == 2) {
            mode = (DebugMode)0;
        }
    }
}
    

