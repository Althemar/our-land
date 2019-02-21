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

    public override string BonusName() {
        return "Cooks";
    }

    public override string BonusEffect(int level) {
        if (this.level < level)
            return "";
        switch (level) {
            case 1:
                return "<u>Rank 1 :</u>\n" +
                        "Consume less food\n" +
                        "•  +" + foodConsumptionReduction + " <sprite name=\"Food\"> each turn\n\n";
            case 2:
                return "<u>Rank 2 :</u>\n" +
                        "You can get wool from Muings\n\n";
        }
        return "";
    }
}
