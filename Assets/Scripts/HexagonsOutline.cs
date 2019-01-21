using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexagonsOutline : MonoBehaviour
{
    private Mesh mesh;

    public void InitMesh(List<TileProperties> reachables) {
        mesh = GetComponent<MeshFilter>().mesh;
        transform.position = new Vector3(0, 0, -0.1f);
        List<Vector3> vertices = new List<Vector3>();

        TileProperties current = reachables[0];
        for (int i = 1; i < reachables.Count; i++) {
            if (reachables[i].Position.y > current.Position.y) {
                current = reachables[i];
            }
        }
        TileProperties meshBegin = current;

        HexDirection neighborIndexBegin = HexDirection.NW;
        int nbRectangles = GetNextReachable(ref vertices, ref current, ref neighborIndexBegin);
        bool quitBegin = true;

        while (true) {
            TileProperties previous = current;
            bool isBeginAgain = (current == meshBegin && quitBegin);
            nbRectangles += GetNextReachable(ref vertices, ref current, ref neighborIndexBegin, isBeginAgain);
            if (previous == meshBegin && neighborIndexBegin == HexDirection.NE) {
                break;
            }
        }

        mesh.vertices = vertices.ToArray();
        CreateTriangles(vertices, nbRectangles);  
    }

    // Get the next rechable tiles for a tile limit. Read tiles clockwise
    private int GetNextReachable(ref List<Vector3> vertices, ref TileProperties current, ref HexDirection begin, bool stopAtNW = false) {
        HexDirection currentDirection = begin.Next();
        HexDirection end = begin;
        if (stopAtNW) {
            end = HexDirection.NE;
        }
        int rectangleCount = 0;

        // Iterate through neighbors clockwise
        while (currentDirection != end + 1 || begin.Next() == currentDirection) {

            TileProperties neighbor = current.GetNeighbor(currentDirection);

            // If neighbor is not reachable, add vertices
            if ((neighbor && !neighbor.IsInReachables) || !neighbor) {
                rectangleCount++;
                AddVertices(ref vertices, current, currentDirection, begin);
                currentDirection = currentDirection.Next();
            }
            // else current is next neighbor
            else if (neighbor && neighbor.IsInReachables) {
                begin = (currentDirection).Opposite();
                current = neighbor;
                return rectangleCount;
            }
        }
        if (stopAtNW) {
            begin = HexDirection.NE;
        }
        return rectangleCount;
    }

    // Add 2 vertices to the mesh
    private void AddVertices(ref List<Vector3> vertices, TileProperties current, HexDirection currentDirection, HexDirection begin) {
        Vector3 left = current.transform.position + current.Grid.Metrics.GetCorner(false, currentDirection);
        Vector3 other;
        if (currentDirection != begin.Next()) {
            other = left + current.Grid.Metrics.GetCorner(false, currentDirection) * 0.1f;
        }
        else {
            HexDirection previousBorder = begin.Previous().Opposite();
            Vector3 currentOffset = current.Grid.Metrics.GetCorner(false, currentDirection) + current.Grid.Metrics.GetCorner(true, currentDirection);
            Vector3 previousOffset = current.Grid.Metrics.GetCorner(false, previousBorder) + current.Grid.Metrics.GetCorner(true, previousBorder);
            other = left + (currentOffset + previousOffset) * 0.04f;
        }
        vertices.Add(left);
        vertices.Add(other);
    }

    private void CreateTriangles(List<Vector3> vertices, int nbRectangles) {
        int trianglesSize = nbRectangles * 6;
        int[] triangles = new int[trianglesSize];
        for (int ti = 0, vi = 0, i = 0; i < nbRectangles - 1; ti += 6, vi += 2, i++) {
            triangles[ti] = vi;
            triangles[ti + 1] = triangles[ti + 4] = vi + 1;
            triangles[ti + 2] = triangles[ti + 3] = vi + 2;
            triangles[ti + 5] = vi + 3;
        }

        triangles[trianglesSize - 6] = vertices.Count - 2;
        triangles[trianglesSize - 5] = triangles[trianglesSize - 2] = vertices.Count - 1;
        triangles[trianglesSize - 4] = triangles[trianglesSize - 3] = 0;
        triangles[trianglesSize - 1] = 1;
        mesh.triangles = triangles;
    }

    public void Clear() {
        mesh.Clear();
    }
}
