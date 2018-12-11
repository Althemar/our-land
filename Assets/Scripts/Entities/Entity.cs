using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : Updatable
{
    public EntitySO entitySO;

    protected float population;
    protected TileProperties tile;
    

    public TileProperties Tile
    {
        get => tile;
    }

    protected virtual void Start() {
        GetComponent<SpriteRenderer>().sprite = entitySO.sprite;
    }

    public override void AddToTurnManager() {
        TurnManager.Instance.AddToUpdate(entitySO, this);
    }

    public override void RemoveFromTurnManager() {
        TurnManager.Instance.RemoveFromUpdate(entitySO, this);
    }


    public virtual void Initialize(float population = -1) {
        if (tile == null) {
            Vector3Int cellPosition = HexagonalGrid.Instance.Tilemap.WorldToCell(transform.position);
            tile = HexagonalGrid.Instance.GetTile(new HexCoordinates(cellPosition.x, cellPosition.y));
        }
        AddToTurnManager();
        if (population == -1) {
            this.population = entitySO.basePopulation;
        }
        else {
            this.population = population;
        }
    }

    public override void UpdateTurn() {
        base.UpdateTurn();
    }
    
    public void Eaten(float damage) {
        population -= damage;
        if (population <= 0) {
            Kill();
        }
    }
   
    public TileProperties GetFreeAdjacentTile(EntityType type) {
        TileProperties[] neighbors = tile.GetNeighbors();
        List<TileProperties> freeTiles = new List<TileProperties>();
        foreach (TileProperties neighbor in neighbors) {
            if (neighbor && entitySO.availableTiles.Contains(neighbor.Tile) && !neighbor.whirlwind &&
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
        if (population < entitySO.populationMax) {
            population += population * entitySO.reproductionRate;
            if (population > entitySO.populationMax) {
                population = entitySO.populationMax;
            }
        }
    }

    public void DecreasePopulation() {
        population -= population * entitySO.deathRate;
    }

    public void TryCreateAnotherEntity(EntityType type) {
        if (population >= entitySO.populationMax) {
            TileProperties adjacent = GetFreeAdjacentTile(type);
            if (adjacent != null) {
                

                population /= 2;
                Entity entity = Instantiate(gameObject, adjacent.transform.position, Quaternion.identity, transform.parent).GetComponent<Entity>();
                entity.tile = adjacent;
                
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
                entity.Initialize(population);
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
        RemoveFromTurnManager();
        if (this != null) {
            Destroy(gameObject);
        }
    }
}
