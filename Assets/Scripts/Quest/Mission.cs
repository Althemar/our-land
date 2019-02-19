using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission : MonoBehaviour
{
    public string title;
    public string lore;
    public string accomplishedLore;

    [BoxGroup("Turn limit")]
    public bool turnLimit;
    [BoxGroup("Turn limit")]
    public int remainingTurns = 0;

    [ReorderableList]
    public Objective[] missionObjectives;

    [ReorderableList]
    public Mission[] nextMission;

    [HideInInspector]
    public bool failed;

    public delegate void UpdatedTurns(int turns);
    public UpdatedTurns OnRemainingTurnsUpdated;

    public void StartMission() {
        foreach (Objective objective in missionObjectives) {
            objective.StartObjective();
        }
    }

    public bool Evaluate() {
        if(turnLimit && remainingTurns > 0) {
            remainingTurns--;
            OnRemainingTurnsUpdated?.Invoke(remainingTurns);
            if (remainingTurns <= 0) {
                failed = true;
                return false;
            }
        }

        bool completed = true;
        foreach (Objective objective in missionObjectives) {
            if (!objective.Evaluate() && !objective.optional) {
                completed = false;
            }
        }
        return completed;
    }   
}
