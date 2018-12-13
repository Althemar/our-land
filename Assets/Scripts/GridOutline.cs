using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GridOutline : MonoBehaviour {
    private Mesh mesh;
    public HexagonalGrid grid;

    public float outlineSize = 0.05f;

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
                nbRect += 3;
            }
        }
        
        mesh.vertices = vertices.ToArray();
        CreateTriangles(vertices, nbRect);
    }

    // Add 2 vertices to the mesh
    private void AddVertices(ref List<Vector3> vertices, TileProperties current, HexDirection direction) {
        Vector3 left = current.transform.position + current.Grid.Metrics.GetCorner(false, direction);
        Vector3 other = left + current.Grid.Metrics.GetCorner(false, direction) * outlineSize;
        left -= current.Grid.Metrics.GetCorner(false, direction) * outlineSize;

        vertices.Add(left);
        vertices.Add(other);
    }

    private void CreateTriangles(List<Vector3> vertices, int nbRectangles) {
        int trianglesSize = nbRectangles * 6;
        int[] triangles = new int[trianglesSize];
        for (int ti = 0, vi = 0, i = 0; i < nbRectangles; ti += 6, vi += 2, i++) {
            if (i != 0 && i % 3 == 0)
                vi += 2;
            triangles[ti] = vi;
            triangles[ti + 1] = triangles[ti + 4] = vi + 1;
            triangles[ti + 2] = triangles[ti + 3] = vi + 2;
            triangles[ti + 5] = vi + 3;
        }

        mesh.triangles = triangles;
    }

    public void Clear() {
        mesh.Clear();
    }
}
