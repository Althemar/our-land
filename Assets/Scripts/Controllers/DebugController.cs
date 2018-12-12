using UnityEngine;

/*
 * DebugController
 * Debug keys for displaying coordinates, distances
 */

public class DebugController : MonoBehaviour
{
    public HexagonalGrid hexagonalGrid;
    public Movable tmpMovable;
    public HexGridLabels gridLabels;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) {
            gridLabels.SwitchDisplay();
        }
        if (Input.GetKeyDown(KeyCode.O)) {
            hexagonalGrid.ChangeCoordinateSystem(HexCoordinatesType.offset);
            gridLabels.RefreshCoordinates();
        }
        if (Input.GetKeyDown(KeyCode.A)) {
            hexagonalGrid.ChangeCoordinateSystem(HexCoordinatesType.axial);
            gridLabels.RefreshCoordinates();
        }
        if (Input.GetKeyDown(KeyCode.C)) {
            hexagonalGrid.ChangeCoordinateSystem(HexCoordinatesType.cubic);
            gridLabels.RefreshCoordinates();
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            gridLabels.RefreshDistances(tmpMovable.CurrentTile);
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            tmpMovable.DebugMovable.SwitchMode();
        }
        if (Input.GetKeyDown(KeyCode.Q)) {
            tmpMovable.DebugMovable.ActivateDebug();
        }
    }
}
