using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveReachResources : Objective
{
    public ResourceType resource;
    public int goal;

    public override void StartObjective() {
    }

    public override bool Evaluate() {

        if (!completed && GameManager.Instance.motherShip.Inventory.GetResource(resource) >= goal) {
            completed = true;
        }
        base.Evaluate();

        return completed;
    }

    public override string GetProgressText() {
        return "Reach " + GameManager.Instance.motherShip.Inventory.GetResource(resource) + " / " + goal + " " + resource.name;
    }

    public override int Goal() {
        return goal;
    }

    public override int Progress() {
        return (int)GameManager.Instance.motherShip.Inventory.GetResource(resource);
    }

    public override Sprite IconCompletion() {
        return resource.icon;
    }
}
