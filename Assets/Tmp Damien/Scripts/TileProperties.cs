using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileProperties : MonoBehaviour
{
    /*
     * Members
     */

    public TileProperties[] neighbors;

    private Vector3Int position;
    private HexCoordinates coordinates;
    private CustomTile tile;
    private HexagonalGrid grid;
    private Tilemap tilemap;

    private bool isInReachables;

    private static Vector3Int[] cubeDirections = { new Vector3Int(0, 1, -1), new Vector3Int(1, 0, -1), new Vector3Int(1, -1, 0),
                                           new Vector3Int(0, -1, 1), new Vector3Int(-1, 0, 1), new Vector3Int(-1, 1, 0)
                                         };

    /*
     * Properties
     */

    public HexCoordinates Coordinates
    {
        get => coordinates;
    }

    public Vector3Int Position
    {
        get => position;
        set {
            position = value;
            coordinates = new HexCoordinates(position.x, position.y);
        }
    }

    public CustomTile Tile
    {
        get => tile;
        set => tile = value;
    }

    public Tilemap Tilemap
    {
        get => tilemap;
    }

    public HexagonalGrid Grid
    {
        get => grid;
        set => grid = value;
    }

    public bool IsInReachables
    {
        get => isInReachables;
        set => isInReachables = value;
    }

    /*
     * Methods
     */

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
            coordinates.ChangeCoordinatesType(HexCoordinatesType.cubic);
            TileProperties tile = grid.GetTile(coordinates + direction);
            if (tile != null) {
                SetNeighbor((HexDirection)i, tile);
            }
        }
    }

    // Get all tiles at range
    public List<TileProperties> InRange(int range) {
        List<TileProperties> tilesInRange = new List<TileProperties>();
        for (int x = -range; x <= range; x++) {
            for (int y = -range; y <= range; y++) {
                int z = -y - x;
                HexCoordinates coordinatesInRange = new HexCoordinates(x, y, z);
                coordinates.ChangeCoordinatesType(HexCoordinatesType.cubic);
                tilesInRange.Add(grid.GetTile(coordinates + coordinatesInRange));
            }
        }
        return tilesInRange;
    }

    // Get reachable tiles (take in count if tile is walkable and walk cost)
    public List<TileProperties> TilesReachable(int movement) {
        List<TileProperties> visited = new List<TileProperties>();
        visited.Add(this);

        List<TileProperties>[] fringes = new List<TileProperties>[movement+1];
        
        fringes[0] = new List<TileProperties>();
        fringes[0].Add(this);
        isInReachables = true;

        for (int i = 1; i <= movement; i++) {
            fringes[i] = new List<TileProperties>();
        }

        for (int i = 1; i <= movement+1; i++) {
            foreach (TileProperties previousTile in fringes[i - 1]) {
                TileProperties[] neighbors = previousTile.GetNeighbors();
                for (int j = 0; j < neighbors.Length; j++) {
                    TileProperties neighbor = neighbors[j];
                    if (neighbor && !visited.Contains(neighbor) && neighbor.Tile.canWalkThrough) {
                        int distance = i-1 + neighbor.Tile.walkCost;
                        if (distance <= movement) {
                            fringes[distance].Add(neighbor);
                            neighbor.isInReachables = true;
                            visited.Add(neighbor);
                        }
                    }
                }
            }
        }
        return visited;
    }
    
}
