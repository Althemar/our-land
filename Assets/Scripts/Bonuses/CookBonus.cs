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
        string str = base.BonusEffect(level);
        switch (level) {
            case 1:
                return str +
                        "Consume less food\n" +
                        "•  +" + foodConsumptionReduction + " <sprite name=\"Food\"> each turn\n";
            case 2:
                return str +
                        "You can get wool from Muings\n";
        }
        return "";
    }
}
