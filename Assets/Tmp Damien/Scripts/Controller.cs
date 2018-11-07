using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Controller : MonoBehaviour
{
    public Tilemap tilemap;
    public Movable tmpMovable;
    public HexagonalGridPositions gridPositions;

    Camera cam;

    void Start() {
        cam = Camera.main;
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 worldPosition = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);

            CustomTile tile = tilemap.GetTile<CustomTile>(cellPosition);
            if (tile.canWalkThrough) {
                Vector3 cellWorldPosition = tilemap.GetCellCenterWorld(cellPosition);
                tmpMovable.MoveTo(cellWorldPosition);
            }
        }
        if (Input.GetKeyDown(KeyCode.P)) {
            gridPositions.SwitchDisplay();
        }
    }

    
}
