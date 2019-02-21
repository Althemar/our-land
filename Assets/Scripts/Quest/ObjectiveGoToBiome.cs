using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveGoToBiome : Objective
{
    public CustomTile.TerrainType terrain;

    public override void StartObjective() {
    }

    public override bool Evaluate() {
        if (!completed && GameManager.Instance.motherShip.Movable.CurrentTile.Tile.terrainType == terrain) {
            completed = true;
        }
        base.Evaluate();

        return completed;
    }
    
    public override int Goal() {
        return 1;
    }

    public override int Progress() {
        return completed ? 1 : 0;
    }
}
