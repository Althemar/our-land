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
                DisplayTileCoordinate(tile.Value, text);
                coordinatesTexts.Add(tile.Key, text);
            }
        }
    }

    public void SwitchDisplay() {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void Refresh() {
        foreach (KeyValuePair<Vector3Int, TileProperties> tile in hexagonalGrid.Tiles) {
            Vector3Int cellCoordinates = new Vector3Int(tile.Value.Position.x, tile.Value.Position.y, 0);
            TMP_Text text = coordinatesTexts[tile.Key];
            DisplayTileCoordinate(tile.Value, text);
        }
    }

    void DisplayTileCoordinate(TileProperties tileProperties, TMP_Text text) {
        text.text = (tileProperties.Coordinates.x) + "," + (tileProperties.Coordinates.y);
        if (tileProperties.Coordinates.coordinatesType == HexCoordinatesType.cubic) {
            text.text += "\n" + tileProperties.Coordinates.z;
        }
    }
}
