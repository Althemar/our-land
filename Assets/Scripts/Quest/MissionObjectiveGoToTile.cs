using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionObjectiveGoToTile : MissionObjective
{
    private TileProperties tile;

    private SpriteRenderer spriteRenderer;

    void Update() {
        if (GameManager.Instance.FrameCount == 0) {
            Vector3Int cellPosition = HexagonalGrid.Instance.Tilemap.WorldToCell(transform.position);
            tile = HexagonalGrid.Instance.GetTile(cellPosition);
            transform.position = tile.transform.position;
        }
    }

    public override void StartObjective() {
        base.StartObjective();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = true;
    }

    public override bool Evaluate() {
        if (!completed && GameManager.Instance.motherShip.Movable.CurrentTile == tile) {
            spriteRenderer.enabled = false;
            completed = true;
        }
        return completed;
    }
}
