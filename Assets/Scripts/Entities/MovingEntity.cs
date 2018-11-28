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

    private EntityHungerState hunger;

    protected override void Start() {
        base.Start();
        movable = GetComponent<Movable>();
        movable.hexGrid = TurnManager.Instance.grid;
        movable.OnReachEndTile += EndMoving;

        movingEntitySO = entitySO as MovingEntitySO;
        hunger = EntityHungerState.Full;
        GetComponent<SpriteRenderer>().sortingOrder = 3;

    }

    private void Update() {
        if (Time.frameCount == 1) {
            Initialize();
        }
    }



    public override void UpdateTurn() {
        DecreaseFood();
        if (hunger == EntityHungerState.Full) {
            if (currentFood < starvationTreshold) { 
                hunger = EntityHungerState.Hungry;
            }
            else {      
                IncreasePopulation();
                UpdateFoodTresholds();
            }
        }

        bool waitForMove = false;
        if (hunger == EntityHungerState.Hungry) {
            if (currentFood == 0) {
                DecreasePopulation();
                UpdateFoodTresholds();
                if (population <= 0.2f) {
                    RemoveFromTurnManager();
                    Destroy(gameObject);
                }
            }
            TileProperties nearest = tile.NearestEntity(movingEntitySO.foods.ToArray());
            if (nearest) {
                bool move = true;
                bool stopBefore = false;
                Entity food;
                if (!nearest.movingEntity || nearest.movingEntity == this) {
                    food = nearest.staticEntity;
                    if (nearest == tile) {
                        move = false;
                    }
                }
                else {
                    food = nearest.movingEntity;
                    if (nearest.Coordinates.Distance(tile.Coordinates) == 1) {
                        move = false;
                    }
                    else {
                        stopBefore = true;
                    }
                }
                if (move) {
                    
                    Stack<TileProperties> path = AStarSearch.Path(tile, nearest);
                    movable.MoveToward(path, movingEntitySO.movementPoints, stopBefore);        // Stop before tile if food is moving entity
                    waitForMove = true;
                }
                else {
                    // Harvest
                    currentFood += food.entitySO.foodWhenHarvested;
                    food.DecreasePopulation();
                    if (currentFood > satietyTreshold) {
                        currentFood = satietyTreshold;
                        hunger = EntityHungerState.Full;
                    }
                }
            }
        }
        if (!waitForMove) {
            EndTurn();
        }
    }

    private void DecreaseFood() {
        currentFood -= movingEntitySO.foodConsumption * population;
        if (currentFood < 0) {
            currentFood = 0;
        }
    }

    public void Harvested() {

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
        tile = movable.CurrentTile;
        EndTurn();
    }
}
