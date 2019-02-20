using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurviveWinter : Objective
{
    public override void StartObjective() {
    }


    public override bool Evaluate() {
        base.Evaluate();
        return false;
    }

    public override string GetProgressText() {
        return "Survive winter";
    }
}
