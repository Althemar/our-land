using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEntity : Entity
{
    private Movable movable;
    private float currentFood;

    private float satietyTreshold;
    private float starvationTreshold;

    [HideInInspector]
    public MovingEntitySO movingEntitySO;

    protected override void Start() {
        base.Start();
        movable = GetComponent<Movable>();
        movable.hexGrid = TurnManager.Instance.grid;
        movable.OnReachEndTile += EndMoving;

        movingEntitySO = entitySO as MovingEntitySO;
       
    }

    private void Update() {
        if (Time.frameCount == 1) {
            Initialize();
        }
    }



    public override void UpdateTurn() {
        if (currentFood > starvationTreshold) {
            IncreasePopulation();
            UpdateFoodTresholds();
        }
        else if (currentFood == 0) {
            population -= 1;
            UpdateFoodTresholds();
        }
        currentFood -= movingEntitySO.foodConsumption * population;
        TileProperties nearest = tile.NearestEntity(movingEntitySO.foods.ToArray());
        if (nearest) {
            Stack<TileProperties> path = AStarSearch.Path(tile, nearest);
            movable.MoveToward(path, movingEntitySO.movementPoints);
        }
        EndTurn();
    }

    public override void Initialize() {
        base.Initialize();

        tile.movingEntity = this;

        UpdateFoodTresholds();
        currentFood = satietyTreshold;
    }

    private void UpdateFoodTresholds() {
        satietyTreshold = movingEntitySO.satietyThreshold * population;
        starvationTreshold = movingEntitySO.starvationThreshold * population;
    }

    void EndMoving() {
        EndTurn();
    }
}
