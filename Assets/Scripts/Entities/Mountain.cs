using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mountain : MonoBehaviour {

    void Awake() {
        Vector3Int cellPosition = HexagonalGrid.Instance.Tilemap.WorldToCell(transform.position);
        transform.position = HexagonalGrid.Instance.Tilemap.GetCellCenterWorld(cellPosition) + new Vector3(0, 0, 1);

        TileProperties tile = HexagonalGrid.Instance.GetTile(new HexCoordinates(cellPosition.x, cellPosition.y));
        tile.asMountain = true;
        GetComponent<SpriteRenderer>().sortingOrder = -tile.Position.y;
    }

}
