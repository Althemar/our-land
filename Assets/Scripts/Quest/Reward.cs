﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum RewardType
{
    Resource,
    TechPoint
}

[Serializable]
public class Reward
{
    public RewardType rewardType;
    public ResourceType resource;
    public Bonus bonus;
    public int amount;

    public void GetReward() {
        if (rewardType == RewardType.Resource) {
            GameManager.Instance.motherShip.AddItem(resource, amount, MotherShip.ActionType.QuestReward);
        }
        else if (rewardType == RewardType.TechPoint) {
            bonus.UpdateLevel(amount);
        }
    }

    public string Display() {
        if (rewardType == RewardType.Resource) {
            return amount + " " + resource.name;
        }
        else if (rewardType == RewardType.TechPoint){
            return amount + " level to " + bonus.name;
        }
        return "";
    }
}
