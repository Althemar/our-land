using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterBonus : Bonus {

    public int foodBonus = 2;

    public override void BonusEffectItem(MotherShip.ActionType action, ResourceType resource, ref int amount) {
        if (base.DoBonus) {
            if (action == MotherShip.ActionType.Harvest && resource == GameManager.Instance.motherShip.foodResource) {
                amount += foodBonus;
            }
        }
    }

    public override void BonusEffectEndTurn() {

    }

}
