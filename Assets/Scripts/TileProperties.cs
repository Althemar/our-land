using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileProperties : MonoBehaviour {
    /*
     * Members
     */
    
    [HideInInspector]
    public TileProperties[] neighbors;

    private Vector3Int position;
    private HexCoordinates coordinates;
    private CustomTile tile;
    private HexagonalGrid grid;
    private Tilemap tilemap;
    
    public Movable movable;
    public Movable movablePreview;

    private bool isInReachables;
    private float actionPointCost = -1;
    
    public StaticEntity staticEntity;
    public MovingEntity movingEntity;
    public Wind wind;
    public Whirlwind whirlwind;


    public GameObject addonsGameObjects;
    public GameObject bordersGameObjects;
    public GameObject riversGameObjects;
    public SpriteRenderer hexagonLayer;


    private static Vector3Int[] cubeDirections = { new Vector3Int(0, 1, -1), new Vector3Int(1, 0, -1), new Vector3Int(1, -1, 0),
                                           new Vector3Int(0, -1, 1), new Vector3Int(-1, 0, 1), new Vector3Int(-1, 1, 0)
                                         };

    [HideInInspector]
    public float humidity;
    [HideInInspector]
    public bool asRiver;
    private bool[] rivers;
    private River[] riverJonction;
    [HideInInspector]
    public bool asLake;
    [HideInInspector]
    public float windDryness = 0;

    [HideInInspector]
    public List<HexDirection> nextTilesInCorridor;
    [HideInInspector]
    public HexDirection previousTileInCorridor;
    [HideInInspector]
    public HashSet<WindOrigin> woAffectingTile;
    [HideInInspector]
    public HashSet<WindOrigin> woOnTile;
    [HideInInspector]
    public WindOrigin windOrigin;

    [HideInInspector]
    public bool needRefresh = true;

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

    public float ActionPointCost
    {
        get => actionPointCost;
        set => actionPointCost = value;
    }

    /*
     * Methods
     */

    private void Awake() {
        neighbors = new TileProperties[6];
        rivers = new bool[6];
        riverJonction = new River[6];
        
        addonsGameObjects.transform.parent = transform;
        bordersGameObjects.transform.parent = transform;
        riversGameObjects.transform.parent = transform;
        nextTilesInCorridor = new List<HexDirection>();
        woAffectingTile = new HashSet<WindOrigin>();
        woOnTile = new HashSet<WindOrigin>();

    }

    public void InitializeTile(Vector3Int position, HexagonalGrid grid, Tilemap tilemap) {
        this.position = position;
        coordinates = new HexCoordinates(position.x, position.y);

        this.grid = grid;
        this.tilemap = tilemap;
        //tilemap.SetColor(position, new Color(0.5f, 0, 0, 0.6f));

    }

    public void SetTile(CustomTile tile) {
        this.tile = tile;
    }

    public void ResetAddon() {
        foreach (Transform t in this.addonsGameObjects.transform)
            Destroy(t.gameObject);
    }

    public void ResetTile() {
        foreach (Transform t in bordersGameObjects.transform)
            Destroy(t.gameObject);
        foreach (Transform t in riversGameObjects.transform)
            Destroy(t.gameObject);
    }

    public void SetAddon() {
        if (tile == null)
            return;

        foreach (KeyValuePair<float, SpriteList> pair in tile.addons) {
            float rand = Random.value;
            if (rand < pair.Key && pair.Value.sprites.Count > 0) {
                CreateSprite(pair.Value.sprites[Random.Range(0, pair.Value.sprites.Count)], addonsGameObjects, tile.addonSortingOffset, tile.addonLayer, tile.addonLayer != 0);
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

    public void ResetRiver() {
        rivers = new bool[6];
        riverJonction = new River[6];
        asLake = false;
        humidity = 0;
    }

    public void SetRiver(HexDirection direction, River r) {
        HexDirection opposite = direction.Opposite();
        rivers[(int)direction] = true;
        if(GetNeighbor(direction))
            GetNeighbor(direction).rivers[(int)opposite] = true;
        asRiver = true;
        if (GetNeighbor(direction))
            GetNeighbor(direction).asRiver = true;


        if (r.counterClockwise) {
            if (riverJonction[(int)direction] == null) {
                riverJonction[(int)direction] = r;
                if (GetNeighbor(direction))
                    GetNeighbor(direction).riverJonction[(int)direction.Opposite().Next()] = r;
                if (GetNeighbor(direction.Previous()))
                    GetNeighbor(direction.Previous()).riverJonction[(int)direction.Next().Next()] = r;
            }
            else {
                riverJonction[(int)direction].force += r.force;
                r.force = 0;
                r.doLake = false;
            }

        }
        else {
            if (riverJonction[(int)direction.Next()] == null) {
                if (riverJonction[(int)direction.Next()] != null)
                    r.force++;
                riverJonction[(int)direction.Next()] = r;
                if(GetNeighbor(direction))
                    GetNeighbor(direction).riverJonction[(int)direction.Opposite()] = r;
                if (GetNeighbor(direction.Next()))
                    GetNeighbor(direction.Next()).riverJonction[(int)direction.Previous()] = r;
            }
            else {
                riverJonction[(int)direction.Next()].force += r.force;
                r.force = 0;
                r.doLake = false;
            }
        }
    }

    public void PutGlacier() {
        if (this.Tile && this.Tile.riverSource) {
            SpriteRenderer spriteRenderer = new GameObject().AddComponent<SpriteRenderer>();
            spriteRenderer.transform.parent = addonsGameObjects.transform;
            spriteRenderer.transform.position = transform.position;
            spriteRenderer.sprite = grid.humidity.glacier;
            spriteRenderer.sortingOrder = 15 - Position.y;
            spriteRenderer.gameObject.layer = 13;
        }
    }

    public void PutLake() {
        if (this.asLake) {
            ResetAddon();

            if (GetNeighbor(HexDirection.SW).asLake && GetNeighbor(HexDirection.SE).asLake)
                CreateSprite(grid.humidity.triLakeN, riversGameObjects, -150);
            else if (GetNeighbor(HexDirection.NE).asLake && GetNeighbor(HexDirection.E).asLake)
                CreateSprite(grid.humidity.triLakeSW, riversGameObjects, -150);
            else if (GetNeighbor(HexDirection.NW).asLake && GetNeighbor(HexDirection.W).asLake)
                CreateSprite(grid.humidity.triLakeSE, riversGameObjects, -150);
            else if (GetNeighbor(HexDirection.NW).asLake && GetNeighbor(HexDirection.NE).asLake)
                CreateSprite(grid.humidity.triLakeS, riversGameObjects, -150);
            else if (GetNeighbor(HexDirection.SE).asLake && GetNeighbor(HexDirection.E).asLake)
                CreateSprite(grid.humidity.triLakeNW, riversGameObjects, -150);
            else if (GetNeighbor(HexDirection.SW).asLake && GetNeighbor(HexDirection.W).asLake)
                CreateSprite(grid.humidity.triLakeNE, riversGameObjects, -150);
            else
                CreateSprite(grid.humidity.lake, riversGameObjects, -150);
        }
    }

    public void PutRivers() {
        if (tile == null)
            return;

        for (int i = 0; i < 6; i++) {
            if (!rivers[i])
                continue;
            Sprite sprite = grid.humidity.ERiver;
            switch ((HexDirection)i) {
                case HexDirection.NE:
                    CreateSprite(grid.humidity.NERiver, riversGameObjects, -300);
                    
                    if (GetNeighbor(HexDirection.NE).rivers[(int)HexDirection.SE] && !rivers[(int)HexDirection.E])
                        CreateSprite(grid.humidity.NEInterNW, riversGameObjects, -300);

                    if (rivers[(int)HexDirection.NW] && !GetNeighbor(HexDirection.NE).rivers[(int)HexDirection.W])
                        CreateSprite(grid.humidity.NWInterNE, riversGameObjects, -300);

                    if (!rivers[(int)HexDirection.NW] && !GetNeighbor(HexDirection.NE).rivers[(int)HexDirection.W])
                        CreateSprite(grid.humidity.NERivNW, riversGameObjects, -300);
                    if (!rivers[(int)HexDirection.E] && !GetNeighbor(HexDirection.NE).rivers[(int)HexDirection.SE])
                        CreateSprite(grid.humidity.NERivSE, riversGameObjects, -300);
                    break;
                case HexDirection.E:
                    CreateSprite(grid.humidity.ERiver, riversGameObjects, -300);
                    if (rivers[(int)HexDirection.NE] && GetNeighbor(HexDirection.E).rivers[(int)HexDirection.NW])
                        CreateSprite(grid.humidity.EInterNEW, riversGameObjects, -300);
                    else if (rivers[(int)HexDirection.NE])
                        CreateSprite(grid.humidity.EInterNE, riversGameObjects, -300);
                    else if (GetNeighbor(HexDirection.E).rivers[(int)HexDirection.NW])
                        CreateSprite(grid.humidity.EInterNW, riversGameObjects, -300);
                    else
                        CreateSprite(grid.humidity.ERivN, riversGameObjects, -300);

                    if (rivers[(int)HexDirection.SE] && GetNeighbor(HexDirection.E).rivers[(int)HexDirection.SW])
                        CreateSprite(grid.humidity.EInterSEW, riversGameObjects, -300);
                    else if (rivers[(int)HexDirection.SE])
                        CreateSprite(grid.humidity.EInterSE, riversGameObjects, -300);
                    else if (GetNeighbor(HexDirection.E).rivers[(int)HexDirection.SW])
                        CreateSprite(grid.humidity.EInterSW, riversGameObjects, -300);
                    else
                        CreateSprite(grid.humidity.ERivS, riversGameObjects, -300);

                    break;
                case HexDirection.SE:
                    CreateSprite(grid.humidity.SERiver, riversGameObjects, -300);
                    if (!rivers[(int)HexDirection.SW] && !GetNeighbor(HexDirection.SE).rivers[(int)HexDirection.W])
                        CreateSprite(grid.humidity.SERivSW, riversGameObjects, -300);
                    if (!rivers[(int)HexDirection.E] && !GetNeighbor(HexDirection.SE).rivers[(int)HexDirection.NE])
                        CreateSprite(grid.humidity.SERivNE, riversGameObjects, -300);
                    break;
            }

            
        }
    }

    private void CreateSprite(Sprite sprite, GameObject parent = null, int sorting = 5, int layer = 0, bool defaultLayer = false) {
        SpriteRenderer spriteRenderer = new GameObject().AddComponent<SpriteRenderer>();
        if(parent)
            spriteRenderer.transform.parent = parent.transform;
        else
            spriteRenderer.transform.parent = transform;
        spriteRenderer.transform.position = transform.position;
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = sorting - Position.y;
        if(!defaultLayer)
            spriteRenderer.sortingLayerID = HexagonalGrid.Instance.layerID;
        spriteRenderer.gameObject.layer = layer;
    }

    public void SetBorders() {
        for (int i = 0; i < 6; i++) {
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
            spriteRenderer.transform.parent = bordersGameObjects.transform;
            spriteRenderer.transform.position = transform.position + grid.Metrics.GetBorder((int)direction) * -0.06f;
            spriteRenderer.sprite = borders[Random.Range(0, borders.Count)];
            spriteRenderer.sortingOrder = -750;
            spriteRenderer.sortingLayerID = HexagonalGrid.Instance.layerID;
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

                HexCoordinates other = coordinates + coordinatesInRange;
                if (other.Distance(coordinates) <= range) {
                    tilesInRange.Add(grid.GetTile(coordinates + coordinatesInRange));
                }
            }
        }
        return tilesInRange;
    }



    // Get reachable tiles (take in count if tile is walkable and walk cost)
    public List<TileProperties> TilesReachable(int movement, int reach) {
        List<TileProperties> visited = new List<TileProperties>();
        visited.Add(this);
        actionPointCost = 0;


        List<TileProperties>[] fringes = new List<TileProperties>[movement + 1];

        fringes[0] = new List<TileProperties>();
        fringes[0].Add(this);
        isInReachables = true;

        for (int i = 1; i <= movement; i++) {
            fringes[i] = new List<TileProperties>();
        }
        int cost = 1;
        int currentReach = 0;
        for (int i = 1; i <= movement + 1; i++) {
            currentReach++;
            foreach (TileProperties previousTile in fringes[i - 1]) {
                TileProperties[] neighbors = previousTile.GetNeighbors();
                for (int j = 0; j < neighbors.Length; j++) {
                    TileProperties neighbor = neighbors[j];
                    if (neighbor && !visited.Contains(neighbor) && neighbor.Tile && neighbor.Tile.canWalkThrough && !neighbor.movable && !neighbor.asLake) {
                        int distance = i - 1 + neighbor.Tile.walkCost;
                        if (distance <= movement) {
                            fringes[distance].Add(neighbor);
                            neighbor.actionPointCost = cost;
                            visited.Add(neighbor);

                        }
                    }
                }
            }
            if (currentReach >= reach) {
                currentReach = 0;
                cost++;
            }
        }
        return visited;
    }

    public TileProperties NearestEntity(EntitySO[] entities, int maxDistance = -1, bool preview = false) {
        HashSet<TileProperties> visited = new HashSet<TileProperties>();
        visited.Add(this);

        List<List<TileProperties>> fringes = new List<List<TileProperties>>();

        fringes.Add(new List<TileProperties>());
        fringes[0].Add(this);
        if ((staticEntity || movingEntity) && ContainsEntity(entities, false, preview)) {
            return this;
        }

        int i = 1;
        while (true) {
            foreach (TileProperties previousTile in fringes[i - 1]) {
                TileProperties[] neighbors = previousTile.GetNeighbors();
                for (int j = 0; j < neighbors.Length; j++) {
                    TileProperties neighbor = neighbors[j];
                    if (neighbor && neighbor.Tile && !visited.Contains(neighbor) && neighbor.IsWalkable()) {
                        int distance = i - 1 + neighbor.Tile.walkCost;

                        while (fringes.Count <= distance) {
                            fringes.Add(new List<TileProperties>());
                        }

                        if (neighbor.ContainsEntity(entities, true, preview)) {
                            return neighbor;
                        }
                        else if (neighbor.movable) {

                        }
                        else {
                            fringes[distance].Add(neighbor);
                        }
                            visited.Add(neighbor);

                    }
                }
            }
            i++;
            if (i > fringes.Count || (maxDistance > -1 && i > maxDistance)) {
                break;
            }
        }
        return null;
    }

    public bool StaticEntityIsReachable() {
        return !movingEntity;
    }

    public bool ContainsEntity(EntitySO[] entities, bool checkIfReachable = false, bool preview = false) {
        for (int i = 0; i < entities.Length; i++) {
            if (ContainsEntity(entities[i], checkIfReachable, preview)) {
                return true;
            }
        }
        return false;
    }

    public bool IsEmpty(bool preview = false) {
        return !staticEntity && ((!movingEntity && !preview) || (!movablePreview && preview))&& !movable;
    }

    public bool ContainsEntity(EntitySO entity, bool checkIfReachable = false, bool preview = false) {
        return (staticEntity && entity.GetType() == typeof(StaticEntitySO) && staticEntity.staticEntitySO == entity && (!checkIfReachable || (!movable && !preview) || (!movablePreview && preview)))
             || (movingEntity && entity.GetType() == typeof(MovingEntitySO) && movingEntity.movingEntitySO == entity);
    }

    public bool IsWalkable() {
        return !asLake && !windOrigin && !tile.riverSource && tile.terrainType != CustomTile.TerrainType.Mountain;
    }

}
