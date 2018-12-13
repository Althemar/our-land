using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindOrigin : Updatable
{
    public HexDirection direction;
    public int size = 2;
    public int turnsBetweenSqualls = 3;

    public int radius;
    public int baseDryness;

    private int turnCount;
    private TileProperties tile;

    private bool producingWind;
    private int currentSquallLength;

    private Wind lastProduced;

    private List<TileProperties> corridor;
    private List<TileProperties> tilesAffected;

   

    private void Awake() {
        turnCount = turnsBetweenSqualls;
        AddToTurnManager();
    }

    private void Update() {
        if (GameManager.Instance.FrameCount == 0) {
            Vector3Int cellPosition = HexagonalGrid.Instance.Tilemap.WorldToCell(transform.position);
            tile = HexagonalGrid.Instance.GetTile(new HexCoordinates(cellPosition.x, cellPosition.y));
            //ComputeWindCorridor();
        }
    }

    public override void UpdateTurn() {
        base.UpdateTurn();
        if (turnCount == turnsBetweenSqualls && currentSquallLength < size) {
            Wind newWind = WindManager.Instance.WindsPool.Pop();
            newWind.InitializeChildWind(tile.GetNeighbor(direction), null, direction);
            if (currentSquallLength > 0) {
                for (int i = 0; i < lastProduced.next.Count; i++) {
                    lastProduced.next[i].previous = newWind;
                    newWind.next.Add(lastProduced.next[i]);
                }
                lastProduced.previous = newWind;
            }
            lastProduced = newWind;
            currentSquallLength++;

        }
        else if (turnCount == turnsBetweenSqualls) {
            currentSquallLength = 0;
            turnCount = 1;
        }
        else {
            turnCount++;
        }
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
        if (affectedTile.Tile && nextTile.Tile && !ExpandCorridor(ref remainingTiles, affectedTile, previousDirection)) { 
            ExpandCorridor(ref remainingTiles, affectedTile, previousDirection.Previous());
            ExpandCorridor(ref remainingTiles, affectedTile, previousDirection.Next());
        }
    }

    public bool ExpandCorridor(ref Stack<TileProperties> remainingTiles, TileProperties affectedTile, HexDirection nextDirection) {
        TileProperties nextTile = affectedTile.GetNeighbor(nextDirection);
        if (!nextTile.Tile) {
            return false;
        }
        else if (CanCreateWindOnTile(nextTile)) {
            nextTile.nextTilesInCorridor.Add(nextDirection);
            nextTile.previousTileInCorridor = nextDirection;
           // nextTile.Tilemap.SetColor(nextTile.Position, Color.red);
            remainingTiles.Push(nextTile);
            
            for (int x = -radius; x <= radius; x++) {
                for (int y = -radius; y <= radius; y++) {
                    int z = -y - x;
                    HexCoordinates coordinatesInRange = new HexCoordinates(x, y, z);

                    HexCoordinates other = nextTile.Coordinates + coordinatesInRange;
                    int distance = other.Distance(nextTile.Coordinates);
                    if (distance <= radius) {
                        TileProperties neighbor = nextTile.Grid.GetTile(other);
                        if (!neighbor) {
                            continue;
                        }
                       // neighbor.Tilemap.SetColor(neighbor.Position, Color.red);
                        float newWindDryness = -(baseDryness - distance);
                        if (newWindDryness < neighbor.windDryness) {
                            neighbor.humidity -= neighbor.windDryness;
                            neighbor.windDryness = newWindDryness;
                            neighbor.humidity += neighbor.windDryness;
                        }
                        neighbor.woAffectingTiles.Add(this);
                    }
                }
            }
            return true;
        }
        return false;
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
