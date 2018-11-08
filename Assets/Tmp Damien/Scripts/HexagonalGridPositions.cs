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

    List<TMP_Text> coordinatesTexts;

    void Start()
    {
        
        
    }

    private void Update() {
        if (Time.frameCount == 3) {
            coordinatesTexts = new List<TMP_Text>();

            for (int i = 0; i < hexagonalGrid.Tiles.Count; i++) {
                TileDatas tileDatas = hexagonalGrid.Tiles[i];
                Vector3Int cellCoordinates = new Vector3Int(tileDatas.Position.x, tileDatas.Position.y, 0);
                TMP_Text text = Instantiate(tilePositionPrefab, tilemap.GetCellCenterWorld(cellCoordinates), Quaternion.identity, transform);
                text.text = (tileDatas.Coordinates.x) + "," + (tileDatas.Coordinates.y);
                coordinatesTexts.Add(text);
            }
        }
    }

    public void SwitchDisplay() {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void Refresh() {
        for (int i = 0; i < hexagonalGrid.Tiles.Count; i++) {
            TileDatas tileDatas = hexagonalGrid.Tiles[i];
            Vector3Int cellCoordinates = new Vector3Int(tileDatas.Position.x, tileDatas.Position.y, 0);
            TMP_Text text = coordinatesTexts[i];
            text.text = (tileDatas.Coordinates.x) + "," + (tileDatas.Coordinates.y);
            if (tileDatas.Coordinates.coordinatesType == HexCoordinates.HexCoordinatesType.cubic) {
                text.text += "\n" + tileDatas.Coordinates.z;
            }
        }
    }
}
