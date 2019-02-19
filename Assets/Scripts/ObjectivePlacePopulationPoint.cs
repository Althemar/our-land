using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectivePlacePopulationPoint : Objective
{
    public Bonus bonus;

    public override void StartObjective() {
    }

    public override bool Evaluate() {

        if (!completed && bonus.IsActive) {
            completed = true;
        }
        base.Evaluate();

        return completed;
    }
}
