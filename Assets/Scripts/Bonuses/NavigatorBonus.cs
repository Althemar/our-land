using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigatorBonus : Bonus {

    public int fuelReduction = 1;

    public override void BonusEffectItem(MotherShip.ActionType action, ResourceType resource, ref int amount) {
        int bonus = fuelReduction + GetBonusUpgrade();
        if (base.DoBonus) {
            if (action == MotherShip.ActionType.Move && resource == GameManager.Instance.motherShip.fuelResource) {
                amount += bonus;
            }
        }
    }

    public override void BonusEffectEndTurn() {

    }
}
