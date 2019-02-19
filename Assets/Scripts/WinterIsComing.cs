using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinterIsComing : Objective
{
    public int remainingTurns;

    public override void StartObjective() {

    }

    public override bool Evaluate() {
        remainingTurns--;
        if (remainingTurns == 0) {
            completed = true;
        }

        base.Evaluate();
        return completed;
    }

    public override string GetProgressText() {
        return "Prepare for winter ! ( in " + remainingTurns + " turns)";
    }

}
