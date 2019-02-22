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

    public override string BonusName() {
        return "Navigators";
    }

    public override string BonusEffect(int level) {
        string str = base.BonusEffect(level);
        switch (level) {
            case 1:
                return str +
                        "Consume less wood\n" +
                        "•  -" + fuelReduction + " <sprite name=\"Wood\"> by cell\n";
            case 2:
                return str +
                        "Windy tiles are now cost free\n";
        }
        return "";
    }
}
