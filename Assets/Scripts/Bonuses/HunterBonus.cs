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

    public override string BonusName() {
        return "Hunters";
    }

    public override string BonusEffect(int level) {
        string str = base.BonusEffect(level);
        switch (level) {
            case 1:
                return str +
                        "Harvest more food\n" +
                        "•  +" + foodBonus + " <sprite name=\"Food\"> per harvest\n\n";
            case 2:
                return str +
                        "You can fish from lakes\n\n";
        }
        return "";
    }
}
