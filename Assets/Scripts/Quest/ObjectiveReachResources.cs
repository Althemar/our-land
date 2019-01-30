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
        return completed;
    }
}
