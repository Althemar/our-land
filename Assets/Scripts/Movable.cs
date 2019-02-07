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
    public bool canUseWind;
    public bool canPassAboveEntities;
    public bool canPassAboveMontains;
    public bool canPassAboveLakes;
    public bool canPassAboveWindOrigins;

    public HexagonalGrid hexGrid;

    public AnimationCurve movementCurve;
    
    private Tilemap tilemap;
    private DebugMovable debug;

    //movement
    private Vector3 beginPos;
    private Vector3 targetPos;
    private TileProperties targetTile;
    private bool moving;
    private float progress;
    private TileProperties currentTile;

    private List<TileProperties> reachableTiles;

    public delegate void OnMovableDelegate();

    public event OnMovableDelegate OnReachEndTile;
    
    public delegate void OnDirectionDelegate(HexDirection dir, bool noDir = false);

    public event OnDirectionDelegate OnChangeDirection;

    private TileProperties[] pArray;
    private bool needChangeDir;

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

    public TileProperties[] Path
    {
        get { return pArray; }
        set { pArray = value; }
    }

    public bool Moving
    {
        get => moving;
        set => moving = value;
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
        if (GameManager.Instance.FrameCount == 0) {
            Vector3Int cellPosition = tilemap.WorldToCell(transform.position);
            currentTile = hexGrid.GetTile(new HexCoordinates(cellPosition.x, cellPosition.y));
            currentTile.movable = this;
            transform.position = HexagonalGrid.Instance.Tilemap.GetCellCenterWorld(currentTile.Position);
        }
        if (moving) {
            if (TurnManager.Instance != null && TurnManager.Instance.isFastTurn) {
                progress += speed * TurnManager.Instance.fastTurnSpeedMultiplicator * Time.deltaTime;
            }
            else {
                progress += speed * Time.deltaTime;
            }
            
            transform.position = Interpolate(progress);

            if(progress >= 1) {
                progress = 0;
                moving = false;
                OnReachEndTile?.Invoke();
            }
        }
    }

    public Vector3 Interpolate(float t) {
        float tCell = movementCurve.Evaluate(t) * (pArray.Length - 1);
        if (tCell < 0)
            tCell = 0;
        if (tCell > pArray.Length - 1)
            tCell = pArray.Length - 1;

        if(needChangeDir && OnChangeDirection != null) {
            for (HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; dir++) {
                if (pArray[Mathf.FloorToInt(tCell)].GetNeighbor(dir) == pArray[Mathf.CeilToInt(tCell)]) {
                    OnChangeDirection(dir);
                    break;
                }
            }
            needChangeDir = false;
        }

        Vector3 res = Vector3.Lerp(tilemap.CellToWorld(pArray[Mathf.FloorToInt(tCell)].Position), tilemap.CellToWorld(pArray[Mathf.CeilToInt(tCell)].Position), tCell - Mathf.FloorToInt(tCell));

        if(currentTile != pArray[Mathf.FloorToInt(tCell)]) {
            currentTile.movable = null;
            currentTile = pArray[Mathf.FloorToInt(tCell)];
            currentTile.movable = this;
            needChangeDir = true;
        }

        return res;
    }

    public void MoveToTile(TileProperties goal, bool calculatePath = true) {
        if (!moving) {
            if (calculatePath) {
                pArray = AStarSearch.Path(currentTile, goal).ToArray();
            }

            beginPos = transform.position;
            needChangeDir = true;
            moving = true;
            progress = 0;

            currentTile.movable = null;
            goal.movable = this;
        }
    }

    public TileProperties MoveToward(Stack<TileProperties> totalPath, int movementPoints, bool stopBefore = false) {
        TileProperties lastTile = null;
        if (totalPath.Count == 0) {
            Debug.Log("0");
        }
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
                    
                }
                lastTile = tile;
            }
            else {
                break;
            }
        }

        pArray = new TileProperties[pathList.Count];
        for (int i = pathList.Count - 1; i >= 0; i--) {
            pArray[i] = pathList[i];
        }

        if (lastTile) {
            MoveToTile(lastTile, false);
        }
        
        return lastTile;
    }
}

    
