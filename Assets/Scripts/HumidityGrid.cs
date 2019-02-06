using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumidityGrid : MonoBehaviour
{
    public int riverRadius = 5;
    public int riverForce = 5;
    public int normalHumityTreshold;
    public int highHumityTreshold;

    public CustomTile lowHumidityTile;
    public CustomTile normalHumidityTile;
    public CustomTile highHumidityTile;

    public Sprite lake;
    public Sprite triLakeN;
    public Sprite triLakeSW;
    public Sprite triLakeSE;
    public Sprite triLakeS;
    public Sprite triLakeNW;
    public Sprite triLakeNE;
    
    public Sprite glacier;

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


    public Sprite NEInterNW;
    public Sprite NWInterNE;

    public Sprite NERivNW;
    public Sprite NERivSE;

    public Sprite SERivNE;
    public Sprite SERivSW;

    private HexagonalGrid grid;

    private void Awake() {
        grid = GetComponent<HexagonalGrid>();

        Console.AddCommand("computeHumidity", CmdCompute, "Recompute the humidity");
    }

    void CmdCompute(string[] args) {
        if (args.Length == 1) {
            int n = 0;
            if (!int.TryParse(args[0], out n)) {
                Console.Write("Error: Invalid amount");
                return;
            }

            Compute(n);
        }
        else {
            Console.Write("Usage: computeHumidity [n] \nCompute humidity at step n");
        }
    }

    public void Compute(int debug = -1) {

        foreach (WindOrigin wo in WindManager.Instance.windOrigins) {
            //wo.InitCorridor();
        }

        for (int i = 0; i < grid.tilesArray.GetLength(0); i++) {
            for (int j = 0; j < grid.tilesArray.GetLength(1); j++) {
                if (grid.tilesArray[i, j] != null) {
                    grid.tilesArray[i, j].Tile = grid.Tilemap.GetTile(grid.tilesArray[i, j].Coordinates.OffsetCoordinates) as CustomTile;
                    grid.tilesArray[i, j].windDryness = 0;
                    grid.tilesArray[i, j].nextTilesInCorridor.Clear();
                }
            }
        }

        for (int i = 0; i < grid.tilesArray.GetLength(0); i++) {
            for (int j = 0; j < grid.tilesArray.GetLength(1); j++) {
                if (grid.tilesArray[i, j] != null) {
                    grid.tilesArray[i, j].ResetRiver();
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
        int loop = 0;
        do {
            if (debug != -1 && loop >= debug)
                break;

            extend = false;
            River best = null;
            foreach (River r in riverList) {
                if (r.force > 0) {
                    if(best == null || best.force < r.force)
                        best = r;
                }
            }
            if(best != null) {
                best.ExtendRiver(grid);
                extend = true;
            }
            loop++;
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

        //ComputeDryness();

        UpdateTiles();

    }

    public void ComputeDryness() {
        foreach (WindOrigin wo in WindManager.Instance.windOrigins) {
           // wo.ComputeWindCorridor();
        }
    }

    public void ComputeWinds() {
        foreach (WindOrigin wo in WindManager.Instance.windOrigins) {
           // wo.InitCorridor();
        }

        ComputeDryness();
    }

    public void UpdateTile(TileProperties tile) {
        CustomTile previousCustomTile = tile.Tile;
        //UpdateCustomTile(tile);
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
        /*
        for (int i = 0; i < grid.tilesArray.GetLength(0); i++) {
            for (int j = 0; j < grid.tilesArray.GetLength(1); j++) {
                if (grid.tilesArray[i, j] != null) {
                    if (grid.tilesArray[i, j].Tile && grid.tilesArray[i, j].Tile.humidityDependant) {
                        if (grid.tilesArray[i, j].humidity < 1) {
                            grid.tilesArray[i, j].needRefresh |= grid.tilesArray[i, j].Tile != lowHumidityTile;
                            grid.Tilemap.SetTile(grid.tilesArray[i, j].Coordinates.OffsetCoordinates, lowHumidityTile);
                        }
                        else if (grid.tilesArray[i, j].humidity > 6) {
                            grid.tilesArray[i, j].needRefresh |= grid.tilesArray[i, j].Tile != highHumidityTile;
                            grid.Tilemap.SetTile(grid.tilesArray[i, j].Coordinates.OffsetCoordinates, highHumidityTile);
                        }
                        else {
                            grid.tilesArray[i, j].needRefresh |= grid.tilesArray[i, j].Tile != normalHumidityTile;
                            grid.Tilemap.SetTile(grid.tilesArray[i, j].Coordinates.OffsetCoordinates, normalHumidityTile);
                        }
                    }
                }
            }
        }*/

        for (int i = 0; i < grid.tilesArray.GetLength(0); i++) {
            for (int j = 0; j < grid.tilesArray.GetLength(1); j++) {
                if (grid.tilesArray[i, j] != null && grid.tilesArray[i, j].needRefresh) {
                    grid.Tilemap.RefreshTile(grid.tilesArray[i, j].Coordinates.OffsetCoordinates);

                    grid.tilesArray[i, j].ResetAddon();
                    grid.tilesArray[i, j].SetAddon();
                    grid.tilesArray[i, j].needRefresh = false;
                }
                grid.tilesArray[i, j].ResetTile();
                grid.tilesArray[i, j].SetBorders();
                grid.tilesArray[i, j].PutLake();
                grid.tilesArray[i, j].PutRivers();
                grid.tilesArray[i, j].PutGlacier();
            }
        }
    }

    public void UpdateCustomTile(TileProperties tile) {
        if (tile.Tile && tile.Tile.humidityDependant) {
            if (tile.humidity < normalHumityTreshold)
                grid.Tilemap.SetTile(tile.Coordinates.OffsetCoordinates, lowHumidityTile);
            else if (tile.humidity > highHumityTreshold)
                grid.Tilemap.SetTile(tile.Coordinates.OffsetCoordinates, highHumidityTile);
            else
                grid.Tilemap.SetTile(tile.Coordinates.OffsetCoordinates, normalHumidityTile);
        }
    }
}
