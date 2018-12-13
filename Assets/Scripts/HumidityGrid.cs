using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumidityGrid : MonoBehaviour
{
    public int riverRadius = 5;
    public int riverForce = 5;

    public CustomTile lowHumidityTile;
    public CustomTile normalHumidityTile;
    public CustomTile highHumidityTile;

    public Sprite lake;
    public Sprite triLakeN;
    public Sprite triLakeSW;
    public Sprite triLakeSE;

    public Sprite NERiver;
    public Sprite ERiver;
    public Sprite SERiver;

    public Sprite EInterNE;
    public Sprite EInterNW;
    public Sprite EInterNEW;
    public Sprite ERivN;

    public Sprite EInterSE;
    public Sprite EInterSW;
    public Sprite EInterSEW;
    public Sprite ERivS;


    public Sprite NERivNW;
    public Sprite NERivSE;

    public Sprite SERivNE;
    public Sprite SERivSW;

    private HexagonalGrid grid;

    private void Awake() {
        grid = GetComponent<HexagonalGrid>();
    }

    public void Compute() {
        grid.Tilemap.RefreshAllTiles();

        for (int i = 0; i < grid.tilesArray.GetLength(0); i++) {
            for (int j = 0; j < grid.tilesArray.GetLength(1); j++) {
                if (grid.tilesArray[i, j] != null) {
                    grid.tilesArray[i, j].ResetRiver();
                    grid.tilesArray[i, j].windDryness = 0;
                    grid.tilesArray[i, j].nextTilesInCorridor.Clear();
                }
            }
        }

        List<River> riverList = new List<River>();

        for (int i = 0; i < grid.tilesArray.GetLength(0); i++) {
            for (int j = 0; j < grid.tilesArray.GetLength(1); j++) {
                TileProperties prop = grid.tilesArray[i, j];
                if (prop != null) {
                    if (prop.Tile && prop.Tile.riverSource) {
                        River R = new River(grid.tilesArray[i, j].Coordinates, prop.Tile.riverDirection, prop.Tile.riverCounterClockwise, riverForce);
                        riverList.Add(R);
                    }
                }
            }
        }

        bool extend;
        do {
            extend = false;
            foreach (River r in riverList) {
                if (r.force > 0) {
                    r.ExtendRiver(grid);
                    extend = true;
                }
            }
        } while (extend);

        foreach (River r in riverList) {
            r.PutLac(grid);
        }


        for (int i = 0; i < grid.tilesArray.GetLength(0); i++) {
            for (int j = 0; j < grid.tilesArray.GetLength(1); j++) {
                TileProperties prop = grid.tilesArray[i, j];
                if (prop != null && (prop.asLake || prop.asRiver)) {
                    prop.humidity = riverRadius;
                    //this.SetColor(prop.Coordinates.OffsetCoordinates, Color.red);
                }
            }
        }

        for (int b = 0; b < riverRadius; b++) {
            for (int i = 0; i < grid.tilesArray.GetLength(0); i++) {
                for (int j = 0; j < grid.tilesArray.GetLength(1); j++) {
                    TileProperties prop = grid.tilesArray[i, j];
                    if (prop != null) {
                        foreach (var N in prop.GetNeighbors()) {
                            if (!N)
                                continue;
                            N.humidity = Mathf.Max(prop.humidity - 1, N.humidity);
                        }
                    }
                }
            }
        }

        ComputeDryness();

        UpdateTiles();

    }

    public void ComputeDryness() {
        foreach (WindOrigin wo in WindManager.Instance.windOrigins) {
            wo.ComputeWindCorridor();
        }
    }

    public void UpdateTile(TileProperties tile) {
        CustomTile previousCustomTile = tile.Tile;
        UpdateCustomTile(tile);
        if (previousCustomTile != tile.Tile) {
            tile.ResetTile();
            tile.SetAddon();
            foreach (TileProperties neighbor in tile.GetNeighbors()) {
                neighbor.ResetTile();
                neighbor.SetBorders();
                neighbor.PutRivers();
            }
            tile.SetBorders();
            tile.PutRivers();
        }
    }

    public void UpdateTiles() {
        CustomTile previousCustomTile = tile.Tile;

        for (int i = 0; i < grid.tilesArray.GetLength(0); i++) {
            for (int j = 0; j < grid.tilesArray.GetLength(1); j++) {
                if (grid.tilesArray[i, j] != null) {
                    UpdateCustomTile(grid.tilesArray[i, j]);
                }
            }
        }
        grid.Tilemap.RefreshAllTiles();
        grid.ResetTiles();
        grid.SetAddons();
        grid.SetBorders();
        grid.PutRivers();
    }

    public void UpdateCustomTile(TileProperties tile) {
        if (tile.Tile && tile.Tile.humidityDependant) {
            if (tile.humidity < 1)
                grid.Tilemap.SetTile(tile.Coordinates.OffsetCoordinates, lowHumidityTile);
            else if (tile.humidity > 6)
                grid.Tilemap.SetTile(tile.Coordinates.OffsetCoordinates, highHumidityTile);
            else
                grid.Tilemap.SetTile(tile.Coordinates.OffsetCoordinates, normalHumidityTile);
        }
    }
}
