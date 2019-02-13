using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using static Spine.AnimationState;
using Spine;

[RequireComponent(typeof(StateController))]
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

    [HideInInspector]
    public List<SkeletonAnimation> activatedSkeletons;

    public int remainingTurnsBeforReproduction = -1;

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

    protected virtual void Awake() {
        stateController = GetComponent<StateController>();
        stateController.SetupAI(true);
    }

    protected virtual void Start() {
        remainingTurnsBeforReproduction = entitySO.nbTurnsBeforeReproduction;
    }

    public override void AddToTurnManager() {
        TurnManager.Instance.AddToUpdate(entitySO, this);
    }

    public override void RemoveFromTurnManager() {
        TurnManager.Instance.RemoveFromUpdate(entitySO, this);
    }

    public abstract EntityType GetEntityType();

    protected bool isInit = false;
    public virtual void Initialize(int population = -1) {
        if (isInit)
            return;

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

        isInit = true;
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

    public override void LateUpdateTurn() {
        base.LateUpdateTurn();
        stateController.LateTurnUpdate();
        EndTurn();
    }


    public void Eaten(int damage) {
        population -= damage;
        if (population <= 0) {
            Kill();
        } else
            OnPopulationChange?.Invoke();
    }
   
    public TileProperties GetFreeAdjacentTile(EntityType type) {
        TileProperties[] neighbors = tile.GetNeighbors();
        List<TileProperties> freeTiles = new List<TileProperties>();
        foreach (TileProperties neighbor in neighbors) {
            if (neighbor && entitySO.availableTiles.Contains(neighbor.Tile) && !neighbor.asLake && !neighbor.asMountain &&
                    ((type == EntityType.Moving && neighbor.movingEntity == null && neighbor.movable == null)
                 || (type == EntityType.Static && neighbor.staticEntity == null && neighbor.movable != GameManager.Instance.motherShip.Movable))) {
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
        OnPopulationChange?.Invoke();
    }

    

    public void DecreasePopulation() {
        population -= entitySO.deathRate;
        if (population <= 0 && !entitySO.renewWhenZero) {
            Kill();
        }
        if (population <= 0 && populationPoint) {
            //populationPoint.RemovePopulationPoint();
        }
        if (population < 0) {
            population = 0;
        }
        OnPopulationChange?.Invoke();
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

                if (type == EntityType.Moving) {
                    MovingEntity mv = entity as MovingEntity;
                    adjacent.movingEntity = mv;
                    adjacent.movable = mv.GetComponent<Movable>();
                    adjacent.movable.CurrentTile = adjacent;
                    if (mv.isHungry) {
                        mv.ChangeAnimation("Hungry", true);
                    }
                    else {
                        mv.ChangeAnimation("Idle", true);
                    }
                }
                else {
                    adjacent.staticEntity = entity as StaticEntity;
                }

                if (adjacent.wind && WindManager.Instance.blockingEntities.Contains(entity.entitySO)) {
                    WindOrigin wo = adjacent.wind.windOrigin;
                    if (adjacent.wind.previous) {
                        TileProperties previous = adjacent.wind.previous.tile;
                        HexDirection dir = adjacent.wind.direction;
                        adjacent.wind.DestroyWind(true);
                        wo.ComputeWindCorridor(previous, dir);
                    }
                    else {
                        adjacent.wind.DestroyWind(true);
                    }
                }

                entity.Initialize(1);
            }
        }
    }

    public void Kill() {
        if (tile.movingEntity == this) {
            tile.movingEntity = null;
            tile.movable = null;
        }
        else {
            tile.staticEntity = null;
        }

        if (WindManager.Instance.blockingEntities.Contains(entitySO)) {
            for (int i = 0; i < 6; i++) {
                TileProperties neighbor = tile.GetNeighbor((HexDirection)i);
                if ((neighbor.wind && neighbor.wind.direction == ((HexDirection)i).Opposite()) 
                    || (neighbor.windOrigin && neighbor.windOrigin.direction == ((HexDirection)i).Opposite())) {
                    if (neighbor.wind) {
                        WindOrigin wo = neighbor.wind.windOrigin;
                        while (neighbor.wind.next.Count > 0) {
                            neighbor.wind.next[0].DestroyWind(true);
                        }
                        wo.ComputeWindCorridor(neighbor.wind.tile, neighbor.wind.direction);
                    }
                    else if (neighbor.windOrigin){
                        WindOrigin wo = neighbor.windOrigin;
                        wo.ComputeWindCorridor();
                    }
                    break;
                }
            }
        }
        if (updating) {
            EndTurn();
        }
        RemoveFromTurnManager();
        if (this != null) {
            Destroy(gameObject);
            ChangeAnimation("Death", false);
        }
    }

    public void ChangeAnimation(string animationName, bool loop = false, TrackEntryDelegate entry = null) {
        foreach (SkeletonAnimation skeletonAnimation in activatedSkeletons) {
            if (animationName == "Death") {
                skeletonAnimation.state.Complete += EndDeath;
            }
            if (animationName == "Eating") {
                skeletonAnimation.state.Complete += entry;
            }
            skeletonAnimation.state.ClearTrack(0);
            skeletonAnimation.state.SetAnimation(0, animationName, loop);
        }
    }

    private void EndDeath(TrackEntry trackEntry) {
        Destroy(gameObject);
    }

}
