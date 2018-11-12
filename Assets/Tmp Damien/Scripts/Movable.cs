﻿using System.Collections;
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
    TileProperties goalTile;

    DebugMovable debug;
    Stack<TileProperties> path;

    public TileProperties CurrentTile
    {
        get { return currentTile; }
    }

    public TileProperties TargetTile
    {
        get { return goalTile; }
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
                if (path.Count == 0) {
                    moving = false;
                    debug.Mode = DebugMovable.DebugMode.Neighbors;
                    debug.UpdateDebug();
                }
                else {
                    debug.UpdateDebug();
                    beginPos = transform.position;
                    targetPos = tilemap.CellToWorld(path.Pop().Position);
                }
                progress = 0;
                Vector3Int cellPosition = tilemap.WorldToCell(transform.position);
                currentTile = hexGrid.GetTile(new HexCoordinates(cellPosition.x, cellPosition.y));

            }
        }
    }

    public void MoveTo(TileProperties goal) {
        if (!moving) {
            path = AStarSearch.Path(currentTile, goal);
            goalTile = goal;

            debug.Mode = DebugMovable.DebugMode.Path;
            debug.UpdateDebug();

            targetPos = tilemap.CellToWorld(path.Pop().Position);
            beginPos = transform.position;
            moving = true;
            progress = 0;
        }
    }

}

    
