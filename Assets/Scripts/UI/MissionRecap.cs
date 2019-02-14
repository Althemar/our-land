using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionRecap : MonoBehaviour {

    public TextMeshProUGUI questTitle;
    public TextMeshProUGUI questGoal;
    public Slider completion;
    public TextMeshProUGUI completionText;

    private Mission mission;
    private Dictionary<Objective, ObjectiveProgressUI> objectives;

    private void Awake() {
        objectives = new Dictionary<Objective, ObjectiveProgressUI>();
    }

    public void Initialize(Mission mission) {
        this.mission = mission;

        questTitle.text = mission.title;
        completion.value = 0;
        completionText.text = "<voffset=0.5em>" + 0 + "</voffset>/<voffset=-0.5em>" + mission.missionObjectives.Length + "</voffset>";
        //this.questGoal.text = mission.;

        //<voffset=0.5em>x</voffset>/<voffset=-0.5em>x</voffset>
    }

}
