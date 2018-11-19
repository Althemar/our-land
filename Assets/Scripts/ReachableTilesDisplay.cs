using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * ReachableTilesDisplay
 * Display a preview for the movement : 
 *      - Procedural mesh that cover the limit of the available tiles
 *      - Color the tiles in the path to the pointed tile
 */
 
public class ReachableTilesDisplay : MonoBehaviour
{
    private bool displaying;                   
    private Movable movable;                    
    private TileProperties currentPointedTile;      // current pointed tile by the mouse. used to refresh the path   

    private List<TileProperties> reachables;    
    private Stack<TileProperties> currentPath;  

    private Mesh mesh;

    public bool Displaying
    {
        get => displaying;
    }

    public TileProperties CurrentTile
    {
        get => currentPointedTile;
    }

    private void Start() {
        mesh = GetComponent<MeshFilter>().mesh;
    }


    public void InitReachableTiles(List<TileProperties> reachables, TileProperties tile, Movable movable) {
        if (movable.Moving) {
            return;
        }
        this.movable = movable;
        this.reachables = reachables;
        displaying = true;
        GetLimits();

        foreach (TileProperties reachable in reachables) {
            reachable.IsInReachables = false;
        }
    }

    public void GetLimits() {
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
    public int GetNextReachable(ref List<Vector3> vertices, ref TileProperties current, ref HexDirection begin, bool stopAtNW = false) {
        HexDirection currentDirection = begin.Next();
        HexDirection end = begin;
        if (stopAtNW) {
            end = HexDirection.NE;
        }
        int rectangleCount = 0;

        // Iterate through neighbors clockwise
        while (currentDirection != end+1 || begin.Next() == currentDirection) {

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
    public void AddVertices(ref List<Vector3> vertices, TileProperties current, HexDirection currentDirection, HexDirection begin) {
        Vector3 left = current.transform.position + current.Grid.Metrics.GetCorner(false, currentDirection);
        Vector3 other;
        if (currentDirection != begin.Next()) {
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
    }

    public void CreateTriangles(List<Vector3> vertices, int nbRectangles) {
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

    public void UndisplayReachables() {
        displaying = false;
        for (int i = 0; i < reachables.Count; i++) {
            reachables[i].Tilemap.SetColor(reachables[i].Position, Color.white);
        }
        mesh.Clear();
    }

    public void ColorPath(Stack<TileProperties> path, Color color) {
        while (path.Count > 0) {
            TileProperties tile = path.Pop();
            tile.Tilemap.SetColor(tile.Position, color);
        }
    }

    public void RefreshPath(TileProperties tile) {
        if (tile != currentPointedTile) {
            if (reachables.Contains(currentPointedTile)) {
                ColorPath(currentPath, Color.white);
            }
            currentPointedTile = tile;
            if (reachables.Contains(currentPointedTile)) {
                currentPath = AStarSearch.Path(movable.CurrentTile, tile);
                ColorPath(new Stack<TileProperties>(currentPath), new Color(1f, 0.3f, 0.3f, 1f));
            }
        }
    }
}
