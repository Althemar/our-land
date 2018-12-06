using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

/*
 * HexagonalGrid
 * Contains informations about the grid
 */

public class HexagonalGrid : MonoBehaviour {
    /*
     * Members
     */
    public static HexagonalGrid Instance;

    public Grid grid;
    public TileProperties[,] tilesArray;
    private Vector3Int arrayOffset;

    private Tilemap tilemap;
    private HexMetrics metrics;

    public CustomTile lowHumidityTile;
    public CustomTile normalHumidityTile;
    public CustomTile highHumidityTile;

    /*
     * Properties
     */
     
    public Tilemap Tilemap {
        get => tilemap;
    }

    public HexMetrics Metrics {
        get => metrics;
    }

    /*
     * Methods
     */

    private void Awake() {
        if (Instance == null) {
            Instance = this;

            tilemap = GetComponent<Tilemap>();
            metrics = new HexMetrics(grid.cellSize.y / 2, grid.cellSize.x / 2);

            tilemap.CompressBounds();
            tilesArray = new TileProperties[tilemap.cellBounds.size.x + 2, tilemap.cellBounds.size.y + 2];

            arrayOffset = tilemap.cellBounds.position - new Vector3Int(1, 1, 0);
            for (int i = 0; i < tilesArray.GetLength(0); i++) {
                for (int j = 0; j < tilesArray.GetLength(1); j++) {
                    GameObject obj = new GameObject("Tile Data");
                    obj.transform.SetParent(this.transform);
                    obj.transform.localPosition = tilemap.GetCellCenterWorld(new Vector3Int(i + arrayOffset.x, j + arrayOffset.y, 0));
                    TileProperties prop = obj.AddComponent<TileProperties>();
                    prop.InitializeTile(new Vector3Int(i + arrayOffset.x, j + arrayOffset.y, 0), this, tilemap);
                    tilesArray[i, j] = prop;
                }
            }
            SetNeighbors();
        }
        else {
            Destroy(gameObject);
        }
    }

    private void Update() {
        if (Time.frameCount == 1) {
            //SetNeighbors();
            //SetBorders();
            ComputeHumidity();
        }

        if (Input.GetKey(KeyCode.R))
            ComputeHumidity();
    }

    void ComputeHumidity() {
        //tilemap
        PriorityQueue<River> riverList = new PriorityQueue<River>();

        for (int i = 0; i < tilesArray.GetLength(0); i++) {
            for (int j = 0; j < tilesArray.GetLength(1); j++) {
                TileProperties prop = tilesArray[i, j];
                if (prop != null) {
                    if (prop.Tile && tilesArray[i, j].Tile.riverSource) {
                        River R = new River(new Vector3Int(i, j, 0), HexDirection.SW, HexDirection.E);
                        R.distance = 0;
                        riverList.Enqueue(R, R.distance);
                    }
                }
            }
        }

        while(riverList.Count > 0) {
            River riv = riverList.Dequeue();

            if (riv.source.x < 0 || riv.source.x >= tilesArray.GetLength(0))
                continue;
            if (riv.source.y < 0 || riv.source.y >= tilesArray.GetLength(1))
                continue;

            TileProperties neigh = tilesArray[riv.source.x, riv.source.y].GetNeighbor(riv.directionFirst);
            neigh.SetRiver(riv.directionSecond);

            tilemap.SetColor(neigh.Position, Color.red);

            riv.source = new Vector3Int(neigh.Position.x - arrayOffset.x, neigh.Position.y - arrayOffset.y, 0);

            HexDirection tmp = riv.directionFirst;
            riv.directionFirst = riv.directionSecond;
            riv.directionSecond = tmp;

            riv.distance++;

            if(riv.distance < 10)
                riverList.Enqueue(riv, riv.distance);
        }

        for (int i = 0; i < tilesArray.GetLength(0); i++) {
            for (int j = 0; j < tilesArray.GetLength(1); j++) {
                if (tilesArray[i, j] != null) {
                    if(tilesArray[i, j].Tile && tilesArray[i, j].Tile.humidityDependant) {
                        if (tilesArray[i, j].humidity < 0.1f)
                            tilemap.SetTile(new Vector3Int(i + arrayOffset.x, j + arrayOffset.y, 0), lowHumidityTile);
                        else if (tilesArray[i, j].humidity > 0.9f)
                            tilemap.SetTile(new Vector3Int(i + arrayOffset.x, j + arrayOffset.y, 0), highHumidityTile);
                        else
                            tilemap.SetTile(new Vector3Int(i + arrayOffset.x, j + arrayOffset.y, 0), normalHumidityTile);
                    }
                }
            }
        }

        tilemap.RefreshAllTiles();
        ResetTiles();
        SetAddons();
        SetBorders();
        PutRivers();
    }

    public void AddTile(TileProperties tile) {
        if (tilesArray != null) {
            tilesArray[tile.Position.x - arrayOffset.x, tile.Position.y - arrayOffset.y] = tile;

            tile.Grid = this;
        }
    }

    public TileProperties GetTile(HexCoordinates coordinates) {
        return GetTile(coordinates.OffsetCoordinates);
    }

    // Must be offset coordinates
    public TileProperties GetTile(Vector3Int coordinates) {
        if (coordinates.x - arrayOffset.x < 0 || coordinates.x - arrayOffset.x >= tilesArray.GetLength(0))
            return null;
        if (coordinates.y - arrayOffset.y < 0 || coordinates.y - arrayOffset.y >= tilesArray.GetLength(1))
            return null;
        return tilesArray[coordinates.x - arrayOffset.x, coordinates.y - arrayOffset.y];
    }

    public void SetNeighbors() {
        for (int i = 0; i < tilesArray.GetLength(0); i++) {
            for (int j = 0; j < tilesArray.GetLength(1); j++) {
                if (tilesArray[i, j] != null)
                    tilesArray[i, j].SetNeighbors();
            }
        }
    }

    public void ResetTiles() {
        for (int i = 0; i < tilesArray.GetLength(0); i++) {
            for (int j = 0; j < tilesArray.GetLength(1); j++) {
                if (tilesArray[i, j] != null)
                    tilesArray[i, j].ResetTile();
            }
        }
    }

    public void SetBorders() {
        for (int i = 0; i < tilesArray.GetLength(0); i++) {
            for (int j = 0; j < tilesArray.GetLength(1); j++) {
                if (tilesArray[i, j] != null)
                    tilesArray[i, j].SetBorders();
            }
        }
    }

    public void PutRivers() {
        for (int i = 0; i < tilesArray.GetLength(0); i++) {
            for (int j = 0; j < tilesArray.GetLength(1); j++) {
                if (tilesArray[i, j] != null)
                    tilesArray[i, j].PutRivers();
            }
        }
    }
    public void SetAddons() {
        for (int i = 0; i < tilesArray.GetLength(0); i++) {
            for (int j = 0; j < tilesArray.GetLength(1); j++) {
                if (tilesArray[i, j] != null)
                    tilesArray[i, j].SetAddon();
            }
        }
    }

    public TileProperties GetRandomTile() {
        if (tilesArray.Length == 0) {
            return null;
        }
        return tilesArray[UnityEngine.Random.Range(0, tilesArray.GetLength(0)), UnityEngine.Random.Range(0, tilesArray.GetLength(1))];
    }
}

