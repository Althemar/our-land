using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEntity : Entity
{
    private Movable movable;
    private float food;

    private MovingEntitySO movingEntitySO;

    protected override void Start() {
        base.Start();
        movable = GetComponent<Movable>();
        movable.hexGrid = TurnManager.Instance.grid;
        movable.OnReachEndTile += EndMoving;

        movingEntitySO = entitySO as MovingEntitySO;

        Initialize();
    }

    private void Update() {
        if (Time.frameCount == 1) {
            Initialize();
        }
    }



    public override void UpdateTurn() {
        food -= movingEntitySO.foodConsumption * population;
    }

    public override void Initialize() {
        base.Initialize();

        tile.movingEntity = this;
        food = 0;
    }

    void EndMoving() {
        EndTurn();
    }
}
