using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class HexagonalGridPositions : MonoBehaviour
{
    public Tilemap tilemap;
    public TMP_Text tilePositionPrefab;

    void Start()
    {
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

        for (int x = 0; x < bounds.size.x; x++) {
            for (int y = 0; y < bounds.size.y; y++) {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null) {
                    Vector3Int textPosition = new Vector3Int(x + bounds.position.x, y + bounds.position.y, 0);
                    TMP_Text text = Instantiate(tilePositionPrefab, tilemap.GetCellCenterWorld(textPosition), Quaternion.identity, transform);
                    text.text = textPosition.x + "," + textPosition.y;
                }
            }
        }
    }

    public void SwitchDisplay() {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
