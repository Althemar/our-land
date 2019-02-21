using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinterIsComing : Objective
{
    public int remainingTurns;
    int turnBegin;

    public override void StartObjective() {
        turnBegin = remainingTurns;
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

    public override int Goal() {
        return turnBegin;
    }

    public override int Progress() {
        return turnBegin - remainingTurns;
    }
}
