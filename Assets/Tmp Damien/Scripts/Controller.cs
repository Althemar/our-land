using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Controller : MonoBehaviour
{
    public HexagonalGrid hexagonalGrid;
    public Movable tmpMovable;
    public HexGridLabels gridPositions;

    Camera cam;
    ReachableTilesDisplay reachableTiles;

    void Start() {
        cam = Camera.main;
        reachableTiles = GetComponent<ReachableTilesDisplay>();
    }


    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 worldPosition = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = hexagonalGrid.Tilemap.WorldToCell(worldPosition);

            CustomTile tile = hexagonalGrid.Tilemap.GetTile<CustomTile>(cellPosition);
            if (tile.canWalkThrough) {
                HexCoordinates coordinates = new HexCoordinates(cellPosition, HexCoordinatesType.offset);
                tmpMovable.MoveTo(hexagonalGrid.GetTile(coordinates));
            }
        }
        if (Input.GetKeyDown(KeyCode.P)) {
            gridPositions.SwitchDisplay();
        }
        if (Input.GetKeyDown(KeyCode.O)) {
            hexagonalGrid.ChangeCoordinateSystem(HexCoordinatesType.offset);
            gridPositions.RefreshCoordinates();
        }
        if (Input.GetKeyDown(KeyCode.A)) {
            hexagonalGrid.ChangeCoordinateSystem(HexCoordinatesType.axial);
            gridPositions.RefreshCoordinates();
        }
        if (Input.GetKeyDown(KeyCode.C)) {
            hexagonalGrid.ChangeCoordinateSystem(HexCoordinatesType.cubic);
            gridPositions.RefreshCoordinates();
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            gridPositions.RefreshDistances(tmpMovable.CurrentTile);
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            tmpMovable.DebugMovable.SwitchMode();
        }
        if (Input.GetMouseButtonDown(1)) {
            Vector3 worldPosition = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = hexagonalGrid.Tilemap.WorldToCell(worldPosition);
            HexCoordinates coordinates = new HexCoordinates(cellPosition, HexCoordinatesType.offset);
            TileProperties tile = hexagonalGrid.GetTile(coordinates);
            reachableTiles.InitReachableTiles(tmpMovable.CurrentTile.TilesReachable(tmpMovable.walkDistance), tile, tmpMovable);
        }
        else if (Input.GetMouseButtonUp(1)) {
            reachableTiles.UndisplayReachables();
            Vector3 worldPosition = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = hexagonalGrid.Tilemap.WorldToCell(worldPosition);
            HexCoordinates coordinates = new HexCoordinates(cellPosition, HexCoordinatesType.offset);
            tmpMovable.MoveTo(hexagonalGrid.GetTile(coordinates));
        }
        if (reachableTiles.Displaying) {
            Vector3 worldPosition = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = hexagonalGrid.Tilemap.WorldToCell(worldPosition);
            HexCoordinates coordinates = new HexCoordinates(cellPosition, HexCoordinatesType.offset);
            reachableTiles.RefreshPath(hexagonalGrid.GetTile(coordinates));
        }
    }

    
}
