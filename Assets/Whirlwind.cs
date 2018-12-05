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
            tile.whirlwind = null;
            EndTurn();
            RemoveFromTurnManager();
            Destroy(gameObject);
        }
        else {
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
