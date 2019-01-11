using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Decisions/IsHungry")]
public class IsHungryDecision : Decision 
{
    public override bool Decide (StateController controller)
    {
        return controller.entity.reserve >= controller.entity.population;
    }
}
