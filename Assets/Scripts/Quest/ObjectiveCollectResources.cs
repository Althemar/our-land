using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveCollectResources : Objective
{
    public ResourceType resource;
    public int goal;
    public bool onSameSpot;

    private int count = 0;
    private TileProperties currentTile = null;

    public override void StartObjective() {
        GameManager.Instance.motherShip.OnResourceGained += CollectResource;
        if (onSameSpot) {
            GameManager.Instance.motherShip.OnBeginMoving += ResetCount;
        }
    }

    public void ResetCount()
    {
        count = 0;
    }

    private void CollectResource(ResourceType resource, int count) {
        if (this.resource == resource && this.count < goal) {
            this.count += count;
            if (this.count > goal) {
                this.count = goal;
            }
        }
    }

    public override bool Evaluate() {

        if (!completed && count >= goal) {
            completed = true;
        }
        base.Evaluate();

        return completed;
    }

    public override string GetProgressText() {
        return "Collect " + count + " / " + goal + " " + resource.name;
    }
}
