using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class OutlineWalkable : MonoBehaviour {
    private Mesh mesh;
    public HexagonalGrid grid;

    public float outlineSize = 0.05f;
    public Color baseColor = new Color(1, 1, 1, 0.4f);

    public List<int>[,] tilesTriangles;

    public void Start() {
        mesh = GetComponent<MeshFilter>().mesh;
        CreateMesh();
    }

    public void CreateMesh() {
        transform.position = new Vector3(0, 0, -0.1f);

        mesh.Clear();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        int[,] tilesNbRect = new int[grid.tilesArray.GetLength(0), grid.tilesArray.GetLength(1)];
        int nbRect = 0;
        for (int x = 0; x < grid.tilesArray.GetLength(0); x++) {
            for (int y = 0; y < grid.tilesArray.GetLength(1); y++) {
                TileProperties tile = grid.tilesArray[x, y];
                if (!tile || tile.IsWalkable())
                    continue;
                for(HexDirection h = (HexDirection)0; h < (HexDirection)6; h++) {
                    if (!tile.GetNeighbor(h) || tile.GetNeighbor(h).IsWalkable()) {
                        if (!tile.GetNeighbor(h.Previous()) || tile.GetNeighbor(h.Previous()).IsWalkable())
                            AddVertices2(ref vertices, ref uvs, tile, h, true);
                        else
                            AddVertices(ref vertices, ref uvs, tile, h, true);

                        if (!tile.GetNeighbor(h.Next()) || tile.GetNeighbor(h.Next()).IsWalkable())
                            AddVertices2(ref vertices, ref uvs, tile, h.Next(), false);
                        else
                            AddVertices(ref vertices, ref uvs, tile, h.Next(), false);

                        tilesNbRect[x, y]++;
                        nbRect++;
                    }
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
                if (!tile || tile.IsWalkable())
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

        mesh.triangles = triangles;
        Color[] col = new Color[vertices.Count];
        for (int i = 0; i < vertices.Count; i++) {
            col[i] = baseColor;
        }
        mesh.colors = col;

        GetComponent<MeshRenderer>().sortingLayerName = "World Layer";
        GetComponent<MeshRenderer>().sortingOrder = -500;
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
