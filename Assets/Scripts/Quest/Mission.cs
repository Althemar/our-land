using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission : MonoBehaviour
{
    public string title;
    public string lore;
    public string accomplishedLore;
    public string awards;

    [ReorderableList]
    public Objective[] missionObjectives;

    [ReorderableList]
    public Mission[] nextMission;

    public void StartMission() {
        foreach (Objective objective in missionObjectives) {
            objective.StartObjective();
        }
    }

    public bool Evaluate() {
        foreach (Objective objective in missionObjectives) {
            if (!objective.Evaluate()) {
                return false;
            }
        }
        return true;
    }   
}
