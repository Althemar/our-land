using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileProperties : MonoBehaviour
{
    Vector3Int position;
    HexCoordinates coordinates;
    CustomTile tile;
    HexagonalGrid grid;
    Tilemap tilemap;

    public TileProperties[] neighbors;

    static Vector3Int[] cubeDirections = { new Vector3Int(0, 1, -1), new Vector3Int(1, 0, -1), new Vector3Int(1, -1, 0),
                                           new Vector3Int(0, -1, 1), new Vector3Int(-1, 0, 1), new Vector3Int(-1, 1, 0)
                                         };
    

    public HexCoordinates Coordinates
    {
        get { return coordinates; }
    }

    public Vector3Int Position
    {
        get { return position; }
        set {
            position = value;
            coordinates = new HexCoordinates(position.x, position.y);
        }
    }

    public CustomTile Tile
    {
        get { return tile; }
        set { tile = value; }
    }

    public Tilemap Tilemap
    {
        get { return tilemap; }
    }



    private void Awake() {
        neighbors = new TileProperties[6];
    }

    public void InitializeTile(Vector3Int position, CustomTile tile, HexagonalGrid grid, ITilemap tilemap) {
        this.position = position;
        coordinates = new HexCoordinates(position.x, position.y);
        this.tile = tile;
        this.grid = grid;
        this.tilemap = tilemap.GetComponent<Tilemap>();
    }

    public TileProperties GetNeighbor(HexDirection direction) {
        return neighbors[(int)direction];
    }

    public TileProperties[] GetNeighbors() {
        return neighbors;
    }

    void SetNeighbor(HexDirection direction, TileProperties cell) {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }

    public void SetNeighbors() {
        for (int i = 0; i < 6; i++) {
            HexCoordinates direction = new HexCoordinates(cubeDirections[i]);
            //direction.ChangeCoordinatesType(HexCoordinatesType.offset);
            HexCoordinates cubicPosition = new HexCoordinates(position.x, position.y);
            cubicPosition.ChangeCoordinatesType(HexCoordinatesType.cubic);
            TileProperties tile = grid.GetTile(new HexCoordinates(cubicPosition.toVector3Int() + direction.toVector3Int()));
            if (tile != null) {
                SetNeighbor((HexDirection)i, tile);
            }
        }
    }
}
