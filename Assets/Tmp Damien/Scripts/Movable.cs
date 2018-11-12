using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Movable : MonoBehaviour
{
    public float speed;
    public Tilemap tilemap;
    public HexagonalGrid hexGrid;

    Vector3 beginPos;
    Vector3 targetPos;
    bool moving;
    float progress;
    TileProperties currentTile;

    DebugMovable debug;
    Stack<TileProperties> path;

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


    private void Update() {
        if (Time.frameCount == 1) {
            Vector3Int cellPosition = tilemap.WorldToCell(transform.position);
            currentTile = hexGrid.GetTile(new HexCoordinates(cellPosition));
        }
        if (moving) {
            progress += speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(beginPos, targetPos, progress);
            if (transform.position == targetPos) {
                moving = false;
                progress = 0;
                Vector3Int cellPosition = tilemap.WorldToCell(transform.position);
                currentTile = hexGrid.GetTile(new HexCoordinates(cellPosition.x, cellPosition.y));
                //debug.UpdateDebug();
            }
        }
    }

    public void MoveTo(TileProperties goal) {
        if (!moving) {
            path = hexGrid.Path(currentTile, goal);
            debug.UpdateDebug(DebugMovable.DebugMode.Path);
            beginPos = transform.position;
            //this.targetPos = goal;
            moving = true;
            progress = 0;
        }
    }

}

    
