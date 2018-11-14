using UnityEngine;

/*
 * DebugController
 * Debug keys for displaying coordinates, distances
 */

public class DebugController : MonoBehaviour
{
    public HexagonalGrid hexagonalGrid;
    public Movable tmpMovable;
    public HexGridLabels gridPositions;

    // Update is called once per frame
    void Update()
    {
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
