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
}
