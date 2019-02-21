using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission : MonoBehaviour
{
    public string title;
    [Multiline]
    public string lore;
    public string accomplishedLore;
    public Sprite questImage;

    [BoxGroup("Turn limit")]
    public bool turnLimit;
    [BoxGroup("Turn limit")]
    public int remainingTurns = 0;

    public Objective mainObjectives;
    public Objective secondaryObjectives;

    [ReorderableList]
    public Mission[] nextMission;

    [HideInInspector]
    public bool failed;

    public delegate void UpdatedTurns(int turns);
    public UpdatedTurns OnRemainingTurnsUpdated;

    public void StartMission() {
        mainObjectives.StartObjective();
        secondaryObjectives?.StartObjective();
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

        secondaryObjectives?.Evaluate();
        return mainObjectives.Evaluate();
    }   
}
