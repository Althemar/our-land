using System;
using System.Collections.Generic;
using UnityEngine;

public class DebugMovable : MonoBehaviour
{
    public enum DebugMode
    {
        Neighbors,
        Path
    }

    private Movable movable;
    private List<TileProperties> coloredTiles;
    private bool displayDebug = false;

    DebugMode mode = DebugMode.Neighbors;

    public DebugMode Mode
    {
        get => mode;
        set => mode = value;
    }

    private void Start() {
        movable = GetComponent<Movable>();
        movable.DebugMovable = this;
        coloredTiles = new List<TileProperties>();
    }

    public void UpdateDebug() {
        if (!displayDebug) {
            return;
        }

        ClearTiles();
       
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
            /*Stack<TileProperties> path = new Stack<TileProperties>(movable.Path);
            while (path.Count > 0) {
                TileProperties tileOnPath = path.Pop();
                tileOnPath.Tilemap.SetColor(tileOnPath.Position, Color.red);
                coloredTiles.Add(tileOnPath);
            }*/
        }
    }

    public void ActivateDebug() {
        ClearTiles();
        displayDebug = !displayDebug;
    }

    private void ClearTiles() {
        for (int i = 0; i < coloredTiles.Count; i++) {

            coloredTiles[i].Tilemap.SetColor(coloredTiles[i].Position, Color.white);
        }
        coloredTiles.Clear();
    }

    public void SwitchMode() {
        mode++;
        if ((int)mode == Enum.GetNames(typeof(DebugMode)).Length) {
            mode = 0;
        }
    }
}
    

