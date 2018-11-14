using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/*
 * HexagonalGrid
 * Contains informations about the grid
 */

public class HexagonalGrid : MonoBehaviour
{
    /*
     * Members
     */

    public Grid grid;
    public Dictionary<Vector3Int, TileProperties> tiles; // Key : Tile position in offset, Value : tile

    Tilemap tilemap;
    HexCoordinatesType coordinatesType;
    HexMetrics metrics;

    /*
     * Properties
     */

    public Dictionary<Vector3Int, TileProperties> Tiles
    {
        get => tiles;
    }

    public Tilemap Tilemap
    {
        get => tilemap;
    }

    public HexMetrics Metrics
    {
        get => metrics;
    }

    /*
     * Methods
     */
   
    private void Awake() {
        tiles = new Dictionary<Vector3Int, TileProperties>();
        tilemap = GetComponent<Tilemap>();
        coordinatesType = HexCoordinatesType.offset;
        metrics = new HexMetrics(grid.cellSize.y / 2);
    }

    private void Update() {
        if (Time.frameCount == 1) {
            SetNeighbors();
        }
    }

    public void AddTile(TileProperties tile) {
        if (tiles != null) {
            tiles.Add(tile.Position, tile);
            tile.Grid = this;
        }
    }

    public TileProperties GetTile(HexCoordinates coordinates) {
        if (coordinates.coordinatesType != HexCoordinatesType.offset) {
            coordinates.ChangeCoordinatesType(HexCoordinatesType.offset); 
        }
        return GetTile(coordinates.ToVector3Int());
    }

    // Must be offset coordinates
    public TileProperties GetTile(Vector3Int coordinates) {
        tiles.TryGetValue(coordinates, out TileProperties tile);
        return tile;
    }

    public void SetNeighbors() {
        foreach (KeyValuePair<Vector3Int, TileProperties> tile in tiles) {
            tile.Value.SetNeighbors();
        }
    }

    public void ChangeCoordinateSystem(HexCoordinatesType type) {
        foreach (KeyValuePair<Vector3Int, TileProperties> tile in tiles) {
            tile.Value.Coordinates.ChangeCoordinatesType(type);
        }
        coordinatesType = type;
    }
}

