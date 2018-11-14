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
            reachables[i].Tilemap.SetColor(reachables[i].Position, new Color(0.6f, 0.6f, 1f,1f));
        }
        GetLimits();
    }

    public void GetLimits() {
        TileProperties begin = reachables[reachables.Count - 1];
        TileProperties current = begin;
        HexDirection neighborIndexBegin = HexDirection.NW;

        GetNextReachable(ref current, ref neighborIndexBegin);

        while (current != begin) {
            GetNextReachable(ref current, ref neighborIndexBegin);
        }

        GetNextReachable(ref current, ref neighborIndexBegin, true);
    }

    public void GetNextReachable(ref TileProperties current, ref HexDirection begin, bool stopAtNW = false) {
        HexDirection currentDirection = begin.Next();
        HexDirection end = begin;
        if (stopAtNW) {
            end = HexDirection.NE;
        }

        while (currentDirection != end) {
            TileProperties neighbor = current.GetNeighbor(currentDirection);
            if (neighbor && !neighbor.IsInReachables) {
                Vector3 left = current.transform.position + current.Grid.Metrics.GetCorner(false, currentDirection);
                //Vector3 right = current.transform.position + current.Grid.Metrics.GetCorner(true, currentDirection);
                vertices.Add(left);
                //vertices.Add(right);
                currentDirection = currentDirection.Next();
            }
            else if (neighbor && neighbor.IsInReachables) {
                begin = (currentDirection).Opposite();
                current = neighbor;
                return;
            }
        }
    }

    public void UndisplayReachables() {
        displaying = false;
        for (int i = 0; i < reachables.Count; i++) {
            reachables[i].Tilemap.SetColor(reachables[i].Position, Color.white);
        }
        //vertices.Clear();
    }

    private void OnDrawGizmos() {
        if (vertices == null) {
            return;
        }
        Gizmos.color = Color.black;
        for (int i = 0; i < vertices.Count; i++) {
            Gizmos.DrawSphere(vertices[i], 0.1f);
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
                ColorPath(currentPath, new Color(0.6f, 0.6f, 1f, 1f));
            }
            currentTile = tile;
            if (reachables.Contains(currentTile)) {
                currentPath = AStarSearch.Path(movable.CurrentTile, tile);
                ColorPath(new Stack<TileProperties>(currentPath), new Color(1f, 0.6f, 0.6f, 1f));
            }
        }
    }
}
