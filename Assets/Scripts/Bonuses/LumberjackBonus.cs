using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LumberjackBonus : Bonus {

    public int fuelBonus = 2;

    public override void BonusEffectItem(MotherShip.ActionType action, ResourceType resource, ref int amount) {
        int bonus = fuelBonus + GetBonusUpgrade();
        if (base.DoBonus) {
            if (action == MotherShip.ActionType.Harvest && resource == GameManager.Instance.motherShip.fuelResource) {
                amount += bonus;
            }
        }
    }

    public override void BonusEffectEndTurn() {

    }

}
