using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class MissionUI : MonoBehaviour
{
    public ObjectiveProgressUI objectiveProgressPrefab;
    public TMP_Text title;

    private Mission mission;
    private Dictionary<Objective, ObjectiveProgressUI> objectives;

    private void Awake() {
        objectives = new Dictionary<Objective, ObjectiveProgressUI>();
    }

    public void Initialize(Mission mission) {
        this.mission = mission;
        foreach (Objective objective in mission.missionObjectives) {
            ObjectiveProgressUI objectiveUI = Instantiate(objectiveProgressPrefab, transform.parent).GetComponent<ObjectiveProgressUI>();
            objectives.Add(objective, objectiveUI);
            objectiveUI.Initialize(objective);
            title.text = mission.title;
        }
    }

    public void UpdateObjective(Objective objective) {
        //objectives[objective].UpdateText();
    }

    public void DestroyObjectivesUI() {
        foreach (KeyValuePair<Objective, ObjectiveProgressUI> objective in objectives) {
            Destroy(objective.Value.gameObject);
        }
    }

    public void UpdateMissionUI() {
        foreach (KeyValuePair<Objective, ObjectiveProgressUI> objective in objectives) {
                
        }
    }
}
