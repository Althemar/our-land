using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookBonus : Bonus {

    public int foodConsumptionReduction = 1;

    public override void BonusEffectItem(MotherShip.ActionType action, ResourceType resource, ref int amount) {
        int bonus = foodConsumptionReduction + GetBonusUpgrade();
        if (base.DoBonus) {
            if (action == MotherShip.ActionType.FoodConsumption && resource == GameManager.Instance.motherShip.foodResource) {
                amount += bonus;
            }
        }
    }

    public override void BonusEffectEndTurn() {

    }

}
