using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveGoToTile : Objective {
    private TileProperties tile;
    private SpriteRenderer spriteRenderer;


    public bool woodLimit;
    public int maximumWood;

    private int usedWood = 0;

    void Start() {
        Vector3Int cellPosition = HexagonalGrid.Instance.Tilemap.WorldToCell(transform.position);
        tile = HexagonalGrid.Instance.GetTile(cellPosition);
        transform.position = tile.transform.position;
    }

    public override void StartObjective() {

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer) {
            spriteRenderer.enabled = true;
        }
    }
    

    public override bool Evaluate() {

        if (!completed && GameManager.Instance.motherShip.Movable.CurrentTile == tile) {
            spriteRenderer.enabled = false;
            completed = true;
        }
        base.Evaluate();

        return completed;
    }

    public override string GetProgressText() {
         return "Reach the tile";
    }
}
