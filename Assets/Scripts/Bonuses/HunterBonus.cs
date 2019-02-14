using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterBonus : Bonus {

    public int foodBonus = 2;  

    public override void BonusEffectItem(MotherShip.ActionType action, ResourceType resource, ref int amount) {
        int bonus = foodBonus + GetBonusUpgrade();
        if (DoBonus) {
            if (action == MotherShip.ActionType.Harvest && resource == GameManager.Instance.motherShip.foodResource) {
                amount += bonus;
            }
        }
    }

    public override void BonusEffectEndTurn() {

    }

}
