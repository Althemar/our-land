using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class MissionUI : MonoBehaviour
{
    public ObjectiveProgressUI objectiveProgressPrefab;
    public RemainingTurnsUI remainingTurnsPrefab;
    public TMP_Text title;

    private Mission mission;
    private Dictionary<Objective, ObjectiveProgressUI> objectives;
    private RemainingTurnsUI turnsUI;

    private void Awake() {
        objectives = new Dictionary<Objective, ObjectiveProgressUI>();
    }

    public void Initialize(Mission mission) {
        this.mission = mission;

        if (mission.turnLimit) {
            turnsUI = Instantiate(remainingTurnsPrefab, transform.parent);
            turnsUI.UpdateRemainingTurns(mission.remainingTurns);
            mission.OnRemainingTurnsUpdated += turnsUI.UpdateRemainingTurns;
        }

        foreach (Objective objective in mission.missionObjectives) {
            ObjectiveProgressUI objectiveUI = Instantiate(objectiveProgressPrefab, transform.parent);
            objectives.Add(objective, objectiveUI);
            objectiveUI.Initialize(objective);
            title.text = mission.title;
        }
    }

    public void DestroyObjectivesUI() {
        if (mission.turnLimit) {
            Destroy(turnsUI.gameObject);
        }
        foreach (KeyValuePair<Objective, ObjectiveProgressUI> objective in objectives) {
            Destroy(objective.Value.gameObject);
        }
    }
}
