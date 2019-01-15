using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToNearestTile : Action
{
    public List<CustomTile> tilesToReach;
    public bool stopOnTile;
    public int range;

    public override void Act(StateController controller) {
        
    }

    public override void OnEnterState(StateController controller) {
        throw new System.NotImplementedException();
    }

    public override void OnExitState(StateController controller) {
        throw new System.NotImplementedException();
    }
}
