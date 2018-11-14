using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Movable))]
public class ReachableTilesDisplay : MonoBehaviour
{
    public GameObject edgePrefab;

    List<TileProperties> reachables;
    Stack<TileProperties> currentPath;
    bool displaying;
    Movable movable;
    TileProperties currentTile;


    List<Vector3> vertices;
    Mesh mesh;
    TileProperties meshBegin;
   

    public bool Displaying
    {
        get => displaying;
    }

    public TileProperties CurrentTile
    {
        get => currentTile;
    }

    private void Start() {
        movable = GetComponent<Movable>();
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = new List<Vector3>();
    }


    public void InitReachableTiles(List<TileProperties> reachables, TileProperties tile, Movable movable) {
        if (movable.Moving) {
            return;
        }
        this.movable = movable;
        this.reachables = reachables;
        displaying = true;
        for (int i = 0; i < reachables.Count; i++) {
            //reachables[i].Tilemap.SetColor(reachables[i].Position, new Color(0.6f, 0.6f, 1f,1f));
        }
        GetLimits();

        foreach (TileProperties reachable in reachables) {
            reachable.IsInReachables = false;
        }
    }

    public void GetLimits() {
        vertices.Clear();
        meshBegin = reachables[reachables.Count - 1];
        TileProperties current = meshBegin;
        HexDirection neighborIndexBegin = HexDirection.NW;
        
        int nbRectangles = GetNextReachable(ref current, ref neighborIndexBegin);
        while (current != meshBegin) {
            nbRectangles += GetNextReachable(ref current, ref neighborIndexBegin);
        }

        nbRectangles += GetNextReachable(ref current, ref neighborIndexBegin, true);
        vertices.Add(vertices[0]);
        vertices.Add(vertices[1]);
        mesh.vertices = vertices.ToArray();
        
        int[] triangles = new int[nbRectangles * 6];
        for (int ti = 0, vi = 0, i = 0; i < nbRectangles; ti+=6, vi+=2, i++) {
            triangles[ti] = vi;
            triangles[ti + 1] = triangles[ti + 4] = vi + 1;
            triangles[ti + 2] = triangles[ti + 3] = vi + 2;
            triangles[ti + 5] = vi + 3;
        }
        mesh.triangles = triangles;
    }

    public int GetNextReachable(ref TileProperties current, ref HexDirection begin, bool stopAtNW = false) {
        HexDirection currentDirection = begin.Next();
        HexDirection end = begin;
        if (stopAtNW) {
            end = HexDirection.NE;
        }
        int rectangleCount = 0;

        
        while (currentDirection != end+1 || begin.Next() == currentDirection) {
            TileProperties neighbor = current.GetNeighbor(currentDirection);
            if (neighbor && !neighbor.IsInReachables) {
                rectangleCount++;
                Vector3 left = current.transform.position + current.Grid.Metrics.GetCorner(false, currentDirection);
                Vector3 other;
                if (currentDirection != begin.Next() || (current == meshBegin && currentDirection == HexDirection.NE)) {
                    other = left + current.Grid.Metrics.GetCorner(false, currentDirection) * 0.15f;
                }
                else {
                    HexDirection previousBorder = begin.Previous().Opposite();
                    Vector3 currentOffset = current.Grid.Metrics.GetCorner(false, currentDirection) + current.Grid.Metrics.GetCorner(true, currentDirection);
                    Vector3 previousOffset = current.Grid.Metrics.GetCorner(false, previousBorder) + current.Grid.Metrics.GetCorner(true, previousBorder);
                    other = left + (currentOffset + previousOffset) * 0.05f;
                }                
                vertices.Add(left);
                vertices.Add(other);
                currentDirection = currentDirection.Next();
            }
            else if (neighbor && neighbor.IsInReachables) {
                begin = (currentDirection).Opposite();
                current = neighbor;
                return rectangleCount;
            }
            else {
                currentDirection = currentDirection.Next();
            }
        }
        return rectangleCount;
    }

    public void UndisplayReachables() {
        displaying = false;
        for (int i = 0; i < reachables.Count; i++) {
            reachables[i].Tilemap.SetColor(reachables[i].Position, Color.white);
        }
        
        mesh.Clear();
    }

    private void OnDrawGizmos() {
        if (vertices == null) {
            return;
        }
        Gizmos.color = Color.black;
        for (int i = 0; i < vertices.Count; i++) {
            Gizmos.DrawSphere(vertices[i], 0.05f);
        }
    }

    public void ColorPath(Stack<TileProperties> path, Color color) {
        while (path.Count > 0) {
            TileProperties tile = path.Pop();
            tile.Tilemap.SetColor(tile.Position, color);
        }
    }

    public void RefreshPath(TileProperties tile) {
        if (tile != currentTile) {
            if (reachables.Contains(currentTile)) {
                ColorPath(currentPath, Color.white);
            }
            currentTile = tile;
            if (reachables.Contains(currentTile)) {
                currentPath = AStarSearch.Path(movable.CurrentTile, tile);
                ColorPath(new Stack<TileProperties>(currentPath), new Color(1f, 0.3f, 0.3f, 1f));
            }
        }
    }
}
