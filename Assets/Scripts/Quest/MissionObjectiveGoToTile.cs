using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionObjectiveGoToTile : MissionObjective
{
    private TileProperties tile;



    void Update() {
        if (GameManager.Instance.FrameCount == 0) {
            Vector3Int cellPosition = HexagonalGrid.Instance.Tilemap.WorldToCell(transform.position);
            tile = HexagonalGrid.Instance.GetTile(cellPosition);
            transform.position = tile.transform.position;

        }
    }

    public override void StartObjective() {
        GetComponent<SpriteRenderer>().enabled = true;
    }

    public override bool Evaluate() {
        return (GameManager.Instance.motherShip.Movable.CurrentTile == tile);
    }
}
