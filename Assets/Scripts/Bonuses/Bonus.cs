using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Bonus : Updatable {
    int progress = 0;
    bool active = false;
    int activeTurns = 0;
    MotherShip ship;

    protected int level = 1;
    protected bool bonus;

    [ReorderableList]
    public List<Upgrade> upgrades;

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
            bonus = false;
            activeTurns = 0;
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
        if(active && level < upgrades.Count) {
            if (activeTurns == 1) {
                bonus = true;
            }
            upgrades[level].remainingResearchTurns--;
            if (upgrades[level].remainingResearchTurns == 0) {
                NextLevel();
            }
            activeTurns++;
            progress++;
        }
        EndTurn();
    }

    private void NextLevel() {
        if (level < upgrades.Count) {
            upgrades[level].DoUpgrade();
            level++;
        }
    }

    public void UpdateLevel(int amount) {
        while (amount > 0) {
            NextLevel();
            amount--;
        }
        progress = 0;
    }

    protected int GetBonusUpgrade() {
        int bonus = 0;
        foreach (Upgrade upgrade in upgrades) {
            if (!upgrade.unlocked) {
                break;
            }
            else if (upgrade is BonusPlusUpgrade) {
                bonus += (upgrade as BonusPlusUpgrade).bonus;
            }
        }
        return bonus;
    }

    public abstract void BonusEffectItem(MotherShip.ActionType action, ResourceType resource, ref int amount);
    public abstract void BonusEffectEndTurn();

    public abstract string BonusName();
    public abstract string BonusEffect(int level);
}
