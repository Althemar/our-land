using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : Updatable
{
    public EntitySO entitySO;

    protected float population;
    protected TileProperties tile;

    protected virtual void Start() {
        GetComponent<SpriteRenderer>().sprite = entitySO.sprite;
        
    }

    public override void AddToTurnManager() {
        TurnManager.Instance.AddToUpdate(entitySO, this);
    }

    public override void RemoveFromTurnManager() {
        TurnManager.Instance.RemoveFromUpdate(entitySO, this);
    }


    public virtual void Initialize() {
        if (tile == null) {
            Vector3Int cellPosition = HexagonalGrid.Instance.Tilemap.WorldToCell(transform.position);
            tile = HexagonalGrid.Instance.GetTile(new HexCoordinates(cellPosition.x, cellPosition.y));
        }
        AddToTurnManager();
        population = entitySO.basePopulation;
        GetComponent<SpriteRenderer>().sortingOrder = 3;
    }

    public override void UpdateTurn() {
    }
    
   

    public TileProperties GetFreeAdjacentTile(EntityType type) {
        TileProperties[] neighbors = tile.GetNeighbors();
        List<TileProperties> freeTiles = new List<TileProperties>();
        foreach (TileProperties neighbor in neighbors) {

            if (neighbor && 
                    ((type == EntityType.Moving && neighbor.movingEntity == null)
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
                Entity entity = Instantiate(gameObject, adjacent.transform.position, Quaternion.identity).GetComponent<Entity>();
                entity.tile = adjacent;
                if (type == EntityType.Moving) {
                    adjacent.movingEntity = entity as MovingEntity;
                }
                else {
                    adjacent.staticEntity = entity as StaticEntity;
                }
                entity.Initialize();
            }
        }
    }

}
