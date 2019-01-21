using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Actions/Reproduce")]
public class ReproduceAction : Action 
{
    public override void Act (StateController controller)
    {
         Entity entity = controller.entity;
        if (entity.remainingTurnsBeforReproduction == 0) {
            entity.IncreasePopulation();
            entity.remainingTurnsBeforReproduction = entity.entitySO.nbTurnsBeforeReproduction;
        }
        else
            entity.remainingTurnsBeforReproduction--;
        
    }

    public override void OnExitState(StateController controller) {  
        
    }

    public override void OnEnterState(StateController controller) {

    }
}