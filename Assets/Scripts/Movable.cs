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

    private int movementPoints;
    private bool useMovementPoints;
    private bool stopBefore;

    private List<TileProperties> reachableTiles;

    public delegate void OnMovableDelegate();

    public event OnMovableDelegate OnReachEndTile;



    /*
     * Properties
     */

    public TileProperties CurrentTile
    {
        get { return currentTile; }
        set => currentTile = value;
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
        movementPoints = -1;
    }

    private void Update() {
        if (Time.frameCount == 1) {
            Vector3Int cellPosition = tilemap.WorldToCell(transform.position);
            currentTile = hexGrid.GetTile(new HexCoordinates(cellPosition));
            currentTile.currentMovable = this;
        }
        if (moving) {
            progress += speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(beginPos, targetPos, progress);
            if (transform.position == targetPos) {
               
                progress = 0;
                Vector3Int cellPosition = tilemap.WorldToCell(transform.position);
                currentTile.currentMovable = null;
                currentTile = hexGrid.GetTile(new HexCoordinates(cellPosition.x, cellPosition.y));
                currentTile.currentMovable = this;
                //debug.UpdateDebug();
                if (path.Count == 0 || (path.Count == 1 && stopBefore)) {
                    EndMoving();
                }
                else {
                    TileProperties nextTile = path.Pop();
                    if (!useMovementPoints || movementPoints > 0) {
                        beginPos = transform.position;
                        targetPos = tilemap.CellToWorld(nextTile.Position);

                        movementPoints -= nextTile.Tile.walkCost;
                    }
                    else {
                        EndMoving();
                    }
                }
            }
        }
    }

    public void MoveToTile(TileProperties goal) {
        if (!moving) {
            path = AStarSearch.Path(currentTile, goal);

            targetPos = tilemap.CellToWorld(path.Pop().Position);

            beginPos = transform.position;
            moving = true;
            progress = 0;
            useMovementPoints = false;
            stopBefore = false;

            currentTile.currentMovable = null;
            goal.currentMovable = this;
        }
    }

    public void MoveToward(Stack<TileProperties> path, int movementPoints, bool stopBefore = false) {
        this.path = path;
        targetPos = tilemap.CellToWorld(path.Pop().Position);

        beginPos = transform.position;
        moving = true;
        progress = 0;

        this.movementPoints = movementPoints;
        useMovementPoints = true;
        this.stopBefore = stopBefore;

        currentTile.currentMovable = null;

    }

    public void EndMoving() {
        moving = false;
        if (OnReachEndTile != null) {
            OnReachEndTile();
        }
    }

}

    
