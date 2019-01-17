using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : Updatable
{
    public EntitySO entitySO;

    [SerializeField]
    public int population;
    protected TileProperties tile;
    
    public int basePopulation; 
    public int reserve;

    private StateController stateController;
    private bool harvestedThisTurn = false;
    private int harvestedBonus = 0;

    public delegate void OnPopulationChangeDelegate();
    public static OnPopulationChangeDelegate OnPopulationChange;

    public ActivePopulationPoint populationPoint;
    public TileProperties Tile
    {
        get => tile;
    }

    public int HarvestedBonus {
        get => harvestedBonus;
    }

    void Awake() {
        stateController = GetComponent<StateController>();
        stateController.SetupAI(true);
    }

    protected virtual void Start() {

    }

    public override void AddToTurnManager() {
        TurnManager.Instance.AddToUpdate(entitySO, this);
    }

    public override void RemoveFromTurnManager() {
        TurnManager.Instance.RemoveFromUpdate(entitySO, this);
    }

    public abstract EntityType GetEntityType();


    public virtual void Initialize(int population = -1) {
        if (tile == null) {
            Vector3Int cellPosition = HexagonalGrid.Instance.Tilemap.WorldToCell(transform.position);
            transform.position = HexagonalGrid.Instance.Tilemap.GetCellCenterWorld(cellPosition);
            tile = HexagonalGrid.Instance.GetTile(new HexCoordinates(cellPosition.x, cellPosition.y));
        }
        AddToTurnManager();
        if (population == -1) {
            this.population = basePopulation;
        }
        else {
            this.population = population;
        }
        reserve = 0;
    }

    public override void UpdateTurn() {
        base.UpdateTurn();

        if (!harvestedThisTurn) {
            stateController.TurnUpdate();
            harvestedBonus = 0;
        }
        else {
            harvestedBonus++;
            if (harvestedBonus > 2)
                harvestedBonus = 2;
        }

        harvestedThisTurn = false;
    }
    
    public void Eaten(int damage) {
        population -= damage;
        if (population <= 0) {
            Kill();
        }
        OnPopulationChange();
    }
   
    public TileProperties GetFreeAdjacentTile(EntityType type) {
        TileProperties[] neighbors = tile.GetNeighbors();
        List<TileProperties> freeTiles = new List<TileProperties>();
        foreach (TileProperties neighbor in neighbors) {
            if (neighbor && entitySO.availableTiles.Contains(neighbor.Tile) && !neighbor.whirlwind && !neighbor.asLake &&
                    ((type == EntityType.Moving && neighbor.movingEntity == null && neighbor.currentMovable == null)
                 || (type == EntityType.Static && neighbor.staticEntity == null))) {
                freeTiles.Add(neighbor);
            }
        }
        if (freeTiles.Count > 0) {
            return freeTiles[Random.Range(0, freeTiles.Count)];
        }
        else {
            return null;
        }
    }

    public void IncreasePopulation() {
        population += entitySO.reproductionRate;
        if (population > entitySO.populationMax) {
            population = entitySO.populationMax;
            TryCreateAnotherEntity(GetEntityType());    
        }
        OnPopulationChange();
    }

    public void DecreasePopulation() {
        population -= entitySO.deathRate;
        if (population <= 0) {
            Kill();
        }
        OnPopulationChange();
    }

    public void Harvest() {
        DecreasePopulation();
        harvestedThisTurn = true;
    }

    public void TryCreateAnotherEntity(EntityType type) {
        if (population >= entitySO.populationMax) {
            TileProperties adjacent = GetFreeAdjacentTile(type);
            if (adjacent != null) {
                population = 2;
                Entity entity = Instantiate(gameObject, adjacent.transform.position, Quaternion.identity, transform.parent).GetComponent<Entity>();
                entity.tile = adjacent;

                if (WindManager.Instance.blockingEntities.Contains(entity.entitySO)){
                    bool computeWind = false;
                    if (adjacent.woOnTile.Count > 0) {
                        computeWind = true;
                    }
                    for (int i = 0; i < 6; i++) {
                        TileProperties neighbor = adjacent.GetNeighbor((HexDirection)i);
                        if (neighbor.woOnTile.Count > 0 && neighbor.previousTileInCorridor == ((HexDirection)i).Opposite()) {
                            computeWind = true;
                        }
                    }
                    if (computeWind) {
                        adjacent.Grid.humidity.Compute();
                    }

                }


                if (type == EntityType.Moving) {
                    adjacent.movingEntity = entity as MovingEntity;
                    adjacent.currentMovable = entity.GetComponent<Movable>();
                }
                else {
                    adjacent.staticEntity = entity as StaticEntity;
                    if (adjacent.wind) {
                        adjacent.wind.DestroyWind();
                    }
                }
                entity.Initialize(1);
            }
        }
    }

    public void Kill() {
        if (tile.movingEntity == this) {
            tile.movingEntity = null;
            tile.currentMovable = null;
        }
        else {
            tile.staticEntity = null;
        }

        if (WindManager.Instance.blockingEntities.Contains(entitySO)) {
            bool computeWind = false;
            for (int i = 0; i < 6; i++) {
                TileProperties neighbor = tile.GetNeighbor((HexDirection)i);
                if (neighbor.woOnTile.Count > 0 && neighbor.previousTileInCorridor == ((HexDirection)i).Opposite()) {
                    computeWind = true;
                }
            }
            if (computeWind) {
                tile.Grid.humidity.Compute();
            }

        }
        RemoveFromTurnManager();
        if (this != null) {
            Destroy(gameObject);
        }

    }
}
