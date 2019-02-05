using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bonus : Updatable {
    int progress = 0;
    int level = 0;
    bool active = false;
    bool bonus = false;
    int activeTurn = 0;
    MotherShip ship;

    public int Progress {
        get => progress;
    }
    public int Level {
        get => level;
    }
    public bool IsActive {
        get => active;
    }
    public bool DoBonus {
        get => bonus;
    }

    void Start() {
        ship = GameManager.Instance.motherShip;
        AddToTurnManager();
    }

    void OnDestroy() {
        RemoveFromTurnManager();
        ship = null;
    }

    public override void AddToTurnManager() {
        TurnManager.Instance.AddToUpdate(this);
        ship.bonuses.Add(this);
    }

    public override void RemoveFromTurnManager() {
        TurnManager.Instance.RemoveFromUpdate(this);
        ship.bonuses.Remove(this);
    }

    public void ToogleActive() {
        if(active) {
            active = false;
            ship.remainingPopulationPoints++;
            ship.OnRemainingPointsChanged?.Invoke();
        } else {
            if (ship.remainingPopulationPoints <= 0)
                return;
            active = true;
            ship.remainingPopulationPoints--;
            ship.OnRemainingPointsChanged?.Invoke();
        }
    }
    
    public override void UpdateTurn() {
        base.UpdateTurn();
        if(active) {
            if (activeTurn > 0) {
                progress++;
                bonus = true;
                Debug.Log(gameObject.name + " progress");
            }
            else
                Debug.Log(gameObject.name + " activated");
            activeTurn++;
        } else {
            activeTurn = 0;
            bonus = false;
        }
        EndTurn();
    }

    public abstract void BonusEffectItem(MotherShip.ActionType action, ResourceType resource, ref int amount);
    public abstract void BonusEffectEndTurn();
}
