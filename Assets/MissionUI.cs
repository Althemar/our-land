using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
        foreach (Objective objective in mission.missionObjectives) {
            Instantiate(objectiveProgressPrefab, transform).Initialize(objective);
            title.text = mission.title;
        }
    }

    public void UpdateObjective(Objective objective) {
        //objectives[objective].UpdateText();
    }
}
