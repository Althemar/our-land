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

    public override string BonusName() {
        return "Lumberjacks";
    }

    public override string BonusEffect(int level) {
        string str = base.BonusEffect(level);
        switch (level) {
            case 1:
                return str +
                        "Harvest more wood\n" +
                        "•  +" + fuelBonus + " <sprite name=\"Wood\"> per harvest\n";
            case 2:
                return str +
                        "You can plant trees\n";
        }
        return "";
    }
}
