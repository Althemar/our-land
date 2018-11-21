using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public EntitySO entitySO;

    private Movable movable;

    public void Start() {
        GetComponent<SpriteRenderer>().sprite = entitySO.sprite;
        TurnManager.Instance.AddToUpdate(this);
        if (entitySO.canMove) {
            movable = gameObject.AddComponent<Movable>();
            movable.speed = 6;
            movable.hexGrid = TurnManager.Instance.grid;
            movable.OnReachEndTile += EndMoving;
        }
    }

    public void UpdateEntity() {
        if (entitySO.canMove) {
            movable.MoveTo(TurnManager.Instance.grid.GetRandomTile());
        }
    }

    void EndMoving() {
        TurnManager.Instance.EntityUpdated();
    }
}
