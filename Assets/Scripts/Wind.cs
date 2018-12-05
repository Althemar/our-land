using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : Updatable
{
    public int baseSize;
    public HexDirection direction;

    private Wind previous;
    private List<Wind> next;

    private TileProperties tile;

    bool previousAlreadyUpdated;

    private void Update() {
        if (Time.frameCount == 1) {
            Vector3Int cellPosition = HexagonalGrid.Instance.Tilemap.WorldToCell(transform.position);
            TileProperties tile = HexagonalGrid.Instance.GetTile(new HexCoordinates(cellPosition.x, cellPosition.y));

            InitializeChildWind(tile, null, direction);

            Wind current = this;
            for (int i = 0; i < baseSize-1; i++) {
                Wind newWind = new GameObject().AddComponent<Wind>();
                current.next.Add(newWind);
                newWind.transform.parent = transform.parent;

                TileProperties nextTile = current.tile.GetNeighbor(direction);
                newWind.InitializeChildWind(nextTile, current, direction);
                current = newWind;
            }
        }
    }

    public void InitializeChildWind(TileProperties tile, Wind previous, HexDirection direction) {
        transform.position = tile.transform.position;
        this.previous = previous;
        this.tile = tile;
        this.direction = direction;
        next = new List<Wind>();
        tile.Tilemap.SetColor(tile.Position, Color.red);
        AddToTurnManager();
        tile.wind = this;
    }

    public override void UpdateTurn() {
        base.UpdateTurn();

        TileProperties nextTile = tile.GetNeighbor(direction);

        // Remove last wind
        bool destroy = false;
        if (!previous && !previousAlreadyUpdated) {
            tile.Tilemap.SetColor(tile.Position, Color.white);
            tile.wind = null;
            

            for (int i = 0; i < next.Count; i++) {
                next[i].previous = null;
                next[i].previousAlreadyUpdated = true;
            }
            RemoveFromTurnManager();
            EndTurn();
            destroy = true;
        }

        // Add wind at begin
        if (nextTile && !nextTile.wind && !nextTile.whirlwind && !TryCreateNewWind(direction)) {
            TryCreateNewWind(direction.Previous());
            TryCreateNewWind(direction.Next());
        }
        else if (nextTile && nextTile.wind && !next.Contains(nextTile.wind)) {
            nextTile.wind.DestroyWind();
            nextTile.whirlwind = Instantiate(WindManager.Instance.whirlwind, nextTile.transform.position, Quaternion.identity, transform.parent).GetComponent<Whirlwind>();
            nextTile.whirlwind.InitializeWhirlwind(nextTile);
        }
 
        if (destroy) {
            Destroy(gameObject);
        }

        previousAlreadyUpdated = false;

        EndTurn();
    }

    public void DestroyWind() {
        if (previous) {
            previous.next.Remove(this);
        }
        for (int i = 0; i < next.Count; i++) {
            next[i].previous = null;
        }
        tile.wind = null;
        tile.Tilemap.SetColor(tile.Position, Color.white);
        RemoveFromTurnManager();
        Destroy(gameObject);
    }



    private bool TryCreateNewWind(HexDirection nextDirection) {
        TileProperties nextTile = tile.GetNeighbor(nextDirection);
        if (CanCreateWindOnTile(nextTile)) {
            Wind newWind = new GameObject().AddComponent<Wind>();
            next.Add(newWind);
            newWind.transform.parent = transform.parent;
            newWind.InitializeChildWind(nextTile, this, nextDirection);
            return true;
        }
        return false;
    }

    public bool CanCreateWindOnTile(TileProperties nextTile) {
        if (!nextTile || nextTile.wind) {
            return false;
        }
        if (WindManager.Instance.blockingEntities.Contains(nextTile.staticEntity) || WindManager.Instance.blockingEntities.Contains(nextTile.movingEntity)) {
            return false;
        }
        if (WindManager.Instance.blockingTiles.Contains(nextTile.Tile)) {
            return false;
        }
        return true;
    }
   
    public override void AddToTurnManager() {
        TurnManager.Instance.AddToUpdate(this);
    }

    public override void RemoveFromTurnManager() {
        TurnManager.Instance.RemoveFromUpdate(this);
    }
}
