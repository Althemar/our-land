using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Spine.Unity;

public class Movable : MonoBehaviour
{
    /*
     * Members
     */
     
    public float speed;
    public int walkDistance;
    public HexagonalGrid hexGrid;

    public SkeletonAnimation spine;
    
    private Tilemap tilemap;
    private DebugMovable debug;

    //movement
    private Stack<TileProperties> path;
    private Vector3 beginPos;
    private Vector3 targetPos;
    private TileProperties targetTile;
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
                currentTile = targetTile;   
                currentTile.currentMovable = this;
                if (path.Count == 0 || (path.Count == 1 && stopBefore)) {
                    EndMoving();
                }
                else {
                    targetTile = path.Pop();
                    if (!useMovementPoints || movementPoints > 0) {
                        beginPos = transform.position;
                        targetPos = tilemap.CellToWorld(targetTile.Position);

                        movementPoints -= targetTile.Tile.walkCost;
                        UpdateSpriteDirection();
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
            targetTile = path.Pop();
            targetTile = path.Pop();
            targetPos = tilemap.CellToWorld(targetTile.Position);

            UpdateSpriteDirection();

            beginPos = transform.position;
            moving = true;
            progress = 0;
            useMovementPoints = false;
            stopBefore = false;

            currentTile.currentMovable = null;
            goal.currentMovable = this;
        }
    }


    void UpdateSpriteDirection() {
        if (spine == null)
            return;

        if (targetTile.Coordinates.X == currentTile.Coordinates.X) {
            spine.skeleton.ScaleX = 1;
            if (targetTile.Coordinates.Y < currentTile.Coordinates.Y)
                spine.skeleton.SetSkin("Front_Left");
            else
                spine.skeleton.SetSkin("Back_Right");
        }
        if (targetTile.Coordinates.Y == currentTile.Coordinates.Y) {
            spine.skeleton.SetSkin("Profile_Left");
            spine.skeleton.ScaleX = (targetTile.Coordinates.Z < currentTile.Coordinates.Z) ? -1 : 1;
        }
        if (targetTile.Coordinates.Z == currentTile.Coordinates.Z) {
            spine.skeleton.ScaleX = -1;
            if (targetTile.Coordinates.Y < currentTile.Coordinates.Y)
                spine.skeleton.SetSkin("Front_Left");
            else
                spine.skeleton.SetSkin("Back_Right");
        }
    }

    public void MoveToward(Stack<TileProperties> path, int movementPoints, bool stopBefore = false) {
        this.path = path;
        targetTile = path.Pop();
        targetTile = path.Pop();

        targetPos = tilemap.CellToWorld(targetTile.Position);

        beginPos = transform.position;
        moving = true;
        progress = 0;

        this.movementPoints = movementPoints;
        this.movementPoints -= targetTile.Tile.walkCost;

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

    
