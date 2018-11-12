using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class HexagonalGridPositions : MonoBehaviour
{
    public Tilemap tilemap;
    public HexagonalGrid hexagonalGrid;
    public TMP_Text tilePositionPrefab;

    Dictionary<Vector3Int, TMP_Text> coordinatesTexts;

    private void Update() {
        if (Time.frameCount == 1) {
            coordinatesTexts = new Dictionary<Vector3Int, TMP_Text>();
            foreach (KeyValuePair<Vector3Int, TileProperties> tile in hexagonalGrid.Tiles) {
                Vector3Int cellCoordinates = new Vector3Int(tile.Value.Position.x, tile.Value.Position.y, 0);
                TMP_Text text = Instantiate(tilePositionPrefab, tilemap.GetCellCenterWorld(cellCoordinates), Quaternion.identity, transform);
                text.text = "";
                coordinatesTexts.Add(tile.Key, text);
            }
        }
    }

    public void SwitchDisplay() {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void RefreshCoordinates() {
        foreach (KeyValuePair<Vector3Int, TileProperties> tile in hexagonalGrid.Tiles) {
            TMP_Text text = coordinatesTexts[tile.Key];
            text.text = (tile.Value.Coordinates.x) + "," + (tile.Value.Coordinates.y);
            if (tile.Value.Coordinates.coordinatesType == HexCoordinatesType.cubic) {
                text.text += "\n" + tile.Value.Coordinates.z;
            }
        }
    }

    public void RefreshDistances(TileProperties origin) {
        foreach (KeyValuePair<Vector3Int, TileProperties> tile in hexagonalGrid.Tiles) {
            TMP_Text text = coordinatesTexts[tile.Key];
            text.text = origin.Coordinates.Distance(tile.Value.Coordinates).ToString();
        }
    }

    
}
