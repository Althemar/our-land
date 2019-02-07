using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindOrigin : Updatable
{
    public HexDirection direction;

    public bool activeAtBeginning;
    public int turnsActive;
    [ReorderableList]
    public List<WindOrigin> nextWindOrigin;

    private TileProperties tile;
    private bool producingWind;
    private Wind firstWind;

    private int remainingActiveTurns;
    
    private void Awake() {
        AddToTurnManager();
    }

    private void Update() {
        if (GameManager.Instance.FrameCount == 0) {
            Vector3Int cellPosition = HexagonalGrid.Instance.Tilemap.WorldToCell(transform.position);
            tile = HexagonalGrid.Instance.GetTile(new HexCoordinates(cellPosition.x, cellPosition.y));
            tile.windOrigin = this;

            if (activeAtBeginning) {
                On();
            }
        }
    }

    public override void UpdateTurn() {
        base.UpdateTurn();

        if (producingWind) {
            remainingActiveTurns--;
            if (remainingActiveTurns < 0) {
                Off();
            }
        }

        EndTurn();
    }

    public override void AddToTurnManager() {
        TurnManager.Instance.AddToUpdate(this);
    }

    public override void RemoveFromTurnManager() {
        TurnManager.Instance.RemoveFromUpdate(this);
    }

    public void ComputeWindCorridor(TileProperties beginTile = null, HexDirection beginDirection = HexDirection.E) {
        Stack<TileProperties> remainingTiles = new Stack<TileProperties>();
        if (!tile) {
            Vector3Int cellPosition = HexagonalGrid.Instance.Tilemap.WorldToCell(transform.position);
            tile = HexagonalGrid.Instance.GetTile(new HexCoordinates(cellPosition.x, cellPosition.y));
        }

        if (!beginTile) {
            TryExpandCorridor(ref remainingTiles, tile, direction, false);
            if (remainingTiles.Count > 0) {
                firstWind = remainingTiles.Peek().wind;
            }
        }
        else {
            bool tryNeighbors;
            if (beginTile == tile) {
                tryNeighbors = false;
            }
            else if (beginTile.wind.previous && beginTile.wind.direction == beginTile.wind.previous.direction) {
                if (beginDirection == beginTile.wind.direction) {
                    tryNeighbors = true;
                }
                else {
                    tryNeighbors = false;
                }
            }
            else if (beginTile.wind.previous) {
                tryNeighbors = true;
            }
            else {
                tryNeighbors = false;
            }

            TryExpandCorridor(ref remainingTiles, beginTile, beginDirection, tryNeighbors);   
        }


        while (remainingTiles.Count > 0) {
            TileProperties affectedTile = remainingTiles.Pop();
            if (affectedTile) {
                 TryExpandCorridor(ref remainingTiles, affectedTile, affectedTile.wind.direction);
            }
        }       
    }

    public void TryExpandCorridor(ref Stack<TileProperties> remainingTiles, TileProperties affectedTile, HexDirection previousDirection, bool tryNeighbors = true) {
        TileProperties nextTile = affectedTile.GetNeighbor(previousDirection);

        if (affectedTile.Tile && nextTile.Tile) {
            // Stop the wind if collide with another wind
            if (nextTile.wind) {
                if (nextTile.wind.windOrigin != affectedTile.wind.windOrigin) {
                    foreach (Wind wind in nextTile.wind.next) {
                        wind.DestroyWind(true);
                    }
                }
            }

            // Try to expend in the next direction, else, expend in previous and next direction
            else if (!ExpandCorridor(ref remainingTiles, affectedTile, previousDirection) && tryNeighbors) {
                ExpandCorridor(ref remainingTiles, affectedTile, previousDirection.Previous());
                ExpandCorridor(ref remainingTiles, affectedTile, previousDirection.Next());
            }
        }  
    }

    public bool ExpandCorridor(ref Stack<TileProperties> remainingTiles, TileProperties affectedTile, HexDirection nextDirection) {
        TileProperties nextTile = affectedTile.GetNeighbor(nextDirection);

        if (CanCreateWindOnTile(nextTile)) {
            //nextTile.Tilemap.SetColor(nextTile.Position, Color.red);
            remainingTiles.Push(nextTile);

            Wind newWind = WindManager.Instance.WindsPool.Pop();
            newWind.InitializeChildWind(nextTile, affectedTile.wind, nextDirection);

            return true;
        }
        return false;
    }


    public bool CanCreateWindOnTile(TileProperties nextTile) {
        if (WindManager.Instance.blockingTiles.Contains(nextTile.Tile) || nextTile.asMountain) {
            return false;
        }
        if ((nextTile.staticEntity && WindManager.Instance.blockingEntities.Contains(nextTile.staticEntity.staticEntitySO))
            || (nextTile.movingEntity && WindManager.Instance.blockingEntities.Contains(nextTile.movingEntity.movingEntitySO))) {
            return false;
        }
        return true;
    }

    public void Off() {
        if (producingWind) {
            firstWind.DestroyWind(true);
            producingWind = false;
            foreach (WindOrigin wo in nextWindOrigin) {
                wo.On();
            }
        }
    }

    public void On() {
        if (!producingWind) {
            ComputeWindCorridor();

            producingWind = true;
            remainingActiveTurns = turnsActive;
        }
    }
}

