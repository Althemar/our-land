using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movable))]
public class MovingEntity : Entity
{
    private Movable movable;
    [SerializeField]
    private float currentFood;

    private float satietyTreshold;
    private float starvationTreshold;

    [HideInInspector]
    public MovingEntitySO movingEntitySO;

    private EntityHungerState hunger;

    private Entity target;
    private bool stopBefore;

    // UGLY TO IMPROVE
    private Transform canvasWorldSpace;

    protected override void Start() {
        base.Start();
        movable = GetComponent<Movable>();
        movable.hexGrid = TurnManager.Instance.grid;
        movable.OnReachEndTile += EndMoving;
        if (tile) {
            movable.CurrentTile = tile;
            tile.currentMovable = movable;
        }
        

        movingEntitySO = entitySO as MovingEntitySO;
        hunger = EntityHungerState.Full;
        GetComponent<SpriteRenderer>().sortingOrder = 15;

        // UGLY TO IMPROVE
        canvasWorldSpace = GameObject.Find("Canvas World Space").transform;
    }

    private void Update() {
        if (GameManager.Instance.FrameCount == 0) {
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
            else if (currentFood >= starvationTreshold) {
                IncreasePopulation();
                TryCreateAnotherEntity(EntityType.Moving);
                UpdateFoodTresholds();

                TileProperties[] neighbors = tile.GetNeighbors();
                TileProperties next = neighbors[Random.Range(0, 6)];
                if (next && !next.currentMovable) {
                    Stack<TileProperties> path = AStarSearch.Path(tile, next, entitySO.availableTiles);
                    if (path != null) {
                        tile.currentMovable = null;
                        tile.movingEntity = null;
                        tile = movable.MoveToward(path, movingEntitySO.movementPoints, stopBefore);
                        tile.movingEntity = this;
                    }
                }
            }
        }
        
        

        bool waitForMove = false;
        if (hunger == EntityHungerState.Hungry) {
            if (currentFood == 0) {
                DecreasePopulation();
                UpdateFoodTresholds();
                if (population <= 0.2f) {
                    EndTurn();
                    Kill();
                }
            }
            TileProperties nearest = tile.NearestEntity(movingEntitySO.foods.ToArray(), 15);
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
                        tile.currentMovable = null;
                        tile.movingEntity = null;
                        tile = movable.MoveToward(path, movingEntitySO.movementPoints, stopBefore);        // Stop before tile if food is moving entity
                        tile.movingEntity = this;

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
        currentFood += target.entitySO.foodWhenHarvested * population; //j'ai rajouté * population
        target.Eaten(movingEntitySO.damageWhenEat * population); //j'ai rajouté * population

        // UGLY TO MOVE
        if (movingEntitySO.eatFeedback) {
            Vector3 position = (this.transform.position + target.transform.position) / 2f;
            KillFeedbackUI harvested = Instantiate(movingEntitySO.eatFeedback, position, Quaternion.identity, canvasWorldSpace).GetComponent<KillFeedbackUI>();
            harvested.Initialize();
        }

        if (currentFood > satietyTreshold) {
            //currentFood = satietyTreshold;
            hunger = EntityHungerState.Full;
        }
    }

    public override void Initialize(float population = -1) {
        base.Initialize(population);

        tile.movingEntity = this;

        UpdateFoodTresholds();
        currentFood = satietyTreshold;
    }

    private void UpdateFoodTresholds() {
        satietyTreshold = movingEntitySO.satietyThreshold * Mathf.Floor(population); //j'ai floor la pop
        starvationTreshold = movingEntitySO.starvationThreshold * Mathf.Floor(population); //j'ai floor la pop
    }

    void EndMoving() {
        tile.movingEntity = null;
        tile = movable.CurrentTile;
        tile.movingEntity = this;
        if (target) {
            int distance = target.Tile.Coordinates.Distance(tile.Coordinates);
            if ((stopBefore && distance == 1) || distance == 0) {
                Harvest();
            }
        }
       
        EndTurn();
    }
}
