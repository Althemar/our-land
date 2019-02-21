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

    public override int Goal() {
        return 1;
    }

    public override int Progress() {
        return completed ? 1 : 0;
    }
}
