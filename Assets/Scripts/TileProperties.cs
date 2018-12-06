﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileProperties : MonoBehaviour {
    /*
     * Members
     */

    public TileProperties[] neighbors;

    private Vector3Int position;
    private HexCoordinates coordinates;
    private CustomTile tile;
    private HexagonalGrid grid;
    private Tilemap tilemap;
    public Movable currentMovable;

    private bool isInReachables;

    public StaticEntity staticEntity;
    public MovingEntity movingEntity;

    public float humidity;

    private static Vector3Int[] cubeDirections = { new Vector3Int(0, 1, -1), new Vector3Int(1, 0, -1), new Vector3Int(1, -1, 0),
                                           new Vector3Int(0, -1, 1), new Vector3Int(-1, 0, 1), new Vector3Int(-1, 1, 0)
                                         };

    private bool[] asRiver;

    /*
     * Properties
     */

    public HexCoordinates Coordinates {
        get => coordinates;
    }

    public Vector3Int Position {
        get => position;
        set {
            position = value;
            coordinates = new HexCoordinates(position.x, position.y);
        }
    }

    public CustomTile Tile {
        get => tile;
        set => tile = value;
    }

    public Tilemap Tilemap {
        get => tilemap;
    }

    public HexagonalGrid Grid {
        get => grid;
        set => grid = value;
    }

    public bool IsInReachables {
        get => isInReachables;
        set => isInReachables = value;
    }

    /*
     * Methods
     */

    private void Awake() {
        neighbors = new TileProperties[6];
        asRiver = new bool[6];
    }

    public void InitializeTile(Vector3Int position, HexagonalGrid grid, Tilemap tilemap) {
        this.position = position;
        coordinates = new HexCoordinates(position.x, position.y);

        this.grid = grid;
        this.tilemap = tilemap;
    }

    public void SetTile(CustomTile tile) {
        this.tile = tile;
    }

    public void ResetTile() {
        foreach (Transform t in this.transform)
            Destroy(t.gameObject);
    }

    public void SetAddon() {
        if (tile == null)
            return;

        foreach (KeyValuePair<float, SpriteList> pair in tile.addons) {
            float rand = Random.value;
            if (rand < pair.Key && pair.Value.sprites.Count > 0) {
                SpriteRenderer spriteRenderer = new GameObject().AddComponent<SpriteRenderer>();
                spriteRenderer.transform.parent = transform;
                spriteRenderer.transform.position = transform.position;
                spriteRenderer.sprite = pair.Value.sprites[Random.Range(0, pair.Value.sprites.Count)];
                spriteRenderer.sortingOrder = 1;
            }
        }
    }

    public TileProperties GetNeighbor(HexDirection direction) {
        return neighbors[(int)direction];
    }

    public TileProperties[] GetNeighbors() {
        return neighbors;
    }

    void SetNeighbor(HexDirection direction, TileProperties cell) {
        HexDirection opposite = direction.Opposite();

        neighbors[(int)direction] = cell;
        cell.neighbors[(int)opposite] = this;
    }

    public void SetNeighbors() {
        for (int i = 0; i < 3; i++) {
            HexCoordinates direction = new HexCoordinates(cubeDirections[i]);
            TileProperties tile = grid.GetTile(coordinates + direction);
            if (tile != null) {
                SetNeighbor((HexDirection)i, tile);
            }
        }
    }

    public void SetRiver(HexDirection direction) {
        HexDirection opposite = direction.Opposite();
        asRiver[(int)direction] = true;
        GetNeighbor(direction).asRiver[(int)opposite] = true;
    }

    public void PutRivers() {
        if (tile == null)
            return;

        for (int i = 0; i < asRiver.Length; i++) {
            if (!asRiver[i])
                continue;
            BorderDictionary dic = null;
            switch ((HexDirection)i) {
                case HexDirection.NW:
                case HexDirection.NE:
                    dic = tile.bordersNW;
                    break;
                case HexDirection.W:
                case HexDirection.E:
                    dic = tile.bordersW;
                    break;
                case HexDirection.SW:
                case HexDirection.SE:
                    dic = tile.bordersSW;
                    break;
            }

            SpriteRenderer spriteRenderer = new GameObject().AddComponent<SpriteRenderer>();
            Debug.Log(spriteRenderer);
            spriteRenderer.transform.parent = transform;
            spriteRenderer.transform.position = transform.position + grid.Metrics.GetBorder(i);
            spriteRenderer.sprite = dic[CustomTile.TerrainType.Water].sprites[0];
            spriteRenderer.sortingOrder = 3;
            if (i <= 2) {
                spriteRenderer.flipX = true;
            }
        }


    }

    public void SetBorders() {
        for (int i = 0; i < 3; i++) {
            HexCoordinates direction = new HexCoordinates(cubeDirections[i]);
            TileProperties tile = grid.GetTile(coordinates + direction);
            if (tile != null) {
                SetBorder((HexDirection)i, tile);
            }
        }
    }

    void SetBorder(HexDirection direction, TileProperties cell) {
        if (tile == null || cell.tile == null)
            return;
        if (cell.tile != tile) {
            HexDirection opposite = direction.Opposite();
            SetBorder(direction, cell.tile.terrainType);
            cell.SetBorder(opposite, tile.terrainType);
        }
    }

    public void SetBorder(HexDirection direction, CustomTile.TerrainType terrainType) {
        if (tile == null)
            return;

        BorderDictionary dic = null;
        switch (direction) {
            case HexDirection.NW:
            case HexDirection.NE:
                dic = tile.bordersNW;
                break;
            case HexDirection.W:
            case HexDirection.E:
                dic = tile.bordersW;
                break;
            case HexDirection.SW:
            case HexDirection.SE:
                dic = tile.bordersSW;
                break;
        }
        List<Sprite> borders;
        if (dic == null || !dic.ContainsKey(terrainType)) {
            return;
        }
        borders = dic[terrainType].sprites;
        if (borders != null && borders.Count > 0) {
            SpriteRenderer spriteRenderer = new GameObject().AddComponent<SpriteRenderer>();
            spriteRenderer.transform.parent = transform;
            spriteRenderer.transform.position = transform.position + grid.Metrics.GetBorder((int)direction) * -0.06f;
            spriteRenderer.sprite = borders[Random.Range(0, borders.Count)];
            spriteRenderer.sortingOrder = 1;
            if ((int)direction <= 2) {
                spriteRenderer.flipX = true;
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
                tilesInRange.Add(grid.GetTile(coordinates + coordinatesInRange));
            }
        }
        return tilesInRange;
    }

    // Get reachable tiles (take in count if tile is walkable and walk cost)
    public List<TileProperties> TilesReachable(int movement) {
        Dictionary<Vector3Int, TileProperties> visitDic = new Dictionary<Vector3Int, TileProperties>();
        visitDic.Add(this.Position, this);

        List<TileProperties>[] fringes = new List<TileProperties>[movement + 1];

        fringes[0] = new List<TileProperties>();
        fringes[0].Add(this);
        isInReachables = true;

        for (int i = 1; i <= movement; i++) {
            fringes[i] = new List<TileProperties>();
        }

        for (int i = 1; i <= movement + 1; i++) {
            foreach (TileProperties previousTile in fringes[i - 1]) {
                TileProperties[] neighbors = previousTile.GetNeighbors();
                for (int j = 0; j < neighbors.Length; j++) {
                    TileProperties neighbor = neighbors[j];
                    if (neighbor && !visitDic.ContainsKey(neighbor.Position) && neighbor.Tile.canWalkThrough) {
                        int distance = i - 1 + neighbor.Tile.walkCost;
                        if (distance <= movement) {
                            fringes[distance].Add(neighbor);
                            neighbor.isInReachables = true;
                            visitDic.Add(neighbor.Position, neighbor);
                        }
                    }
                }
            }
        }
        List<TileProperties> visited = new List<TileProperties>();
        foreach (var d in visitDic)
            visited.Add(d.Value);

        return visited;
    }

    public TileProperties NearestEntity(EntitySO[] entities) {
        List<TileProperties> visited = new List<TileProperties>();
        visited.Add(this);

        List<List<TileProperties>> fringes = new List<List<TileProperties>>();

        fringes.Add(new List<TileProperties>());
        fringes[0].Add(this);
        if ((staticEntity || movingEntity) && ContainsEntity(entities, false)) {
            return this;
        }

        int i = 1;
        while (true) {
            foreach (TileProperties previousTile in fringes[i - 1]) {
                TileProperties[] neighbors = previousTile.GetNeighbors();
                for (int j = 0; j < neighbors.Length; j++) {
                    TileProperties neighbor = neighbors[j];
                    if (neighbor && !visited.Contains(neighbor) && neighbor.Tile.canWalkThrough) {
                        int distance = i - 1 + neighbor.Tile.walkCost;

                        while (fringes.Count <= distance) {
                            fringes.Add(new List<TileProperties>());
                        }

                        if (neighbor.ContainsEntity(entities, true)) {
                            return neighbor;
                        }
                        fringes[distance].Add(neighbor);
                        visited.Add(neighbor);
                    }
                }
            }
            i++;
            if (i > fringes.Count) {
                break;
            }
        }
        return null;
    }

    public bool StaticEntityIsReachable() {
        return !movingEntity;
    }

    public bool ContainsEntity(EntitySO[] entities, bool checkIfReachable = false) {
        for (int i = 0; i < entities.Length; i++) {
            if (ContainsEntity(entities[i], checkIfReachable)) {
                return true;
            }
        }
        return false;
    }

    public bool ContainsEntity(EntitySO entity, bool checkIfReachable = false) {
        return (staticEntity && entity.GetType() == typeof(StaticEntitySO) && staticEntity.staticEntitySO == entity && (!checkIfReachable || !movingEntity))
             || (movingEntity && entity.GetType() == typeof(MovingEntitySO) && movingEntity.movingEntitySO == entity);
    }

}
