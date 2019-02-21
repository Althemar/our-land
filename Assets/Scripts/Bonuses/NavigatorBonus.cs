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
        if (this.level < level)
            return "";
        switch (level) {
            case 1:
                return "<u>Rank 1 :</u>\n" +
                        "Consume less wood\n" +
                        "•  -" + fuelReduction + " <sprite name=\"Wood\"> by cell\n\n";
            case 2:
                return "<u>Rank 2 :</u>\n" +
                        "Windy tiles are now cost free\n\n";
        }
        return "";
    }
}
