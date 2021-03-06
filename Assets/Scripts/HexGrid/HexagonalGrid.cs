﻿using System.Collections.Generic;
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

    public int layerID;

    public Grid grid;
    public TileProperties[,] tilesArray;
    private Vector3Int arrayOffset;

    private Tilemap tilemap;
    private HexMetrics metrics;

    public HumidityGrid humidity;
    public GridOutline outline;

    public GameObject TileGameObject;

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
            layerID = GetComponent<TilemapRenderer>().sortingLayerID;
            metrics = new HexMetrics(grid.cellSize.y / 2, grid.cellSize.x / 2);

            tilemap.CompressBounds();
            tilesArray = new TileProperties[tilemap.cellBounds.size.x + 2, tilemap.cellBounds.size.y + 2];

            arrayOffset = tilemap.cellBounds.position - new Vector3Int(1, 1, 0);
            for (int i = 0; i < tilesArray.GetLength(0); i++) {
                for (int j = 0; j < tilesArray.GetLength(1); j++) {
                    Vector3 position = tilemap.GetCellCenterWorld(new Vector3Int(i + arrayOffset.x, j + arrayOffset.y, 0));
                    TileProperties prop = Instantiate(TileGameObject, position, Quaternion.identity, transform).GetComponent<TileProperties>();
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
        if (GameManager.Instance.FrameCount == 1) {
            //SetNeighbors();
            //SetBorders();
            if(humidity)
                humidity.Compute();
        }

        if (humidity && Input.GetKeyDown(KeyCode.R))
            humidity.Compute();
    }

    public void SetColor(Vector3Int coordinates, Color col) {
        tilemap.SetColor(coordinates, col);
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

    // Must be offset coordinates
    public void SetTileColor(Vector3Int coordinates, Color c) {
        if (coordinates.x - arrayOffset.x < 0 || coordinates.x - arrayOffset.x >= tilesArray.GetLength(0))
            return;
        if (coordinates.y - arrayOffset.y < 0 || coordinates.y - arrayOffset.y >= tilesArray.GetLength(1))
            return;
        outline.SetTileColor(coordinates.x - arrayOffset.x, coordinates.y - arrayOffset.y, c);
    }

    // Must be offset coordinates
    public void ResetTileColor(Vector3Int coordinates) {
        if (coordinates.x - arrayOffset.x < 0 || coordinates.x - arrayOffset.x >= tilesArray.GetLength(0))
            return;
        if (coordinates.y - arrayOffset.y < 0 || coordinates.y - arrayOffset.y >= tilesArray.GetLength(1))
            return;
        outline.ResetTileColor(coordinates.x - arrayOffset.x, coordinates.y - arrayOffset.y);
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
                if (tilesArray[i, j] != null) {
                    tilesArray[i, j].SetAddon();
                }
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

