using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Actions/TreeIdle")]
public class TreeIdleAction : Action 
{
    public override void Act (StateController controller)
    {
        StaticEntity entity = controller.entity as StaticEntity;
        if (entity.remainingTurnsBeforReproduction == 0) {
            entity.IncreasePopulation();
            entity.remainingTurnsBeforReproduction = entity.staticEntitySO.nbTurnsBeforeReproduction;
        }
        else
            entity.remainingTurnsBeforReproduction--;
        //entity.UpdateSprite();
        
    }

    public override void OnExitState(StateController controller) {  
        
    }

    public override void OnEnterState(StateController controller) {

    }
}