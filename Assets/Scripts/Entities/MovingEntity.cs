using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movable))]
public class MovingEntity : Entity
{
    private Movable movable;
    private float currentFood;

    private float satietyTreshold;
    private float starvationTreshold;

    [HideInInspector]
    public MovingEntitySO movingEntitySO;

    private EntityHungerState hunger;

    private Entity target;
    private bool stopBefore;

    protected override void Start() {
        base.Start();
        movable = GetComponent<Movable>();
        movable.hexGrid = TurnManager.Instance.grid;
        movable.OnReachEndTile += EndMoving;
        if (tile) {
            movable.CurrentTile = tile;
        }
        

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
        base.UpdateTurn();
        DecreaseFood();
        if (hunger == EntityHungerState.Full) {
            if (currentFood < starvationTreshold) { 
                hunger = EntityHungerState.Hungry;
            }
        }
        
        if (currentFood > starvationTreshold) {
            IncreasePopulation();
            TryCreateAnotherEntity(EntityType.Moving);
            UpdateFoodTresholds();
        }

        bool waitForMove = false;
        if (hunger == EntityHungerState.Hungry) {
            if (currentFood == 0) {
                DecreasePopulation();
                UpdateFoodTresholds();
                if (population <= 0.2f) {
                    Kill();
                }
            }
            TileProperties nearest = tile.NearestEntity(movingEntitySO.foods.ToArray());
            if (nearest) {
                bool move = true;
                stopBefore = false;
                if (!nearest.movingEntity || nearest.movingEntity == this) {
                    target = nearest.staticEntity;
                    if (nearest == tile) {
                        move = false;
                    }
                }
                else {
                    target = nearest.movingEntity;
                    if (nearest.Coordinates.Distance(tile.Coordinates) == 1) {
                        move = false;
                    }
                    else {
                        stopBefore = true;
                    }
                }
                if (move) {
                    Stack<TileProperties> path = AStarSearch.Path(tile, nearest, entitySO.availableTiles);
                    if (path == null) {
                        EndTurn();
                    }
                    else {
                        movable.MoveToward(path, movingEntitySO.movementPoints, stopBefore);        // Stop before tile if food is moving entity
                        waitForMove = true;
                    }
                    
                }
                else {
                    Harvest();
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

    private void Harvest() {
        currentFood += target.entitySO.foodWhenHarvested;
        target.Eaten(movingEntitySO.damageWhenEat);
        if (currentFood > satietyTreshold) {
            currentFood = satietyTreshold;
            hunger = EntityHungerState.Full;
        }
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
        tile.movingEntity = null;
        tile = movable.CurrentTile;
        tile.movingEntity = this;
        int distance = target.Tile.Coordinates.Distance(tile.Coordinates);
        if ((stopBefore && distance == 1) || distance == 0) {
            Harvest();
        }
        EndTurn();
    }
}
