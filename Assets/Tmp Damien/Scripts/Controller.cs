using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Controller : MonoBehaviour
{
    public HexagonalGrid hexagonalGrid;
    public Movable tmpMovable;
    public HexagonalGridPositions gridPositions;

    Camera cam;

    void Start() {
        cam = Camera.main;
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 worldPosition = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = hexagonalGrid.Tilemap.WorldToCell(worldPosition);

            CustomTile tile = hexagonalGrid.Tilemap.GetTile<CustomTile>(cellPosition);
            if (tile.canWalkThrough) {
                tmpMovable.MoveTo(hexagonalGrid.GetTile(new HexCoordinates(cellPosition, HexCoordinatesType.offset)));
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
    }

    
}
