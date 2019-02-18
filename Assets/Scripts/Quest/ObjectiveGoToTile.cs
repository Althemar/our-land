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

        if (woodLimit) {
            GameManager.Instance.motherShip.OnBeginMoving += ConsumeWood;
        }
    }

    public void ConsumeWood() {
        usedWood += (int)Mathf.Floor(GameManager.Instance.motherShip.targetTile.ActionPointCost);
        if (!completed && usedWood > maximumWood) {
            failed = true;
        }
        if (usedWood >= maximumWood) {
            usedWood = maximumWood;
        }
        OnUpdate?.Invoke();
    }

    public override bool Evaluate() {

        if (!completed && usedWood > maximumWood) {
            failed = true;
        }

        if (!completed && GameManager.Instance.motherShip.Movable.CurrentTile == tile) {
            spriteRenderer.enabled = false;
            completed = true;
        }
        base.Evaluate();

        return completed;
    }

    public override string GetProgressText() {
        if (!woodLimit) {
            return "Reach the tile";
        }
        else {
            return "Reach the tile using less than " + (maximumWood - usedWood) + " wood";
        }
    }
}
