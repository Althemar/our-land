using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HexagonalGrid : MonoBehaviour
{
    public Dictionary<Vector3Int, TileProperties> tiles;

    Tilemap tilemap;
    HexCoordinatesType coordinatesType;

    public Dictionary<Vector3Int, TileProperties> Tiles
    {
        get { return tiles; }
    }

    private void Awake() {
        tiles = new Dictionary<Vector3Int, TileProperties>();
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
            coordinates = HexCoordinates.GetOffsetCoordinates(coordinates); 
        }
        TileProperties tile;
        if (tiles.TryGetValue(coordinates.toVector3Int(), out tile)) {
            return tile;
        }
        else {
            return null;
        }
    }

    public void ChangeCoordinateSystem(HexCoordinatesType type) {
        foreach (KeyValuePair<Vector3Int, TileProperties> tile in tiles) {
            tile.Value.Coordinates.ChangeCoordinatesType(type);
        }
        coordinatesType = type;
    }
}

