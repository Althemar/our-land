using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MouseController : MonoBehaviour
{
    public HexagonalGrid hexGrid;
    public Movable tmpMovable;

    Camera cam;
    ReachableTilesDisplay reachableTiles;

    void Start() {
        cam = Camera.main;
        reachableTiles = GetComponent<ReachableTilesDisplay>();
    }

    void Update() {
        if (Input.GetMouseButtonDown(1)) {
            TileProperties tile = GetTile();
            tmpMovable.ReachableTiles = tmpMovable.CurrentTile.TilesReachable(tmpMovable.walkDistance);
            reachableTiles.InitReachableTiles(tmpMovable.ReachableTiles, tile, tmpMovable);
        }
        else if (Input.GetMouseButtonUp(1)) {
            reachableTiles.UndisplayReachables();
            if (tmpMovable.ReachableTiles.Contains(GetTile())) {
                tmpMovable.MoveTo(GetTile());
            }
        }
        if (reachableTiles.Displaying) {
            reachableTiles.RefreshPath(GetTile());
        }
    }

    TileProperties GetTile() {
        Vector3Int cellPosition = GetCellPositon();
        TileProperties tile = hexGrid.GetTile(cellPosition);
        return tile;
    }

    Vector3Int GetCellPositon() {
        Vector3 worldPosition = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = hexGrid.Tilemap.WorldToCell(worldPosition);
        return cellPosition;
    }
}
