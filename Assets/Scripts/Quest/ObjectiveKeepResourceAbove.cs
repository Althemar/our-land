using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveKeepResourceAbove : Objective
{
    public ResourceType resource;
    public int amount;
    public int turns;

    private int remainingTurns;

    public override void StartObjective() {
        remainingTurns = turns;
    }

    public override bool Evaluate() {

        if (!completed) {
            if (GameManager.Instance.motherShip.Inventory.GetResource(resource) >= amount && remainingTurns > 0) {
                remainingTurns--;
                if (remainingTurns == 0) {
                    completed = true;
                }
            }
            else {
                remainingTurns++;
            }
        }
        base.Evaluate();

        return completed;
    }

    public override string GetProgressText() {
        return "Keep " + resource.name + " above " + amount + " during " + remainingTurns + " turns";
    }
}

