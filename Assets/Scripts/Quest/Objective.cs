using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class Objective : MonoBehaviour
{
    public string description;
    public bool targetWithCamera;
    public string progressText;

    protected bool completed;
    protected bool failed;

    public bool optional;

    public List<Reward> rewards;

    public bool Completed { get => completed; }
    public bool Failed { get => failed; }

    public delegate void Updated();
    public Updated OnUpdate;

    public virtual bool Evaluate() {
        OnUpdate?.Invoke();
        return false;
    }

    public abstract void StartObjective();

    public virtual string GetProgressText() {
        return progressText;
    }
    
    public virtual int Goal() {
        return 0;
    }
    
    public virtual int Progress() {
        return 0;
    }


    public virtual Sprite IconCompletion() {
        return null;
    }

    public virtual Sprite IconReward() {
        if(rewards.Count == 0)
            return null;

        switch (rewards[0].rewardType) {
            case RewardType.Resource:
                return rewards[0].resource.icon;
            case RewardType.TechPoint:
                return null;
        }
        return null;
    }
}
