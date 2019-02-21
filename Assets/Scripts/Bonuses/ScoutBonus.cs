using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoutBonus : Bonus {
    
    public int scoutRandMin = 1;
    public int scoutRandMax = 2;

    public override void BonusEffectItem(MotherShip.ActionType action, ResourceType resource, ref int amount) {

    }

    public override void BonusEffectEndTurn() {
        int bonus = scoutRandMin + GetBonusUpgrade();

        if (base.DoBonus) {
            int nbItem = Random.Range(bonus, bonus + 1);
            if(Random.value < 0.5f)
                GameManager.Instance.motherShip.AddItem(GameManager.Instance.motherShip.fuelResource, nbItem, MotherShip.ActionType.Bonus);
            else
                GameManager.Instance.motherShip.AddItem(GameManager.Instance.motherShip.foodResource, nbItem, MotherShip.ActionType.Bonus);
        }
    }

    public override string BonusName() {
        return "Scouts";
    }

    public override string BonusEffect(int level) {
        if (this.level < level)
            return "";
        switch (level) {
            case 1:
                return "<u>Rank 1 :</u>\n" +
                        "Get random resources\n" +
                        "•  +" + scoutRandMin + " or +" + scoutRandMax + " <sprite name=\"Wood\"> or <sprite name=\"Food\"> each turn\n\n";
            case 2:
                return "<u>Rank 2 :</u>\n" +
                        "Predict winter\n\n";
        }
        return "";
    }
}
