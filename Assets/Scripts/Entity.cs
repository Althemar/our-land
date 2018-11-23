using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : Updatable
{
    public EntitySO entitySO;

    private Movable movable;

    public void Start() {
        GetComponent<SpriteRenderer>().sprite = entitySO.sprite;
        AddToTurnManager();
        if (entitySO.canMove) {
            movable = gameObject.AddComponent<Movable>();
            movable.speed = 15;
            movable.hexGrid = TurnManager.Instance.grid;
            movable.OnReachEndTile += EndMoving;
        }
    }

    public override void UpdateTurn() {
        if (entitySO.canMove) {
            movable.MoveTo(TurnManager.Instance.grid.GetRandomTile());
        }
    }

    void EndMoving() {
        EndTurn();
    }

    public override void AddToTurnManager() {
        TurnManager.Instance.AddToUpdate(entitySO, this);
    }

    public override void RemoveFromTurnManager() {
        TurnManager.Instance.RemoveFromUpdate(entitySO, this);
    }
}
