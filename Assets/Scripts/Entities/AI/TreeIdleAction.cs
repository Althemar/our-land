using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Actions/TreeIdle")]
public class TreeIdleAction : Action 
{
    public override void Act (StateController controller)
    {
        StaticEntity entity = controller.entity as StaticEntity;
        if (entity.remainingTurnsBeforReproduction == 0)
            entity.IncreasePopulation();
        else
            entity.remainingTurnsBeforReproduction--;
        
    }

    public override void OnExitState(StateController controller) {  
        
    }

    public override void OnEnterState(StateController controller) {

    }
}