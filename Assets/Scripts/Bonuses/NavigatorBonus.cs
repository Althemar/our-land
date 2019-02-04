using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigatorBonus : Bonus {

    public int fuelReduction = 1;

    public override void BonusEffectItem(MotherShip.ActionType action, ResourceType resource, ref int amount) {
        if (base.DoBonus) {
            if (action == MotherShip.ActionType.Move && resource == GameManager.Instance.motherShip.fuelResource) {
                amount -= fuelReduction;
            }
        }
    }

    public override void BonusEffectEndTurn() {

    }

}
