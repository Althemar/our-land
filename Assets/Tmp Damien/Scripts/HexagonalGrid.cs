using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/*
 * HexagonalGrid
 */

[RequireComponent(typeof(Tilemap))]
public class HexagonalGrid : MonoBehaviour
{
    /*
     * Members
     */

    public Dictionary<Vector3Int, TileProperties> tiles;

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
        TileProperties tile = null;
        tiles.TryGetValue(coordinates.ToVector3Int(), out tile);
        return tile;
    }

    public void ChangeCoordinateSystem(HexCoordinatesType type) {
        foreach (KeyValuePair<Vector3Int, TileProperties> tile in tiles) {
            tile.Value.Coordinates.ChangeCoordinatesType(type);
        }
        coordinatesType = type;
    }

    public Stack<TileProperties> Path(TileProperties begin, TileProperties end) {
        PriorityQueue<TileProperties> frontier = new PriorityQueue<TileProperties>();
        frontier.Enqueue(begin, 0);
        Dictionary<TileProperties, TileProperties> cameFrom = new Dictionary<TileProperties, TileProperties>();
        Dictionary<TileProperties, int> costSoFar = new Dictionary<TileProperties, int>();
        cameFrom.Add(begin, begin);
        costSoFar.Add(begin, 0);

        while (frontier.Count != 0) {
            TileProperties current = frontier.Dequeue();
            if (current == end) {
                break;
            }

            TileProperties[] neighbors = current.GetNeighbors();
            foreach (TileProperties next in neighbors) {
                if (next == null || !next.Tile.canWalkThrough) {
                    continue;
                }
                int newCost = costSoFar[current] + 1;
                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next]) {
                    costSoFar[next] = newCost;
                    double priority = newCost + next.Coordinates.Distance(end.Coordinates);
                    frontier.Enqueue(next, priority);
                    cameFrom[next] = current;
                }
            }
        }

        Stack<TileProperties> path = new Stack<TileProperties>();
        path.Push(end);
        TileProperties currentTile = cameFrom[end];
        path.Push(currentTile);
        while (currentTile != begin) {
            currentTile = cameFrom[currentTile];
            path.Push(currentTile);
        }

        return path;

    }
}

