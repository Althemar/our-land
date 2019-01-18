using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Decisions/IsHungry")]
public class IsHungryDecision : Decision 
{
    public override bool Decide (StateController controller)
    {
        MovingEntity entity = controller.entity as MovingEntity;
       
        return entity.isHungry;
    }
}
