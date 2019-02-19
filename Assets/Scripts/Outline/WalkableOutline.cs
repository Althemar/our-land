using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class WalkableOutline : MonoBehaviour {
    private Mesh mesh;
    public HexagonalGrid grid;
    private MotherShip ship;
    GameObject tileInaccessible;
    public Sprite inaccessibleSprite;

    public float outlineSize = 0.05f;

    public void Awake() {
        mesh = GetComponent<MeshFilter>().mesh;
        tileInaccessible = new GameObject("Tiles Inaccessible");
        tileInaccessible.transform.parent = this.transform;
    }

    public void Start() {
        ship = GameManager.Instance.motherShip;
        ship.OnTurnBegin += CreateMesh;

        CreateMesh();
    }

    public void ShowOutline() {
        GetComponent<MeshRenderer>().enabled = true;
        tileInaccessible.SetActive(true);
    }
    public void HideOutline() {
        GetComponent<MeshRenderer>().enabled = false;
        tileInaccessible.SetActive(false);
    }

    public List<TileProperties> TilesReachable(TileProperties start, int maxWood) {
        List<TileProperties> visited = new List<TileProperties>();
        visited.Add(start);
        start.ActionPointCost = 0;
        start.IsInReachables = true;


        PriorityQueue<TileProperties> toVisit = new PriorityQueue<TileProperties>();

        TileProperties[] neighbors = start.GetNeighbors();
        for (int i = 0; i < neighbors.Length; i++) {
            TileProperties neighbor = neighbors[i];
            if (neighbor && !visited.Contains(neighbor) && neighbor.Tile && neighbor.Tile.canWalkThrough && !neighbor.asMountain) {
                neighbor.ActionPointCost = 0;
                neighbor.IsInReachables = true;
                toVisit.Enqueue(neighbor, 0);
                visited.Add(neighbor);
            }
        }


        while (toVisit.Count > 0) {
            TileProperties tile = toVisit.Dequeue();

            neighbors = tile.GetNeighbors();
            for (int i = 0; i < neighbors.Length; i++) {
                TileProperties neighbor = neighbors[i];
                if (neighbor && !visited.Contains(neighbor) && neighbor.Tile && neighbor.Tile.canWalkThrough && !neighbor.asMountain) {
                    float cost;
                    if (FreeWindMovement(tile, neighbor)) {
                        cost = tile.ActionPointCost;
                    }
                    else {
                        if (tile.ActionPointCost > 0) {
                            cost = Mathf.Round(tile.ActionPointCost * ship.movementDistanceMultiplicator);
                        }
                        else {
                            cost = ship.movementBaseCost;
                        }
                    }


                    if (cost <= maxWood) {
                        neighbor.ActionPointCost = cost;
                        neighbor.IsInReachables = true;
                        toVisit.Enqueue(neighbor, cost);
                        visited.Add(neighbor);
                    }
                }
            }
        }
        return visited;
    }

    private bool FreeWindMovement(TileProperties current, TileProperties next) {
        if (next.wind && ship.Movable.canUseWind) { // Free movement if wind
            HexDirection movableDir = current.Coordinates.Direction(next.Coordinates);

            HexDirection beginDir = (current.wind) ? current.wind.direction : movableDir;
            if (current.wind
                && ((beginDir == next.wind.direction && beginDir == movableDir)
                || (beginDir == next.wind.direction.Previous() && next.wind.direction == movableDir)
                || (beginDir == next.wind.direction.Next() && next.wind.direction == movableDir))) {
                return true;
            }
            else if (!current.wind && (beginDir == next.wind.direction
                    || beginDir == next.wind.direction.Previous()
                    || beginDir == next.wind.direction.Next())) {
                return false;
            }
        }
        return false;
    }

    public void CreateMesh() {
        HideOutline();
        List<TileProperties> tiles = TilesReachable(ship.Movable.CurrentTile, (int)ship.Inventory.GetResource(ship.fuelResource));
        foreach (TileProperties tile in tiles)
            tile.IsInReachables = true;

        transform.position = new Vector3(0, 0, -0.1f);

        foreach(Transform child in tileInaccessible.transform)
            Destroy(child.gameObject);
        mesh.Clear();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> verticesSimple = new List<Vector3>();
        List<Vector3> verticesHex = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<Vector2> uvsSimple = new List<Vector2>();
        int[,] tilesNbRect = new int[grid.tilesArray.GetLength(0), grid.tilesArray.GetLength(1)];
        int[,] tilesNbRectSimple = new int[grid.tilesArray.GetLength(0), grid.tilesArray.GetLength(1)];
        int nbRect = 0;
        int nbRectSimple = 0;
        for (int x = 0; x < grid.tilesArray.GetLength(0); x++) {
            for (int y = 0; y < grid.tilesArray.GetLength(1); y++) {
                TileProperties tile = grid.tilesArray[x, y];

                if (!tile)
                    continue;

                if (tile.IsInReachables && (tile.staticEntity || tile.movingEntity || tile.asLake || tile.windOrigin || tile.Tile.terrainType == CustomTile.TerrainType.Water)) {
                    GameObject cantGo = new GameObject();
                    cantGo.transform.parent = tileInaccessible.transform;
                    cantGo.transform.position = tile.transform.position;
                    SpriteRenderer renderer = cantGo.AddComponent<SpriteRenderer>();
                    renderer.sortingOrder = -1000;
                    renderer.sprite = inaccessibleSprite;
                }

                if (tile.IsWalkable() && tile.IsInReachables)
                    continue;

                for (HexDirection h = (HexDirection)0; h < (HexDirection)6; h++) {
                    if (!tile.GetNeighbor(h) || !tile.GetNeighbor(h.Previous()) || !tile.GetNeighbor(h.Next()))
                        continue;

                    if (tile.GetNeighbor(h).IsWalkable() && tile.GetNeighbor(h).IsInReachables) {
                        if(tile.IsWalkable()) { // SIMPLE OUTLINE
                            if (tile.GetNeighbor(h.Previous()).IsWalkable() && tile.GetNeighbor(h.Previous()).IsInReachables)
                                AddVertices2(ref verticesSimple, ref uvsSimple, tile, h, true);
                            else
                                AddVertices(ref verticesSimple, ref uvsSimple, tile, h, true);

                            if (tile.GetNeighbor(h.Next()).IsWalkable() && tile.GetNeighbor(h.Next()).IsInReachables)
                                AddVertices2(ref verticesSimple, ref uvsSimple, tile, h.Next(), false);
                            else
                                AddVertices(ref verticesSimple, ref uvsSimple, tile, h.Next(), false);

                            tilesNbRectSimple[x, y]++;
                            nbRectSimple++;
                        } else {
                            if (tile.GetNeighbor(h.Previous()).IsWalkable() && tile.GetNeighbor(h.Previous()).IsInReachables)
                                AddVertices2(ref vertices, ref uvs, tile, h, true);
                            else
                                AddVertices(ref vertices, ref uvs, tile, h, true);

                            if (tile.GetNeighbor(h.Next()).IsWalkable() && tile.GetNeighbor(h.Next()).IsInReachables)
                                AddVertices2(ref vertices, ref uvs, tile, h.Next(), false);
                            else
                                AddVertices(ref vertices, ref uvs, tile, h.Next(), false);

                            tilesNbRect[x, y]++;
                            nbRect++;
                        }
                    }
                }
            }
        }

        vertices.AddRange(verticesSimple);
        uvs.AddRange(uvsSimple);
        mesh.vertices = vertices.ToArray();
        mesh.uv = uvs.ToArray();

        int vi = 0;

        int trianglesSize = nbRect * 6;
        int[] triangles = new int[trianglesSize];
        int ti = 0;
        for (int x = 0; x < grid.tilesArray.GetLength(0); x++) {
            for (int y = 0; y < grid.tilesArray.GetLength(1); y++) {
                TileProperties tile = grid.tilesArray[x, y];
                if (!tile || (tile.IsWalkable() && tile.IsInReachables))
                    continue;
                for (int i = 0; i < tilesNbRect[x, y]; i++) {
                    triangles[ti] = vi;
                    triangles[ti + 1] = triangles[ti + 4] = vi + 1;
                    triangles[ti + 2] = triangles[ti + 3] = vi + 2;
                    triangles[ti + 5] = vi + 3;

                    ti += 6;
                    vi += 4;
                }
            }
        }

        int trianglesSizeSimple = nbRectSimple * 6;
        int[] trianglesSimple = new int[trianglesSizeSimple];
        ti = 0;
        for (int x = 0; x < grid.tilesArray.GetLength(0); x++) {
            for (int y = 0; y < grid.tilesArray.GetLength(1); y++) {
                TileProperties tile = grid.tilesArray[x, y];
                if (!tile || (tile.IsWalkable() && tile.IsInReachables))
                    continue;
                for (int i = 0; i < tilesNbRectSimple[x, y]; i++) {
                    trianglesSimple[ti] = vi;
                    trianglesSimple[ti + 1] = trianglesSimple[ti + 4] = vi + 1;
                    trianglesSimple[ti + 2] = trianglesSimple[ti + 3] = vi + 2;
                    trianglesSimple[ti + 5] = vi + 3;

                    ti += 6;
                    vi += 4;
                }
            }
        }

        mesh.subMeshCount = 2;
        mesh.SetTriangles(triangles, 0);
        mesh.SetTriangles(trianglesSimple, 1);

        GetComponent<MeshRenderer>().sortingLayerName = "World Layer";
        GetComponent<MeshRenderer>().sortingOrder = -500;
        foreach (TileProperties tile in tiles)
            tile.IsInReachables = false;
    }

    // Add 2 vertices to the mesh
    private void AddVertices(ref List<Vector3> vertices, ref List<Vector2> uvs, TileProperties current, HexDirection direction, bool prev) {
        Vector3 left = current.transform.position + current.Grid.Metrics.GetCorner(false, direction);
        Vector3 other = left - current.Grid.Metrics.GetCorner(false, prev ? direction.Next() : direction.Previous()) * outlineSize;

        vertices.Add(other);
        vertices.Add(left);
        uvs.Add(new Vector2(prev ? 0 : 1, 1));
        uvs.Add(new Vector2(prev ? 0 : 1, 0));
    }

    private void AddVertices2(ref List<Vector3> vertices, ref List<Vector2> uvs, TileProperties current, HexDirection direction, bool prev) {
        Vector3 left = current.transform.position + current.Grid.Metrics.GetCorner(false, direction);
        Vector3 other = left - current.Grid.Metrics.GetCorner(false, direction) * outlineSize;

        vertices.Add(other);
        vertices.Add(left);
        uvs.Add(new Vector2(prev ? 0 : 1, 1));
        uvs.Add(new Vector2(prev ? 0 : 1, 0));
    }

    public void Clear() {
        mesh.Clear();
    }
}
