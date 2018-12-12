using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*
 * HexGridLabels
 * UI for cell labels. Used for displaying coordinates or distance on cells
 */

public class HexGridLabels : MonoBehaviour {
    public HexagonalGrid grid;
    public TMP_Text tilePositionPrefab;

    private Dictionary<Vector3Int, TMP_Text> cellTexts;

    private void Update() {
        if (Time.frameCount == 1) {
            cellTexts = new Dictionary<Vector3Int, TMP_Text>();

            for (int i = 0; i < grid.tilesArray.GetLength(0); i++) {
                for (int j = 0; j < grid.tilesArray.GetLength(1); j++) {
                    TileProperties tile = grid.tilesArray[i, j];
                    Vector3 cellWorldPosition = grid.Tilemap.GetCellCenterWorld(tile.Position);
                    TMP_Text text = Instantiate(tilePositionPrefab, cellWorldPosition, Quaternion.identity, transform);
                    text.text = "";
                    cellTexts.Add(tile.Coordinates.CubicCoordinates, text);
                }
            }
        }
    }

    public void SwitchDisplay() {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void RefreshCoordinates(HexCoordinatesType coordinates) {
        for (int i = 0; i < grid.tilesArray.GetLength(0); i++) {
            for (int j = 0; j < grid.tilesArray.GetLength(1); j++) {
                TileProperties tile = grid.tilesArray[i, j];
                TMP_Text text = cellTexts[tile.Coordinates.CubicCoordinates];

                if (coordinates == HexCoordinatesType.cubic) {
                    text.text = (tile.Coordinates.CubicCoordinates.x) + "," + (tile.Coordinates.CubicCoordinates.y);
                    text.text += "\n" + tile.Coordinates.CubicCoordinates.z;
                }
                else if (coordinates == HexCoordinatesType.axial) {
                    text.text = (tile.Coordinates.AxialCoordinates.x) + "," + (tile.Coordinates.AxialCoordinates.y);
                }
                else {
                    text.text = (tile.Coordinates.OffsetCoordinates.x) + "," + (tile.Coordinates.OffsetCoordinates.y);
                }
            }
        }
    }

    public void RefreshDistances(TileProperties origin) {
        for (int i = 0; i < grid.tilesArray.GetLength(0); i++) {
            for (int j = 0; j < grid.tilesArray.GetLength(1); j++) {
                TileProperties tile = grid.tilesArray[i, j];
                TMP_Text text = cellTexts[tile.Coordinates.CubicCoordinates];
                text.text = origin.Coordinates.Distance(tile.Coordinates).ToString();
            }
        }
    }
}
