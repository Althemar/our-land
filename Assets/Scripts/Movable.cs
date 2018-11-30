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
        path = new Stack<TileProperties>();
        tilemap = hexGrid.GetComponent<Tilemap>();
    }

    private void Update() {
        if (Time.frameCount == 1) {
            Vector3Int cellPosition = tilemap.WorldToCell(transform.position);
            currentTile = hexGrid.GetTile(new HexCoordinates(cellPosition.x, cellPosition.y));
            currentTile.currentMovable = this;
        }
        if (moving) {
            if (FastTurns.Instance != null && FastTurns.Instance.isFastTurn) {
                progress += speed * FastTurns.Instance.speedMultiplicator * Time.deltaTime;
            }
            else {
                progress += speed * Time.deltaTime;
            }
            
            
            transform.position = Vector3.MoveTowards(beginPos, targetPos, progress);
            if (transform.position == targetPos) {
                progress = 0;
                Vector3Int cellPosition = tilemap.WorldToCell(transform.position);

                currentTile.currentMovable = null;
                currentTile = targetTile;   
                currentTile.currentMovable = this;
                if (path.Count == 0) {
                    EndMoving();
                }
                else {
                    if (path.Count >= 1) {
                        targetTile = path.Pop();
                        beginPos = transform.position;
                        targetPos = tilemap.CellToWorld(targetTile.Position);
                        
                        UpdateSpriteDirection();
                    }
                    else {
                        EndMoving();
                    }
                }
            }
        }
    }

    public void MoveToTile(TileProperties goal, bool calculatePath = true) {
        if (!moving) {
            if (calculatePath) {
                path = AStarSearch.Path(currentTile, goal);
            }
            
            path.Pop();
            targetTile = path.Pop();
            targetPos = tilemap.CellToWorld(targetTile.Position);

            UpdateSpriteDirection();

            beginPos = transform.position;
            moving = true;
            progress = 0;

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

    public TileProperties MoveToward(Stack<TileProperties> totalPath, int movementPoints, bool stopBefore = false) {
        TileProperties lastTile = null;
        List<TileProperties> pathList = new List<TileProperties>();
        while (totalPath.Count > 0) {
            TileProperties tile = totalPath.Pop();
            if (stopBefore && totalPath.Count == 0) {
                break;
            }
            else if (movementPoints > 0) {
                pathList.Add(tile);
                if (tile != currentTile) {
                    movementPoints -= tile.Tile.walkCost;
                    lastTile = tile;
                }
            }
            else {
                break;
            }
        }

        path.Clear();
        for (int i = pathList.Count - 1; i >= 0; i--) {
            path.Push(pathList[i]);
        }

        MoveToTile(lastTile, false);
        return lastTile;
    }

    public void EndMoving() {
        moving = false;
        if (OnReachEndTile != null) {
            OnReachEndTile();
        }
    }

}

    
