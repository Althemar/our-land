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

    public TileProperties CurrentTile
    {
        get { return currentTile; }
    }

    void Start() {
        debug = GetComponent<DebugMovable>();
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
                currentTile = hexGrid.GetTile(new HexCoordinates(cellPosition));
                debug.UpdateDebug();
            }
        }
    }

    public void MoveTo(Vector3 targetPos) {
        if (!moving) {
            beginPos = transform.position;
            this.targetPos = targetPos;
            moving = true;
            progress = 0;
        }
    }

}

    
