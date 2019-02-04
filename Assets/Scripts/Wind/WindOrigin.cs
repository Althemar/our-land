using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindOrigin : Updatable
{
    public HexDirection direction;
    // public int size = 2;
    // public int turnsBetweenSqualls = 3;

    // private int turnCount;
    private TileProperties tile;

    private bool producingWind;
    //private int currentSquallLength;

    private Wind lastProduced;

    private List<TileProperties> corridor;
    
   

    private void Awake() {
        AddToTurnManager();
        corridor = new List<TileProperties>();        
    }

    private void Update() {
        if (GameManager.Instance.FrameCount == 0) {
            Vector3Int cellPosition = HexagonalGrid.Instance.Tilemap.WorldToCell(transform.position);
            tile = HexagonalGrid.Instance.GetTile(new HexCoordinates(cellPosition.x, cellPosition.y));
            tile.windOrigin = this;

            ComputeWindCorridor();
        }
    }

    public override void UpdateTurn() {
        base.UpdateTurn();

        // NEW WIND
        /*
            

        
        */
        EndTurn();
    }

    public override void AddToTurnManager() {
        TurnManager.Instance.AddToUpdate(this);
    }

    public override void RemoveFromTurnManager() {
        TurnManager.Instance.RemoveFromUpdate(this);
    }

    public void ComputeWindCorridor() {

        Stack<TileProperties> remainingTiles = new Stack<TileProperties>();

        if (!tile) {
            Vector3Int cellPosition = HexagonalGrid.Instance.Tilemap.WorldToCell(transform.position);
            tile = HexagonalGrid.Instance.GetTile(new HexCoordinates(cellPosition.x, cellPosition.y));
        }
        
        TryExpandCorridor(ref remainingTiles, tile, direction);

        while (remainingTiles.Count > 0) {
            TileProperties affectedTile = remainingTiles.Pop();
            if (affectedTile) {
                 TryExpandCorridor(ref remainingTiles, affectedTile, affectedTile.previousTileInCorridor);
            }
        }       
    }

    public void TryExpandCorridor(ref Stack<TileProperties> remainingTiles, TileProperties affectedTile, HexDirection previousDirection) {
        TileProperties nextTile = affectedTile.GetNeighbor(previousDirection);
        // Try to expend in the next direction, else, expend in previous and next direction
        if (affectedTile.Tile && nextTile.Tile && !ExpandCorridor(ref remainingTiles, affectedTile, previousDirection)) { 
            ExpandCorridor(ref remainingTiles, affectedTile, previousDirection.Previous());
            ExpandCorridor(ref remainingTiles, affectedTile, previousDirection.Next());
        }
    }

    public bool ExpandCorridor(ref Stack<TileProperties> remainingTiles, TileProperties affectedTile, HexDirection nextDirection) {
        TileProperties nextTile = affectedTile.GetNeighbor(nextDirection);

        if (CanCreateWindOnTile(nextTile)) {
            nextTile.nextTilesInCorridor.Add(nextDirection);
            nextTile.previousTileInCorridor = nextDirection;
            nextTile.Tilemap.SetColor(nextTile.Position, Color.red);
            remainingTiles.Push(nextTile);
            nextTile.woOnTile.Add(this);
            corridor.Add(nextTile);

            Wind newWind = WindManager.Instance.WindsPool.Pop();
            newWind.InitializeChildWind(nextTile, null, nextDirection);
            if (lastProduced) {
                for (int i = 0; i < lastProduced.next.Count; i++) {
                    lastProduced.next[i].previous = newWind;
                    newWind.next.Add(lastProduced.next[i]);
                }
                lastProduced.previous = newWind;
            }
            lastProduced = newWind;

            return true;
        }
        return false;
    }

    public void InitCorridor() {
        for (int i = 0; i < corridor.Count; i++) {
            corridor[i].Tilemap.SetColor(corridor[i].Position, Color.white);
        }
    }

    public bool CanCreateWindOnTile(TileProperties nextTile) {
        if (WindManager.Instance.blockingTiles.Contains(nextTile.Tile)) {
            return false;
        }
        if ((nextTile.staticEntity && WindManager.Instance.blockingEntities.Contains(nextTile.staticEntity.staticEntitySO))
            || (nextTile.movingEntity && WindManager.Instance.blockingEntities.Contains(nextTile.movingEntity.movingEntitySO))) {
            return false;
        }
        return true;
    }
}
