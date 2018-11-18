using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*
 * HexGridLabels
 * UI for cell labels. Used for displaying coordinates or distance on cells
 */

public class HexGridLabels : MonoBehaviour
{
    public HexagonalGrid grid;
    public TMP_Text tilePositionPrefab;

    private Dictionary<Vector3Int, TMP_Text> cellTexts;

    private void Update() {
        if (Time.frameCount == 1) {
            cellTexts = new Dictionary<Vector3Int, TMP_Text>();
            foreach (KeyValuePair<Vector3Int, TileProperties> tile in grid.Tiles) {
                Vector3 cellWorldPosition = grid.Tilemap.GetCellCenterWorld(tile.Value.Position);
                TMP_Text text = Instantiate(tilePositionPrefab, cellWorldPosition, Quaternion.identity, transform);
                text.text = "";
                cellTexts.Add(tile.Key, text);
            }
        }
    }

    public void SwitchDisplay() {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void RefreshCoordinates() {
        foreach (KeyValuePair<Vector3Int, TileProperties> tile in grid.Tiles) {
            TMP_Text text = cellTexts[tile.Key];
            text.text = (tile.Value.Coordinates.X) + "," + (tile.Value.Coordinates.Y);
            if (tile.Value.Coordinates.coordinatesType == HexCoordinatesType.cubic) {
                text.text += "\n" + tile.Value.Coordinates.Z;
            }
        }
    }

    public void RefreshDistances(TileProperties origin) {
        foreach (KeyValuePair<Vector3Int, TileProperties> tile in grid.Tiles) {
            TMP_Text text = cellTexts[tile.Key];
            text.text = origin.Coordinates.Distance(tile.Value.Coordinates).ToString();
        }
    }
}
