using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class OutlineWalkable : MonoBehaviour {
    private Mesh mesh;
    public HexagonalGrid grid;
    private MotherShip ship;

    public float outlineSize = 0.05f;
    public Color baseColor = new Color(1, 1, 1, 0.4f);

    public List<int>[,] tilesTriangles;

    public void Awake() {
        mesh = GetComponent<MeshFilter>().mesh;
    }

    public void Start() {
        ship = GameManager.Instance.motherShip;
        ship.OnTurnBegin += CreateMesh;

        CreateMesh();
        //TestReachable();
    }

    void TestReachable() {
        List<TileProperties> tiles = TilesReachable(ship.Movable.CurrentTile, (int)ship.Inventory.GetResource(ship.fuelResource));
        foreach (TileProperties tile in tiles) {
            tile.hexagonLayer.color = Color.red;
        }
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
        List<TileProperties> tiles = TilesReachable(ship.Movable.CurrentTile, (int)ship.Inventory.GetResource(ship.fuelResource));
        foreach (TileProperties tile in tiles)
            tile.IsInReachables = true;

        transform.position = new Vector3(0, 0, -0.1f);

        mesh.Clear();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> vertices2 = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<Vector2> uvs2 = new List<Vector2>();
        int[,] tilesNbRect = new int[grid.tilesArray.GetLength(0), grid.tilesArray.GetLength(1)];
        int[,] tilesNbRect2 = new int[grid.tilesArray.GetLength(0), grid.tilesArray.GetLength(1)];
        int nbRect = 0;
        int nbRect2 = 0;
        for (int x = 0; x < grid.tilesArray.GetLength(0); x++) {
            for (int y = 0; y < grid.tilesArray.GetLength(1); y++) {
                TileProperties tile = grid.tilesArray[x, y];

                if (!tile || (tile.IsWalkable() && tile.IsInReachables))
                    continue;
                for (HexDirection h = (HexDirection)0; h < (HexDirection)6; h++) {
                    if (!tile.GetNeighbor(h) || !tile.GetNeighbor(h.Previous()) || !tile.GetNeighbor(h.Next()))
                        continue;

                    if (tile.GetNeighbor(h).IsWalkable() && tile.GetNeighbor(h).IsInReachables) {
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
                    /*
                    if (tile.GetNeighbor(h).IsInReachables) {
                        if (tile.GetNeighbor(h.Previous()).IsInReachables)
                            AddVertices2(ref vertices2, ref uvs2, tile, h, true);
                        else
                            AddVertices(ref vertices2, ref uvs2, tile, h, true);

                        if (tile.GetNeighbor(h.Next()).IsInReachables)
                            AddVertices2(ref vertices2, ref uvs2, tile, h.Next(), false);
                        else
                            AddVertices(ref vertices2, ref uvs2, tile, h.Next(), false);

                        tilesNbRect2[x, y]++;
                        nbRect2++;
                    }*/
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.uv = uvs.ToArray();


        int trianglesSize = nbRect * 6;
        int[] triangles = new int[trianglesSize];
        int ti = 0, vi = 0;
        tilesTriangles = new List<int>[grid.tilesArray.GetLength(0), grid.tilesArray.GetLength(1)];
        for (int x = 0; x < grid.tilesArray.GetLength(0); x++) {
            for (int y = 0; y < grid.tilesArray.GetLength(1); y++) {
                TileProperties tile = grid.tilesArray[x, y];
                if (!tile || (tile.IsWalkable() && tile.IsInReachables))
                    continue;
                tilesTriangles[x, y] = new List<int>();
                for (int i = 0; i < tilesNbRect[x, y]; i++) {
                    triangles[ti] = vi;
                    triangles[ti + 1] = triangles[ti + 4] = vi + 1;
                    triangles[ti + 2] = triangles[ti + 3] = vi + 2;
                    triangles[ti + 5] = vi + 3;
                    for (int v = 0; v < 4; v++)
                        tilesTriangles[x, y].Add(vi + v);
                    ti += 6;
                    vi += 4;
                }
                //vi += 2;
            }
        }
        
        mesh.SetTriangles(triangles, 0);
        Color[] col = new Color[vertices.Count];
        for (int i = 0; i < vertices.Count; i++) {
            col[i] = baseColor;
        }
        mesh.colors = col;

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

    public void SetTileColor(int x, int y, Color c) {
        Color[] col = mesh.colors;
        foreach (int vIndex in tilesTriangles[x, y]) {
            col[vIndex] = c;
        }
        mesh.colors = col;
    }

    public void ResetTileColor(int x, int y) {
        Color[] col = mesh.colors;
        foreach (int vIndex in tilesTriangles[x, y]) {
            col[vIndex] = baseColor;
        }
        mesh.colors = col;
    }

    public void ResetTiles() {
        Color[] col = mesh.colors;
        for (int i = 0; i < col.Length; i++) {
            col[i] = baseColor;
        }
        mesh.colors = col;
    }
}
