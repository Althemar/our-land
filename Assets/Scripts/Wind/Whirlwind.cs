using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whirlwind : Updatable
{
    private TileProperties tile;

    public void InitializeWhirlwind(TileProperties tile) {
        this.tile = tile;
        AddToTurnManager();
    }

    public override void UpdateTurn() {
        bool destroy = true;
        int nbWinds = 0;
        for (int i = 0; i < 6; i++) {
            TileProperties neighbor = tile.GetNeighbor((HexDirection)i);

            if (neighbor.wind && neighbor.wind.direction.Opposite() == (HexDirection)i) {
                nbWinds++;
                if (nbWinds == 2) {
                    destroy = false;
                    break;
                }
            }
        }
        if (destroy) {
            EndTurn();
            RemoveFromTurnManager();
            WindManager.Instance.WhirldwindsPool.Push(this);
        }
        else {
            if (tile.staticEntity) {
                tile.staticEntity.Kill();
            }
            if (tile.movingEntity) {
                tile.movingEntity.Kill();
            }
            EndTurn();
        }
    }

    public override void AddToTurnManager() {
        TurnManager.Instance.AddToUpdate(this);
    }

    public override void RemoveFromTurnManager() {
        TurnManager.Instance.RemoveFromUpdate(this);
    }

}
