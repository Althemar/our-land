using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindOrigin : Updatable
{
    public HexDirection direction;
    public int size = 2;
    public int turnsBetweenSqualls = 3;

    private int turnCount;
    private TileProperties tile;

    private bool producingWind;
    private int currentSquallLength;

    private Wind lastProduced;

    private void Awake() {
        turnCount = turnsBetweenSqualls;
        AddToTurnManager();
    }

    private void Update() {
        if (GameManager.Instance.FrameCount == 0) {
            Vector3Int cellPosition = HexagonalGrid.Instance.Tilemap.WorldToCell(transform.position);
            tile = HexagonalGrid.Instance.GetTile(new HexCoordinates(cellPosition.x, cellPosition.y));
        }
    }

    public override void UpdateTurn() {
        base.UpdateTurn();
        if (turnCount == turnsBetweenSqualls && currentSquallLength < size) {
            Wind newWind = WindManager.Instance.WindsPool.Pop();
            newWind.InitializeChildWind(tile.GetNeighbor(direction), null, direction);
            if (currentSquallLength > 0) {
                for (int i = 0; i < lastProduced.next.Count; i++) {
                    lastProduced.next[i].previous = newWind;
                    newWind.next.Add(lastProduced.next[i]);
                }
                lastProduced.previous = newWind;
            }
            lastProduced = newWind;
            currentSquallLength++;

        }
        else if (turnCount == turnsBetweenSqualls) {
            currentSquallLength = 0;
            turnCount = 1;
        }
        else {
            turnCount++;
        }
        EndTurn();
    }

    public override void AddToTurnManager() {
        TurnManager.Instance.AddToUpdate(this);
    }

    public override void RemoveFromTurnManager() {
        TurnManager.Instance.RemoveFromUpdate(this);
    }
}
