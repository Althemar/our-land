using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GridOutline : MonoBehaviour {
    private Mesh mesh;
    public HexagonalGrid grid;

    public float outlineSize = 0.05f;
    public float outlineGap = 0.03f;
    public Color baseColor = new Color(1, 1, 1, 0.4f);

    public List<int>[,] tilesTriangles;

    public void Start() {
        mesh = GetComponent<MeshFilter>().mesh;
        transform.position = new Vector3(0, 0, -0.1f);
        List<Vector3> vertices = new List<Vector3>();

        int nbRect = 0;
        for (int x = 0; x < grid.tilesArray.GetLength(0); x++) {
            for (int y = 0; y < grid.tilesArray.GetLength(1); y++) {
                TileProperties tile = grid.tilesArray[x, y];
                if (!tile)
                    continue;
                AddVertices(ref vertices, tile, HexDirection.NE);
                AddVertices(ref vertices, tile, HexDirection.E);
                AddVertices(ref vertices, tile, HexDirection.SE);
                AddVertices(ref vertices, tile, HexDirection.SW);
                AddVertices(ref vertices, tile, HexDirection.W);
                AddVertices(ref vertices, tile, HexDirection.NW);
                AddVertices(ref vertices, tile, HexDirection.NE);
                nbRect += 6;
            }
        }
        
        mesh.vertices = vertices.ToArray();


        int trianglesSize = nbRect * 6;
        int[] triangles = new int[trianglesSize];
        int ti = 0, vi = 0;
        tilesTriangles = new List<int>[grid.tilesArray.GetLength(0), grid.tilesArray.GetLength(1)];
        for (int x = 0; x < grid.tilesArray.GetLength(0); x++) {
            for (int y = 0; y < grid.tilesArray.GetLength(1); y++) {
                TileProperties tile = grid.tilesArray[x, y];
                if (!tile)
                    continue;
                tilesTriangles[x, y] = new List<int>();
                for (int i = 0; i < 6; i++) {
                    triangles[ti] = vi;
                    triangles[ti + 1] = triangles[ti + 4] = vi + 1;
                    triangles[ti + 2] = triangles[ti + 3] = vi + 2;
                    triangles[ti + 5] = vi + 3;
                    for(int v = 0; v < 4; v++)
                        tilesTriangles[x, y].Add(vi + v);
                    ti += 6;
                    vi += 2;
                }
                vi += 2;
            }
        }
        
        mesh.triangles = triangles;
        Color[] col = new Color[vertices.Count];
        for (int i = 0; i < vertices.Count; i++) {
            col[i] = baseColor;
        }
        mesh.colors = col;
    }

    // Add 2 vertices to the mesh
    private void AddVertices(ref List<Vector3> vertices, TileProperties current, HexDirection direction) {
        Vector3 left = current.transform.position + current.Grid.Metrics.GetCorner(false, direction);
        Vector3 other = left + current.Grid.Metrics.GetCorner(false, direction) * outlineSize;
        left -= current.Grid.Metrics.GetCorner(false, direction) * outlineSize;

        other -= current.Grid.Metrics.GetCorner(false, direction) * outlineGap;
        left -= current.Grid.Metrics.GetCorner(false, direction) * outlineGap;

        vertices.Add(left);
        vertices.Add(other);
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
