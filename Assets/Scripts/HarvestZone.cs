using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FillZone : MonoBehaviour {
    private Mesh mesh;

    void Start() {
        //EXAMPLE
        List<TileProperties> tilesInRange = GameManager.Instance.motherShip.Movable.CurrentTile.InRange(1);
        InitMesh(tilesInRange);
    }

    public void InitMesh(List<TileProperties> reachables) {
        mesh = GetComponent<MeshFilter>().mesh;
        transform.position = new Vector3(0, 0, -0.1f);
        List<Vector3> vertices = new List<Vector3>();

        foreach (TileProperties current in reachables)
            AddVertices(ref vertices, current);

        mesh.vertices = vertices.ToArray();
        CreateTriangles(vertices, reachables.Count);
    }

    // Add 2 vertices to the mesh
    private void AddVertices(ref List<Vector3> vertices, TileProperties current) {
        HexDirection currentDirection = HexDirection.NE;
        for (int i = 0; i < 6; i++) {
            Vector3 corner = current.transform.position + current.Grid.Metrics.GetCorner(false, currentDirection);
            vertices.Add(corner);
            currentDirection = currentDirection.Next();
        }
    }

    private void CreateTriangles(List<Vector3> vertices, int nbHexagon) {
        int trianglesSize = nbHexagon * 4 * 3;
        int[] triangles = new int[trianglesSize];
        for (int ti = 0, vi = 0; ti < trianglesSize; ti += (4 * 3), vi += 6) {
            triangles[ti] = vi;
            triangles[ti + 1] = vi + 1;
            triangles[ti + 2] = vi + 2;

            triangles[ti + 3] = vi + 2;
            triangles[ti + 4] = vi + 3;
            triangles[ti + 5] = vi + 4;

            triangles[ti + 6] = vi + 4;
            triangles[ti + 7] = vi + 5;
            triangles[ti + 8] = vi;

            triangles[ti + 9] = vi;
            triangles[ti + 10] = vi + 2;
            triangles[ti + 11] = vi + 4;
        }

        mesh.triangles = triangles;
    }

    public void Clear() {
        mesh.Clear();
    }
}
