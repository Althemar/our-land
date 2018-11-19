using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Movable : MonoBehaviour
{
    /*
     * Members
     */
     
    public float speed;
    public int walkDistance;
    public HexagonalGrid hexGrid;
    
    private Tilemap tilemap;
    private DebugMovable debug;

    //movement
    private Stack<TileProperties> path;
    private Vector3 beginPos;
    private Vector3 targetPos;
    private bool moving;
    private float progress;
    private TileProperties currentTile;

    private List<TileProperties> reachableTiles;

    /*
     * Properties
     */

    public TileProperties CurrentTile
    {
        get { return currentTile; }
    }

    public DebugMovable DebugMovable
    {
        get { return debug; }
        set { debug = value; }
    }

    public Stack<TileProperties> Path
    {
        get { return path; }
    }

    public bool Moving
    {
        get => moving;
    }

    public List<TileProperties> ReachableTiles
    {
        get => reachableTiles;
        set => reachableTiles = value;
    }

    /*
     * Methods
     */

    private void Start() {
        tilemap = hexGrid.GetComponent<Tilemap>();
    }

    private void Update() {
        if (Time.frameCount == 1) {
            Vector3Int cellPosition = tilemap.WorldToCell(transform.position);
            currentTile = hexGrid.GetTile(new HexCoordinates(cellPosition));
        }
        if (moving) {
            progress += speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(beginPos, targetPos, progress);
            if (transform.position == targetPos) {
               
                
                progress = 0;
                Vector3Int cellPosition = tilemap.WorldToCell(transform.position);
                currentTile = hexGrid.GetTile(new HexCoordinates(cellPosition.x, cellPosition.y));
                debug.UpdateDebug();

                if (path.Count == 0) {
                    moving = false;
                }
                else {
                    beginPos = transform.position;
                    targetPos = tilemap.CellToWorld(path.Pop().Position);
                }
            }
        }
    }

    public void MoveTo(TileProperties goal) {
        if (!moving) {
            path = AStarSearch.Path(currentTile, goal);

            targetPos = tilemap.CellToWorld(path.Pop().Position);
            beginPos = transform.position;
            moving = true;
            progress = 0;
        }
    }

}

    
