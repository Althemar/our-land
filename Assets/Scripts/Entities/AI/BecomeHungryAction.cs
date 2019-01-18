using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "PluggableAI/Actions/BecomeHungry")]
public class BecomeHungryAction : Action {
    public override void Act(StateController controller) {

        MovingEntity entity = controller.entity as MovingEntity;
        
        entity.remainingTurnsBeforeHungry -= 1;
        if (entity.remainingTurnsBeforeHungry <= 0) {
            entity.isHungry = true;
            entity.remainingTurnsBeforeHungry  = entity.movingEntitySO.nbTurnsToBeHungry;
        }
    }

    
    public override void OnExitState(StateController controller) {
        
    }

    public override void OnEnterState(StateController controller) {

    }
}

