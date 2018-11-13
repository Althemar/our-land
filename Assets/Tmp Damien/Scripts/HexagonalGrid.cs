using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/*
 * HexagonalGrid
 * Contains informations about the grid
 */

[RequireComponent(typeof(Tilemap))]
public class HexagonalGrid : MonoBehaviour
{
    /*
     * Members
     */

    public Dictionary<Vector3Int, TileProperties> tiles; // Key : Tile position in offset, Value : tile

    Tilemap tilemap;
    HexCoordinatesType coordinatesType;

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

    /*
     * Methods
     */
   
    private void Awake() {
        tiles = new Dictionary<Vector3Int, TileProperties>();
        tilemap = GetComponent<Tilemap>();
        coordinatesType = HexCoordinatesType.offset;
    }

    private void Update() {
        if (Time.frameCount == 1) {
            foreach (KeyValuePair<Vector3Int, TileProperties> tile in tiles) {
                tile.Value.SetNeighbors();
            }
        }
    }

    public void AddTile(TileProperties tile) {
        if (tiles != null) {
            tiles.Add(tile.Position, tile);
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
        TileProperties tile = null;
        tiles.TryGetValue(coordinates, out tile);
        return tile;
    }

    public void ChangeCoordinateSystem(HexCoordinatesType type) {
        foreach (KeyValuePair<Vector3Int, TileProperties> tile in tiles) {
            tile.Value.Coordinates.ChangeCoordinatesType(type);
        }
        coordinatesType = type;
    }
}

